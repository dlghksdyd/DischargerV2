using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.ViewModels;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DischargerV2.MVVM.Enums
{
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
}

namespace DischargerV2.MVVM.Models
{
    public class ModelSetMode : BindableBase
    {
        private int _dischargerIndex;
        public int DischargerIndex
        {
            get
            {
                return _dischargerIndex;
            }
            set
            {
                SetProperty(ref _dischargerIndex, value);
            }
        }

        private string _dischargerName;
        public string DischargerName
        {
            get
            {
                return _dischargerName;
            }
            set
            {
                SetProperty(ref _dischargerName, value);
            }
        }

        private int _tempModuleIndex;
        public int TempModuleIndex
        {
            get
            {
                return _tempModuleIndex;
            }
            set
            {
                SetProperty(ref _tempModuleIndex, value);
            }
        }

        private int _tempModuleChannel;
        public int TempModuleChannel
        {
            get
            {
                return _tempModuleChannel;
            }
            set
            {
                SetProperty(ref _tempModuleChannel, value);
            }
        }

        private EDischargeMode _mode = EDischargeMode.Preset;
        public EDischargeMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                SetProperty(ref _mode, value);
            }
        }
    }
}