using DischargerV2.MVVM.Enums;
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

namespace DischargerV2.MVVM.Models
{
    public class ModelSetMode_Preset : BindableBase
    {
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

        private List<string> _batteryTypeList = new List<string>();
        public List<string> BatteryTypeList 
        { 
            get 
            { 
                return _batteryTypeList;
            } 
            set
            {
                SetProperty(ref _batteryTypeList, value);
            }
        }

        private string _selectedBatteryType = string.Empty;
        public string SelectedBatteryType
        {
            get
            {
                return _selectedBatteryType;
            }
            set
            {
                SetProperty(ref _selectedBatteryType, value);
            }
        }

        private string _currentSoC;
        public string CurrentSoC
        {
            get
            {
                return _currentSoC;
            }
            set
            {
                SetProperty(ref _currentSoC, value);
            }
        }

        private EDischargeTarget _eDischargeTarget = EDischargeTarget.Full;
        public EDischargeTarget EDischargeTarget
        {
            get
            {
                return _eDischargeTarget;
            }
            set
            {
                SetProperty(ref _eDischargeTarget, value);
            }
        }

        private string _targetVoltage = string.Empty;
        public string TargetVoltage
        {
            get
            {
                return _targetVoltage;
            }
            set
            {
                SetProperty(ref _targetVoltage, value);
            }
        }

        private string _targetSoC = string.Empty;
        public string TargetSoC
        {
            get
            {
                return _targetSoC;
            }
            set
            {
                SetProperty(ref _targetSoC, value);
            }
        }
    }
}