using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DischargerV2.MVVM.Models
{
    public class ModelTopmenu : BindableBase
    {
        private string _datetime = "2025-01-09 15:03:24";
        public string DateTime
        {
            get
            {
                return _datetime;
            }
            set
            {
                SetProperty(ref _datetime, value);
            }
        }

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get
            {
                return _isPopupOpen;
            }
            set
            {
                SetProperty(ref _isPopupOpen, value);
            }
        }
    }
}