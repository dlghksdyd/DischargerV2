using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Automation.Peers;
using Peak.Can.Basic;

namespace Peak.Can.Common
{
    using TPCANHandle = System.UInt16;

    public enum EClockFrequency : byte
    {
        Mhz_08 = 8,
        Mhz_20 = 20,
        Mhz_24 = 24,
        Mhz_30 = 30,
        Mhz_40 = 40,
        Mhz_60 = 60,
        Mhz_80 = 80,
    }

    public enum ENominalBitrate : byte
    {
        MBit_1,
        KBit_800,
        KBit_500,
        KBit_250,
        KBit_200,
        KBit_125,
        KBit_100,
        KBit_95,
        KBit_83,
        KBit_50,
        KBit_47,
        KBit_40,
        KBit_33,
        KBit_20,
        KBit_10,
        KBit_5,
    }

    public enum EDataBitrate : byte
    {
        MBit_02,
        MBit_04,
        MBit_06,
        MBit_08,
        MBit_10,
        MBit_12,
    }

    public class BitrateSegment
    {
        // The bit rate prescaler.
        public uint Brp { get; private set; }
        // The time segment 1.
        public uint Tseg1 { get; private set; }
        // The time segment 2.
        public uint Tseg2 { get; private set; }
        // The resynchronization jump width.
        public uint Sjw { get; private set; }

        public BitrateSegment(uint brp, uint tseg1, uint tseg2, uint sjw)
        {
            Brp = brp;
            Tseg1 = tseg1;
            Tseg2 = tseg2;
            Sjw = sjw;
        }
    }

    public class PCANClockFrequencyParameter
    {
        private EClockFrequency _enum { get; set; }
        private string _value { get; set; }

        public PCANClockFrequencyParameter(EClockFrequency freq)
        {
            _enum = freq;

            if (freq == EClockFrequency.Mhz_08) _value = "08 MHz";
            if (freq == EClockFrequency.Mhz_20) _value = "20 MHz";
            if (freq == EClockFrequency.Mhz_24) _value = "24 MHz";
            if (freq == EClockFrequency.Mhz_30) _value = "30 MHz";
            if (freq == EClockFrequency.Mhz_40) _value = "40 MHz";
            if (freq == EClockFrequency.Mhz_60) _value = "60 MHz";
            if (freq == EClockFrequency.Mhz_80) _value = "80 MHz";
        }

        public static implicit operator string(PCANClockFrequencyParameter freq)
        {
            return freq._value;
        }

        public static implicit operator PCANClockFrequencyParameter(string freq)
        {
            if (freq == "08 MHz") return new PCANClockFrequencyParameter(EClockFrequency.Mhz_08);
            if (freq == "20 MHz") return new PCANClockFrequencyParameter(EClockFrequency.Mhz_20);
            if (freq == "24 MHz") return new PCANClockFrequencyParameter(EClockFrequency.Mhz_24);
            if (freq == "30 MHz") return new PCANClockFrequencyParameter(EClockFrequency.Mhz_30);
            if (freq == "40 MHz") return new PCANClockFrequencyParameter(EClockFrequency.Mhz_40);
            if (freq == "60 MHz") return new PCANClockFrequencyParameter(EClockFrequency.Mhz_60);
            if (freq == "80 MHz") return new PCANClockFrequencyParameter(EClockFrequency.Mhz_80);

            return null;
        }

        public EClockFrequency ToEnum()
        {
            return _enum;
        }

        public override string ToString()
        {
            return _value;
        }
    }

    public class PCANNominalBitrateParameter
    {
        private ENominalBitrate _enum { get; set; }
        private string _value { get; set; }

        public PCANNominalBitrateParameter(ENominalBitrate bitrate)
        {
            _enum = bitrate;

            if (bitrate == ENominalBitrate.MBit_1) _value = "1 Mbit/s";
            if (bitrate == ENominalBitrate.KBit_800) _value = "800 kbit/s";
            if (bitrate == ENominalBitrate.KBit_500) _value = "500 kbit/s";
            if (bitrate == ENominalBitrate.KBit_250) _value = "250 kbit/s";
            if (bitrate == ENominalBitrate.KBit_200) _value = "200 kbit/s";
            if (bitrate == ENominalBitrate.KBit_125) _value = "125 kbit/s";
            if (bitrate == ENominalBitrate.KBit_100) _value = "100 kbit/s";
            if (bitrate == ENominalBitrate.KBit_95) _value = "95.238 kbit/s";
            if (bitrate == ENominalBitrate.KBit_83) _value = "83.333 kbit/s";
            if (bitrate == ENominalBitrate.KBit_50) _value = "50 kbit/s";
            if (bitrate == ENominalBitrate.KBit_47) _value = "47.619 kbit/s";
            if (bitrate == ENominalBitrate.KBit_40) _value = "40 kbit/s";
            if (bitrate == ENominalBitrate.KBit_33) _value = "33.333 kbit/s";
            if (bitrate == ENominalBitrate.KBit_20) _value = "20 kbit/s";
            if (bitrate == ENominalBitrate.KBit_10) _value = "10 kbit/s";
            if (bitrate == ENominalBitrate.KBit_5) _value = "5 kbit/s";
        }

