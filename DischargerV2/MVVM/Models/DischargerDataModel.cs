using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.MVVM.Models
{
    public class DischargerDataModel : BindableBase
    {
        private double _current = 0;
        public double Current
        {
            get { return _current; }
            set
            {
                SetProperty(ref _current, value);
            }
        }

        private double _voltage = 0;
        public double Voltage
        {
            get { return _voltage; }
            set
            {
                SetProperty(ref _voltage, value);
            }
        }
    }
}
