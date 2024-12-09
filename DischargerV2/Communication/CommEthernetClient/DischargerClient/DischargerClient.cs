using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
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

using Ethernet.Client.Common;
using Repository.Common;
using Utility.Common;
using Ethernet.Client.Basic;
using Repository.Structures;

namespace Ethernet.Client.Discharger
{
    public enum EDischargerState
    {
        Disconnect,
        Connecting,
        Connected,
        Discharging,
        Pause,
        Stop,
        Finish,
        Alert,
        Error,
    }

    public class EthernetClientDischargerStart
    {
        public string DischargerName;
        public short DischargerChannel;
        public IPAddress IpAddress;
        public int EthernetPort;
        public int TimeOutMs;
    }

    public class ReceiveDischargerData
    {
        public EErrorCode ErrorCode = EErrorCode.Online;
        public EChannelStatus ChannelStatus = EChannelStatus.Standby0;
        public double ReceiveBatteryVoltage = 0;
        public double ReceiveDischargeCurrent = 0;
    }

    public class EthernetClientDischarger
    {
        /// <summary>
        /// key: IP Address
        /// </summary>
        private static Dictionary<IPAddress, byte> SerialNumbers = new Dictionary<IPAddress, byte>();
        /// <summary>
        /// key: IP Address
        /// </summary>
        private static Dictionary<IPAddress, object> SerialNumberDataLock = new Dictionary<IPAddress, object>();

        private EthernetClientDischargerStart Parameters = null;

        private EthernetClient DischargerClient = null;

        private System.Timers.Timer ReadInfoTimer = null;

        /// <summary>
        /// 수신 받은 데이터
        /// </summary>
        private ReceiveDischargerData ReceiveData = new ReceiveDischargerData();
        /// <summary>
        /// 방전기 상태
        /// </summary>
        private EDischargerState DischargerState = EDischargerState.Disconnect;

        public EthernetClientDischarger(EthernetClientDischargerStart clientStart)
        {
            Parameters = clientStart;

            if (!SerialNumbers.ContainsKey(Parameters.IpAddress))
            {
                SerialNumbers[Parameters.IpAddress] = 0;
            }

            if (!SerialNumberDataLock.ContainsKey(Parameters.IpAddress))
            {
                SerialNumberDataLock[Parameters.IpAddress] = new object();
            }
        }

        public bool Start()
        {
            EthernetClientStart clientStart = new EthernetClientStart();
            clientStart.DeviceName = Parameters.DischargerName;
            clientStart.IpAddress = Parameters.IpAddress;
            clientStart.EthernetPort = Parameters.EthernetPort;
            clientStart.TimeOutMs = Parameters.TimeOutMs;
            clientStart.WriteFunction = WriteData;
            clientStart.ReadFunction = ReadData;
            clientStart.ParseFunction = ParseData;
            DischargerClient = new EthernetClient();

            var result = DischargerClient.Connect(clientStart);
            if (result != ENClientStatus.OK)
            {
                return false;
            }

            /// 이전 에러 상태 초기화
            SendCommand_ClearAlarm();

            ReadInfoTimer = new System.Timers.Timer();
            ReadInfoTimer.Interval = 1000;
            ReadInfoTimer.Elapsed += ReadInfoTimer_Elapsed;
            ReadInfoTimer.Start();

            return true;
        }

        public void Dispose()
        {
            DischargerClient?.Disconnect();

            ReadInfoTimer?.Stop();
            ReadInfoTimer = null;
        }

        public ReceiveDischargerData GetDatas()
        {
            /// Deep Copy
            ReceiveDischargerData temp = new ReceiveDischargerData();
            temp.ErrorCode = ReceiveData.ErrorCode;
            temp.ChannelStatus = ReceiveData.ChannelStatus;
            temp.ReceiveBatteryVoltage = ReceiveData.ReceiveBatteryVoltage;
            temp.ReceiveDischargeCurrent = ReceiveData.ReceiveDischargeCurrent;

            return temp;
        }

        public EDischargerState GetState()
        {
            return DischargerState;
        }

        private byte GetPacketSerialNumber()
        {
            lock (SerialNumberDataLock[Parameters.IpAddress])
            {
                return SerialNumbers[Parameters.IpAddress]++;
            }
        }

        private void ReadInfoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool result = SendCommand_RequestChannelInfo();
            if (result == false)
            {
                ReadInfoTimer?.Stop();
                ReadInfoTimer = null;
            }
        }

        public bool SendCommand_StartDischarge(EWorkMode workMode, double setValue, double limitingValue)
        {
            byte[] writeBuffer = CreateStartDischargeCommand(
                Parameters.DischargerChannel, workMode, setValue, limitingValue);

            bool result = DischargerClient.ProcessPacket(writeBuffer);
            if (result != true)
            {
                return false;
            }

            return true;
        }

