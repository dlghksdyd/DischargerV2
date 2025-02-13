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
    public class ModelSetMode_Simple : BindableBase
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

        private string _standardVoltage;
        public string StandardVoltage
        {
            get
            {
                return _standardVoltage;
            }
            set
            {
                SetProperty(ref _standardVoltage, value);
            }
        }

        private string _standardCapacity;
        public string StandardCapacity
        {
            get
            {
                return _standardCapacity;
            }
            set
            {
                SetProperty(ref _standardCapacity, value);
            }
        }

        private EDischargeType _eDischargeType = EDischargeType.Full;
        public EDischargeType EDischargeType
        {
            get
            {
                return _eDischargeType;
            }
            set
            {
                SetProperty(ref _eDischargeType, value);
            }
        }

        private string _targetVoltage;
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
    }
}