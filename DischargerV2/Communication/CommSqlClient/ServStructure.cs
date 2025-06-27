using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlClient.Server
{
    public class TABLE_MST_USER_INFO
    {
        public string USER_ID { get; set; }
        public string USER_NM { get; set; }
        public string PASSWD { get; set; }
        public string ADMIN_GROUP { get; set; }
        public string USE_YN { get; set; }
    }

    public class TABLE_SYS_STS_SDC
    {
        public string MC_CD { get; set; }
        public int MC_CH { get; set; }
        public string USER_NM { get; set; }

        public string DischargerName { get; set; }
        public string DischargerState { get; set; }
        public string DischargerVoltage { get; set; }
        public string DischargerCurrent { get; set; }
        public string DischargerTemp { get; set; }

        public string DischargeMode { get; set; }
        public string DischargeTarget { get; set; }
        public string LogFileName { get; set; }
        public long ProgressTime { get; set; }

        public DateTime MC_DTM { get; set; }
    }
}
