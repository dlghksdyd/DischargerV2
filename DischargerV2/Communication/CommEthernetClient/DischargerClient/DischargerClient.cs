using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Ethernet.Client.Common;

namespace Ethernet.Client.Discharger
{
    public enum EDischargerState : byte
    {
        None = 0x0,

        Disconnected = 0x10,
        Connecting = 0x11,
        Ready = 0x12,
        Discharging = 0x13,
        Pause = 0x14,

        SafetyOutOfRange = 0x20,
        ReturnCodeError = 0x21,
        ChStatusError = 0x22,
        DeviceError = 0x23,
    }

    public enum EDischargerClientError
    {
        Ok,
        InvalidDischargerState,
        FailProcessPacket,
    }

    public class EthernetClientDischargerStart
    {
        public string DischargerName = string.Empty;
        public short DischargerChannel = short.MaxValue;
        public IPAddress IpAddress = IPAddress.None;
        public int EthernetPort = int.MaxValue;
        public int TimeOutMs = int.MaxValue;
        public double SafetyVoltageMax = double.MaxValue;
        public double SafetyVoltageMin = double.MaxValue;
        public double SafetyCurrentMax = double.MaxValue;
        public double SafetyCurrentMin = double.MaxValue;
    }

    public class DischargerDatas
    {
        /// <summary>
        /// 수신 받은 데이터
        /// </summary>
        public uint ErrorCode { get; set; } = 0;
        public EReturnCode ReturnCode { get; set; } = EReturnCode.Success;
        public EChannelStatus ChannelStatus { get; set; } = EChannelStatus.Standby0;
        public double ReceiveBatteryVoltage { get; set; } = 0;
        public double ReceiveDischargeCurrent { get; set; } = 0;

        /// <summary>
        /// Safety 데이터
        /// </summary>
        public double SafetyVoltageMax { get; set; } = 0.0;
        public double SafetyVoltageMin { get; set; } = 0.0;
        public double SafetyCurrentMax { get; set; } = 0.0;
        public double SafetyCurrentMin { get; set; } = 0.0;

        /// <summary>
        /// 방전시작시간
        /// </summary>
        public DateTime DischargingStartTime { get; set; } = DateTime.MinValue;
    }

    public class DischargerInfo
    {
        /// <summary>
        /// 방전기 정보
        /// </summary>
        public string Name { get; set; } = string.Empty;
        public short Channel { get; set; } = short.MaxValue;
        public IPAddress IpAddress { get; set; }
        public int EthernetPort { get; set; } = int.MaxValue;
        public int TimeOutMs { get; set; } = int.MaxValue;

        /// <summary>
        /// 방전기 스펙
        /// </summary>
        public double SpecVoltage { get; set; } = double.MaxValue;
        public double SpecCurrent { get; set; } = double.MaxValue;

        /// <summary>
        /// 안전 조건
        /// </summary>
        public double SafetyVoltageMax { get; set; } = double.MaxValue;
        public double SafetyVoltageMin { get; set; } = double.MaxValue;
        public double SafetyCurrentMax { get; set; } = double.MaxValue;
        public double SafetyCurrentMin { get; set; } = double.MaxValue;
        public double SafetyTempMax { get; set; } = double.MaxValue;
        public double SafetyTempMin { get; set; } = double.MaxValue;
    }

    public class EthernetClientDischarger
    {
        private class LogArgument
        {
            public LogArgument(string logMessage)
            {
                LogMessage = logMessage;
            }

            public string LogMessage { get; set; } = string.Empty;
            public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        }

        /// <summary>
        /// key: IP Address
        /// </summary>
        private static Dictionary<string, byte> _serialNumbers = new Dictionary<string, byte>();
        /// <summary>
        /// key: IP Address
        /// </summary>
        private static Dictionary<string, object> _serialNumberDataLock = new Dictionary<string, object>();

        private object _packetLock = new object();

        private EthernetClientDischargerStart _parameters = null;
        private EthernetClient _dischargerClient = null;

        private System.Timers.Timer ReadInfoTimer = null;

