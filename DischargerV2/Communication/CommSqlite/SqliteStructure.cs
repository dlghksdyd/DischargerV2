using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqlite.Common
{
    public enum EDischargerModel
    {
        MBDC, MBDC_S
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
