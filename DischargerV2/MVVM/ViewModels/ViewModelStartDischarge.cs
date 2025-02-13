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
        private ModelStartDischarge _model = new ModelStartDischarge();
        public ModelStartDischarge Model
        {
            get => _model;
            set
            {
                SetProperty(ref _model, value);
            }
        }
        #endregion

        #region Property
        private static ViewModelStartDischarge _instance = new ViewModelStartDischarge();
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
        #endregion

        public ViewModelStartDischarge()
        {
            _instance = this;
        }
    }
}