        public bool SendCommand_SetSafetyCondition(double voltageMax, double voltageMin, double currentMax, double currentMin)
        {
            byte[] writeBuffer = CreateSetSafetyConditionCommand(Parameters.DischargerChannel,
                voltageMax, voltageMin, currentMax, currentMin);

            bool result = DischargerClient.ProcessPacket(writeBuffer);
            if (result != true)
            {
                return false;
            }

            return true;
        }

        public bool SendCommand_StopDischarge()
        {
            byte[] writeBuffer = CreateStopDischargeCommand(Parameters.DischargerChannel);

            bool result = DischargerClient.ProcessPacket(writeBuffer);
            if (result != true)
            {
                return false;
            }

            return true;
        }

        public bool SendCommand_ClearAlarm()
        {
            byte[] writeBuffer = CreateClearAlarmCommand(Parameters.DischargerChannel);

            bool result = DischargerClient.ProcessPacket(writeBuffer);
            if (result != true)
            {
                return false;
            }

            return true;
        }

        public bool SendCommand_RequestChannelInfo()
        {
            byte[] writeBuffer = CreateChannelInfoCommand(Parameters.DischargerChannel);

            bool result = DischargerClient.ProcessPacket(writeBuffer);
            if (result != true)
            {
                return false;
            }

            return true;
        }

        private bool ReadData(int _handle, out byte[] _actualReadBuffer)
        {
            /// 데이터 읽기
            byte[] readBuffer = new byte[EthernetClientConstant.MAX_DATA_LENGTH];
            ENClientBasicStatus result = EthernetClientBasic.Read(_handle, readBuffer, 0, readBuffer.Length);
            if (result != ENClientBasicStatus.EN_ERROR_OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                _actualReadBuffer = new byte[0];

                return false;
            }

            /// 실제 데이터 가져오기
            byte[] dataByteArray = readBuffer.ExtractSubArray(DCCPacketConstant.PACKET_HEADER_SIZE, Marshal.SizeOf(typeof(SetSafetyCondition.Reply)));
            SetSafetyCondition.Reply reply = dataByteArray.FromByteArrayToPacket<SetSafetyCondition.Reply>();
            if (reply.CommandCode == ECommandCode.ChannelInfo)
            {
                short length = (short)(Marshal.SizeOf(typeof(Discharger.ChannelInfo.Reply)) + DCCPacketConstant.PACKET_HEADER_SIZE);
                _actualReadBuffer = readBuffer.ExtractSubArray(0, length);
            }
            else
            {
                short length = (short)(Marshal.SizeOf(typeof(SetSafetyCondition.Reply)) + DCCPacketConstant.PACKET_HEADER_SIZE);
                _actualReadBuffer = readBuffer.ExtractSubArray(0, length);
            }

            return true;
        }

        private bool WriteData(int _handle, byte[] _writeBuffer)
        {
            if (_writeBuffer == null || _writeBuffer.Length == 0)
            {
                return true;
            }

            ENClientBasicStatus result = EthernetClientBasic.Write(_handle, _writeBuffer, 0, _writeBuffer.Length);
            if (result != ENClientBasicStatus.EN_ERROR_OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                return false;
            }

            return true;
        }

        private bool ParseData(byte[] _readBuffer)
        {
            /// 커맨드 코드 가져오기
            byte[] dataByteArray = _readBuffer.ExtractSubArray(DCCPacketConstant.PACKET_HEADER_SIZE, 6);
            SetSafetyCondition.Reply reply = dataByteArray.FromByteArrayToPacket<SetSafetyCondition.Reply>();

            if (reply.CommandCode == ECommandCode.RequestCommand)
            {
                /// 리턴 코드 검사
                if (reply.ReturnCode != EReturnCode.Success)
                {
                    DischargerState = EDischargerState.Error;
                    return false;
                }
            }
            else if (reply.CommandCode == ECommandCode.ChannelInfo)
            {
                short length = (short)Marshal.SizeOf(typeof(ChannelInfo.Reply));
                dataByteArray = _readBuffer.ExtractSubArray(DCCPacketConstant.PACKET_HEADER_SIZE, length);
                ChannelInfo.Reply channelInfo = dataByteArray.FromByteArrayToPacket<Discharger.ChannelInfo.Reply>();

                /// 채널 상태 업데이트
                ReceiveData.ErrorCode = channelInfo.ErrorCode;
                ReceiveData.ChannelStatus = channelInfo.ChannelStatus;

                /// 전압, 전류 값 업데이트
                ReceiveData.ReceiveBatteryVoltage = channelInfo.BatteryVoltage;
                ReceiveData.ReceiveDischargeCurrent = channelInfo.BatteryCurrent;

                /// 리턴 코드 검사
                if (channelInfo.ReturnCode != EReturnCode.Success)
                {
                    DischargerState = EDischargerState.Error;
                    return false;
                }

                /// 에러코드 검사
                if (channelInfo.ErrorCode != EErrorCode.Online)
                {
                    DischargerState = EDischargerState.Error;
                    return false;
                }

                /// 채널 상태 검사
                if (channelInfo.ChannelStatus == EChannelStatus.Error)
                {
                    DischargerState = EDischargerState.Error;
                    return false;
                }
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
