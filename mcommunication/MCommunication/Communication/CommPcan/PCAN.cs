using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Peak.Can.Basic;
using TPCANHandle = System.UInt16;

namespace Peak.Can.Common
{
    public class PCANStartParameter
    {
        public uint DeviceId { get; set; } = uint.MaxValue;
        public TPCANBaudrate BaudRate { get; set; } = TPCANBaudrate.PCAN_BAUD_500K;
        public TPCANType HardwareType { get; private set; } = TPCANType.PCAN_TYPE_ISA_SJA;
        public uint IOPort { get; private set; } = Convert.ToUInt32("0100", 16);
        public TPCANHandle Interrupt { get; private set; } = Convert.ToUInt16("3");
    }

    public class PCANStartParameterFD
    {
        public ulong DeviceId { get; set; } = ulong.MaxValue;
        public PCANClockFrequencyParameter ClockFrequency { get; set; } = null;
        public PCANNominalBitrateParameter NominalBitrate { get; set; } = null;
        public PCANDataBitrateParameter DataBitrate { get; set; } = null;
    }

    public class CANMessage
    {
        public TPCANMsgFD Message;
        public DateTime DateTime;

        public CANMessage(TPCANMsgFD message, DateTime dateTime)
        {
            Message = message;
            DateTime = dateTime;
        }
    }

    public class CANDeviceInfo
    {
        public ulong DeviceId;
        public byte ChannelNumber;
        public bool IsFD;
    }

    public enum ECANErrorStatus
    {
        OK,

        FAIL_RESET,
        ALREADY_CONNECTED,
        INVALID_PARAMETER,
        INVALID_BITRATE_PARAMETER,
        NOT_EXIST_CAN_CHANNEL,
        FAIL_INITIALIZE,
        FAIL_RESET_EVENT,
        INVALID_RECEIVE_MESSAGE_STATE,
        FAIL_SET_ACCEPTANCE_FILTER,
        WRITE_ERROR_OCCUR,
        WRITE_ERROR_OCCUR_FD,
        READ_ERROR_OCCUR,
        READ_ERROR_OCCUR_FD,
    }

    public enum ECANState
    {
        Disconnect,

        ConnectedNormal,
        ConnectedFD,

        ReceiveMessageNormal,
        ReceiveMessageFD,
    }

    public class PCAN
    {
        private int ReceiveThreadNumber = int.MaxValue;

        private static PCANBitrateFD BitrateFD = new PCANBitrateFD();

        private TPCANHandle PcanHandle = TPCANHandle.MaxValue;

        private object DisconnectLock = new object();

        private List<List<CANMessage>> MessageLists = new List<List<CANMessage>>();
        private List<object> MessageLocks = new List<object>();

        private List<Thread> ReadThreads = new List<Thread>();

        private ECANState CanState = ECANState.Disconnect;

        private AutoResetEvent ReceiveAutoResetEvent = null;

        public PCAN()
        {
            GetChannelInfo(0, false, out PcanHandle);
        }

        public ECANErrorStatus Reset()
        {
            var result = PCANBasic.Reset(PcanHandle);
            if (result != TPCANStatus.PCAN_ERROR_OK)
            {
                return ECANErrorStatus.FAIL_RESET;
            }

            return ECANErrorStatus.OK;
        }

        public ECANErrorStatus Connect(PCANStartParameter parameters, out TPCANHandle handle)
        {
            handle = TPCANHandle.MaxValue;

            if (CanState != ECANState.Disconnect)
            {
                return ECANErrorStatus.ALREADY_CONNECTED;
            }

            if (!IsParameterValid(parameters))
            {
                return ECANErrorStatus.INVALID_PARAMETER;
            }

            ECANErrorStatus result = GetChannelInfo(parameters.DeviceId, false, out handle);
            if (result != ECANErrorStatus.OK)
            {
                return result;
            }

            TPCANStatus tpResult = PCANBasic.Initialize(handle,
                parameters.BaudRate, parameters.HardwareType,
                parameters.IOPort, parameters.Interrupt);
            if (tpResult != TPCANStatus.PCAN_ERROR_OK)
            {
                return ECANErrorStatus.FAIL_INITIALIZE;
            }

            PcanHandle = handle;

            CanState = ECANState.ConnectedNormal;

            return ECANErrorStatus.OK;
        }

