using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Windows.Media.Media3D;

namespace Ethernet.Client.Discharger
{
    public class PacketConstant
    {
        public const int PACKET_HEADER_SIZE = 14;
    }

    public enum EParameterIndex : short
    {
        VoltageUpperLimit = 1,
        VoltageLowerLimit = 2,
        CurrentUpperLimit = 3,
        CurrentLowerLimit = 4,
        PowerUpperLimit = 5,
        PowerLowerLimit = 6,
        Start = 16,
        WorkMode = 17,
        /** 
         * Set the positive value to charge，
         * Set the negative value to discharge；
         * standby：0
         * CV mode：voltage(V)
         * CC mode：current(A)
         * CP mode：power(W)
         * CR：value of resistance (mΩ)
         * Standby：0
         * CC/CV：voltage(V) **/
        SetValue = 18,
        WorkModeClearAlarm = 19,
        LimitingValues = 20,
        DioControl = 55,
    }

    public enum EWorkMode 
    {
        Standby0 = 0,
        CvMode = 1,
        CcMode = 2,
        CpMode = 3,
        CrMode = 4,
        Standby5 = 5,
        CcCvMode = 6,
    }

    public enum ECommandCode : short
    {
        None = 0x0,

        SetSafetyCondition = 0x0001,
        StartDischarge = 0x0002,
        StopDischarge = 0x0003,
        ClearAlarm = 0x0004,
        LampControl = 0x0005,

        SetParameter = 0x3001,
        GetChannelInfo = 0x3012,
    }

    public enum EResultCode : short
    {
        Success = 0x00,
        FailWriteCommand = 0x10,
        FailReadCommand = 0x20,
        FailReadData = 0x21,
        FailParseData = 0x30,
    }

    public enum EReturnCode : int
    {
        Success = 0,
        Error = 1,
    }

    public enum EChannelStatus : short
    {
        Standby0 = 0,
        CvMode = 1,
        CcMode = 2,
        CpMode = 3,
        CrMode = 4,
        Standby5 = 5,
        CcCvMode = 6,
        Error = 15,
    }

    public enum EDioControl : uint
    {
        AcTrip = 0x00000001,
        TowerLampRed = 0x00000002,
        TowerLampYellow = 0x00000004,
        TowerLampGreen = 0x00000008,
        TowerLampBuzzer = 0x00000010,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class PacketHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] StartIdentifier = new byte[8] { 0xef, 0xef, 0xef, 0xef, 0xef, 0xef, 0xef, 0xef };
        public short Length;
        private byte VersionNumber = 0;
        public byte SerialNumber = 0;
        private byte TotalPacketNum = 1;
        private byte CurrentPacketNum = 1;

        public byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(StartIdentifier);                // 8 bytes
            bytes.AddRange(BitConverter.GetBytes(Length));  // 2 bytes (short)
            bytes.Add(VersionNumber);                       // 1 byte
            bytes.Add(SerialNumber);                        // 1 byte
            bytes.Add(TotalPacketNum);                      // 1 byte
            bytes.Add(CurrentPacketNum);                    // 1 byte

            return bytes.ToArray(); // 총 14 bytes
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class PacketTail
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private byte[] Tail = new byte[2] { 0xee, 0xee };

