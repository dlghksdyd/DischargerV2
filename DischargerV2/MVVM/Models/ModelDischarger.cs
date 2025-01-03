using Ethernet.Client.Discharger;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.MVVM.Models
{
    /// <summary>
    /// 방전기로부터 수신 받은 데이터 모음
    /// </summary>
    public class ModelDischarger : BindableBase
    {
        private string _selectedDischargerName = string.Empty;
        public string SelectedDischargerName
        {
            get { return _selectedDischargerName; }
            set
            {
                SetProperty(ref _selectedDischargerName, value);
            }
        }

        private List<string> _dischargerNameList = new List<string>();
        public List<string> DischargerNameList
        {
            get { return _dischargerNameList; }
            set
            {
                SetProperty(ref _dischargerNameList, value);
            }
        }

        private ObservableCollection<DischargerDatas> _dischargerDatas = new ObservableCollection<DischargerDatas>();
        public ObservableCollection<DischargerDatas> DischargerDatas
        {
            get { return _dischargerDatas; }
            set
            {
                SetProperty(ref _dischargerDatas, value);
            }
        }

        private ObservableCollection<DischargerInfo> _dischargerInfos = new ObservableCollection<DischargerInfo>();
        public ObservableCollection<DischargerInfo> DischargerInfos
        {
            get { return _dischargerInfos; }
            set
            {
                SetProperty(ref _dischargerInfos, value);
            }
        }

        private ObservableCollection<EDischargerState> _dischargerStates = new ObservableCollection<EDischargerState>();
        public ObservableCollection<EDischargerState> DischargerStates
        {
            get { return _dischargerStates; }
            set
            {
                SetProperty(ref _dischargerStates, value);
            }
        }
    }
}
