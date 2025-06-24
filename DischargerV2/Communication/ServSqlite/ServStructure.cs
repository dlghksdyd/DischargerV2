using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlite.Server
{
    public class TABLE_MST_USER_INFO
    {
        public string USER_ID { get; set; }
        public string USER_NM { get; set; }
        public string PASSWD { get; set; }
        public string ADMIN_GROUP { get; set; }
        public string USE_YN { get; set; }
    }

    public class TABLE_MST_MACHINE
    {
        public string MD_CD { get; set; }
        public string MTYPE { get; set; }
        public string MC_NM { get; set; }
        public string MC_IP { get; set; }
        public string LIST_ORDER { get; set; }
        public string USE_YN { get; set; }
        public string REG_ID { get; set; }
        public DateTime REG_DTM { get; set; }

        public string SDC_MODEL { get; set; }
        public string SDC_TYPE { get; set; }
        public string SDC_CH { get; set; }
        public string SDC_SPEC_VOLT { get; set; }
        public string SDC_SPEC_CURR { get; set; }
        public string SDC_TEMPMD_USE { get; set; }
        public string SDC_TEMPMD_COM { get; set; }
        public string SDC_TEMPMD_CH { get; set; }
        public string SDC_TEMP_CH { get; set; }
    }

    public class TABLE_SYS_STS_SDC
    {
        public string MD_CD { get; set; }
        public string MC_NM { get; set; }
        public string USER_NM { get; set; }
        public string DischargeMode { get; set; }
        public string DischargeTarget { get; set; }
        public string DischargerState { get; set; }
        public string LogFileName { get; set; }
        public string DischargerVoltage { get; set; }
        public string DischargerCurrent { get; set; }
        public string DischargerTemp { get; set; }
        public long RunningTime { get; set; }
        public long TotalTime { get; set; }
        public DateTime MC_DTM { get; set; }
    }
}
