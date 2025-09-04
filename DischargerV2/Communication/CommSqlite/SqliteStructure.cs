using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlite.Common
{
    public enum EDischargerModel
    {
        [Description("자체 제작 방전기_구버전")]
        MBDC_A1,
        [Description("자체 제작 방전기_신버전_경광등 제어 필요")]
        MBDC_A2,
        [Description("Sinexcel 방전기_구버전")]
        MBDC_S1,
        [Description("Sinexcel 방전기_신버전_Protocol V3.4 이상")]
        MBDC_S2, 
    }

    public enum EDischargeType
    {
        Load,
        Regen,
    }

    public class TableUserInfo
    {
        public string UserId;
        public string Password;
        public string UserName;
        public bool IsAdmin;
    }

    public class TableDischargerInfo
    {
        public string DischargerName;
        public EDischargerModel Model;
        public EDischargeType Type;
        public short DischargerChannel;
        public double SpecVoltage;
        public double SpecCurrent;
        public string IpAddress;
        public bool IsTempModule;
        public string TempModuleComPort;
        public string TempModuleChannel;
        public string TempChannel;
    }

    public class TableDischargerModel
    {
        public int Id;
        public EDischargerModel Model;
        public EDischargeType Type;
        public int Channel;
        public double SpecVoltage;
        public double SpecCurrent;
        public double SafetyVoltMax;
        public double SafetyVoltMin;
        public double SafetyCurrentMax;
        public double SafetyCurrentMin;
        public double SafetyTempMax;
        public double SafetyTempMin;
    }

    public class TableDischargerErrorCode
    {
        public uint Code;
        public string Name;
        public string Title;
        public string Description;
        public string Cause;
        public string Action;

        public TableDischargerErrorCode()
        {

        }

        public TableDischargerErrorCode(uint code, string name, string title, string description, string cause, string action)
        {
            this.Code = code;
            this.Name = name;
            this.Title = title;
            this.Description = description;
            this.Cause = cause;
            this.Action = action;
        }
    }
}
