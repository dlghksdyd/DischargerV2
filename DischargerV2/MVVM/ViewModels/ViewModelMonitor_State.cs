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
using System.Threading;
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
        public DelegateCommand<string> ChangedStateCommand { get; set; }
        public DelegateCommand PauseDischargeCommand { get; set; }
        public DelegateCommand ResumeDischargeCommand { get; set; }
        public DelegateCommand StopDischargeCommand { get; set; }
        public DelegateCommand ReturnSetModeCommand { get; set; }
        #endregion

        #region Model
        public ModelMonitor_State Model { get; set; } = new ModelMonitor_State();
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

            ChangedStateCommand = new DelegateCommand<string>(ChangedState);
            PauseDischargeCommand = new DelegateCommand(PauseDischarge);
            ResumeDischargeCommand = new DelegateCommand(ResumeDischarge);
            StopDischargeCommand = new DelegateCommand(StopDischarge);
            ReturnSetModeCommand = new DelegateCommand(ReturnSetMode);
        }

        public void ChangedState(string state)
        {
            if (Model.State == EDischargerState.Discharging.ToString())
            {
                // 방전 완료되었을 때
                if (state == EDischargerState.Ready.ToString())
                {
                    Model.PauseNResumeIsEnable = false;
                    Model.StopIsEnable = false;
                    Model.FinishIsEnable = true;
                }
                // 에러 발생하였을 때
                else if (state == EDischargerState.SafetyOutOfRange.ToString() ||
                         state == EDischargerState.ReturnCodeError.ToString() ||
                         state == EDischargerState.ChStatusError.ToString() ||
                         state == EDischargerState.DeviceError.ToString())
                {
                    Model.PauseNResumeIsEnable = false;
                    Model.StopIsEnable = false;
                    Model.FinishIsEnable = true;
                }
            }
            Model.State = state;
        }

        private void PauseDischarge()
        {
            string dischargerName = ViewModelSetMode.Instance.Model.DischargerName;

            ViewModelSetMode.Instance.ViewModelDictionary[dischargerName].PauseDischarge();
        }

        private void ResumeDischarge()
        {
            string dischargerName = ViewModelSetMode.Instance.Model.DischargerName;

            ViewModelSetMode.Instance.ViewModelDictionary[dischargerName].ResumeDischarge();
        }

        private void StopDischarge()
        {
            string dischargerName = ViewModelSetMode.Instance.Model.DischargerName;

            ViewModelSetMode.Instance.ViewModelDictionary[dischargerName].StopDischarge();

            int dischargerIndex = ViewModelSetMode.Instance.Model.DischargerIndex;
            
            // 방전기 동작이 멈출때까지 기다림
            while (ViewModelDischarger.Instance.Model.DischargerStates[dischargerIndex] != EDischargerState.Ready)
            {
                Thread.Sleep(100);
            }

            Model.PauseNResumeIsEnable = false;
            Model.StopIsEnable = false;
            Model.FinishIsEnable = true;
        }

        private void ReturnSetMode()
        {
            // Monitor -> SetMode 화면 전환
            ViewModelMain.Instance.SetIsStartedArray(false);

            // Button IsEnable Binding 값 초기화
            Model.PauseNResumeIsEnable = true;
            Model.StopIsEnable = true;
            Model.FinishIsEnable = false;
        }
    }
}