        public ECANErrorStatus ConnectFD(PCANStartParameterFD parameters, out TPCANHandle handle)
        {
            handle = TPCANHandle.MaxValue;

            if (CanState != ECANState.Disconnect)
            {
                return ECANErrorStatus.ALREADY_CONNECTED;
            }

            if (!IsParameterValidFD(parameters))
            {
                return ECANErrorStatus.INVALID_PARAMETER;
            }

            string bitrateFD = BitrateFD.GetBitrateFDString(
                parameters.ClockFrequency.ToEnum(),
                parameters.NominalBitrate.ToEnum(),
                parameters.DataBitrate.ToEnum());
            if (bitrateFD == string.Empty)
            {
                return ECANErrorStatus.INVALID_BITRATE_PARAMETER;
            }

            ECANErrorStatus result = GetChannelInfo(parameters.DeviceId, true, out handle);
            if (result != ECANErrorStatus.OK)
            {
                return ECANErrorStatus.NOT_EXIST_CAN_CHANNEL;
            }

            TPCANStatus tpResult = PCANBasic.InitializeFD(handle, bitrateFD);
            if (tpResult != TPCANStatus.PCAN_ERROR_OK)
            {
                return ECANErrorStatus.FAIL_INITIALIZE;
            }

            PcanHandle = handle;

            CanState = ECANState.ConnectedFD;

            return ECANErrorStatus.OK;
        }

        public ECANErrorStatus ConnectFD(uint deviceId, EClockFrequency clockFrequency, BitrateSegment nominalBitrate, BitrateSegment dataBitrate, out TPCANHandle handle)
        {
            handle = TPCANHandle.MaxValue;

            if (CanState != ECANState.Disconnect)
            {
                return ECANErrorStatus.ALREADY_CONNECTED;
            }

            string bitrateFD = BitrateFD.GetBitrateFDString(clockFrequency, nominalBitrate, dataBitrate);
            if (bitrateFD == string.Empty)
            {
                return ECANErrorStatus.INVALID_BITRATE_PARAMETER;
            }

            ECANErrorStatus result = GetChannelInfo(deviceId, true, out handle);
            if (result != ECANErrorStatus.OK)
            {
                return ECANErrorStatus.NOT_EXIST_CAN_CHANNEL;
            }

            TPCANStatus tpResult = PCANBasic.InitializeFD(handle, bitrateFD);
            if (tpResult != TPCANStatus.PCAN_ERROR_OK)
            {
                return ECANErrorStatus.FAIL_INITIALIZE;
            }

            PcanHandle = handle;

            CanState = ECANState.ConnectedFD;

            return ECANErrorStatus.OK;
        }

        public void Disconnect()
        {
            lock (DisconnectLock)
            {
                if (CanState != ECANState.Disconnect)
                {
                    PCANBasic.Uninitialize(PcanHandle);
                }
                CanState = ECANState.Disconnect;

                if (ReceiveThreadNumber == int.MaxValue) return;
                if (ReadThreads.Count == 0) return;

                for (int i = 0; i < ReceiveThreadNumber; i++)
                {
                    ReadThreads[i]?.Abort();
                    ReadThreads[i] = null;
                }

                MessageLists.Clear();
                MessageLocks.Clear();
                ReadThreads.Clear();

                ReceiveAutoResetEvent?.Close();
                ReceiveAutoResetEvent = null;
            }
        }

        public ECANErrorStatus StartReceiveThread(int receiveThreadNumber)
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            ReceiveThreadNumber = receiveThreadNumber;
            ReceiveAutoResetEvent = new AutoResetEvent(true);

            MessageLists.Clear();
            MessageLocks.Clear();

            for (int i = 0; i < ReceiveThreadNumber; i++)
            {
                MessageLists.Add(new List<CANMessage>());
                MessageLocks.Add(new object());
            }

            // Receive 안하고있을 때 들어오는 값들이 쌓여있어 Reset 필요
            PCANBasic.Reset(PcanHandle);

            iBuffer = Convert.ToUInt32(ReceiveAutoResetEvent.SafeWaitHandle.DangerousGetHandle().ToInt32());
            // Sets the handle of the Receive-Event.
            //
            stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                return ECANErrorStatus.FAIL_RESET_EVENT;
            }

