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
    public class ModelMonitor_State : BindableBase
    {
        private bool _pauseNResumeIsEnable = true;
        public bool PauseNResumeIsEnable
        {
            get
            {
                return _pauseNResumeIsEnable;
            }
            set
            {
                SetProperty(ref _pauseNResumeIsEnable, value);
            }
        }

        private bool _stopIsEnable = true;
        public bool StopIsEnable
        {
            get
            {
                return _stopIsEnable;
            }
            set
            {
                SetProperty(ref _stopIsEnable, value);
            }
        }

        private bool _finishIsEnable = false;
        public bool FinishIsEnable
        {
            get
            {
                return _finishIsEnable;
            }
            set
            {
                SetProperty(ref _finishIsEnable, value);
            }
        }
    }
}