using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.MVVM.Enums
{
    public enum ELanguage
    {
        [Description ("es-US")]
        English,
        [Description("ko-KR")]
        Korean
    }

    public enum EDischargeMode
    {
        Preset, Step, Simple
    }

    public enum EDischargeTarget
    {
        Full, Zero, Voltage, SoC
    }

    public enum EDischargerData
    {
        Voltage, Current, Temp, SoC,
        SafetyVoltageMin, SafetyVoltageMax,
        SafetyCurrentMin, SafetyCurrentMax,
        SafetyTempMin, SafetyTempMax
    }

    public enum EMonitorState
    {
        All, Connected, Fault
    }
}
