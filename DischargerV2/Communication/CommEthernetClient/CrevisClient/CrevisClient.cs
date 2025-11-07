using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Text;

namespace DischargerV2.Communication.CommEthernetClient.CrevisClient
{
    public class CrevisClient : IDisposable
    {
        // ---- Win32 INI helpers (local) ----
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private static string ReadIniValue(string path, string section, string key, string defaultValue)
        {
            try
            {
                var sb = new StringBuilder(1024);
                GetPrivateProfileString(section, key, defaultValue ?? string.Empty, sb, sb.Capacity, path);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.ReadIniValue error: {ex.Message}");
                return defaultValue;
            }
        }

        // ---- MODBUS constants (from manual sections 8.1.x / 8.2.x) ----
        private enum ModbusFunction : byte
        {
            ReadCoils = 0x01,
            ReadDiscreteInputs = 0x02,
            ReadHoldingRegisters = 0x03,
            ReadInputRegisters = 0x04,
            // others omitted for brevity
        }

        // ---- MBAP state ----
        private ushort _transactionId = 0; // auto increment per request
        public byte UnitId { get; set; } = 0x01; // configurable if needed
        private ushort _lastRequestedStartAddress = 0;
        private ushort _lastRequestedQuantity = 0;

        // Helper: bytes -> hex string (truncated)
        private static string BytesToHex(byte[] data, int count, int maxBytes = 256)
        {
            if (data == null || count <= 0) return string.Empty;
            int len = Math.Min(count, Math.Min(data.Length, maxBytes));
            var sb = new StringBuilder(len * 3);
            for (int i = 0; i < len; i++)
            {
                sb.Append(data[i].ToString("X2"));
                if (i < len - 1) sb.Append(' ');
            }
            if (count > len) sb.Append(" ...");
            return sb.ToString();
        }

        // ---- Configuration / counts ----
        private int _temperatureChannelCount = 1;
        public int TempCount
        {
            get { return _temperatureChannelCount; }
            set
            {
                _temperatureChannelCount = value;
                _calibrationOffsets = new float[_temperatureChannelCount];
                _totalChannelCount = _voltageChannelCount + _temperatureChannelCount;
                ResizeDataBuffers();
            }
        }

        private int _voltageChannelCount = 0;
        public int VoltCount
        {
            get { return _voltageChannelCount; }
            set
            {
                _voltageChannelCount = value;
                _totalChannelCount = _voltageChannelCount + _temperatureChannelCount;
                ResizeDataBuffers();
            }
        }

        private int _totalChannelCount = 8;

        // System-level calibration list (optional)
        public List<float> SysCalList { get; set; } = new List<float>();
        private float[] _calibrationOffsets = new float[1];

        private bool _isControlEnabled;
        public bool ControlEnabled
        {
            get { return _isControlEnabled; }
            set { _isControlEnabled = value; }
        }

        // ---- Network config ----
        private string _ipAddress = "192.168.1.200";
        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        private int _ipPort = 502; // Modbus TCP default
        public int IpPort
        {
            get { return _ipPort; }
            set { _ipPort = value; }
        }

        // External index (device index)
        public int Index { get; set; }

        // ---- Events ----
        public delegate void RaiseConnectEventHandler(int pch);
        public delegate void RaiseDisconnectEventHandler(int pch);
        public delegate void ReadPacketEventHandler(int ch, float[] rvalue);

        public RaiseConnectEventHandler Connected = null;
        public RaiseDisconnectEventHandler Disconnected = null;
        public ReadPacketEventHandler ReadCrevisData = null;

        // ---- Latest data snapshot ----
        private readonly object _snapshotLock = new object();
        private List<float> _latestVoltages = new List<float>();
        private List<float> _latestTemperatures = new List<float>();

        public float[] LastVoltages
        {
            get { lock (_snapshotLock) { return _latestVoltages.ToArray(); } }
        }
        public float[] LastTemperatures
        {
            get { lock (_snapshotLock) { return _latestTemperatures.ToArray(); } }
        }

