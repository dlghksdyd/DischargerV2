using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.Database
{
    [Table("TblDeviceInfo")]
    public class TblDeviceInfo
    {
        [Key]
        [Column("DevName")]
        public string DevName { get; set; }
        [Column("DevCode")]
        public string DevCode { get; set; }
        [Column("DchrgType")]
        public string DchrgType { get; set; }
        [Column("ChCount")]
        public string ChCount { get; set; }
        [Column("SpecV")]
        public string SpecV { get; set; }
        [Column("SpecA")]
        public string SpecA { get; set; }
        [Column("ConType")]
        public string ConType { get; set; }
        [Column("ConInfo")]
        public string ConInfo { get; set; }
        [Column("ComPort")]
        public string ComPort { get; set; }
        [Column("PortCh")]
        public string PortCh { get; set; }

        [Column("create_dt")]
        public DateTime CreateDt { get; set; }

        public TblDeviceInfo() { }

        public TblDeviceInfo(string DevName, string DevCode, string DchrgType, string ChCount, string SpecV,
            string SpecA, string ConType, string ConInfo, string ComPort, string PortCh)
        {
            this.DevName = DevName;
            this.DevCode = DevCode;
            this.DchrgType = DchrgType;
            this.ChCount = ChCount;
            this.SpecV = SpecV;
            this.SpecA = SpecA;
            this.ConType = ConType;
            this.ConInfo = ConInfo;
            this.ComPort = ComPort;
            this.PortCh = PortCh;
        }
    }
}
