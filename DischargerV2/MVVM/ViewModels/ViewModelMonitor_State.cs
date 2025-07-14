using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using Prism.Commands;
using Prism.Mvvm;
using ScottPlot;
using SqlClient.Server;
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
                // Server DB 사용 (통합 관제 연동)
                if (!ViewModelLogin.Instance.IsLocalDb())
                {
                    string dischargerName = ViewModelSetMode.Instance.Model.DischargerName;

                    // UpdateData SetMode Data 
                    var updateData = new TABLE_SYS_STS_SDC();
                    updateData.MC_CD = ViewModelLogin.Instance.Model.MachineCode;
                    updateData.MC_CH = ViewModelSetMode.Instance.Model.DischargerIndex + 1;
                    updateData.USER_ID = ViewModelLogin.Instance.Model.UserId;
                    updateData.DischargeMode = string.Empty;
                    updateData.DischargeTarget = string.Empty;
                    updateData.LogFileName = string.Empty;

                    SqlClientStatus.UpdateData_Set(updateData);
                }

                // 방전 모드 설정 돌아가기
                ViewModelMain.Instance.SetIsStartedArray(false);

                // PhaseNo 초기화
                ViewModelSetMode.Instance.StartDischargeDictionary[ViewModelSetMode.Instance.Model.DischargerName].PhaseIndex = -1;

                // 방전 모드 설정 돌아가기 Trace Log 저장
                DischargerData dischargerComm = new DischargerData();
                dischargerComm.Name = ViewModelSetMode.Instance.Model.DischargerName;

                new LogTrace(ELogTrace.SYSTEM_OK_RETURN_SETMODE, dischargerComm);
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.SYSTEM_ERROR_RETURN_SETMODE, ex);

                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");
            }
        }
    }
}
