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
    public class ViewModelMonitor_Step : BindableBase
    {
        #region
        public event EventHandler PhaseNoChanged;
        #endregion

        #region Command
        #endregion

        #region Model
        public ModelMonitor_Step Model { get; set; } = new ModelMonitor_Step();
        #endregion

        #region Property
        private static ViewModelMonitor_Step _instance = new ViewModelMonitor_Step();
        public static ViewModelMonitor_Step Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelMonitor_Step();
                }
                return _instance;
            }
        }

        public int PhaseNo
        {
            get => Model.PhaseNo;
            set => Model.PhaseNo = value;
        }
        #endregion

        public ViewModelMonitor_Step()
        {
            _instance = this;
        }

        public void SetPhaseNo(int phaseNo)
        {
            PhaseNo = phaseNo;

            if (PhaseNoChanged != null)
            {
                PhaseNoChanged(Instance, EventArgs.Empty);
            }
        }
    }
}
