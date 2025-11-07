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
        // Win32 INI
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

        // MODBUS
        private enum ModbusFunction : byte { ReadInputRegisters = 0x04 }
        private const float RegisterScale = 0.1f; // raw register scaling
        private ushort _transactionId = 0;
        public byte UnitId { get; set; } = 0x01;
        private readonly Dictionary<ushort, (ushort start, ushort qty, DateTime time)> _pendingRequests = new Dictionary<ushort, (ushort, ushort, DateTime)>();
        private readonly Dictionary<byte, string> _modbusExceptionMap = new Dictionary<byte, string>
        {
            {0x01, "Illegal Function"},
            {0x02, "Illegal Data Address"},
            {0x03, "Illegal Data Value"},
            {0x04, "Slave Device Failure"},
            {0x05, "Acknowledge"},
            {0x06, "Slave Device Busy"},
            {0x08, "Memory Parity Error"},
            {0x0A, "Gateway Path Unavailable"},
            {0x0B, "Gateway Target Device Failed to Respond"},
        };

        // Logging verbosity
        public bool VerboseLogging { get; set; } = true;

        private static string BytesToHex(IReadOnlyList<byte> data, int offset, int count, int maxBytes = 256)
        {
            if (data == null || count <= 0) return string.Empty;
            int len = Math.Min(count, Math.Min(maxBytes, data.Count - offset));
            var sb = new StringBuilder(len * 3);
            for (int i = 0; i < len; i++) { sb.Append(data[offset + i].ToString("X2")); if (i < len - 1) sb.Append(' '); }
            if (count > len) sb.Append(" ...");
            return sb.ToString();
        }

        // Channel configuration
        private int _temperatureChannelCount = 1;
        public int TempCount
        {
            get => _temperatureChannelCount;
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
            get => _voltageChannelCount;
            set
            {
                _voltageChannelCount = value;
                _totalChannelCount = _voltageChannelCount + _temperatureChannelCount;
                ResizeDataBuffers();
            }
        }
        private int _totalChannelCount = 8;
        public List<float> SysCalList { get; set; } = new List<float>();
        private float[] _calibrationOffsets = new float[1];

        // Control
        public bool ControlEnabled { get; set; } = true;

        // Network config
        public string IpAddress { get; set; } = "192.168.1.200";
        public int IpPort { get; set; } = 502;
        public int Index { get; set; }

        // Events
        public delegate void RaiseConnectEventHandler(int pch);
        public delegate void RaiseDisconnectEventHandler(int pch);
        public delegate void ReadPacketEventHandler(int ch, float[] rvalue);
        public RaiseConnectEventHandler Connected;
        public RaiseDisconnectEventHandler Disconnected;
        public ReadPacketEventHandler ReadCrevisData;

        // Snapshot buffers
        private readonly object _snapshotLock = new object();
        private readonly List<float> _latestVoltages = new List<float>();
        private readonly List<float> _latestTemperatures = new List<float>();
        public float[] LastVoltages { get { lock (_snapshotLock) { return _latestVoltages.ToArray(); } } }
        public float[] LastTemperatures { get { lock (_snapshotLock) { return _latestTemperatures.ToArray(); } } }

        private static void EnsureListSize(List<float> list, int targetSize)
        {
            if (list == null) return;
            if (targetSize < 0) targetSize = 0;
            if (list.Count < targetSize) for (int i = list.Count; i < targetSize; i++) list.Add(0f);
            else if (list.Count > targetSize) list.RemoveRange(targetSize, list.Count - targetSize);
        }
        private void ResizeDataBuffers()
        {
            lock (_snapshotLock)
            {
                EnsureListSize(_latestVoltages, _voltageChannelCount);
                EnsureListSize(_latestTemperatures, _temperatureChannelCount);
            }
        }

        // Logging file
        public string DataLogPath { get; set; } = string.Empty;
        private StreamWriter _logWriter;

        // Socket / threading
        public bool IsConnected = false;
        public Socket Socket;
        private Thread _readerThread;
        private Thread _communicationCheckThread;
        private readonly object _socketLock = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        // Accumulated receive buffer (for frame boundary handling)
        private readonly List<byte> _rxAccum = new List<byte>(4096);

        // Timer state (request polling)
        private struct TimerVar { public bool Enabled; public int Limit; public int Tick; public TimerVar(bool e, int l) { Enabled = e; Limit = l; Tick = 0; } public void Reset() { Enabled = false; Limit = 1; Tick = 0; } }
        private TimerVar _timerSlot1 = new TimerVar(false, 10);
        private System.Timers.Timer _baseTimer100ms;
        private readonly object _timerLock = new object();
        
        private int _retryCommCnt = 0;
        private bool _disposed;

        public CrevisClient() : this(0) { }
        public CrevisClient(int index)
        {
            Index = index;
            _baseTimer100ms = new System.Timers.Timer(100) { AutoReset = true, Enabled = false };
            _baseTimer100ms.Elapsed += (_, __) => OnBaseTimerElapsed();
            ReadCrevisData += (_, values) => OnReadCrevis(values);
        }

        // IDisposable pattern
        ~CrevisClient() { Dispose(false); }
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing)
            {
                try { StopTcp(); } catch { }
                try { _baseTimer100ms?.Stop(); _baseTimer100ms?.Dispose(); } catch { }
                try { CloseLog(); } catch { }
                _cts?.Cancel();
            }
        }

        // Initialization from INI
        public static List<CrevisClient> InitializeFromIni(string iniPath = null, ReadPacketEventHandler onReadHandler = null)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string iniFilePath = string.IsNullOrWhiteSpace(iniPath) ? Path.Combine(baseDirectory, "Crevis.ini") : iniPath;
            var clients = new List<CrevisClient>();
            if (!int.TryParse(ReadIniValue(iniFilePath, "General", "DeviceCount", "1"), out int deviceCount)) deviceCount = 1;
            deviceCount = Math.Max(1, deviceCount);
            for (int i = 0; i < deviceCount; i++)
            {
                string section = $"Device{i}";
                bool isEnabled = bool.TryParse(ReadIniValue(iniFilePath, section, "Enabled", "True"), out bool tmpEnabled) ? tmpEnabled : true;
                string ipAddress = ReadIniValue(iniFilePath, section, "IpAddress", "192.168.0.60");
                int temperatureCount = int.TryParse(ReadIniValue(iniFilePath, section, "TempChannelCount", "2"), out int tc) ? tc : 2;
                int voltageCount = int.TryParse(ReadIniValue(iniFilePath, section, "VoltChannelCount", "0"), out int vc) ? vc : 0;
                var calList = new List<float>();
                for (int c = 1; c <= Math.Max(0, temperatureCount); c++)
                {
                    if (!float.TryParse(ReadIniValue(iniFilePath, section, $"Calibration_{c}", "0"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float val)) val = 0f;
                    calList.Add(val);
                }
                var client = new CrevisClient(i)
                {
                    ControlEnabled = isEnabled,
                    IpAddress = ipAddress,
                    IpPort = 502,
                    DataLogPath = baseDirectory,
                    VoltCount = Math.Max(0, voltageCount),
                    TempCount = Math.Max(0, temperatureCount),
                    SysCalList = calList
                };
                if (onReadHandler != null) client.ReadCrevisData += onReadHandler;
                clients.Add(client);
            }
            return clients;
        }

        // Timer control
        public void SetTimer(int timerId, int intervalMs)
        {
            intervalMs = Math.Max(intervalMs, 100);
            int ticks = Math.Max(1, (int)Math.Round(intervalMs / 100.0));
            if (timerId == 1)
            {
                _timerSlot1 = new TimerVar(true, ticks);
                _baseTimer100ms.Interval = 100;
                _baseTimer100ms.Start();
            }
        }
        public void KillTimer(int timerId)
        {
            if (timerId == 1)
            {
                _timerSlot1.Reset();
                _baseTimer100ms.Stop();
            }
        }

        // Connection management
        public bool StartTcp()
        {
            KillTimer(1);
            if (!IsServerAlive(IpAddress)) { IsConnected = false; return false; }
            lock (_socketLock)
            {
                try { Socket?.Dispose(); } catch { }
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveTimeout = 2000, SendTimeout = 2000 }; // 2s timeouts
            }
            try { Socket.Connect(new IPEndPoint(IPAddress.Parse(IpAddress), IpPort)); }
            catch (SocketException serr) { Debug.WriteLine($"MODBUS Connect error: {serr.Message}"); }
            if (Socket == null || !Socket.Connected) { IsConnected = false; Disconnected?.Invoke(Index); return false; }
            IsConnected = true;
            OpenLog();
            _cts = new CancellationTokenSource();
            StartReaderThread();
            StartCommCheckThread();
            SetTimer(1, 1000); // poll each second
            Connected?.Invoke(Index);
            return true;
        }
        public void StopTcp()
        {
            _cts.Cancel();
            KillTimer(1);
            try { _readerThread?.Join(800); } catch { }
            try { _communicationCheckThread?.Join(800); } catch { }
            lock (_socketLock)
            {
                try { Socket?.Shutdown(SocketShutdown.Both); } catch { }
                try { Socket?.Close(); } catch { }
                Socket = null;
            }
            CloseLog();
            IsConnected = false;
            Disconnected?.Invoke(Index);
        }
        private void StartReaderThread()
        {
            if (_readerThread != null && _readerThread.IsAlive) return;
            _readerThread = new Thread(() =>
            {
                var token = _cts.Token;
                while (!token.IsCancellationRequested)
                {
                    try { ReadSocketAccumulate(); ParseFrames(); }
                    catch (Exception ex) { if (VerboseLogging) Debug.WriteLine($"Reader error: {ex.Message}"); }
                }
            }) { IsBackground = true };
            _readerThread.Start();
        }
        private void StartCommCheckThread()
        {
            if (_communicationCheckThread != null && _communicationCheckThread.IsAlive) return;
            _communicationCheckThread = new Thread(() =>
            {
                var token = _cts.Token;
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                    if (!ControlEnabled) continue;
                    if (!IsSocketConnected())
                    {
                        if (StartTcp()) _retryCommCnt = 0; else { _retryCommCnt++; if (_retryCommCnt > 5) { InjectZeroDataAndDisconnect(); } }
                    }
                    else _retryCommCnt = 0;
                }
            }) { IsBackground = true };
            _communicationCheckThread.Start();
        }
        private void InjectZeroDataAndDisconnect()
        {
            var arr = new float[_totalChannelCount];
            ReadCrevisData?.Invoke(Index, arr);
            Disconnected?.Invoke(Index);
        }

        // Build request
        private byte[] BuildReadInputRegistersRequest(ushort startAddr, ushort quantity)
        {
            _transactionId++;
            _pendingRequests[_transactionId] = (startAddr, quantity, DateTime.UtcNow);
            var frame = new byte[12];
            frame[0] = (byte)(_transactionId >> 8); frame[1] = (byte)(_transactionId & 0xFF);
            frame[2] = 0; frame[3] = 0; // protocol
            frame[4] = 0; frame[5] = 6; // length Unit(1)+PDU(5)
            frame[6] = UnitId;
            frame[7] = (byte)ModbusFunction.ReadInputRegisters;
            frame[8] = (byte)(startAddr >> 8); frame[9] = (byte)(startAddr & 0xFF);
            frame[10] = (byte)(quantity >> 8); frame[11] = (byte)(quantity & 0xFF);
            return frame;
        }

        // Timer callback
        private void OnBaseTimerElapsed()
        {
            try
            {
                if (!_timerSlot1.Enabled) return;
                lock (_timerLock)
                {
                    _timerSlot1.Tick++;
                    if (_timerSlot1.Tick < _timerSlot1.Limit) return;
                    _timerSlot1.Tick = 0;
                }
                SetCalibFactor();
                SendRequest();
            }
            catch (Exception ex) { if (VerboseLogging) Debug.WriteLine($"Timer error: {ex.Message}"); }
        }
        private void SendRequest()
        {
            if (!IsConnected || Socket == null) return;
            try
            {
                ushort quantity = (ushort)_totalChannelCount;
                var req = BuildReadInputRegistersRequest(0, quantity);
                if (VerboseLogging) Debug.WriteLine($"TX TID={_transactionId} FC04 Qty={quantity} Frame={BytesToHex(req,0,req.Length)}");
                Socket.Send(req);
            }
            catch (Exception ex) { Debug.WriteLine($"SendRequest error: {ex.Message}"); }
        }

        // Socket receive accumulation
        private void ReadSocketAccumulate()
        {
            if (!IsSocketConnected()) { Thread.Sleep(100); return; }
            // Non-blocking poll with short timeout
            if (!Socket.Poll(250_000, SelectMode.SelectRead)) return; // 250ms
            int available = Socket.Available;
            if (available <= 0) return;
            var temp = new byte[Math.Min(available, 4096)];
            int read = 0;
            try { read = Socket.Receive(temp, 0, temp.Length, SocketFlags.None); }
            catch (SocketException se) { if (VerboseLogging) Debug.WriteLine($"Receive socket error: {se.Message}"); return; }
            if (read <= 0) return;
            lock (_rxAccum) { _rxAccum.AddRange(temp.AsSpan(0, read).ToArray()); }
        }

        private void ParseFrames()
        {
            lock (_rxAccum)
            {
                // Minimum MBAP = 7 bytes
                while (true)
                {
                    if (_rxAccum.Count < 7) return;
                    // MBAP length field at [4..5]
                    ushort lengthField = (ushort)((_rxAccum[4] << 8) | _rxAccum[5]);
                    int frameLength = 6 + lengthField; // total frame size
                    if (_rxAccum.Count < frameLength) return; // wait for complete frame
                    // Extract frame
                    var frame = _rxAccum.GetRange(0, frameLength).ToArray();
                    _rxAccum.RemoveRange(0, frameLength);
                    ProcessModbusFrame(frame);
                }
            }
        }

        private void ProcessModbusFrame(byte[] frame)
        {
            try
            {
                if (frame.Length < 9) return; // minimal error frame
                ushort rxTid = (ushort)((frame[0] << 8) | frame[1]);
                ushort protocol = (ushort)((frame[2] << 8) | frame[3]);
                ushort lengthField = (ushort)((frame[4] << 8) | frame[5]);
                byte unitId = frame[6];
                byte function = frame[7];
                if (protocol != 0) { if (VerboseLogging) Debug.WriteLine($"Invalid protocol id {protocol}"); return; }
                if (unitId != UnitId) { if (VerboseLogging) Debug.WriteLine($"UnitId mismatch RX={unitId} Expected={UnitId}"); }
                if (!_pendingRequests.TryGetValue(rxTid, out var reqInfo)) { if (VerboseLogging) Debug.WriteLine($"Unexpected TID {rxTid}"); }
                else _pendingRequests.Remove(rxTid);

                // Error response
                if ((function & 0x80) != 0)
                {
                    if (frame.Length >= 9)
                    {
                        byte exCode = frame[8];
                        string desc = _modbusExceptionMap.TryGetValue(exCode, out var d) ? d : "Unknown";
                        Debug.WriteLine($"RX ERR TID={rxTid} FC=0x{(function & 0x7F):X2} Code=0x{exCode:X2} ({desc})");
                    }
                    return;
                }
                if (function != (byte)ModbusFunction.ReadInputRegisters) { if (VerboseLogging) Debug.WriteLine($"Unsupported FC {function:X2}"); return; }

                if (frame.Length < 9) return; // need byte count
                byte byteCount = frame[8];
                if (byteCount + 9 != frame.Length) { if (VerboseLogging) Debug.WriteLine($"Length mismatch ByteCount={byteCount} FrameLen={frame.Length}"); return; }
                int registerCount = byteCount / 2;
                if (registerCount < _totalChannelCount) { if (VerboseLogging) Debug.WriteLine($"Register count {registerCount} < expected {_totalChannelCount}"); }
                var values = new float[_totalChannelCount];
                int temperatureIndex = 0;
                var sbVolt = new StringBuilder();
                var sbTemp = new StringBuilder();
                for (int reg = 0; reg < Math.Min(registerCount, _totalChannelCount); reg++)
                {
                    int dataIndex = 9 + reg * 2;
                    short raw = (short)((frame[dataIndex] << 8) | frame[dataIndex + 1]);
                    if (reg >= _voltageChannelCount)
                    {
                        float val = (raw * RegisterScale) + (temperatureIndex < _calibrationOffsets.Length ? _calibrationOffsets[temperatureIndex] : 0f);
                        values[reg] = val;
                        if (sbTemp.Length > 0) sbTemp.Append(", ");
                        sbTemp.Append(val.ToString("F1"));
                        temperatureIndex++;
                    }
                    else
                    {
                        float val = raw * RegisterScale;
                        values[reg] = val;
                        if (sbVolt.Length > 0) sbVolt.Append(", ");
                        sbVolt.Append(val.ToString("F1"));
                    }
                }
                if (VerboseLogging) Debug.WriteLine($"RX OK TID={rxTid} Regs={registerCount} Volt=[{sbVolt}] Temp=[{sbTemp}] Frame={BytesToHex(frame,0,frame.Length,64)}");
                WriteLog($"TID={rxTid} FC04 Volt=[{sbVolt}] Temp=[{sbTemp}]");
                ReadCrevisData?.Invoke(Index, values);
            }
            catch (Exception ex) { Debug.WriteLine($"ProcessModbusFrame error: {ex.Message}"); }
        }

        // Snapshot update
        private void OnReadCrevis(float[] values)
        {
            if (values == null) return;
            lock (_snapshotLock)
            {
                int vCount = Math.Min(_voltageChannelCount, values.Length);
                int tCount = Math.Min(_temperatureChannelCount, Math.Max(0, values.Length - _voltageChannelCount));
                EnsureListSize(_latestVoltages, _voltageChannelCount);
                EnsureListSize(_latestTemperatures, _temperatureChannelCount);
                for (int i = 0; i < vCount; i++) _latestVoltages[i] = values[i];
                for (int i = 0; i < tCount; i++) _latestTemperatures[i] = values[_voltageChannelCount + i];
            }
        }

        // Calibration
        private void SetCalibFactor()
        {
            for (int i = 0; i < _calibrationOffsets.Length; i++) _calibrationOffsets[i] = 0f;
            if (!ControlEnabled) return;
            for (int i = 0; i < _calibrationOffsets.Length && i < SysCalList.Count; i++) _calibrationOffsets[i] = SysCalList[i];
        }

        // Health checks
        private bool IsServerAlive(string hostName)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send(hostName, 300); // 300ms timeout
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (Exception ex) { if (VerboseLogging) Debug.WriteLine($"Ping error: {ex.Message}"); return false; }
        }
        public bool IsSocketConnected()
        {
            try
            {
                if (Socket == null || !Socket.Connected) return false;
                bool readClosed = Socket.Poll(1, SelectMode.SelectRead) && Socket.Available == 0;
                bool hasError = Socket.Poll(1, SelectMode.SelectError);
                return !(readClosed || hasError);
            }
            catch (Exception ex) { if (VerboseLogging) Debug.WriteLine($"IsSocketConnected error: {ex.Message}"); return false; }
        }

        // File logging
        private void OpenLog()
        {
            try
            {
                CloseLog();
                string baseFolder = string.IsNullOrEmpty(DataLogPath) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "device") : DataLogPath;
                string folder = Path.Combine(baseFolder, "CrevisAUX", DateTime.Now.ToString("yyyy_MM_dd"));
                Directory.CreateDirectory(folder);
                string file = Path.Combine(folder, $"Crevis_{DateTime.Now:yyyyMMddHHmmss}_Device{(Index + 1):D2}.log");
                _logWriter = new StreamWriter(file, true, Encoding.UTF8);
                WriteLog("Created at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                WriteLog("-------------------------------------------");
            }
            catch (Exception ex) { Debug.WriteLine($"OpenLog error: {ex.Message}"); }
        }
        private void CloseLog()
        {
            try { _logWriter?.Flush(); _logWriter?.Close(); } catch (Exception ex) { Debug.WriteLine($"CloseLog error: {ex.Message}"); } finally { _logWriter = null; }
        }
        private void WriteLog(string msg)
        {
            try { _logWriter?.WriteLine(msg); _logWriter?.Flush(); } catch (Exception ex) { Debug.WriteLine($"WriteLog error: {ex.Message}"); }
        }
    }
}
