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
using Utility.Common;
using static DischargerV2.MVVM.Models.ModelStartDischargeConfig;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelMonitor : BindableBase
    {
        #region Command
        #endregion

        #region Model
        private ModelMonitor _model = new ModelMonitor();
        public ModelMonitor Model
        {
            get => _model;
            set
            {
                SetProperty(ref _model, value);
            }
        }
        #endregion

        #region Property
        private static ViewModelMonitor _instance = new ViewModelMonitor();
        public static ViewModelMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelMonitor();
                }
                return _instance;
            }
        }
        #endregion

        public ViewModelMonitor()
        {
            _instance = this;
        }
    }
}
