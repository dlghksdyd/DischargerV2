using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Structures;

namespace Sqlite.Common
{
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
        public string IpAddress;  // IP or ComPort or Guid
        public string TempModuleComPort;
        public int TempModuleChannel;
        public int TempChannel;
    }

    public class TableDischargerModel
    {
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
}