        public static implicit operator string(PCANNominalBitrateParameter bitrate)
        {
            return bitrate._value;
        }

        public static implicit operator PCANNominalBitrateParameter(string bitrate)
        {
            if (bitrate == "1 Mbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.MBit_1);
            if (bitrate == "800 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_800);
            if (bitrate == "500 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_500);
            if (bitrate == "250 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_250);
            if (bitrate == "200 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_200);
            if (bitrate == "125 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_125);
            if (bitrate == "100 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_100);
            if (bitrate == "95.238 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_95);
            if (bitrate == "83.333 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_83);
            if (bitrate == "50 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_50);
            if (bitrate == "47.619 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_47);
            if (bitrate == "40 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_40);
            if (bitrate == "33.333 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_33);
            if (bitrate == "20 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_20);
            if (bitrate == "10 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_10);
            if (bitrate == "5 kbit/s") return new PCANNominalBitrateParameter(ENominalBitrate.KBit_5);

            return null;
        }

        public ENominalBitrate ToEnum()
        {
            return _enum;
        }

        public override string ToString()
        {
            return _value;
        }
    }

    public class PCANDataBitrateParameter
    {
        private EDataBitrate _enum { get; set; }
        private string _value { get; set; }

        public PCANDataBitrateParameter(EDataBitrate bitrate)
        {
            _enum = bitrate;

            if (bitrate == EDataBitrate.MBit_02) _value = "2 Mbit/s";
            if (bitrate == EDataBitrate.MBit_04) _value = "4 Mbit/s";
            if (bitrate == EDataBitrate.MBit_06) _value = "6 Mbit/s";
            if (bitrate == EDataBitrate.MBit_08) _value = "8 Mbit/s";
            if (bitrate == EDataBitrate.MBit_10) _value = "10 Mbit/s";
            if (bitrate == EDataBitrate.MBit_12) _value = "12 Mbit/s";
        }

        public static implicit operator string(PCANDataBitrateParameter bitrate)
        {
            return bitrate._value;
        }

        public static implicit operator PCANDataBitrateParameter(string bitrate)
        {
            if (bitrate == "2 Mbit/s") return new PCANDataBitrateParameter(EDataBitrate.MBit_02);
            if (bitrate == "4 Mbit/s") return new PCANDataBitrateParameter(EDataBitrate.MBit_04);
            if (bitrate == "6 Mbit/s") return new PCANDataBitrateParameter(EDataBitrate.MBit_06);
            if (bitrate == "8 Mbit/s") return new PCANDataBitrateParameter(EDataBitrate.MBit_08);
            if (bitrate == "10 Mbit/s") return new PCANDataBitrateParameter(EDataBitrate.MBit_10);
            if (bitrate == "12 Mbit/s") return new PCANDataBitrateParameter(EDataBitrate.MBit_12);

            return null;
        }

        public EDataBitrate ToEnum()
        {
            return _enum;
        }

        public override string ToString()
        {
            return _value;
        }
    }

    public class PCANParameterManagerFD
    {
        private List<EClockFrequency> ClockFrequencyList = new List<EClockFrequency>();
        private Dictionary<EClockFrequency, List<ENominalBitrate>> NominalBitrateList = new Dictionary<EClockFrequency, List<ENominalBitrate>>();
        private Dictionary<EClockFrequency, List<EDataBitrate>> DataBitrateList = new Dictionary<EClockFrequency, List<EDataBitrate>>();

        #region Clock Frequency
        public List<PCANClockFrequencyParameter> GetClockFrequencyParameterList()
        {
            List<PCANClockFrequencyParameter> parameters = new List<PCANClockFrequencyParameter>();
            foreach (EClockFrequency item in ClockFrequencyList)
            {
                parameters.Add(new PCANClockFrequencyParameter(item));
            }

            return parameters;
        }
        #endregion

        #region Nominal Bitrate
        public List<ENominalBitrate> GetNominalBitrateEnumList(EClockFrequency frequency)
        {
            List<ENominalBitrate> parameters = new List<ENominalBitrate>();
            foreach (ENominalBitrate item in NominalBitrateList[frequency])
            {
                parameters.Add(item);
            }

            return parameters;
        }

        public List<PCANNominalBitrateParameter> GetNominalBitrateParameterList(PCANClockFrequencyParameter frequency)
        {
            List<PCANNominalBitrateParameter> parameters = new List<PCANNominalBitrateParameter>();
            foreach (ENominalBitrate item in NominalBitrateList[frequency.ToEnum()])
            {
                parameters.Add(new PCANNominalBitrateParameter(item));
            }

            return parameters;
        }
        #endregion

        #region Data Bitrate
        public List<EDataBitrate> GetDataBitrateEnumList(EClockFrequency frequency)
        {
            List<EDataBitrate> parameters = new List<EDataBitrate>();
            foreach (EDataBitrate item in DataBitrateList[frequency])
            {
                parameters.Add(item);
            }

            return parameters;
        }

        public List<PCANDataBitrateParameter> GetDataBitrateParameterList(PCANClockFrequencyParameter frequency)
        {
            List<PCANDataBitrateParameter> strings = new List<PCANDataBitrateParameter>();
            foreach (EDataBitrate item in DataBitrateList[frequency.ToEnum()])
            {
                strings.Add(new PCANDataBitrateParameter(item));
            }

            return strings;
        }
        #endregion

        public PCANParameterManagerFD()
        {
            ClockFrequencyList = new List<EClockFrequency>()
            {
                EClockFrequency.Mhz_80, EClockFrequency.Mhz_60, EClockFrequency.Mhz_40, EClockFrequency.Mhz_30, EClockFrequency.Mhz_24, EClockFrequency.Mhz_20
            };

            /// 80 Mhz
            NominalBitrateList[EClockFrequency.Mhz_80] = new List<ENominalBitrate>()
            {
                ENominalBitrate.MBit_1, ENominalBitrate.KBit_800, ENominalBitrate.KBit_500, ENominalBitrate.KBit_250,
                ENominalBitrate.KBit_200, ENominalBitrate.KBit_125, ENominalBitrate.KBit_100, ENominalBitrate.KBit_95,
                ENominalBitrate.KBit_83, ENominalBitrate.KBit_50, ENominalBitrate.KBit_47, ENominalBitrate.KBit_40,
                ENominalBitrate.KBit_33, ENominalBitrate.KBit_20, ENominalBitrate.KBit_10, ENominalBitrate.KBit_5,
            };
            DataBitrateList[EClockFrequency.Mhz_80] = new List<EDataBitrate>()
            {
                EDataBitrate.MBit_02, EDataBitrate.MBit_04, EDataBitrate.MBit_08, EDataBitrate.MBit_10
            };

            /// 60 Mhz
            NominalBitrateList[EClockFrequency.Mhz_60] = new List<ENominalBitrate>() {
                ENominalBitrate.MBit_1, ENominalBitrate.KBit_500, ENominalBitrate.KBit_250
            };
            DataBitrateList[EClockFrequency.Mhz_60] = new List<EDataBitrate>() {
                EDataBitrate.MBit_02, EDataBitrate.MBit_04, EDataBitrate.MBit_06, EDataBitrate.MBit_10,
                EDataBitrate.MBit_12
            };

            /// 40 Mhz
            NominalBitrateList[EClockFrequency.Mhz_40] = new List<ENominalBitrate>() {
                ENominalBitrate.MBit_1, ENominalBitrate.KBit_500, ENominalBitrate.KBit_250
            };
            DataBitrateList[EClockFrequency.Mhz_40] = new List<EDataBitrate>() {
                EDataBitrate.MBit_02, EDataBitrate.MBit_04, EDataBitrate.MBit_08, EDataBitrate.MBit_10
            };

            /// 30 Mhz
            NominalBitrateList[EClockFrequency.Mhz_30] = new List<ENominalBitrate>() {
                ENominalBitrate.MBit_1, ENominalBitrate.KBit_500, ENominalBitrate.KBit_250
            };
            DataBitrateList[EClockFrequency.Mhz_30] = new List<EDataBitrate>() {
                EDataBitrate.MBit_02, EDataBitrate.MBit_06
            };

            /// 24 Mhz
            NominalBitrateList[EClockFrequency.Mhz_24] = new List<ENominalBitrate>() {
                ENominalBitrate.MBit_1, ENominalBitrate.KBit_800, ENominalBitrate.KBit_500, ENominalBitrate.KBit_250,
                ENominalBitrate.KBit_125, ENominalBitrate.KBit_100, ENominalBitrate.KBit_95, ENominalBitrate.KBit_83,
                ENominalBitrate.KBit_50, ENominalBitrate.KBit_47, ENominalBitrate.KBit_33, ENominalBitrate.KBit_20,
                ENominalBitrate.KBit_10, ENominalBitrate.KBit_5,
            };
            DataBitrateList[EClockFrequency.Mhz_24] = new List<EDataBitrate>() {
                EDataBitrate.MBit_02, EDataBitrate.MBit_06
            };

            /// 20 Mhz
            NominalBitrateList[EClockFrequency.Mhz_20] = new List<ENominalBitrate>() {
                ENominalBitrate.MBit_1, ENominalBitrate.KBit_500, ENominalBitrate.KBit_250
            };
            DataBitrateList[EClockFrequency.Mhz_20] = new List<EDataBitrate>() {
                EDataBitrate.MBit_02, EDataBitrate.MBit_04
            };
        }
    }
}