        public byte[] ToByteArray()
        {
            return Tail;
        }
    }

    public class Parameter
    {
        public class SetSafetyCondition
        {
            public ECommandCode CommandCode = ECommandCode.SetParameter;
            private short NumberOfChannels;
            public short ChannelNumber;
            private short NumberOfParameters = 5;
            public EParameterIndex Index1 = EParameterIndex.VoltageUpperLimit;
            public double VoltageUpperLimitValue;
            public EParameterIndex Index2 = EParameterIndex.VoltageLowerLimit;
            public double VoltageLowerLimitValue;
            public EParameterIndex Index3 = EParameterIndex.CurrentUpperLimit;
            public double CurrentUpperLimitValue;
            public EParameterIndex Index4 = EParameterIndex.CurrentLowerLimit;
            public double CurrentLowerLimitValue;
            public EParameterIndex Index5 = EParameterIndex.Start;
            public double FixedValue = 1.0;
        }

        public class StartDischarge
        {
            public ECommandCode CommandCode = ECommandCode.SetParameter;
            private short NumberOfChannels;
            public short ChannelNumber;
            private short NumberOfParameters = 4;
            public EParameterIndex Index1 = EParameterIndex.WorkMode;
            public double WorkMode;
            public EParameterIndex Index2 = EParameterIndex.SetValue;
            public double SetValue;
            public EParameterIndex Index3 = EParameterIndex.LimitingValues;
            public double LimitingValue;
            public EParameterIndex Index4 = EParameterIndex.Start;
            public double FixedValue = 1.0;
        }

        public class StopDischarge
        {
            public ECommandCode CommandCode = ECommandCode.SetParameter;
            private short NumberOfChannels;
            public short ChannelNumber;
            private short NumberOfParameters = 2;
            public EParameterIndex Index1 = EParameterIndex.WorkMode;
            public double WorkMode = 0.0;  
            public EParameterIndex Index2 = EParameterIndex.Start;
            public double FixedValue = 1.0;
        }

        public class ClearAlarm
        {
            public ECommandCode CommandCode = ECommandCode.SetParameter;
            private short NumberOfChannels;
            public short ChannelNumber;
            private short NumberOfParameters = 2;
            public EParameterIndex Index1 = EParameterIndex.WorkModeClearAlarm;
            public EParameterIndex Index2 = EParameterIndex.Start;
            public double FixedValue = 1.0;
        }

        public class LampControl
        {
            public ECommandCode CommandCode = ECommandCode.SetParameter;
            private short NumberOfChannels = 1;
            public short ChannelNumber = 999;
            private short NumberOfParameters = 1;
            public EParameterIndex Index1 = EParameterIndex.DioControl;
            public double DioValue;
        }

        public class Reply
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;

            public Reply Parse(byte[] dataBuffer)
            {
                using (MemoryStream ms = new MemoryStream(dataBuffer))
                {
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        CommandCode = (ECommandCode)reader.ReadInt16();
                        ReturnCode = (EReturnCode)reader.ReadInt32();
                    }
                }
                return this;
            }
        }
    }

    public class ChannelInfo
    {
        public class Reply
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;
            public short NumberOfChannels;
            public Data[] DataArray;

            public Reply Parse(byte[] dataBuffer, short channel = -1)
            {
                using (MemoryStream ms = new MemoryStream(dataBuffer))
                {
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        CommandCode = (ECommandCode)reader.ReadInt16();
                        ReturnCode = (EReturnCode)reader.ReadInt32();
                        NumberOfChannels = reader.ReadInt16();

                        if (channel < 0)
                        {
                            channel = NumberOfChannels;
                        }

                        DataArray = new Data[channel];

                        for (int i = 0; i < channel; i++)
                        {
                            DataArray[i] = new Data();

                            DataArray[i].ChannelNumber = reader.ReadInt16();
                            DataArray[i].ErrorCode = (uint)reader.ReadInt32();
                            DataArray[i].ChannelStatus = (EChannelStatus)reader.ReadInt16();
                            DataArray[i].BatteryVoltage = reader.ReadDouble();
                            DataArray[i].BatteryCurrent = reader.ReadDouble();
                            DataArray[i].DCIR = reader.ReadDouble();
                            DataArray[i].AuxTemp1 = reader.ReadSingle();
                            DataArray[i].AuxTemp2 = reader.ReadSingle();
                            DataArray[i].DOModuleInfo = reader.ReadByte();
                            DataArray[i].DIModuleInfo = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte0 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte1 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte2 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte3 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte4 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte5 = reader.ReadByte();
                            DataArray[i].AuxTemp3 = reader.ReadSingle();
                            DataArray[i].AuxTemp4 = reader.ReadSingle();
                            DataArray[i].Reservation = reader.ReadDouble();
                            DataArray[i].ChargeEnergy = reader.ReadDouble();
                            DataArray[i].DischargeEnergy = reader.ReadDouble();
                        }
                    }
                }
                return this;
            }
        }

        public class Reply_v34
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;
            public short NumberOfChannels;
            public Data_v34[] DataArray;

            public Reply_v34 Parse(byte[] dataBuffer, short channel = -1)
            {
                using (MemoryStream ms = new MemoryStream(dataBuffer))
                {
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                        CommandCode = (ECommandCode)reader.ReadInt16();
                        ReturnCode = (EReturnCode)reader.ReadInt32();
                        NumberOfChannels = reader.ReadInt16();

                        if (channel < 0)
                        {
                            channel = NumberOfChannels;
                        }

                        DataArray = new Data_v34[channel];

                        for (int i = 0; i < channel; i++)
                        {
                            DataArray[i] = new Data_v34();

                            DataArray[i].ChannelNumber = reader.ReadInt16();
                            DataArray[i].ErrorCode = (uint)reader.ReadInt32();
                            DataArray[i].ChannelStatus = (EChannelStatus)reader.ReadInt16();
                            DataArray[i].BatteryVoltage = reader.ReadDouble();
                            DataArray[i].BatteryCurrent = reader.ReadDouble();
                            DataArray[i].DCIR = reader.ReadDouble();
                            DataArray[i].AuxTemp1 = reader.ReadSingle();
                            DataArray[i].AuxTemp2 = reader.ReadSingle();
                            DataArray[i].DOModuleInfo = reader.ReadByte();
                            DataArray[i].DIModuleInfo = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte0 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte1 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte2 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte3 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte4 = reader.ReadByte();
                            DataArray[i].DischargeCapacity.byte5 = reader.ReadByte();
                            DataArray[i].AuxTemp3 = reader.ReadSingle();
                            DataArray[i].AuxTemp4 = reader.ReadSingle();
                            DataArray[i].Reservation = reader.ReadDouble();
                            DataArray[i].ChargeEnergy = reader.ReadDouble();
                            DataArray[i].DischargeEnergy = reader.ReadDouble();

                            DataArray[i].ChannelValue1 = reader.ReadDouble();
                            DataArray[i].ChannelValue2 = reader.ReadDouble();
                            DataArray[i].ChannelValue3 = reader.ReadDouble();
                            DataArray[i].ChannelValue4 = reader.ReadDouble();
                            DataArray[i].ChannelValue5 = reader.ReadDouble();
                            DataArray[i].ChannelValue6 = reader.ReadDouble();
                            DataArray[i].ChannelValue7 = reader.ReadDouble();
                            DataArray[i].ChannelValue8 = reader.ReadDouble();
                        }
                    }
                }
                return this;
            }
        }

        public class Data
        {
            public short ChannelNumber;
            public uint ErrorCode;
            public EChannelStatus ChannelStatus;
            public double BatteryVoltage;
            public double BatteryCurrent;
            public double DCIR;
            public float AuxTemp1;
            public float AuxTemp2;
            public byte DOModuleInfo;
            public byte DIModuleInfo;
            public DischargeCapacity DischargeCapacity = new DischargeCapacity();
            public float AuxTemp3;
            public float AuxTemp4;
            public double Reservation;
            public double ChargeEnergy;
            public double DischargeEnergy;
        }

        public class Data_v34 : Data
        {
            public double ChannelValue1;
            public double ChannelValue2;
            public double ChannelValue3;
            public double ChannelValue4;
            public double ChannelValue5;
            public double ChannelValue6;
            public double ChannelValue7;
            public double ChannelValue8;
        }

        public class DischargeCapacity
        {
            public byte byte0;
            public byte byte1;
            public byte byte2;
            public byte byte3;
            public byte byte4;
            public byte byte5;
        }
    }

    public class DischargerPacketGenerator
    {
        private PacketHeader _header;
        private ECommandCode _command;
        private short _numberOfChannels;
        private List<short> _channels;
        private Dictionary<EParameterIndex, double> _parameters;
        private PacketTail _tail;

        private void Initialize()
        {
            _header = new PacketHeader();
            _command = ECommandCode.None;
            _numberOfChannels = 2;
            _channels = new List<short>();
            _parameters = new Dictionary<EParameterIndex, double>();
            _tail = new PacketTail();
        }

        public void Command(ECommandCode command, byte serialNumber)
        {
            Initialize();

            _header.SerialNumber = serialNumber;
            _command = command;
        }

        public void Channel(params short[] channels)
        {
            _numberOfChannels = (short)channels.Length;

            foreach (var channel in channels)
            {
                _channels.Add(channel);
            }
        }

        public void Parameter(EParameterIndex index, double value)
        {
            _parameters[index] = value;
        }

        public byte[] GeneratePacket()
        {
            if (_command == ECommandCode.None)
                throw new InvalidOperationException("Command must be set before generating a packet.");
            if (_numberOfChannels == 0)
                throw new ArgumentException("At least one channel must be specified.");

            _header.Length = CalculateLengthOfHeader(_command);

            List<byte> packet = new List<byte>();

            // 1. Header
            packet.AddRange(_header.ToByteArray());

            // 2. Command (2 bytes)
            packet.AddRange(BitConverter.GetBytes((short)_command));

            if (_command == ECommandCode.SetParameter)
            {
                // 3. Number of channels (2 bytes)
                packet.AddRange(BitConverter.GetBytes((short)1));

                // 4. Channel number (2 bytes each)
                packet.AddRange(BitConverter.GetBytes(_channels.Last()));

                if (_parameters.Count == 0)
                    throw new ArgumentException("At least one parameter must be specified.");

                // 5. Number of parameters (2 bytes)
                packet.AddRange(BitConverter.GetBytes((short)_parameters.Count));

                // 6. Parameters: index(2) + value(8) → 10 bytes each
                foreach (var kvp in _parameters)
                {
                    packet.AddRange(BitConverter.GetBytes((short)kvp.Key));       // 2 bytes
                    packet.AddRange(BitConverter.GetBytes(kvp.Value));            // 8 bytes
                }
            }
            else if(_command == ECommandCode.GetChannelInfo)
            {
                // 3. Number of channels (2 bytes)
                packet.AddRange(BitConverter.GetBytes((short)2));

                // 4. Channel number (2 bytes each)
                foreach (var ch in _channels)
                    packet.AddRange(BitConverter.GetBytes(ch));
            }

            // 7. Tail
            packet.AddRange(_tail.ToByteArray());
            
            return packet.ToArray();
        }

        private short CalculateLengthOfHeader(ECommandCode eCommandCode)
        {
            const int HeaderSize = 4;                   // Fixed: Length (2) + Version (1) + Serial (1)
            const int CommandCodeSize = 2;              // short
            const int ChannelCountSize = 2;             // short
            const int ChannelEntrySize = 2;             // short per channel
            const int ParameterCountSize = 2;           // short
            const int ParameterEntrySize = 10;          // short (2) + double (8)
            const int TailSize = 2;                     // fixed end marker (0xEEEE)

            int totalLength;

            if (eCommandCode == ECommandCode.GetChannelInfo)
            {
                totalLength = HeaderSize + CommandCodeSize + ChannelCountSize + (_numberOfChannels * ChannelEntrySize);
            }
            else 
            {
                totalLength = HeaderSize + CommandCodeSize + ChannelCountSize + ChannelEntrySize;
                totalLength += ParameterCountSize + (_parameters.Count * ParameterEntrySize);
            }

            totalLength += TailSize;

            return (short)totalLength;
        }
    }
}
