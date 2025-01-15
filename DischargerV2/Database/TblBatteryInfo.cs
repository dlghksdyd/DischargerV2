using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.Database
{
    [Table("TblBatteryInfo")]
    class TblBatteryInfo
    {
        [Key]
        [Column("BatteryType")]
        public string BatteryType { get; set; }
        [Column("Voltage")]
        public string Voltage { get; set; }
        [Column("Capacity")]
        public string Capacity { get; set; }
        [Column("CathodeType")]
        public string CathodeType { get; set; }
        [Column("Configuration")]
        public string Configuration { get; set; }
        [Column("create_dt")]
        public DateTime CreateDt { get; set; }

        public TblBatteryInfo() { }

        public TblBatteryInfo(string BatteryType, string Voltage, string Capacity, string CathodeType, string Configuration)
        {
            this.BatteryType = BatteryType;
            this.Voltage = Voltage;
            this.Capacity = Capacity;
            this.CathodeType = CathodeType;
            this.Configuration = Configuration;
        }
    }
}