        private DischargerDatas _dischargerData = new DischargerDatas();
        private EDischargerState _dischargerState = EDischargerState.None;

        private List<string> _traceLogs = new List<string>();
        private object _traceLogLock = new object();

        private void AddTraceLog(LogArgument logFormat)
        {
            string formattedMessage = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ");
            formattedMessage += _parameters.DischargerName + " - ";
            formattedMessage += logFormat.LogMessage + " ";

            for (int i = 0; i < logFormat.Parameters.Count; i++)
            {
                string key = logFormat.Parameters.Keys.ElementAt(i);
                string value = logFormat.Parameters.Values.ElementAt(i).ToString();

                if (i == 0)
                {
                    formattedMessage += "(";
                }

                formattedMessage += key + ": " + value;

                if (i < logFormat.Parameters.Count - 1)
                {
                    formattedMessage += ", ";
                }
                else
                {
                    formattedMessage += ")";
                }
            }

            lock (_traceLogLock)
            {
                _traceLogs.Add(formattedMessage);
            }
        }

        public bool IsConnected()
        {
            if (_dischargerClient == null)
            {
                return false;
            }

            return _dischargerClient.IsConnected();
        }

        private void ChangeDischargerState(EDischargerState dischargerState)
        {
            if (_dischargerState != dischargerState)
            {
                LogArgument logArgument = new LogArgument("Enter " + dischargerState + " State.");
                logArgument.Parameters["Voltage"] = _dischargerData.ReceiveBatteryVoltage.ToString("F1");
                logArgument.Parameters["Current"] = _dischargerData.ReceiveDischargeCurrent.ToString("F1");
                logArgument.Parameters["ErrorCode"] = _dischargerData.ErrorCode;
                logArgument.Parameters["ReturnCode"] = _dischargerData.ReturnCode;
                logArgument.Parameters["ChannelStatus"] = _dischargerData.ChannelStatus;
                AddTraceLog(logArgument);
            }

            _dischargerState = dischargerState;
        }

        private bool IsParameterValid(EthernetClientDischargerStart parameters)
        {
            if (parameters == null)
            {
                return false;
            }
            if (parameters.DischargerName == string.Empty ||
                parameters.DischargerChannel == short.MaxValue ||
                parameters.IpAddress == IPAddress.None ||
                parameters.EthernetPort == int.MaxValue ||
                parameters.TimeOutMs == int.MaxValue ||
                parameters.SafetyVoltageMax == double.MaxValue ||
                parameters.SafetyVoltageMin == double.MaxValue ||
                parameters.SafetyCurrentMax == double.MaxValue ||
                parameters.SafetyCurrentMin == double.MaxValue)
            {
                return false;
            }

            return true;
        }

        public bool Restart()
        {
            Stop();

            return Start(_parameters);
        }

        public bool Start(EthernetClientDischargerStart parameters)
        {
            _parameters = parameters;

            ChangeDischargerState(EDischargerState.Connecting);

            if (!IsParameterValid(_parameters))
            {
                ChangeDischargerState(EDischargerState.Disconnected);
                return false;
            }

            if (!_serialNumbers.ContainsKey(_parameters.IpAddress.ToString()))
            {
                _serialNumbers[_parameters.IpAddress.ToString()] = 0;
            }

            if (!_serialNumberDataLock.ContainsKey(_parameters.IpAddress.ToString()))
            {
                _serialNumberDataLock[_parameters.IpAddress.ToString()] = new object();
            }

            EthernetClientStart clientStart = new EthernetClientStart();
            clientStart.DeviceName = _parameters.DischargerName;
            clientStart.IpAddress = _parameters.IpAddress;
            clientStart.EthernetPort = _parameters.EthernetPort;
            clientStart.TimeOutMs = _parameters.TimeOutMs;
            clientStart.WriteFunction = WriteData;
            clientStart.ReadFunction = ReadData;
            clientStart.ParseFunction = ParseData;
            _dischargerClient = new EthernetClient();

            var result = _dischargerClient.Connect(clientStart);
            if (result != EthernetClientStatus.OK)
            {
                ChangeDischargerState(EDischargerState.Disconnected);
                return false;
            }

            /// 이전 에러 상태 초기화
            SendCommand_ClearAlarm();

            /// Safety Condition 설정
            bool safetyConditionResult = SendCommand_SetSafetyCondition(
                _parameters.SafetyVoltageMax, _parameters.SafetyCurrentMin,
                _parameters.SafetyCurrentMax, _parameters.SafetyCurrentMin);
            if (safetyConditionResult == true)
            {
                _dischargerData.SafetyCurrentMin = _parameters.SafetyCurrentMin - 2;
                _dischargerData.SafetyCurrentMax = _parameters.SafetyCurrentMax + 2;
                _dischargerData.SafetyVoltageMin = _parameters.SafetyVoltageMin - 2;
                _dischargerData.SafetyVoltageMax = _parameters.SafetyVoltageMax + 2;
            }

            ReadInfoTimer?.Stop();
            ReadInfoTimer = null;
            ReadInfoTimer = new System.Timers.Timer();
            ReadInfoTimer.Interval = 1000;
            ReadInfoTimer.Elapsed += OneSecondTimer_Elapsed;
            ReadInfoTimer.Start();

            ChangeDischargerState(EDischargerState.Ready);

            return true;
        }