        private static void EnsureListSize(List<float> list, int targetSize)
        {
            if (list == null) return;
            if (targetSize < 0) targetSize = 0;
            if (list.Count < targetSize)
            {
                for (int i = list.Count; i < targetSize; i++) list.Add(0f);
            }
            else if (list.Count > targetSize)
            {
                list.RemoveRange(targetSize, list.Count - targetSize);
            }
        }

        private void ResizeDataBuffers()
        {
            lock (_snapshotLock)
            {
                EnsureListSize(_latestVoltages, Math.Max(0, _voltageChannelCount));
                EnsureListSize(_latestTemperatures, Math.Max(0, _temperatureChannelCount));
            }
        }

        // ---- Logging ----
        private string _dataLogDirectory = string.Empty;
        public string DataLogPath
        {
            get { return _dataLogDirectory; }
            set { _dataLogDirectory = value; }
        }
        private StreamWriter _logWriter = null;

        // ---- Socket / threading ----
        public bool IsConnected = false;
        public Socket Socket;

        private Thread _readerThread;
        private Thread _communicationCheckThread;
        private volatile bool _stopRequestedFlag;
        private readonly object _socketLock = new object();
        private readonly object _timerLock = new object();

        // Read buffer
        public const int RxBuffSize = 2096;
        private readonly byte[] _receiveBuffer = new byte[RxBuffSize];
        public int ReceivedByteCount = 0;

        // timer (100ms base tick)
        private struct TimerVar
        {
            public bool Enabled;
            public int Limit; // 1-100ms ticks
            public int Tick;  // 1-100ms ticks

            public TimerVar(bool pEnable, int plimit, int pTick)
            {
                Enabled = pEnable;
                Limit = plimit;
                Tick = pTick;
            }
            public void Reset()
            {
                Enabled = false;
                Limit = 1;
                Tick = 0;
            }
        }
        private TimerVar _timerSlot1 = new TimerVar(false, 1, 0);
        private System.Timers.Timer _baseTimer100ms;

        private DateTime _lastLoopTime = DateTime.MinValue;

        public CrevisClient()
            : this(0)
        {
        }

        public CrevisClient(int index)
        {
            Index = index;
            _baseTimer100ms = new System.Timers.Timer(100)
            {
                AutoReset = true,
                Enabled = false
            };
            _baseTimer100ms.Elapsed += (s, e) => OnBaseTimerElapsed();

            // 기본 핸들러 등록
            ReadCrevisData += (_, values) => OnReadCrevis(values);
        }

        public void Dispose()
        {
            try
            {
                StopTcp();
                _baseTimer100ms?.Stop();
                _baseTimer100ms?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.Dispose error: {ex.Message}");
            }
        }

        // ---- Public: Initialize clients from Crevis.ini ----
        public static List<CrevisClient> InitializeFromIni(string iniPath = null, ReadPacketEventHandler onReadHandler = null)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string iniFilePath = iniPath;
            if (string.IsNullOrWhiteSpace(iniFilePath)) iniFilePath = System.IO.Path.Combine(baseDirectory, "Crevis.ini");

            var clients = new List<CrevisClient>();

            int deviceCount = 1;
            int.TryParse(ReadIniValue(iniFilePath, "General", "DeviceCount", "1"), out deviceCount);
            deviceCount = Math.Max(1, deviceCount);

            for (int i = 0; i < deviceCount; i++)
            {
                string section = $"Device{i}";
                bool isEnabled = true;
                bool.TryParse(ReadIniValue(iniFilePath, section, "Enabled", "True"), out isEnabled);

                string ipAddress = ReadIniValue(iniFilePath, section, "IpAddress", "192.168.0.60");

                int temperatureCount = 2;
                int voltageCount = 0;
                int.TryParse(ReadIniValue(iniFilePath, section, "TempChannelCount", "2"), out temperatureCount);
                int.TryParse(ReadIniValue(iniFilePath, section, "VoltChannelCount", "0"), out voltageCount);

                var calibrationList = new List<float>();
                for (int c = 1; c <= Math.Max(0, temperatureCount); c++)
                {
                    string key = $"Calibration_{c}";
                    float val;
                    if (!float.TryParse(ReadIniValue(iniFilePath, section, key, "0"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out val))
                        val = 0f;
                    calibrationList.Add(val);
                }

                var client = new CrevisClient(i)
                {
                    ControlEnabled = isEnabled,
                    IpAddress = ipAddress,
                    IpPort = 502,
                    DataLogPath = System.IO.Path.Combine(baseDirectory, "device"),
                    VoltCount = Math.Max(0, voltageCount),
                    TempCount = Math.Max(0, temperatureCount),
                    SysCalList = calibrationList
                };

                if (onReadHandler != null)
                    client.ReadCrevisData += onReadHandler;

                clients.Add(client);
            }

            return clients;
        }

