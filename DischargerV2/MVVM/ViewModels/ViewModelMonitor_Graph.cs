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
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Common;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelMonitor_Graph : BindableBase
    {
        #region Command
        #endregion

        #region Model
        public ModelMonitor_Graph Model { get; set; } = new ModelMonitor_Graph();
        #endregion

        #region Property
        private static ViewModelMonitor_Graph _instance = new ViewModelMonitor_Graph();
        public static ViewModelMonitor_Graph Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelMonitor_Graph();
                }
                return _instance;
            }
        }
        #endregion

        public ViewModelMonitor_Graph()
        {
            _instance = this;
        }
    }
}
