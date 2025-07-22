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

        private int _dischargerChannel;
        public int DischargerChannel
        {
            get
            {
                return _dischargerChannel;
            }
            set
            {
                SetProperty(ref _dischargerChannel, value);
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

        private string _targetDetail = string.Empty;
        public string TargetDetail
        {
            get
            {
                return _targetDetail;
            }
            set
            {
                SetProperty(ref _targetDetail, value);
            }
        }

        private string _logFileName = string.Empty;
        public string LogFileName
        {
            get
            {
                return _logFileName;
            }
            set
            {
                SetProperty(ref _logFileName, value);
            }
        }
    }
}