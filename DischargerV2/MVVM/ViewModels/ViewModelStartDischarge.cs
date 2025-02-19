using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelStartDischarge : BindableBase
    {
        #region Command
        #endregion

        #region Model
        #endregion

        #region Property
        private static ViewModelStartDischarge _instance;
        public static ViewModelStartDischarge Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelStartDischarge();
                }
                return _instance;
            }
        }

        private Dictionary<string, object> _viewModelDictionary = new Dictionary<string, object>();
        public Dictionary<string, object> ViewModelDictionary
        {
            get
            {
                return _viewModelDictionary;
            }
            set
            {
                SetProperty(ref _viewModelDictionary, value);
            }
        }
        #endregion

        public ViewModelStartDischarge()
        {
            _instance = this;
        }

        public void StartDischarge(string dischargerName)
        {
            ViewModelDictionary.TryGetValue(dischargerName, out var viewModelStartDischarge);

            if (viewModelStartDischarge is ViewModelStartDischarge_Preset viewModelStartDischarge_Preset)
            {
                viewModelStartDischarge_Preset.StartDischarge();
            }
            else if (viewModelStartDischarge is ViewModelStartDischarge_Step viewModelStartDischarge_Step)
            {
                viewModelStartDischarge_Step.StartDischarge();
            }
            else if (viewModelStartDischarge is ViewModelStartDischarge_Simple viewModelStartDischarge_Simple)
            {
                viewModelStartDischarge_Simple.StartDischarge();
            }
        }
    }
}
