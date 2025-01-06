using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.MVVM.Models
{
    public class ModelTempModule : BindableBase
    {
        private List<string> _tempModuleComportList = new List<string>();
        public List<string> TempModuleComportList
        {
            get { return _tempModuleComportList; }
            set
            {
                SetProperty(ref _tempModuleComportList, value);
            }
        }

        private ObservableCollection<double> _tempDatas = new ObservableCollection<double>();
        public ObservableCollection<double> TempDatas
        {
            get { return _tempDatas; }
            set
            {
                SetProperty(ref _tempDatas, value);
            }
        }
    }
}
