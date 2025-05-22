using Prism.Mvvm;
using Serial.Client.TempModule;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        private ObservableCollection<ObservableCollection<double>> _tempDatas = new ObservableCollection<ObservableCollection<double>>();
        public ObservableCollection<ObservableCollection<double>> TempDatas
        {
            get { return _tempDatas; }
            set
            {
                SetProperty(ref _tempDatas, value);
            }
        }

        /// <summary>
        /// Key : DischargerName (e.g. TEST_DEVICE)
        /// </summary>
        private Dictionary<string, TempModule> _tempModuleDictionary = new Dictionary<string, TempModule>();
        public Dictionary<string, TempModule> TempModuleDictionary
        {
            get
            {
                return _tempModuleDictionary;
            }
            set
            {
                SetProperty(ref _tempModuleDictionary, value);
            }
        }

    }
}