            if (CanState == ECANState.ConnectedNormal)
            {
                CanState = ECANState.ReceiveMessageNormal;

                ReadThreads.Clear();

                for (int i = 0; i < ReceiveThreadNumber; i++)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(ReadMessageThread));
                    thread.Start(i);
                    ReadThreads.Add(thread);
                }
            }
            else if (CanState == ECANState.ConnectedFD)
            {
                CanState = ECANState.ReceiveMessageFD;

                ReadThreads.Clear();

                for (int i = 0; i < ReceiveThreadNumber; i++)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(ReadMessageThreadFD));
                    thread.Start(i);
                    ReadThreads.Add(thread);
                }
            }
            else
            {
                return ECANErrorStatus.INVALID_RECEIVE_MESSAGE_STATE;
            }

            return ECANErrorStatus.OK;
        }

        public bool StopReceiveThread()
        {
            try
            {
                for (int i = 0; i < ReceiveThreadNumber; i++)
                {
                    ReadThreads[i]?.Abort();
                    ReadThreads[i] = null;
                }

                ReceiveAutoResetEvent?.Close();
                ReceiveAutoResetEvent = null;

                if (CanState == ECANState.ReceiveMessageNormal)
                {
                    CanState = ECANState.ConnectedNormal;
                }
                else if (CanState == ECANState.ReceiveMessageFD)
                {
                    CanState = ECANState.ConnectedFD;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<CANDeviceInfo> GetAvailableDeviceIdList()
        {
            List<CANDeviceInfo> deviceInfoList = new List<CANDeviceInfo>();

            try
            {
                TPCANStatus stsResult = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS_COUNT, out uint iChannelsCount, sizeof(uint));
                if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                {
                    TPCANChannelInformation[] info = new TPCANChannelInformation[iChannelsCount];

                    stsResult = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS, info);
                    if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    {
                        foreach (TPCANChannelInformation channel in info)
                        {
                            if ((channel.channel_condition & PCANBasic.PCAN_CHANNEL_AVAILABLE) == PCANBasic.PCAN_CHANNEL_AVAILABLE)
                            {
                                CANDeviceInfo canDeviceInfo = new CANDeviceInfo();
                                canDeviceInfo.DeviceId = channel.device_id;
                                canDeviceInfo.ChannelNumber = (byte)(channel.controller_number + 1);

                                Debug.WriteLine(channel.controller_number);

                                if ((channel.device_features & PCANBasic.FEATURE_FD_CAPABLE) == PCANBasic.FEATURE_FD_CAPABLE)
                                {
                                    canDeviceInfo.IsFD = true;
                                }
                                else
                                {
                                    canDeviceInfo.IsFD = false;
                                }

                                deviceInfoList.Add(canDeviceInfo);
                            }
                        }
                    }
                }
            }
            catch (DllNotFoundException)
            {
                MessageBox.Show("Unable to find the library: PCANBasic.dll !", "Error!");
                Environment.Exit(-1);
            }

            return deviceInfoList;
        }

        public List<CANMessage> GetReceiveMessages()
        {
            lock (DisconnectLock)
            {
                if (MessageLocks.Count == 0)
                {
                    return new List<CANMessage>();
                }

                List<CANMessage> totalMessageList = new List<CANMessage>();

                for (int i = 0; i < ReceiveThreadNumber; i++)
                {
                    lock (MessageLocks[i])
                    {
                        List<CANMessage> temp = MessageLists[i].ConvertAll(x => x).OrderBy(x => x.DateTime).ToList(); // deep copy
                        totalMessageList.AddRange(temp);

                        MessageLists[i].Clear();
                    }
                }

                return totalMessageList;
            }
        }

        public ECANState GetState()
        {
            return CanState;
        }

        public ECANErrorStatus WriteMessage(uint canId, ref byte[] data, bool isExtended = true, bool isRemote = false)
        {
            TPCANMsg CANMsg = new TPCANMsg();
            CANMsg.DATA = new byte[8];
            CANMsg.ID = canId;
            CANMsg.LEN = Convert.ToByte(data.Length);
            CANMsg.MSGTYPE = (isExtended) ? TPCANMessageType.PCAN_MESSAGE_EXTENDED : TPCANMessageType.PCAN_MESSAGE_STANDARD;

            /// If a remote frame will be sent, the data bytes are not important.
            if (isRemote)
                CANMsg.MSGTYPE |= TPCANMessageType.PCAN_MESSAGE_RTR;
            else
            {
                /// We get so much data as the Len of the message
                for (int i = 0; i < GetLengthFromDLC(CANMsg.LEN, true); i++)
                {
                    CANMsg.DATA[i] = data[i];
                }
            }

            /// The message is sent to the configured hardware
            var result = PCANBasic.Write(PcanHandle, ref CANMsg);
            if (result != TPCANStatus.PCAN_ERROR_OK)
            {
                return ECANErrorStatus.WRITE_ERROR_OCCUR;
            }

            return ECANErrorStatus.OK;
        }

        public ECANErrorStatus WriteMessageFD(uint canId, ref byte[] data, bool isExtended = true, bool isRemote = false, bool isFD = true, bool isBRS = false)
        {
            int iLength;
            TPCANMsgFD CANMsg = new TPCANMsgFD();
            CANMsg.DATA = data;
            CANMsg.ID = canId;
            CANMsg.DLC = GetDLCFromLength(data.Length);
            CANMsg.MSGTYPE = (isExtended) ? TPCANMessageType.PCAN_MESSAGE_EXTENDED : TPCANMessageType.PCAN_MESSAGE_STANDARD;
            CANMsg.MSGTYPE |= (isFD) ? TPCANMessageType.PCAN_MESSAGE_FD : TPCANMessageType.PCAN_MESSAGE_STANDARD;
            CANMsg.MSGTYPE |= (isBRS) ? TPCANMessageType.PCAN_MESSAGE_BRS : TPCANMessageType.PCAN_MESSAGE_STANDARD;

            /// If a remote frame will be sent, the data bytes are not important.
            if (isRemote)
                CANMsg.MSGTYPE |= TPCANMessageType.PCAN_MESSAGE_RTR;
            else
            {
                /// We get so much data as the Len of the message
                iLength = GetLengthFromDLC(CANMsg.DLC, (CANMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0);
                for (int i = 0; i < iLength; i++)
                {
                    CANMsg.DATA[i] = data[i];
                }
            }

            /// The message is sent to the configured hardware
            var result = PCANBasic.WriteFD(PcanHandle, ref CANMsg);
            if (result != TPCANStatus.PCAN_ERROR_OK)
            {
                return ECANErrorStatus.WRITE_ERROR_OCCUR_FD;
            }

            return ECANErrorStatus.OK;
        }

        public ECANErrorStatus ReadMessage(int threadIndex)
        {
            TPCANMsg CANMsg;
            TPCANStatus stsResult;

            stsResult = PCANBasic.Read(PcanHandle, out CANMsg);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                ProcessMessage(CANMsg, DateTime.Now, threadIndex);

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                return ECANErrorStatus.READ_ERROR_OCCUR;
            }

            return ECANErrorStatus.OK;
        }

        public ECANErrorStatus ReadMessageFD(int threadIndex)
        {
            TPCANMsgFD CANMsg;
            TPCANStatus stsResult;

            stsResult = PCANBasic.ReadFD(PcanHandle, out CANMsg);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                ProcessMessageFD(CANMsg, DateTime.Now, threadIndex);

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                return ECANErrorStatus.READ_ERROR_OCCUR_FD;
            }

            return ECANErrorStatus.OK;
        }

        public ECANErrorStatus SetAcceptanceFilter(uint acceptanceCode, uint acceptanceMask, bool isExtended)
        {
            ulong acceptanceFilter = ((ulong)acceptanceCode << 32) + (ulong)acceptanceMask;

            ///
            /// acceptanceFilter description.
            ///    code   |  mask
            /// 0xFFFFFFFF_FFFFFFFF
            /// 
            /// If mask bit is 1, the meaning is don't care.
            /// 
            if (isExtended)
            {
                var result = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_29BIT, ref acceptanceFilter, sizeof(ulong));
                if (result != TPCANStatus.PCAN_ERROR_OK)
                {
                    return ECANErrorStatus.FAIL_SET_ACCEPTANCE_FILTER;
                }
            }
            else
            {
                var result = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_11BIT, ref acceptanceFilter, sizeof(ulong));
                if (result != TPCANStatus.PCAN_ERROR_OK)
                {
                    return ECANErrorStatus.FAIL_SET_ACCEPTANCE_FILTER;
                }
            }

            return ECANErrorStatus.OK;
        }

        private void ReadMessageThread(object threadIndex)
        {
            while (CanState != ECANState.Disconnect)
            {
                ReadMessage((int)threadIndex);
            }
        }

        private void ReadMessageThreadFD(object threadIndex)
        {
            while (CanState != ECANState.Disconnect)
            {
                ReadMessageFD((int)threadIndex);
            }
        }

        private bool IsParameterValid(PCANStartParameter parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            if (parameters.DeviceId == uint.MaxValue)
            {
                return false;
            }

            return true;
        }

        private bool IsParameterValidFD(PCANStartParameterFD parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            if (parameters.DeviceId == ulong.MaxValue ||
                parameters.ClockFrequency == null ||
                parameters.NominalBitrate == null ||
                parameters.DataBitrate == null)
            {
                return false;
            }

            return true;
        }

        private ECANErrorStatus GetChannelInfo(ulong deviceId, bool isFD, out TPCANHandle handle)
        {
            try
            {
                TPCANStatus stsResult = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS_COUNT, out uint iChannelsCount, sizeof(uint));
                if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                {
                    TPCANChannelInformation[] info = new TPCANChannelInformation[iChannelsCount];

                    stsResult = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS, info);
                    if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    {
                        foreach (TPCANChannelInformation channel in info)
                        {
                            if ((channel.channel_condition & PCANBasic.PCAN_CHANNEL_AVAILABLE) == PCANBasic.PCAN_CHANNEL_AVAILABLE)
                            {
                                if (channel.device_id == deviceId)
                                {
                                    if (isFD) // FD
                                    {
                                        if ((channel.device_features & PCANBasic.FEATURE_FD_CAPABLE) == PCANBasic.FEATURE_FD_CAPABLE)
                                        {
                                            handle = channel.channel_handle;
                                            return ECANErrorStatus.OK;
                                        }
                                    }
                                    else // Normal
                                    {
                                        handle = channel.channel_handle;
                                        return ECANErrorStatus.OK;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (DllNotFoundException)
            {
                MessageBox.Show("Unable to find the library: PCANBasic.dll !", "Error!");
                Environment.Exit(-1);
            }

            handle = TPCANHandle.MaxValue;

            return ECANErrorStatus.NOT_EXIST_CAN_CHANNEL;
        }

        private void ProcessMessage(TPCANMsg theMsg, DateTime dateTime, int threadIndex)
        {
            TPCANMsgFD newMsg;

            newMsg = new TPCANMsgFD();
            newMsg.DATA = new byte[64];
            newMsg.ID = theMsg.ID;
            newMsg.DLC = theMsg.LEN;
            for (int i = 0; i < ((theMsg.LEN > 8) ? 8 : theMsg.LEN); i++)
                newMsg.DATA[i] = theMsg.DATA[i];
            newMsg.MSGTYPE = theMsg.MSGTYPE;

            ProcessMessageFD(newMsg, dateTime, threadIndex);
        }

        private void ProcessMessageFD(TPCANMsgFD theMsg, DateTime dateTime, int threadIndex)
        {
            lock (MessageLocks[threadIndex])
            {
                MessageLists[threadIndex].Add(new CANMessage(theMsg, dateTime));
            }
        }

        private byte GetDLCFromLength(int dataLength)
        {
            if (dataLength <= 8)
                return (byte)dataLength;
            else if (8 < dataLength && dataLength <= 12)
                return 9;
            else if (12 < dataLength && dataLength <= 16)
                return 10;
            else if (16 < dataLength && dataLength <= 20)
                return 11;
            else if (20 < dataLength && dataLength <= 24)
                return 12;
            else if (24 < dataLength && dataLength <= 32)
                return 13;
            else if (32 < dataLength && dataLength <= 48)
                return 14;
            else //(48 < dataLength && dataLength <= 64)
                return 15;
        }

        private int GetLengthFromDLC(int dlc, bool isSTD)
        {
            if (dlc <= 8)
                return dlc;

            if (isSTD)
                return 8;

            switch (dlc)
            {
                case 9: return 12;
                case 10: return 16;
                case 11: return 20;
                case 12: return 24;
                case 13: return 32;
                case 14: return 48;
                case 15: return 64;
                default: return dlc;
            }
        }

        private class PCANBitrateFD
        {
            private List<BitrateMapping> BitrateFDList = new List<BitrateMapping>();

            public PCANBitrateFD()
            {
                var parameterManager = new PCANParameterManagerFD();

                BitrateFDList.Clear();

                List<ENominalBitrate> nominalBitrateList;
                List<EDataBitrate> dataBitrateList;

                /// 80 Mhz
                nominalBitrateList = parameterManager.GetNominalBitrateEnumList(EClockFrequency.Mhz_80);
                dataBitrateList = parameterManager.GetDataBitrateEnumList(EClockFrequency.Mhz_80);
                foreach (var nominalBitrate in nominalBitrateList)
                {
                    foreach (var dataBitrate in dataBitrateList)
                    {
                        AddBitrateMapping(EClockFrequency.Mhz_80, nominalBitrate, dataBitrate);
                    }
                }

                /// 60 Mhz
                nominalBitrateList = parameterManager.GetNominalBitrateEnumList(EClockFrequency.Mhz_60);
                dataBitrateList = parameterManager.GetDataBitrateEnumList(EClockFrequency.Mhz_60);
                foreach (var nominalBitrate in nominalBitrateList)
                {
                    foreach (var dataBitrate in dataBitrateList)
                    {
                        AddBitrateMapping(EClockFrequency.Mhz_60, nominalBitrate, dataBitrate);
                    }
                }

                /// 40 Mhz
                nominalBitrateList = parameterManager.GetNominalBitrateEnumList(EClockFrequency.Mhz_40);
                dataBitrateList = parameterManager.GetDataBitrateEnumList(EClockFrequency.Mhz_40);
                foreach (var nominalBitrate in nominalBitrateList)
                {
                    foreach (var dataBitrate in dataBitrateList)
                    {
                        AddBitrateMapping(EClockFrequency.Mhz_40, nominalBitrate, dataBitrate);
                    }
                }

                /// 30 Mhz
                nominalBitrateList = parameterManager.GetNominalBitrateEnumList(EClockFrequency.Mhz_30);
                dataBitrateList = parameterManager.GetDataBitrateEnumList(EClockFrequency.Mhz_30);
                foreach (var nominalBitrate in nominalBitrateList)
                {
                    foreach (var dataBitrate in dataBitrateList)
                    {
                        AddBitrateMapping(EClockFrequency.Mhz_30, nominalBitrate, dataBitrate);
                    }
                }

                /// 24 Mhz
                nominalBitrateList = parameterManager.GetNominalBitrateEnumList(EClockFrequency.Mhz_24);
                dataBitrateList = parameterManager.GetDataBitrateEnumList(EClockFrequency.Mhz_24);
                foreach (var nominalBitrate in nominalBitrateList)
                {
                    foreach (var dataBitrate in dataBitrateList)
                    {
                        AddBitrateMapping(EClockFrequency.Mhz_24, nominalBitrate, dataBitrate);
                    }
                }

                /// 20 Mhz
                nominalBitrateList = parameterManager.GetNominalBitrateEnumList(EClockFrequency.Mhz_20);
                dataBitrateList = parameterManager.GetDataBitrateEnumList(EClockFrequency.Mhz_20);
                foreach (var nominalBitrate in nominalBitrateList)
                {
                    foreach (var dataBitrate in dataBitrateList)
                    {
                        AddBitrateMapping(EClockFrequency.Mhz_20, nominalBitrate, dataBitrate);
                    }
                }
            }

            public string GetBitrateFDString(EClockFrequency clockFrequency, BitrateSegment nominalBitrate, BitrateSegment dataBitrate)
            {
                int freqMhz = 0;
                if (clockFrequency == EClockFrequency.Mhz_08) freqMhz = 8;
                if (clockFrequency == EClockFrequency.Mhz_20) freqMhz = 20;
                if (clockFrequency == EClockFrequency.Mhz_24) freqMhz = 24;
                if (clockFrequency == EClockFrequency.Mhz_30) freqMhz = 30;
                if (clockFrequency == EClockFrequency.Mhz_40) freqMhz = 40;
                if (clockFrequency == EClockFrequency.Mhz_60) freqMhz = 60;
                if (clockFrequency == EClockFrequency.Mhz_80) freqMhz = 80;

                string bitrateFD = "";
                bitrateFD += "f_clock_mhz=" + freqMhz + ", ";
                bitrateFD += "nom_brp=" + nominalBitrate.Brp + ", ";
                bitrateFD += "nom_tseg1=" + nominalBitrate.Tseg1 + ", ";
                bitrateFD += "nom_tseg2=" + nominalBitrate.Tseg2 + ", ";
                bitrateFD += "nom_sjw=" + nominalBitrate.Sjw + ", ";
                bitrateFD += "data_brp=" + dataBitrate.Brp + ", ";
                bitrateFD += "data_tseg1=" + dataBitrate.Tseg1 + ", ";
                bitrateFD += "data_tseg2=" + dataBitrate.Tseg2 + ", ";
                bitrateFD += "data_sjw=" + dataBitrate.Sjw;

                return bitrateFD;
            }

            public string GetBitrateFDString(EClockFrequency freq, ENominalBitrate nominalBitrate, EDataBitrate dataBitrate)
            {
                BitrateMapping bitrate = BitrateFDList.Find(x => x.ClockFrequency == freq && x.NominalBitrate == nominalBitrate && x.DataBitrate == dataBitrate);

                if (bitrate == null)
                {
                    return string.Empty;
                }
                else
                {
                    string bitrateFD = "";
                    bitrateFD += "f_clock_mhz=" + (int)freq + ", ";
                    bitrateFD += "nom_brp=" + bitrate.NominalBitrateSegment.Brp + ", ";
                    bitrateFD += "nom_tseg1=" + bitrate.NominalBitrateSegment.Tseg1 + ", ";
                    bitrateFD += "nom_tseg2=" + bitrate.NominalBitrateSegment.Tseg2 + ", ";
                    bitrateFD += "nom_sjw=" + bitrate.NominalBitrateSegment.Sjw + ", ";
                    bitrateFD += "data_brp=" + bitrate.DataBitrateSegment.Brp + ", ";
                    bitrateFD += "data_tseg1=" + bitrate.DataBitrateSegment.Tseg1 + ", ";
                    bitrateFD += "data_tseg2=" + bitrate.DataBitrateSegment.Tseg2 + ", ";
                    bitrateFD += "data_sjw=" + bitrate.DataBitrateSegment.Sjw;

                    return bitrateFD;
                }
            }

            private void AddBitrateMapping(EClockFrequency freq, ENominalBitrate norminalBitrate, EDataBitrate dataBitrate)
            {
                BitrateSegment norminal = null;
                BitrateSegment data = null;

                if (freq == EClockFrequency.Mhz_80)
                {
                    if (norminalBitrate == ENominalBitrate.MBit_1) norminal = new BitrateSegment(10, 5, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_800) norminal = new BitrateSegment(10, 7, 2, 1);
                    if (norminalBitrate == ENominalBitrate.KBit_500) norminal = new BitrateSegment(10, 12, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_250) norminal = new BitrateSegment(20, 12, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_200) norminal = new BitrateSegment(20, 15, 4, 4);
                    if (norminalBitrate == ENominalBitrate.KBit_125) norminal = new BitrateSegment(40, 13, 2, 1);
                    if (norminalBitrate == ENominalBitrate.KBit_100) norminal = new BitrateSegment(40, 16, 3, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_95) norminal = new BitrateSegment(40, 15, 5, 4);
                    if (norminalBitrate == ENominalBitrate.KBit_83) norminal = new BitrateSegment(60, 12, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_50) norminal = new BitrateSegment(80, 16, 3, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_47) norminal = new BitrateSegment(210, 5, 2, 1);
                    if (norminalBitrate == ENominalBitrate.KBit_40) norminal = new BitrateSegment(100, 15, 4, 4);
                    if (norminalBitrate == ENominalBitrate.KBit_33) norminal = new BitrateSegment(120, 16, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_20) norminal = new BitrateSegment(200, 16, 3, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_10) norminal = new BitrateSegment(400, 16, 3, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_5) norminal = new BitrateSegment(640, 16, 8, 2);

                    if (dataBitrate == EDataBitrate.MBit_02) data = new BitrateSegment(4, 7, 2, 1);
                    if (dataBitrate == EDataBitrate.MBit_04) data = new BitrateSegment(2, 7, 2, 1);
                    if (dataBitrate == EDataBitrate.MBit_08) data = new BitrateSegment(1, 7, 2, 1);
                    if (dataBitrate == EDataBitrate.MBit_10) data = new BitrateSegment(1, 5, 2, 1);
                }
                else if (freq == EClockFrequency.Mhz_60)
                {
                    if (norminalBitrate == ENominalBitrate.MBit_1) norminal = new BitrateSegment(5, 8, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_500) norminal = new BitrateSegment(12, 7, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_250) norminal = new BitrateSegment(24, 7, 2, 2);

                    if (dataBitrate == EDataBitrate.MBit_02) data = new BitrateSegment(3, 7, 2, 1);
                    if (dataBitrate == EDataBitrate.MBit_04) data = new BitrateSegment(3, 3, 1, 1);
                    if (dataBitrate == EDataBitrate.MBit_06) data = new BitrateSegment(2, 3, 1, 1);
                    if (dataBitrate == EDataBitrate.MBit_10) data = new BitrateSegment(1, 3, 2, 1);
                    if (dataBitrate == EDataBitrate.MBit_12) data = new BitrateSegment(1, 2, 2, 1);
                }
                else if (freq == EClockFrequency.Mhz_40)
                {
                    if (norminalBitrate == ENominalBitrate.MBit_1) norminal = new BitrateSegment(5, 5, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_500) norminal = new BitrateSegment(5, 11, 4, 4);
                    if (norminalBitrate == ENominalBitrate.KBit_250) norminal = new BitrateSegment(10, 11, 4, 4);

                    if (dataBitrate == EDataBitrate.MBit_02) data = new BitrateSegment(4, 3, 1, 1);
                    if (dataBitrate == EDataBitrate.MBit_04) data = new BitrateSegment(1, 7, 2, 1);
                    if (dataBitrate == EDataBitrate.MBit_08) data = new BitrateSegment(1, 2, 2, 1);
                    if (dataBitrate == EDataBitrate.MBit_10) data = new BitrateSegment(1, 1, 2, 1);
                }
                else if (freq == EClockFrequency.Mhz_30)
                {
                    if (norminalBitrate == ENominalBitrate.MBit_1) norminal = new BitrateSegment(6, 3, 1, 1);
                    if (norminalBitrate == ENominalBitrate.KBit_500) norminal = new BitrateSegment(12, 3, 1, 1);
                    if (norminalBitrate == ENominalBitrate.KBit_250) norminal = new BitrateSegment(12, 7, 2, 2);

                    if (dataBitrate == EDataBitrate.MBit_02) data = new BitrateSegment(3, 3, 1, 1);
                    if (dataBitrate == EDataBitrate.MBit_06) data = new BitrateSegment(1, 2, 2, 1);
                }
                else if (freq == EClockFrequency.Mhz_24)
                {
                    if (norminalBitrate == ENominalBitrate.MBit_1) norminal = new BitrateSegment(3, 5, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_800) norminal = new BitrateSegment(3, 7, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_500) norminal = new BitrateSegment(3, 13, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_250) norminal = new BitrateSegment(6, 13, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_125) norminal = new BitrateSegment(12, 13, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_100) norminal = new BitrateSegment(12, 16, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_95) norminal = new BitrateSegment(12, 15, 5, 5);
                    if (norminalBitrate == ENominalBitrate.KBit_83) norminal = new BitrateSegment(18, 12, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_50) norminal = new BitrateSegment(24, 16, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_47) norminal = new BitrateSegment(63, 5, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_33) norminal = new BitrateSegment(36, 16, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_20) norminal = new BitrateSegment(60, 16, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_10) norminal = new BitrateSegment(120, 16, 3, 3);
                    if (norminalBitrate == ENominalBitrate.KBit_5) norminal = new BitrateSegment(192, 16, 8, 8);

                    if (dataBitrate == EDataBitrate.MBit_02) data = new BitrateSegment(2, 4, 1, 1);
                    if (dataBitrate == EDataBitrate.MBit_06) data = new BitrateSegment(1, 1, 2, 1);
                }
                else if (freq == EClockFrequency.Mhz_20)
                {
                    if (norminalBitrate == ENominalBitrate.MBit_1) norminal = new BitrateSegment(5, 2, 1, 1);
                    if (norminalBitrate == ENominalBitrate.KBit_500) norminal = new BitrateSegment(5, 5, 2, 2);
                    if (norminalBitrate == ENominalBitrate.KBit_250) norminal = new BitrateSegment(5, 12, 3, 3);

                    if (dataBitrate == EDataBitrate.MBit_02) data = new BitrateSegment(2, 3, 1, 1);
                    if (dataBitrate == EDataBitrate.MBit_04) data = new BitrateSegment(1, 2, 2, 1);
                }


                if (norminal == null || data == null)
                {
                    throw new ArgumentException();
                }

                BitrateMapping bitrateMapping = new BitrateMapping();
                bitrateMapping.ClockFrequency = freq;
                bitrateMapping.NominalBitrate = norminalBitrate;
                bitrateMapping.DataBitrate = dataBitrate;
                bitrateMapping.NominalBitrateSegment = norminal;
                bitrateMapping.DataBitrateSegment = data;

                BitrateFDList.Add(bitrateMapping);
            }

            private class BitrateMapping
            {
                public EClockFrequency ClockFrequency { get; set; }
                public ENominalBitrate NominalBitrate { get; set; }
                public EDataBitrate DataBitrate { get; set; }

                public BitrateSegment NominalBitrateSegment { get; set; }
                public BitrateSegment DataBitrateSegment { get; set; }
            }
        }
    }
}