        public void Stop()
        {
            _dischargerClient?.Disconnect();
            _dischargerClient = null;

            ReadInfoTimer?.Stop();
            ReadInfoTimer = null;

            ChangeDischargerState(EDischargerState.Disconnected);
        }

        public DischargerDatas GetDatas()
        {
            /// Deep Copy
            DischargerDatas temp = new DischargerDatas();

            /// receive data
            temp.ErrorCode = _dischargerData.ErrorCode;
            temp.ChannelStatus = _dischargerData.ChannelStatus;
            temp.ReceiveBatteryVoltage = double.Parse(_dischargerData.ReceiveBatteryVoltage.ToString("F1"));
            temp.ReceiveDischargeCurrent = double.Parse((_dischargerData.ReceiveDischargeCurrent).ToString("F1"));

            /// safety
            temp.SafetyCurrentMin = _dischargerData.SafetyCurrentMin;
            temp.SafetyCurrentMax = _dischargerData.SafetyCurrentMax;
            temp.SafetyVoltageMin = _dischargerData.SafetyVoltageMin;
            temp.SafetyVoltageMax = _dischargerData.SafetyVoltageMax;

            /// start time
            temp.DischargingStartTime = _dischargerData.DischargingStartTime;

            return temp;
        }

        public EDischargerState GetState()
        {
            return _dischargerState;
        }

        public List<string> GetTraceLogs()
        {
            lock (_traceLogLock)
            {
                List<string> temp = _traceLogs.ConvertAll(x => x);
                _traceLogs.Clear();

                return temp;
            }
        }

        private byte GetPacketSerialNumber()
        {
            lock (_serialNumberDataLock[_parameters.IpAddress.ToString()])
            {
                return _serialNumbers[_parameters.IpAddress.ToString()]++;
            }
        }

        private void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!IsConnected())
            {
                ChangeDischargerState(EDischargerState.Disconnected);
                return;
            }

            if (_dischargerState == EDischargerState.SafetyOutOfRange ||
                _dischargerState == EDischargerState.ReturnCodeError ||
                _dischargerState == EDischargerState.ChStatusError ||
                _dischargerState == EDischargerState.DeviceError)
            {
                SendCommand_StopDischarge();
                ReadInfoTimer?.Stop();
                ReadInfoTimer = null;
                return;
            }

            SendCommand_RequestChannelInfo();