        // ---- Public control ----
        public void SetTimer(int timerId, int intervalMs)
        {
            intervalMs = Math.Max(intervalMs, 100); // base 100ms
            int tick = (int)((double)intervalMs / 100.0);
            if (tick < 1) tick = 1;

            switch (timerId)
            {
                case 1:
                    _timerSlot1 = new TimerVar(true, tick, 0);
                    _baseTimer100ms.Interval = 100;
                    _baseTimer100ms.Start();
                    break;
            }
        }

        public void KillTimer(int timerId)
        {
            switch (timerId)
            {
                case 1:
                    _timerSlot1.Reset();
                    _baseTimer100ms.Stop();
                    break;
            }
        }

        // ---- Connection management ----
        public bool StartTcp()
        {
            KillTimer(1);

            if (!IsServerAlive(IpAddress))
            {
                IsConnected = false;
                return false;
            }

            // Close old socket if any
            lock (_socketLock)
            {
                try { Socket?.Close(0); }
                catch (Exception ex)
                {
                    Debug.WriteLine($"CrevisClient.StartTcp close socket error: {ex.Message}");
                }
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            try
            {
                var serverEP = new IPEndPoint(IPAddress.Parse(IpAddress), IpPort);
                Socket.Connect(serverEP);
            }
            catch (SocketException serr)
            {
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss (fff)") + " :: Connection Err = " + serr.Message);
            }

            if (Socket == null || !Socket.Connected)
            {
                IsConnected = false;
                Disconnected?.Invoke(Index);
                return false;
            }

            IsConnected = true;
            ReceivedByteCount = 0;
            OpenLog();

            // start threads
            _stopRequestedFlag = false;
            try
            {
                if (_readerThread == null || !_readerThread.IsAlive)
                {
                    _readerThread = new Thread(TcpReadThreadFunc) { IsBackground = true };
                    _readerThread.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.StartTcp start reader thread error: {ex.Message}");
            }

            try
            {
                if (_communicationCheckThread == null || !_communicationCheckThread.IsAlive)
                {
                    _communicationCheckThread = new Thread(StartCommChk) { IsBackground = true };
                    _communicationCheckThread.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.StartTcp start commcheck thread error: {ex.Message}");
            }

            SetTimer(1, 1000); // 1 sec request
            Connected?.Invoke(Index);
            return true;
        }

        public void StopTcp()
        {
            _stopRequestedFlag = true;
            KillTimer(1);

            try
            {
                if (_readerThread != null)
                {
                    if (!_readerThread.Join(500)) _readerThread.Abort();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.StopTcp reader thread stop error: {ex.Message}");
            }
            finally { _readerThread = null; }

            try
            {
                if (_communicationCheckThread != null)
                {
                    if (!_communicationCheckThread.Join(500)) _communicationCheckThread.Abort();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.StopTcp commcheck thread stop error: {ex.Message}");
            }
            finally { _communicationCheckThread = null; }

            lock (_socketLock)
            {
                try { Socket?.Shutdown(SocketShutdown.Both); }
                catch (Exception ex)
                {
                    Debug.WriteLine($"CrevisClient.StopTcp socket shutdown error: {ex.Message}");
                }
                try { Socket?.Close(); }
                catch (Exception ex)
                {
                    Debug.WriteLine($"CrevisClient.StopTcp socket close error: {ex.Message}");
                }
                Socket = null;
            }

            CloseLog();
            IsConnected = false;
            Disconnected?.Invoke(Index);
        }

        // ---- Build MODBUS Read Input Registers request (FC=0x04) ----
        private byte[] BuildReadInputRegistersRequest(ushort startAddr, ushort quantity)
        {
            // MBAP: Transaction(2) + Protocol(2=0) + Length(2) + UnitId(1)
            // PDU: Function(1) + Start(2) + Quantity(2)
            // Total = 7 + 5 = 12 bytes
            _transactionId++; // wrap automatically with ushort
            _lastRequestedStartAddress = startAddr;
            _lastRequestedQuantity = quantity;
            var frame = new byte[12];
            // Transaction
            frame[0] = (byte)(_transactionId >> 8);
            frame[1] = (byte)(_transactionId & 0xFF);
            // Protocol ID = 0
            frame[2] = 0x00;
            frame[3] = 0x00;
            // Length = UnitId(1) + PDU(5) = 6
            frame[4] = 0x00;
            frame[5] = 0x06;
            // Unit Id
            frame[6] = UnitId;
            // Function
            frame[7] = (byte)ModbusFunction.ReadInputRegisters;
            // Start address
            frame[8] = (byte)(startAddr >> 8);
            frame[9] = (byte)(startAddr & 0xFF);
            // Quantity
            frame[10] = (byte)(quantity >> 8);
            frame[11] = (byte)(quantity & 0xFF);
            return frame;
        }

        // ---- Timer callback ----
        private void OnBaseTimerElapsed()
        {
            try
            {
                if (!_timerSlot1.Enabled)
                    return;

                bool shouldFire;
                lock (_timerLock)
                {
                    _timerSlot1.Tick++;
                    if (_timerSlot1.Tick >= _timerSlot1.Limit)
                    {
                        _timerSlot1.Tick = 0;
                        shouldFire = true;
                    }
                    else
                    {
                        shouldFire = false;
                    }
                }

                if (!shouldFire)
                    return;

                SetCalibFactor();
                SendRequest();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TimerEvent100Callback error: {ex.Message}");
            }
        }

        private void SendRequest()
        {
            if (!IsConnected || Socket == null) return;
            try
            {
                // Request totalChannelCount registers starting at 0
                ushort quantity = (ushort)_totalChannelCount;
                var readReq = BuildReadInputRegistersRequest(0, quantity);
                Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CREVIS TX Dev={Index + 1} {IpAddress}:{IpPort} TID={_transactionId} FC=0x04 Start=0 Qty={quantity} Frame={BytesToHex(readReq, readReq.Length)}");
                Socket.Send(readReq);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CREVIS SendRequest error: " + ex.Message);
            }
        }

        // ---- Reader thread ----
        private void TcpReadThreadFunc()
        {
            while (!_stopRequestedFlag)
            {
                try
                {
                    ReadSocketOnce();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"CrevisClient.TcpReadThreadFunc error: {ex.Message}");
                }
                Thread.Sleep(0);
            }
        }

        private void ReadSocketOnce()
        {
            var now = DateTime.Now;
            var elapsedMilliseconds = (now - _lastLoopTime).TotalMilliseconds;
            if (elapsedMilliseconds < 1000)
            {
                int sleepDelayMs = (int)(1000 - elapsedMilliseconds);
                if (sleepDelayMs > 0) Thread.Sleep(sleepDelayMs);
            }

            if (!IsSocketConnected())
            {
                return;
            }

            int bytesRead = 0;
            try
            {
                if (!Socket.Poll(1000 * 1000, SelectMode.SelectRead))
                {
                    _lastLoopTime = DateTime.Now;
                    return;
                }

                bytesRead = Socket.Receive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None);
                if (bytesRead <= 0)
                {
                    try { Socket.Close(); }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"CrevisClient.ReadSocketOnce close socket error: {ex.Message}");
                    }
                    _lastLoopTime = DateTime.Now;
                    return;
                }

                Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CREVIS RX Dev={Index + 1} {IpAddress}:{IpPort} Len={bytesRead} Data={BytesToHex(_receiveBuffer, bytesRead)}");
                ReceivedByteCount = bytesRead;
            }
            catch (Exception ex)
            {
                try { Socket?.Close(); }
                catch (Exception ex2)
                {
                    Debug.WriteLine($"CrevisClient.ReadSocketOnce inner close socket error: {ex2.Message}");
                }
                Debug.WriteLine($"CrevisClient.ReadSocketOnce poll/recv error: {ex.Message}");
                _lastLoopTime = DateTime.Now;
                return;
            }

            ProcessModbusResponse();
            _lastLoopTime = DateTime.Now;
        }

        // ---- MODBUS response parser (supports FC04 & error response) ----
        private void ProcessModbusResponse()
        {
            try
            {
                if (ReceivedByteCount < 9) // minimal MBAP + error PDU size
                    return;

                // MBAP
                ushort rxTid = (ushort)((_receiveBuffer[0] << 8) | _receiveBuffer[1]);
                ushort protocol = (ushort)((_receiveBuffer[2] << 8) | _receiveBuffer[3]);
                ushort lengthField = (ushort)((_receiveBuffer[4] << 8) | _receiveBuffer[5]);
                byte unitId = _receiveBuffer[6];
                byte function = _receiveBuffer[7];

                if (protocol != 0)
                {
                    Debug.WriteLine($"MODBUS RX protocol id invalid: {protocol}");
                    return;
                }

                // Error response (function | 0x80)
                if ((function & 0x80) != 0)
                {
                    if (ReceivedByteCount >= 9)
                    {
                        byte exceptionCode = _receiveBuffer[8];
                        Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CREVIS RX ERR Dev={Index + 1} TID={rxTid} FC=0x{(function & 0x7F):X2} ExCode=0x{exceptionCode:X2}");
                    }
                    return;
                }

                if (function != (byte)ModbusFunction.ReadInputRegisters)
                {
                    Debug.WriteLine($"Unsupported MODBUS function 0x{function:X2}");
                    return;
                }

                // For FC04: PDU = FC(1) + ByteCount(1) + Data(ByteCount)
                if (ReceivedByteCount < 9) return; // need at least FC + ByteCount
                byte byteCount = _receiveBuffer[8];
                int expectedDataBytes = _lastRequestedQuantity * 2;
                if (byteCount != expectedDataBytes)
                {
                    Debug.WriteLine($"ByteCount mismatch. PDU ByteCount={byteCount}, Expected={expectedDataBytes}");
                    if (ReceivedByteCount < 9 + byteCount) return; // still ensure bounds
                }

                int totalNeeded = 9 + byteCount; // MBAP(7) + FC + ByteCount + Data
                if (ReceivedByteCount < totalNeeded)
                {
                    Debug.WriteLine($"Incomplete MODBUS frame. Have={ReceivedByteCount}, Need={totalNeeded}");
                    return;
                }

                int registerCount = byteCount / 2;
                float[] values = new float[_totalChannelCount];
                int temperatureIndex = 0;
                var sbTemps = new StringBuilder();
                string voltLog = string.Empty;

                for (int reg = 0; reg < Math.Min(registerCount, _totalChannelCount); reg++)
                {
                    int dataIndex = 9 + reg * 2;
                    short raw = (short)((_receiveBuffer[dataIndex] << 8) | _receiveBuffer[dataIndex + 1]);
                    if (reg >= _voltageChannelCount)
                    {
                        float val = ((float)raw / 10.0f) + (temperatureIndex < _calibrationOffsets.Length ? _calibrationOffsets[temperatureIndex] : 0f);
                        values[reg] = val;
                        if (sbTemps.Length > 0) sbTemps.Append(", ");
                        sbTemps.Append(val.ToString("F1"));
                        temperatureIndex++;
                    }
                    else
                    {
                        float val = ((float)raw / 10.0f);
                        values[reg] = val;
                        voltLog += " " + val.ToString();
                    }
                }

                Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CREVIS RX OK Dev={Index + 1} TID={rxTid} Unit={unitId} Regs={registerCount} Voltages:{voltLog} Temps:[{sbTemps}] ");
                WriteLog($">> TID={rxTid} FC04 Voltages:{voltLog} Temps:[{sbTemps}] ");
                ReadCrevisData?.Invoke(Index, values);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.ProcessModbusResponse error: {ex.Message}");
            }
        }

        // ---- Comm check ----
        private int _retryCommCnt = 0;
        private void StartCommChk()
        {
            while (!_stopRequestedFlag)
            {
                Thread.Sleep(1000);
                if (!ControlEnabled) continue;

                if (!IsSocketConnected())
                {
                    if (StartTcp())
                    {
                        _retryCommCnt = 0;
                    }
                    else
                    {
                        _retryCommCnt++;
                        if (_retryCommCnt > 5)
                        {
                            float[] values = new float[_totalChannelCount];
                            for (int i = 0; i < _totalChannelCount; i++) values[i] = 0f;
                            ReadCrevisData?.Invoke(Index, values);
                            Disconnected?.Invoke(Index);
                        }
                    }
                }
                else
                {
                    _retryCommCnt = 0;
                }
            }
        }

        // ---- Default snapshot updater ----
        private void OnReadCrevis(float[] values)
        {
            if (values == null) return;

            lock (_snapshotLock)
            {
                int vCount = Math.Min(_voltageChannelCount, values.Length);
                int tCount = Math.Min(_temperatureChannelCount, Math.Max(0, values.Length - _voltageChannelCount));

                EnsureListSize(_latestVoltages, Math.Max(0, _voltageChannelCount));
                EnsureListSize(_latestTemperatures, Math.Max(0, _temperatureChannelCount));

                for (int i = 0; i < vCount; i++)
                    _latestVoltages[i] = values[i];

                for (int i = 0; i < tCount; i++)
                    _latestTemperatures[i] = values[_voltageChannelCount + i];
            }
        }

        // ---- Helpers ----
        private void SetCalibFactor()
        {
            for (int i = 0; i < _calibrationOffsets.Length; i++) _calibrationOffsets[i] = 0;
            if (!ControlEnabled) return;

            try
            {
                for (int i = 0; i < _calibrationOffsets.Length; i++)
                {
                    if (i < SysCalList.Count)
                    {
                        _calibrationOffsets[i] = SysCalList[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.SetCalibFactor error: {ex.Message}");
            }
        }

        private bool IsServerAlive(string hostName)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send(hostName, 10);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.IsServerAlive error: {ex.Message}");
                return false;
            }
        }

        public bool IsSocketConnected()
        {
            try
            {
                if (Socket == null) return false;
                if (!Socket.Connected) return false;

                bool part1 = Socket.Poll(1, SelectMode.SelectRead) && Socket.Available == 0;
                bool part2 = Socket.Poll(1, SelectMode.SelectError);
                if (part1 || part2) return false;

                Socket.Send(Array.Empty<byte>(), 0, 0, SocketFlags.None);
                return true;
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"CrevisClient.IsSocketConnected socket error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.IsSocketConnected error: {ex.Message}");
                return false;
            }
        }

        // ---- Logging ----
        private void OpenLog()
        {
            try
            {
                CloseLog();
                string basePath = string.IsNullOrEmpty(_dataLogDirectory) ? AppDomain.CurrentDomain.BaseDirectory : _dataLogDirectory;
                string folder = Path.Combine(basePath, "device", "CrevisAUX", DateTime.Now.ToString("yyyy_MM_dd"));
                Directory.CreateDirectory(folder);
                string file = Path.Combine(folder, "Crevis_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_Device" + (Index + 1).ToString("D2") + ".log");
                _logWriter = new StreamWriter(file, true, Encoding.UTF8);
                WriteLog("Created at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                WriteLog("-------------------------------------------------");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.OpenLog error: {ex.Message}");
            }
        }

        private void CloseLog()
        {
            try
            {
                if (_logWriter != null)
                {
                    _logWriter.Flush();
                    _logWriter.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.CloseLog error: {ex.Message}");
            }
            finally { _logWriter = null; }
        }

        private void WriteLog(string msg)
        {
            try
            {
                if (_logWriter == null) return;
                _logWriter.WriteLine(msg);
                _logWriter.Flush();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CrevisClient.WriteLog error: {ex.Message}");
            }
        }
    }
}
