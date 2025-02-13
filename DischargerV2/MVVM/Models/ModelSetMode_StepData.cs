using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DischargerV2.MVVM.Models
{
    public class ModelSetMode_StepData : BindableBase
    {
        private string _no;
        public string No
        {
            get
            {
                return _no;
            }
            set
            {
                SetProperty(ref _no, value);
            }
        }

        private bool _isFixedCurrent;
        public bool IsFixedCurrent
        {
            get
            {
                return _isFixedCurrent;
            }
            set
            {
                SetProperty(ref _isFixedCurrent, value);
            }
        }

        private string _voltage;
        public string Voltage
        {
            get
            {
                return _voltage;
            }
            set
            {
                SetProperty(ref _voltage, value);
            }
        }

        private string _current;
        public string Current
        {
            get
            {
                return _current;
            }
            set
            {
                SetProperty(ref _current, value);
            }
        }

        private string _cRate;
        public string CRate
        {
            get
            {
                return _cRate;
            }
            set
            {
                SetProperty(ref _cRate, value);
            }
        }
    }
}