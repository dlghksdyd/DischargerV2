using Ethernet.Client.Discharger;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
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

        private Dictionary<string, DischargerDatas> _dischargerDatas = new Dictionary<string, DischargerDatas>();
        /// <summary>
        /// Key: discharger name
        /// </summary>
        public Dictionary<string, DischargerDatas> DischargerDatas
        {
            get { return _dischargerDatas; }
            set
            {
                SetProperty(ref _dischargerDatas, value);
            }
        }

        private Dictionary<string, DischargerInfo> _dischargerInfos = new Dictionary<string, DischargerInfo>();
        /// <summary>
        /// Key: discharger name
        /// </summary>
        public Dictionary<string, DischargerInfo> DischargerInfos
        {
            get { return _dischargerInfos; }
            set
            {
                SetProperty(ref _dischargerInfos, value);
            }
        }

        private Dictionary<string, EDischargerState> _states = new Dictionary<string, EDischargerState>();
        /// <summary>
        /// Key: Discharger Name
        /// </summary>
        public Dictionary<string, EDischargerState> DischargerStates
        {
            get { return _states; }
            set
            {
                SetProperty(ref _states, value);
            }
        }
    }
}
