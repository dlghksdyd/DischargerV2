using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DischargerV2.MVVM.Models
{
    public class ModelMonitor_Graph : BindableBase
    {
        private bool _isCheckedVoltage = true;
        public bool IsCheckedVoltage
        {
            get
            {
                return _isCheckedVoltage;
            }
            set
            {
                SetProperty(ref _isCheckedVoltage, value);
            }
        }

        private bool _isCheckedCurrent = true;
        public bool IsCheckedCurrent
        {
            get
            {
                return _isCheckedCurrent;
            }
            set
            {
                SetProperty(ref _isCheckedCurrent, value);
            }
        }

        private bool _isCheckedTemp = true;
        public bool IsCheckedTemp
        {
            get
            {
                return _isCheckedTemp;
            }
            set
            {
                SetProperty(ref _isCheckedTemp, value);
            }
        }

        private bool _isCheckedSoc = true;
        public bool IsCheckedSoc
        {
            get
            {
                return _isCheckedSoc;
            }
            set
            {
                SetProperty(ref _isCheckedSoc, value);
            }
        }
    }
}