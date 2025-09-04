using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Mvvm;
using Sqlite.Common;
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
    public class ModelSetMode_SafetyCondition : BindableBase
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

        private EDischargerModel _dischargerModel = EDischargerModel.MBDC_A1;
        public EDischargerModel DischargerModel
        {
            get
            {
                return _dischargerModel;
            }
            set
            {
                SetProperty(ref _dischargerModel, value);
            }
        }

        private string _voltageMin;
        public string VoltageMin
        {
            get
            {
                return _voltageMin;
            }
            set
            {
                SetProperty(ref _voltageMin, value);
            }
        }

        private string _voltageMax;
        public string VoltageMax
        {
            get
            {
                return _voltageMax;
            }
            set
            {
                SetProperty(ref _voltageMax, value);
            }
        }

        private string _currentMin;
        public string CurrentMin
        {
            get
            {
                return _currentMin;
            }
            set
            {
                SetProperty(ref _currentMin, value);
            }
        }

        private string _currentMax;
        public string CurrentMax
        {
            get
            {
                return _currentMax;
            }
            set
            {
                SetProperty(ref _currentMax, value);
            }
        }

        private string _tempMin;
        public string TempMin
        {
            get
            {
                return _tempMin;
            }
            set
            {
                SetProperty(ref _tempMin, value);
            }
        }

        private string _tempMax;
        public string TempMax
        {
            get
            {
                return _tempMax;
            }
            set
            {
                SetProperty(ref _tempMax, value);
            }
        }
    }
}