            if (_dischargerState == EDischargerState.Discharging)
            {
                if (_dischargerData.ReceiveBatteryVoltage < 1 && _dischargerData.ReceiveDischargeCurrent < 0.1)
                {
                    SendCommand_StopDischarge();
                }
            }
        }

        public EDischargerClientError SendCommand_StartDischarge(EWorkMode workMode, double setValue, double limitingValue)
        {
            lock (_packetLock)
            {
                LogArgument logArgument = new LogArgument("Start Discharge.");
                logArgument.Parameters["Current"] = -limitingValue;

                if (_dischargerState == EDischargerState.Discharging ||
                    _dischargerState == EDischargerState.Ready ||
                    _dischargerState == EDischargerState.Pause)
                {
                    byte[] writeBuffer = CreateStartDischargeCommand(
                        _parameters.DischargerChannel, workMode, setValue, limitingValue);

                    bool result = _dischargerClient.ProcessPacket(writeBuffer);
                    if (result != true)
                    {
                        logArgument.Parameters["Result"] = EDischargerClientError.FailProcessPacket;
                        AddTraceLog(logArgument);
                        return EDischargerClientError.FailProcessPacket;
                    }

                    if (_dischargerState == EDischargerState.Ready)
                    {
                        _dischargerData.DischargingStartTime = DateTime.Now;
                    }
                }
                else
                {
                    logArgument.Parameters["Discharger State"] = _dischargerState;
                    logArgument.Parameters["Result"] = EDischargerClientError.InvalidDischargerState;
                    AddTraceLog(logArgument);
                    return EDischargerClientError.InvalidDischargerState;
                }

                logArgument.Parameters["Result"] = EDischargerClientError.Ok;
                AddTraceLog(logArgument);

                return EDischargerClientError.Ok;
            }
        }

        public bool SendCommand_SetSafetyCondition(double voltageMax, double voltageMin, double currentMax, double currentMin)
        {
            lock (_packetLock)
            {
                LogArgument logArgument = new LogArgument("Set Safety Condition.");
                logArgument.Parameters["VoltMax"] = voltageMax;
                logArgument.Parameters["VoltMin"] = voltageMin;
                logArgument.Parameters["CurrentMax"] = currentMax;
                logArgument.Parameters["CurrentMin"] = currentMin;

                byte[] writeBuffer = CreateSetSafetyConditionCommand(
                    _parameters.DischargerChannel,
                    voltageMax, voltageMin, currentMax, currentMin);

                bool result = _dischargerClient.ProcessPacket(writeBuffer);
                if (result != true)
                {
                    logArgument.Parameters["Result"] = "fail";
                    AddTraceLog(logArgument);
                    return false;
                }

                logArgument.Parameters["Result"] = "success";
                AddTraceLog(logArgument);

                return true;
            }
        }

        public EDischargerClientError SendCommand_StopDischarge()
        {
            lock (_packetLock)
            {
                LogArgument logArgument = new LogArgument("Stop Discharge."); ;

                if (_dischargerState == EDischargerState.Discharging ||
                    _dischargerState == EDischargerState.Pause ||
                    _dischargerState == EDischargerState.SafetyOutOfRange ||
                    _dischargerState == EDischargerState.ReturnCodeError ||
                    _dischargerState == EDischargerState.ChStatusError ||
                    _dischargerState == EDischargerState.DeviceError)
                {
                    byte[] writeBuffer = CreateStopDischargeCommand(_parameters.DischargerChannel);

                    bool result = _dischargerClient.ProcessPacket(writeBuffer);
                    if (result != true)
                    {
                        logArgument.Parameters["Result"] = EDischargerClientError.FailProcessPacket;
                        AddTraceLog(logArgument);

                        return EDischargerClientError.FailProcessPacket;
                    }

                    if (_dischargerState == EDischargerState.Pause)
                    {
                        ChangeDischargerState(EDischargerState.Ready);
                    }
                }
                else
                {
                    logArgument.Parameters["Result"] = EDischargerClientError.InvalidDischargerState;
                    AddTraceLog(logArgument);

                    return EDischargerClientError.InvalidDischargerState;
                }

                logArgument.Parameters["Result"] = EDischargerClientError.Ok;
                AddTraceLog(logArgument);

                return EDischargerClientError.Ok;
            }
        }

        public EDischargerClientError SendCommand_PauseDischarge()
        {
            lock (_packetLock)
            {
                LogArgument logArgument = new LogArgument("Pause Discharge.");

                if (_dischargerState == EDischargerState.Discharging)
                {
                    byte[] writeBuffer = CreateStopDischargeCommand(_parameters.DischargerChannel);

                    bool result = _dischargerClient.ProcessPacket(writeBuffer);
                    if (result != true)
                    {
                        logArgument.Parameters["Result"] = EDischargerClientError.FailProcessPacket;
                        AddTraceLog(logArgument);

                        return EDischargerClientError.FailProcessPacket;
                    }

                    ChangeDischargerState(EDischargerState.Pause);
                }
                else
                {
                    logArgument.Parameters["Result"] = EDischargerClientError.InvalidDischargerState;
                    AddTraceLog(logArgument);

                    return EDischargerClientError.InvalidDischargerState;
                }

                logArgument.Parameters["Result"] = EDischargerClientError.Ok;
                AddTraceLog(logArgument);

                return EDischargerClientError.Ok;
            }
        }

        public bool SendCommand_ClearAlarm()
        {
            lock (_packetLock)
            {
                LogArgument logArgument = new LogArgument("Clear Alarm.");

                byte[] writeBuffer = CreateClearAlarmCommand(_parameters.DischargerChannel);

                bool result = _dischargerClient.ProcessPacket(writeBuffer);
                if (result != true)
                {
                    logArgument.Parameters["Result"] = "success";
                    AddTraceLog(logArgument);

                    return false;
                }

                logArgument.Parameters["Result"] = "fail";
                AddTraceLog(logArgument);

                return true;
            }
        }

        public bool SendCommand_RequestChannelInfo()
        {
            lock (_packetLock)
            {
                byte[] writeBuffer = CreateChannelInfoCommand(_parameters.DischargerChannel);

                bool result = _dischargerClient.ProcessPacket(writeBuffer);
                if (result != true)
                {
                    return false;
                }

                return true;
            }
        }

        private bool ReadData(int handle, out byte[] readBuffer)
        {
            EthernetClientStatus result = _dischargerClient.Read(handle, out readBuffer);
            if (result != EthernetClientStatus.OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                return false;
            }

            return true;
        }

        private bool WriteData(int handle, byte[] writeBuffer)
        {
            if (writeBuffer == null || writeBuffer.Length == 0)
            {
                return true;
            }

            EthernetClientStatus result = _dischargerClient.Write(handle, writeBuffer);
            if (result != EthernetClientStatus.OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                return false;
            }

            return true;
        }

        private bool ParseData(byte[] readBuffer)
        {
            try
            {
                if (readBuffer == null || readBuffer.Length == 0)
                {
                    return true;
                }

                /// 커맨드 코드 가져오기
                byte[] dataByteArray = readBuffer.ExtractSubArray(DCCPacketConstant.PACKET_HEADER_SIZE, 6);
                SetSafetyCondition.Reply reply = dataByteArray.FromByteArrayToPacket<SetSafetyCondition.Reply>();

                if (reply.CommandCode == ECommandCode.RequestCommand)
                {
                    /// 리턴 코드 검사
                    if (reply.ReturnCode != EReturnCode.Success)
                    {
                        ChangeDischargerState(EDischargerState.ReturnCodeError);
                    }
                }
                else if (reply.CommandCode == ECommandCode.ChannelInfo)
                {
                    short length = (short)Marshal.SizeOf(typeof(ChannelInfo.Reply));
                    dataByteArray = readBuffer.ExtractSubArray(DCCPacketConstant.PACKET_HEADER_SIZE, length);
                    ChannelInfo.Reply channelInfo = dataByteArray.FromByteArrayToPacket<Discharger.ChannelInfo.Reply>();

                    /// 채널 상태 업데이트
                    _dischargerData.ErrorCode = channelInfo.ErrorCode;
                    _dischargerData.ReturnCode = channelInfo.ReturnCode;
                    _dischargerData.ChannelStatus = channelInfo.ChannelStatus;

                    /// 전압, 전류 값 업데이트
                    _dischargerData.ReceiveBatteryVoltage = channelInfo.BatteryVoltage;
                    _dischargerData.ReceiveDischargeCurrent = -channelInfo.BatteryCurrent;

                    if (channelInfo.BatteryVoltage < _dischargerData.SafetyVoltageMin ||
                        channelInfo.BatteryVoltage > _dischargerData.SafetyVoltageMax ||
                        (-channelInfo.BatteryCurrent) < _dischargerData.SafetyCurrentMin ||
                        (-channelInfo.BatteryCurrent) > _dischargerData.SafetyCurrentMax)
                    {
                        ChangeDischargerState(EDischargerState.SafetyOutOfRange);
                    }
                    else if (channelInfo.ErrorCode != 0) /// 에러코드 검사
                    {
                        ChangeDischargerState(EDischargerState.DeviceError);
                    }
                    else if (channelInfo.ReturnCode != EReturnCode.Success) /// 리턴 코드 검사
                    {
                        ChangeDischargerState(EDischargerState.ReturnCodeError);
                    }
                    else if (channelInfo.ChannelStatus == EChannelStatus.Error) /// 채널 상태 검사
                    {
                        ChangeDischargerState(EDischargerState.ChStatusError);
                    }
                    else if (channelInfo.ChannelStatus == EChannelStatus.Standby0 || channelInfo.ChannelStatus == EChannelStatus.Standby5)
                    {
                        if (_dischargerState != EDischargerState.Pause)
                        {
                            ChangeDischargerState(EDischargerState.Ready);
                        }
                    }
                    else
                    {
                        ChangeDischargerState(EDischargerState.Discharging);
                    }
                }

                if (_dischargerState == EDischargerState.Discharging ||
                    _dischargerState == EDischargerState.Pause)
                {
                    LogArgument logArgument = new LogArgument("Discharger Info.");
                    logArgument.Parameters["Voltage"] = _dischargerData.ReceiveBatteryVoltage.ToString("F1");
                    logArgument.Parameters["Current"] = _dischargerData.ReceiveDischargeCurrent.ToString("F1");
                    logArgument.Parameters["ErrorCode"] = _dischargerData.ErrorCode;
                    logArgument.Parameters["ReturnCode"] = _dischargerData.ReturnCode;
                    logArgument.Parameters["ChannelStatus"] = _dischargerData.ChannelStatus;
                    AddTraceLog(logArgument);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                LogArgument logArgument = new LogArgument("Fail to Parse Discharger Packet.");
                logArgument.Parameters["RawData"] = readBuffer.GetRawDataHexString();
                AddTraceLog(logArgument);

                return false;
            }

            return true;
        }

        private byte[] CreateChannelInfoCommand(short channel)
        {
            int length = Marshal.SizeOf(typeof(ChannelInfo.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            ChannelInfo.Request data = new ChannelInfo.Request();
            data.ChannelNumber = channel;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private byte[] CreateSetSafetyConditionCommand(short channel, double voltageMax, double voltageMin, double currentMax, double currentMin)
        {
            int length = Marshal.SizeOf(typeof(SetSafetyCondition.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            SetSafetyCondition.Request data = new SetSafetyCondition.Request();
            data.ChannelNumber = channel;
            data.VoltageUpperLimitValue = voltageMax;
            data.VoltageLowerLimitValue = voltageMin;
            data.CurrentUpperLimitValue = currentMax;
            data.CurrentLowerLimitValue = currentMin;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private byte[] CreateStartDischargeCommand(short channel, EWorkMode workMode, double setValue, double limitingValue)
        {
            int length = Marshal.SizeOf(typeof(StartDischarge.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            StartDischarge.Request data = new StartDischarge.Request();
            data.ChannelNumber = channel;
            data.WorkMode = (double)workMode;
            data.SetValue = setValue;
            data.LimitingValue = limitingValue;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private byte[] CreateStopDischargeCommand(short channel)
        {
            int length = Marshal.SizeOf(typeof(StopDischarge.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            StopDischarge.Request data = new StopDischarge.Request();
            data.ChannelNumber = channel;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private byte[] CreateClearAlarmCommand(short channel)
        {
            int length = Marshal.SizeOf(typeof(ClearAlarm.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            ClearAlarm.Request data = new ClearAlarm.Request();
            data.ChannelNumber = channel;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }
    }
}
