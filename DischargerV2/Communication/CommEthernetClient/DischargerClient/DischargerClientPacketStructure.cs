using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Ethernet.Client.Discharger.ChannelInfo;

namespace Ethernet.Client.Discharger
{
    public class DCCPacketConstant
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
        RequestCommand = 0x3001,
        ChannelInfo = 0x3012,
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

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class SetSafetyCondition
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Request
        {
            private ECommandCode CommandCode = ECommandCode.RequestCommand;
            private short NumberOfChannels = 1;
            public short ChannelNumber;
            private short NumberOfParameters = 4;
            private EParameterIndex Index1 = EParameterIndex.VoltageUpperLimit;
            public double VoltageUpperLimitValue;
            private EParameterIndex Index2 = EParameterIndex.VoltageLowerLimit;
            public double VoltageLowerLimitValue;
            private EParameterIndex Index3 = EParameterIndex.CurrentUpperLimit;
            public double CurrentUpperLimitValue;
            private EParameterIndex Index4 = EParameterIndex.CurrentLowerLimit;
            public double CurrentLowerLimitValue;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Reply
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class ChannelInfo
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Request
        {
            private ECommandCode CommandCode = ECommandCode.ChannelInfo;
            private short NumberOfChannels = 1;
            public short ChannelNumber;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Reply_Channel1
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public Reply[] ReplyArray = new Reply[1];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Reply_Channel2 
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Reply[] ReplyArray = new Reply[2];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public struct Reply
        {
            public short NumberOfChannels;
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
            public DischargeCapacity DischargeCapacity;
            public float AuxTemp3;
            public float AuxTemp4;
            public double ChargeEnergy;
            public double DischargeEnergy;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class DischargeCapacity
    {
        public byte byte0;
        public byte byte1;
        public byte byte2;
        public byte byte3;
        public byte byte4;
        public byte byte5;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class StartDischarge
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Request
        {
            private ECommandCode CommandCode = ECommandCode.RequestCommand;
            private short NumberOfChannels = 1;
            public short ChannelNumber;
            private short NumberOfParameters = 4;
            private EParameterIndex Index1 = EParameterIndex.WorkMode;
            public double WorkMode;
            private EParameterIndex Index2 = EParameterIndex.SetValue;
            public double SetValue;
            private EParameterIndex Index3 = EParameterIndex.LimitingValues;
            public double LimitingValue;
            private EParameterIndex Index4 = EParameterIndex.Start;
            private double FixedValue = 1.0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Reply
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class StopDischarge
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Request
        {
            private ECommandCode CommandCode = ECommandCode.RequestCommand;
            private short NumberOfChannels = 1;
            public short ChannelNumber;
            private short NumberOfParameters = 2;
            private EParameterIndex Index1 = EParameterIndex.WorkMode;
            private double WorkMode = 0.0;  // standby mode
            private EParameterIndex Index2 = EParameterIndex.Start;
            private double FixedValue = 1.0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Reply
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class ClearAlarm
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Request
        {
            private ECommandCode CommandCode = ECommandCode.RequestCommand;
            private short NumberOfChannels = 1;
            public short ChannelNumber;
            private short NumberOfParameters = 1;
            private EParameterIndex Index1 = EParameterIndex.WorkModeClearAlarm;
            private double FixedValue = 1.0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Reply
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class LampControl
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Request
        {
            private ECommandCode CommandCode = ECommandCode.RequestCommand;
            private short NumberOfChannels = 1;
            public short ChannelNumber;
            private short NumberOfParameters = 1;
            private EParameterIndex Index1 = EParameterIndex.DioControl;
            public double DioValue;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
        public class Reply
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;
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
            _numberOfChannels = 0;
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

            _header.Length = CalculateLengthOfHeader();

            List<byte> packet = new List<byte>();

            // 1. Header
            packet.AddRange(_header.ToByteArray());

            // 2. Command (2 bytes)
            packet.AddRange(BitConverter.GetBytes((short)_command));

            // 3. Channel Count (2 bytes)
            packet.AddRange(BitConverter.GetBytes(_numberOfChannels));

            // 4. Channels (2 bytes each)
            foreach (var ch in _channels)
                packet.AddRange(BitConverter.GetBytes(ch));

            if (_command == ECommandCode.RequestCommand)
            {
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

            // 7. Tail
            packet.AddRange(_tail.ToByteArray());

            return packet.ToArray();
        }

        private short CalculateLengthOfHeader()
        {
            const int HeaderSize = 4;                   // Fixed: Length (2) + Version (1) + Serial (1)
            const int CommandCodeSize = 2;              // short
            const int ChannelCountSize = 2;             // short
            const int ChannelEntrySize = 2;             // short per channel
            const int ParameterCountSize = 2;           // short
            const int ParameterEntrySize = 10;          // short (2) + double (8)
            const int TailSize = 2;                     // fixed end marker (0xEEEE)

            int totalLength = HeaderSize + CommandCodeSize + ChannelCountSize + (_numberOfChannels * ChannelEntrySize);

            if (_command == ECommandCode.RequestCommand)
            {
                totalLength += ParameterCountSize + (_parameters.Count * ParameterEntrySize);
            }

            totalLength += TailSize;

            return (short)totalLength;
        }
    }
}
