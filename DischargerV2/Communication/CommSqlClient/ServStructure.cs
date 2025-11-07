using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlClient.Server
{
    [Table("MST_USER_INFO", Schema = "dbo")]
    public class TABLE_MST_USER_INFO
    {
        [Key]
        [Column("SERIAL", TypeName = "varchar")]
        [Required]
        public string SERIAL { get; set; }

        [Column("USER_ID", TypeName = "varchar")]
        [StringLength(10)]
        public string USER_ID { get; set; }

        [Column("USER_NM", TypeName = "varchar")]
        [StringLength(100)]
        public string USER_NM { get; set; }

        [Column("PASSWD", TypeName = "varchar")]
        [StringLength(20)]
        public string PASSWD { get; set; }

        [Column("ADMIN_GROUP", TypeName = "char")]
        [StringLength(1)]
        public string ADMIN_GROUP { get; set; }

        [Column("USE_YN", TypeName = "char")]
        [StringLength(1)]
        public string USE_YN { get; set; }
    }

    [Table("SYS_STS_SDC", Schema = "dbo")]
    public class TABLE_SYS_STS_SDC
    {
        [Key]
        [Column("MC_CD", Order = 0, TypeName = "varchar")]
        [Required]
        public string MC_CD { get; set; }

        [Key]
        [Column("MC_CH", Order = 1)]
        [Required]
        public int MC_CH { get; set; }

        [Column("USER_ID", TypeName = "varchar")]
        public string USER_ID { get; set; }

        [Column("DischargerName", TypeName = "varchar")]
        public string DischargerName { get; set; }

        [Column("DischargerState", TypeName = "varchar")]
        public string DischargerState { get; set; }

        [Column("DischargerVoltage", TypeName = "varchar")]
        public string DischargerVoltage { get; set; }

        [Column("DischargerCurrent", TypeName = "varchar")]
        public string DischargerCurrent { get; set; }

        [Column("DischargerTemp", TypeName = "varchar")]
        public string DischargerTemp { get; set; }

        [Column("DischargeCapacity_Ah", TypeName = "varchar")]
        public string DischargeCapacity_Ah { get; set; }

        [Column("DischargeCapacity_kWh", TypeName = "varchar")]
        public string DischargeCapacity_kWh { get; set; }

        [Column("DischargePhase", TypeName = "varchar")]
        public string DischargePhase { get; set; }

        [Column("DischargeMode", TypeName = "varchar")]
        public string DischargeMode { get; set; }

        [Column("DischargeTarget", TypeName = "varchar")]
        public string DischargeTarget { get; set; }

        [Column("LogFileName", TypeName = "varchar")]
        public string LogFileName { get; set; }

        [Column("ProgressTime", TypeName = "varchar")]
        public string ProgressTime { get; set; }

        [Column("MC_DTM", TypeName = "datetime2")]
        public DateTime? MC_DTM { get; set; }
    }

    [Table("QLT_HISTORY_ALARM", Schema = "dbo")]
    public class TABLE_QLT_HISTORY_ALARM
    {
        [Column("MTYPE", TypeName = "varchar")]
        [StringLength(50)]
        [Required]
        public string MTYPE { get; set; }

        [Key]
        [Column("MC_CD", Order = 0, TypeName = "varchar")]
        [StringLength(7)]
        [Required]
        public string MC_CD { get; set; }

        [Key]
        [Column("CH_NO", Order = 1)]
        [Required]
        public int CH_NO { get; set; }

        [Column("Alarm_Treat")]
        public int? Alarm_Treat { get; set; }

        [Key]
        [Column("Alarm_Time", Order = 2, TypeName = "datetime2")]
        [Required]
        public DateTime Alarm_Time { get; set; }

        [Column("Alarm_Code")]
        public int? Alarm_Code { get; set; }

        [Column("Alarm_Desc", TypeName = "varchar")]
        [StringLength(80)]
        [Required]
        public string Alarm_Desc { get; set; }

        [Column("Alarm_NewInserted", TypeName = "varchar")]
        [StringLength(1)]
        public string Alarm_NewInserted { get; set; }
    }
}
