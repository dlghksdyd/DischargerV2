using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using Prism.Mvvm;
using ScottPlot.Panels;
using Sqlite.Common;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelStartDischarge : BindableBase
    {
        #region Command
        #endregion

        #region Model
        public ModelStartDischarge Model { get; set; } = new ModelStartDischarge();
        #endregion

        #region Property
        private System.Timers.Timer DischargeTimer = null;

        public int PhaseIndex
        {
            get => Model.PhaseIndex;
            set
            {
                Model.PhaseIndex = value;
            }
        }
        #endregion

        private string _logFileName = string.Empty;

        public void SetLogFileName(string logFileName)
        {
            _logFileName = logFileName;
        }

        public void StartDischarge()
        {
            // 초기화
            PhaseIndex = 0;
            Model.IsEnterLastPhase = false;
            ViewModelMonitor_Graph.Instance.ClearReceiveData(Model.DischargerName);

            ViewModelDischarger.Instance.StartDischarger(new StartDischargerCommandParam()
            {
                DischargerName = Model.DischargerName,
                Voltage = Model.PhaseDataList[PhaseIndex].Voltage,
                Current = -Model.PhaseDataList[PhaseIndex].Current,
                EDischargeTarget = Model.EDischargeTarget,
                LogFileName = _logFileName,
                IsRestart = false,
            });

            DischargeTimer?.Stop();
            DischargeTimer = null;
            DischargeTimer = new System.Timers.Timer();
            DischargeTimer.Interval = 1000;
            DischargeTimer.Elapsed += OneSecondTimer_Elapsed;
            DischargeTimer.Start();
        }

        public void PauseDischarge()
        {
            Thread thread = new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModelPopup_Waiting popupWaiting = new ViewModelPopup_Waiting()
                    {
                        Title = "Wait to pause",
                        Comment = $"Discharger Name: {Model.DischargerName}",
                    };
                    ViewModelMain.Instance.SetViewModelPopup_Waiting(popupWaiting);
                    ViewModelMain.Instance.OpenPopup(ModelMain.EPopup.Waiting);
                });

                ViewModelDischarger.Instance.PauseDischarger(Model.DischargerName);

                Thread.Sleep(3000);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModelMain.Instance.OffPopup();
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public void ResumeDischarge()
        {
            Thread thread = new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModelPopup_Waiting popupWaiting = new ViewModelPopup_Waiting()
                    {
                        Title = "Wait to resume",
                        Comment = $"Discharger Name: {Model.DischargerName}",
                    };
                    ViewModelMain.Instance.SetViewModelPopup_Waiting(popupWaiting);
                    ViewModelMain.Instance.OpenPopup(ModelMain.EPopup.Waiting);
                });

                ViewModelDischarger.Instance.StartDischarger(new StartDischargerCommandParam()
                {
                    DischargerName = Model.DischargerName,
                    Voltage = Model.PhaseDataList[PhaseIndex].Voltage,
                    Current = -Model.PhaseDataList[PhaseIndex].Current,
                    EDischargeTarget = Model.EDischargeTarget,
                    LogFileName = _logFileName,
                    IsRestart = true,
                });

                Thread.Sleep(3000);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModelMain.Instance.OffPopup();
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public void StopDischarge()
        {
            Thread thread = new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModelPopup_Waiting popupWaiting = new ViewModelPopup_Waiting()
                    {
                        Title = "Wait to stop",
                        Comment = $"Discharger Name: {Model.DischargerName}",
                    };
                    ViewModelMain.Instance.SetViewModelPopup_Waiting(popupWaiting);
                    ViewModelMain.Instance.OpenPopup(ModelMain.EPopup.Waiting);
                });

                ViewModelDischarger.Instance.StopDischarger(Model.DischargerName);

                DischargeTimer?.Stop();
                DischargeTimer = null;

                Thread.Sleep(3000);
                
                // 최대 세번 retry
                for (int i = 0; i < 3; i++)
                {
                    if (ViewModelDischarger.Instance.SelectedModel.DischargerState == EDischargerState.Discharging)
                    {
                        ViewModelDischarger.Instance.StopDischarger(Model.DischargerName);
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        break;
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModelMain.Instance.OffPopup();
                });
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                ViewModelDischarger viewModelDischarger = ViewModelDischarger.Instance;
                ViewModelMonitor_Graph viewModelMonitor_Graph = ViewModelMonitor_Graph.Instance;

                var modelDischarger = viewModelDischarger.Model.ToList().Find(x => x.DischargerName == Model.DischargerName);

                EDischargerState receiveState = modelDischarger.DischargerState;

                bool isTempModule = modelDischarger.DischargerInfo.IsTempModule;

                double receiveVoltage = modelDischarger.DischargerData.ReceiveBatteryVoltage;
                double receiveCurrent = modelDischarger.DischargerData.ReceiveDischargeCurrent;

                double safetyTempMin = modelDischarger.DischargerData.SafetyTempMin;
                double safetyTempMax = modelDischarger.DischargerData.SafetyTempMax;

                // 방전기 동작 에러 발생 시, 중단
                if (receiveState == EDischargerState.SafetyOutOfRange ||
                    receiveState == EDischargerState.ReturnCodeError ||
                    receiveState == EDischargerState.ChStatusError ||
                    receiveState == EDischargerState.DeviceError)
                {
                    StopDischarge();
                }

                // 모델별 온도 받아오는 게 다름
                if (isTempModule)
                {
                    double receiveTemp = ViewModelTempModule.Instance.GetTempData(Model.DischargerName);

                    // 온도 안전 조건 확인하는 부분
                    if (receiveTemp < safetyTempMin || receiveTemp > safetyTempMax)
                    {
                        viewModelDischarger.SetDischargerState(Model.DischargerName, EDischargerState.SafetyOutOfRange);
                        return;
                    }

                    // Graph 데이터 전달
                    viewModelMonitor_Graph.SetReceiveData(Model.DischargerName, modelDischarger.DischargerData, receiveTemp);
                }
                else
                {
                    // Graph 데이터 전달
                    viewModelMonitor_Graph.SetReceiveData(Model.DischargerName, modelDischarger.DischargerData);
                }

                // 방전기 동작 설정 및 확인
                if (Model.IsEnterLastPhase == false)
                {
                    // 타겟 전압에 도달했을 경우 Phase 상승
                    if (receiveVoltage <= Model.PhaseDataList[PhaseIndex].Voltage)
                    {
                        // 모든 Phase 끝났을 때
                        if (PhaseIndex == Model.PhaseDataList.Count - 1)
                        {
                            if (Model.EDischargeTarget == Enums.EDischargeTarget.Full)
                            {
                                Model.IsEnterLastPhase = true;
                            }
                            else
                            {
                                StopDischarge();
                            }
                        }
                        else
                        {
                            PhaseIndex += 1;

                            ViewModelMonitor_Step.Instance.UpdatePhaseIndex();

                            viewModelDischarger.StartDischarger(new StartDischargerCommandParam()
                            {
                                DischargerName = Model.DischargerName,
                                Voltage = Model.PhaseDataList[PhaseIndex].Voltage,
                                Current = -Model.PhaseDataList[PhaseIndex].Current,
                                LogFileName = _logFileName
                            });
                        }
                    }
                }
                else
                {
                    // 0.1A 미만이면 방전 자동 중지
                    if (receiveState == EDischargerState.Discharging)
                    {
                        if (receiveCurrent <= 0.1)
                        {
                            StopDischarge();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");
            }
        }
    }
}
