using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
    public class DCCPacketHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] StartIdentifier = new byte[8] { 0xef, 0xef, 0xef, 0xef, 0xef, 0xef, 0xef, 0xef };
        public short Length;
        private byte VersionNumber = 0;
        public byte SerialNumber = 0;
        private byte TotalPacketNum = 1;
        private byte CurrentPacketNum = 1;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
    public class DCCPacketTail
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private byte[] Tail = new byte[2] { 0xee, 0xee };
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
        public class Reply
        {
            public ECommandCode CommandCode;
            public EReturnCode ReturnCode;
            public short NumberOfChannels;
            public short ChannelNumber;
            public uint ErrorCode;
            public EChannelStatus ChannelStatus;
            public double BatteryVoltage;
            public double BatteryCurrent;
            public double DCIR;
            public float AuxTemp1;
            public float AuxTemp2;
            public Int16 DOModuleInfo;
            public Int16 DIModuleInfo;
            public double DischargeCapacity;
            public float AuxTemp3;
            public float AuxTemp4;
            public double ChargeEnergy;
            public double DischargeEnergy;
        }
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
}
