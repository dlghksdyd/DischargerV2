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

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelMonitor_State : BindableBase
    {
        #region Command
        public DelegateCommand PauseDischargeCommand { get; set; }
        public DelegateCommand ResumeDischargeCommand { get; set; }
        public DelegateCommand StopDischargeCommand { get; set; }
        public DelegateCommand ReturnSetModeCommand { get; set; }
        #endregion

        #region Model
        #endregion

        #region Property
        private static ViewModelMonitor_State _instance = new ViewModelMonitor_State();
        public static ViewModelMonitor_State Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelMonitor_State();
                }
                return _instance;
            }
        }
        #endregion

        public ViewModelMonitor_State()
        {
            _instance = this;

            PauseDischargeCommand = new DelegateCommand(PauseDischarge);
            ResumeDischargeCommand = new DelegateCommand(ResumeDischarge);
            StopDischargeCommand = new DelegateCommand(StopDischarge);
            ReturnSetModeCommand = new DelegateCommand(ReturnSetMode);
        }

        private void PauseDischarge()
        {
            ViewModelSetMode.Instance.ViewModelDictionary[ViewModelSetMode.SelectedDischargerName].PauseDischarge();
        }

        private void ResumeDischarge()
        {
            ViewModelSetMode.Instance.ViewModelDictionary[ViewModelSetMode.SelectedDischargerName].ResumeDischarge();
        }

        private void StopDischarge()
        {
            ViewModelSetMode.Instance.ViewModelDictionary[ViewModelSetMode.SelectedDischargerName].StopDischarge();
        }

        private void ReturnSetMode()
        {
            // Monitor -> SetMode 화면 전환
            ViewModelMain.Instance.SetIsStartedArray(false);
        }
    }
}
