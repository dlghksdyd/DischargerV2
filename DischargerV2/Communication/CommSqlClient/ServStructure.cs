using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlClient.Server
{
    [Table("MST_USER_INFO")]
    public class TABLE_MST_USER_INFO
    {
        // SERIAL: NOT NULL (varchar(max))
        [Required]
        public string SERIAL { get; set; }

        // USER_ID: varchar(10) NULL
        [MaxLength(10)]
        public string USER_ID { get; set; }

        // USER_NM: varchar(100) NULL
        [MaxLength(100)]
        public string USER_NM { get; set; }

        // PASSWD: varchar(20) NULL
        [MaxLength(20)]
        public string PASSWD { get; set; }

        // ADMIN_GROUP: char(1) NULL
        [MaxLength(1)]
        public string ADMIN_GROUP { get; set; }

        // USE_YN: char(1) NULL
        [MaxLength(1)]
        public string USE_YN { get; set; }
    }

    [Table("SYS_STS_SDC")]
    public class TABLE_SYS_STS_SDC
    {
        // MC_CD: varchar(max) NULL
        public string MC_CD { get; set; }

        // MC_CH: int NULL
        public int? MC_CH { get; set; }

        // USER_ID: varchar(max) NULL
        public string USER_ID { get; set; }

        // string columns (all NULLable in DB)
        public string DischargerName { get; set; }
        public string DischargerState { get; set; }
        public string DischargerVoltage { get; set; }
        public string DischargerCurrent { get; set; }
        public string DischargerTemp { get; set; }

        public string DischargeCapacity_Ah { get; set; }
        public string DischargeCapacity_kWh { get; set; }
        public string DischargePhase { get; set; }

        public string DischargeMode { get; set; }
        public string DischargeTarget { get; set; }
        public string LogFileName { get; set; }
        public string ProgressTime { get; set; }

        // MC_DTM: datetime2 NULL
        public DateTime? MC_DTM { get; set; }
    }

    [Table("QLT_HISTORY_ALARM")]
    public class TABLE_QLT_HISTORY_ALARM
    {
        // MTYPE: varchar(50) NOT NULL
        [Required]
        [MaxLength(50)]
        public string MTYPE { get; set; }

        // MC_CD: varchar(7) NOT NULL
        [Required]
        [MaxLength(7)]
        public string MC_CD { get; set; }

        // CH_NO: int NOT NULL
        public int CH_NO { get; set; }

        // Alarm_Treat: int NULL
        public int? Alarm_Treat { get; set; }

        // Alarm_Time: datetime2 NOT NULL
        public DateTime Alarm_Time { get; set; }

        // Alarm_Code: int NULL
        public int? Alarm_Code { get; set; }

        // Alarm_Desc: varchar(80) NOT NULL
        [Required]
        [MaxLength(80)]
        public string Alarm_Desc { get; set; }

        // Alarm_NewInserted: varchar(1) NULL
        [MaxLength(1)]
        public string Alarm_NewInserted { get; set; }
    }
}
