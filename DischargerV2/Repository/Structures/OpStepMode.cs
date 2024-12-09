using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Structures
{
    public class StepInfo
    {
        public double TargetVoltage;
        public double DischargeCurrent;
    }

    public class OpStepMode
    {
        /// <summary>
        /// 시간 정보
        /// </summary>
        public DateTime DischargeStartTime;

        /// <summary>
        /// 모듈 스펙
        /// </summary>
        public double StandardCapacity = 0.0;

        /// <summary>
        /// 스텝 정보
        /// </summary>
        public List<StepInfo> StepInfos = new List<StepInfo>();

        /// <summary>
        /// 전압, 전류 프로파일링
        /// </summary>
        public List<double> VoltageHistory = new List<double>();
        public List<double> CurrentHistory = new List<double>();

        /// <summary>
        /// 온도 프로파일링
        /// </summary>
        public List<double> TempHistory = new List<double>();
    }
}



