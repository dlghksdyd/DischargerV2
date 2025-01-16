using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.Database
{
    [Table("TblDischargerTypeInfo")]
    public class TblDischargerTypeInfo
    {
        [Key]
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
        [Column("SafetyHV")]
        public string SafetyHV { get; set; }
        [Column("SafetyLV")]
        public string SafetyLV { get; set; }
        [Column("SafetyHA")]
        public string SafetyHA { get; set; }
        [Column("SafetyLA")]
        public string SafetyLA { get; set; }
        [Column("SafetyHT")]
        public string SafetyHT { get; set; }
        [Column("SafetyLT")]
        public string SafetyLT { get; set; }

        public TblDischargerTypeInfo() { }

        public TblDischargerTypeInfo(string DevCode, string DchrgType, string ChCount, string SpecV, string SpecA,
            string SafetyHV, string SafetyLV, string SafetyHA, string SafetyLA, string SafetyHT, string SafetyLT)
        {
            this.DevCode = DevCode;
            this.DchrgType = DchrgType;
            this.ChCount = ChCount;
            this.SpecV = SpecV;
            this.SpecA = SpecA;
            this.SafetyHV = SafetyHV;
            this.SafetyLV = SafetyLV;
            this.SafetyHA = SafetyHA;
            this.SafetyLA = SafetyLA;
            this.SafetyHT = SafetyHT;
            this.SafetyLT = SafetyLT;
        }
    }
}
