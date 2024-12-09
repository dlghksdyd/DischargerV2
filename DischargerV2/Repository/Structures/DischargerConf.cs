using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Structures
{
    public enum EDischargerModel
    {
        MBDC,
    }

    public enum EDischargeType
    {
        Load,
        Regen,
    }

    public class DischargerConf
    {
        public DischargerConf(string dischargerName)
        {
            DischargerName = dischargerName;
        }

        /// <summary>
        /// 방전기 정보
        /// </summary>
        public string DischargerName;
        public EDischargerModel DischargerModel;
        public EDischargeType DischargeType;
        public short DischargerChannel;
        public string IpAddress;

        /// <summary>
        /// 방전기 스펙
        /// </summary>
        public double SpecVoltage;
        public double SpecCurrent;

        /// <summary>
        /// 안전 조건
        /// </summary>
        public double SafetyVoltageMax;
        public double SafetyVoltageMin;
        public double SafetyCurrentMax;
        public double SafetyCurrentMin;
        public double SafetyTempMax;
        public double SafetyTempMin;

        /// <summary>
        /// 온도 모듈 정보
        /// </summary>
        public string TempModuleComPort;
        public int TempModuleChannel;
        public int TempChannel;
    }
}
