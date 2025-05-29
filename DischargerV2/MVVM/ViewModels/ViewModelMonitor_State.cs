using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using Prism.Commands;
using Prism.Mvvm;
using ScottPlot;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using static DischargerV2.LOG.LogTrace;

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

        #region Property
        public string SelectedDischargerName;

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
            string dischargerName = ViewModelSetMode.Instance.Model.DischargerName;

            ViewModelSetMode.Instance.StartDischargeDictionary[dischargerName].PauseDischarge();
        }

        private void ResumeDischarge()
        {
            string dischargerName = ViewModelSetMode.Instance.Model.DischargerName;

            ViewModelSetMode.Instance.StartDischargeDictionary[dischargerName].ResumeDischarge();
        }

        private void StopDischarge()
        {
            string dischargerName = ViewModelSetMode.Instance.Model.DischargerName;
            ViewModelSetMode.Instance.StartDischargeDictionary[dischargerName].StopDischarge();
        }

        private void ReturnSetMode()
        {
            try
            {
                // 방전 모드 설정 돌아가기
                ViewModelMain.Instance.SetIsStartedArray(false);

                // PhaseNo 초기화
                ViewModelSetMode.Instance.StartDischargeDictionary[ViewModelSetMode.Instance.Model.DischargerName].PhaseIndex = 0;

                // 방전 모드 설정 돌아가기 Trace Log 저장
                DischargerData dischargerComm = new DischargerData();
                dischargerComm.Name = ViewModelSetMode.Instance.Model.DischargerName;

                new LogTrace(ELogTrace.TRACE_RETURN_SETMODE, dischargerComm);
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_RETURN_SETMODE, ex);
            }
        }
    }
}
