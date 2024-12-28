using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.MVVM.Models
{
    public class TempModuleDataModel : BindableBase
    {
        private List<double> _temperatureList = new List<double>();
        public List<double> TemperatureList
        {
            get { return _temperatureList; }
            set
            {
                SetProperty(ref _temperatureList, value);
            }
        }
    }
}
