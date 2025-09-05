using DischargerV2.Languages.Strings;
using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using Prism.Mvvm;
using ScottPlot.Panels;
using ScottPlot.Statistics;
using SqlClient.Server;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using static DischargerV2.LOG.LogDischarge;

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

        private DateTime _startedTime;
        private DateTime _receiveTime;
        private double _receiveVoltage = double.NaN;
        private double _capacity_Ah = double.NaN;
        private double _capacity_kWh = double.NaN;
        private double _dv = double.NaN;
        private double _dq = double.NaN;
        private double _dvdq = double.NaN;
        private double _dvdqAvg = double.NaN;
        private List<double> _dvdqList = new List<double>();

        public void SetLogFileName(string logFileName)
        {
            _logFileName = logFileName;
        }

        public void StartDischarge()
        {
            // 초기화
            _startedTime = DateTime.Now;
            _capacity_Ah = 0;
            _capacity_kWh = 0; 
            _dv = 0;
            _dq = 0;
            _dvdq = 0;
            _dvdqAvg = 0;
            _dvdqList.Clear();
            _isEnterPause = false;

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
            DischargeTimer.Interval = 500;
            DischargeTimer.Elapsed += OneSecondTimer_Elapsed;
            DischargeTimer.Start();
        }

        private bool _isEnterPause = false;
        public void PauseDischarge()
        {
            _isEnterPause = true;

            Thread thread = new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var title = new DynamicString().GetDynamicString("PopupWaiting_Title_Pause");
                    var comment = new DynamicString().GetDynamicString("PopupWaiting_Comment");

                    ViewModelPopup_Waiting viewModelPopup_Waiting = new ViewModelPopup_Waiting()
                    {
                        Title = title,
                        Comment = $"{comment}: {Model.DischargerName}",
                    };

                    ViewModelMain.Instance.SetViewModelPopup_Waiting(viewModelPopup_Waiting);
                    ViewModelMain.Instance.OpenPopup(ModelMain.EPopup.Waiting);
                });

                ViewModelDischarger.Instance.PauseDischarger(Model.DischargerName);

                Thread.Sleep(3000);

                // 최대 세번 retry
                for (int i = 0; i < 3; i++)
                {
                    if (ViewModelDischarger.Instance.SelectedModel.DischargerState == EDischargerState.Discharging)
                    {
                        ViewModelDischarger.Instance.PauseDischarger(Model.DischargerName);

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

        public void ResumeDischarge()
        {
            Thread thread = new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var title = new DynamicString().GetDynamicString("PopupWaiting_Title_Resume");
                    var comment = new DynamicString().GetDynamicString("PopupWaiting_Comment");

                    ViewModelPopup_Waiting viewModelPopup_Waiting = new ViewModelPopup_Waiting()
                    {
                        Title = title,
                        Comment = $"{comment}: {Model.DischargerName}",
                    };
                    
                    ViewModelMain.Instance.SetViewModelPopup_Waiting(viewModelPopup_Waiting);
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

                // 최대 세번 retry
                for (int i = 0; i < 3; i++)
                {
                    if (ViewModelDischarger.Instance.SelectedModel.DischargerState != EDischargerState.Discharging)
                    {
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
                    }
                    else
                    {
                        break;
                    }
                }

                ViewModelDischarger.Instance.SetIsPaused(Model.DischargerName, false);

                Thread.Sleep(1000);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModelMain.Instance.OffPopup();
                });

                _startedTime = DateTime.Now;
                _isEnterPause = false;
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
                    var title = new DynamicString().GetDynamicString("PopupWaiting_Title_Stop");
                    var comment = new DynamicString().GetDynamicString("PopupWaiting_Comment");

                    ViewModelPopup_Waiting viewModelPopup_Waiting = new ViewModelPopup_Waiting()
                    {
                        Title = title,
                        Comment = $"{comment}: {Model.DischargerName}",
                    };
                    
                    ViewModelMain.Instance.SetViewModelPopup_Waiting(viewModelPopup_Waiting);
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
                double receiveTemp = isTempModule ?
                    modelDischarger.DischargerData.ReceiveDischargeTemp = ViewModelTempModule.Instance.GetTempData(Model.DischargerName) :
                    modelDischarger.DischargerData.ReceiveDischargeTemp;

                double safetyVoltageMin = modelDischarger.DischargerData.SafetyVoltageMin;
                double safetyVoltageMax = modelDischarger.DischargerData.SafetyVoltageMax;
                double safetyCurrentMin = modelDischarger.DischargerData.SafetyCurrentMin;
                double safetyCurrentMax = modelDischarger.DischargerData.SafetyCurrentMax;
                double safetyTempMin = modelDischarger.DischargerData.SafetyTempMin;
                double safetyTempMax = modelDischarger.DischargerData.SafetyTempMax;

                if (PhaseIndex < 0)
                {
                    StopDischarge();
                    return;
                }

                // 안전 조건 확인
                if (receiveVoltage < safetyVoltageMin || receiveVoltage > safetyVoltageMax)
                {
                    viewModelDischarger.SetDischargerState(Model.DischargerName, EDischargerState.SafetyOutOfRange);
                }
                if (receiveCurrent < safetyCurrentMin || receiveCurrent > safetyCurrentMax)
                {
                    viewModelDischarger.SetDischargerState(Model.DischargerName, EDischargerState.SafetyOutOfRange);
                }
                if (receiveTemp < safetyTempMin || receiveTemp > safetyTempMax)
                {
                    viewModelDischarger.SetDischargerState(Model.DischargerName, EDischargerState.SafetyOutOfRange);
                }

                // Graph 데이터 전달
                viewModelMonitor_Graph.SetReceiveData(Model.DischargerName, modelDischarger.DischargerData);

                // 방전기 동작 에러 발생 시, 중단
                if (receiveState == EDischargerState.SafetyOutOfRange ||
                    receiveState == EDischargerState.ReturnCodeError ||
                    receiveState == EDischargerState.ChStatusError ||
                    receiveState == EDischargerState.DeviceError)
                {
                    StopDischarge();
                    return;
                }

                // 0V 이하 및 0.1A 미만이면 방전 자동 중지 
                if (receiveVoltage <= 0 && receiveCurrent < 0.1)
                {
                    StopDischarge();
                    return;
                }

                // -1V 이하 및 1A 이하이면 배터리 시료 해체 상태로 판단 → 방전 자동 중지
                if (receiveVoltage <= -1 && receiveCurrent <= 1)
                {
                    StopDischarge();
                    return;
                }

                // 방전기 동작 설정 및 확인
                if (!_isEnterPause)
                {
                    if (Model.IsEnterLastPhase == false)
                    {
                        // dv, dq, 용량 관련 계산 부분
                        if (double.IsNaN(_receiveVoltage))
                        {
                            _receiveTime = DateTime.Now;
                            _receiveVoltage = receiveVoltage;
                            return;
                        }

                        DateTime currentTime = DateTime.Now;

                        TimeSpan startedTimeSpan = currentTime - _startedTime;

                        TimeSpan receiveTimeSpan = currentTime - _receiveTime;
                        int receiveTimeGap = (int)receiveTimeSpan.TotalMilliseconds;

                        _dq = (float)(receiveCurrent * (receiveTimeGap / 1000.0f) / 3600.0f);
                        _dv = (float)(_receiveVoltage - receiveVoltage);

                        _capacity_Ah += _dq;
                        _capacity_kWh += _dq * receiveVoltage / 1000.0f;

                        _dvdq = Math.Abs(_dv / _dq);

                        _dvdqList.Add(_dvdq);

                        if (_dvdqList.Count > 10)
                        {
                            _dvdqList.RemoveAt(0);
                        }

                        if (_dvdqList.Count == 10)
                        {
                            _dvdqAvg = CalcDvdqAvg(_dvdqList);
                        }

                        _receiveTime = DateTime.Now;
                        _receiveVoltage = receiveVoltage;

                        // 방전 로그 저장
                        var dischargeRawData = new LogDischarge.DischargeRawData()
                        {
                            Time = DateTime.Now.ToString("HH:mm:ss"),
                            Current = receiveCurrent.ToString("F1"),
                            Voltage = receiveVoltage.ToString("F1"),
                            Temp = receiveTemp.ToString("F1"),
                            Capacity = _capacity_Ah.ToString("F3"),
                            dv = _dv.ToString("F3"),
                            dq = _dq.ToString("F3"),
                            dvdq = _dvdq.ToString("F3"),
                            destDvdq = Model.Dvdq.ToString("F3"),
                            phase2 = (PhaseIndex == 1) ? "True" : "False",
                            dvdqAvg = _dvdqAvg.ToString("F3"),
                            phase = $"'{PhaseIndex + 1} / {Model.PhaseDataList.Count}"
                        };

                        new LogDischarge(_logFileName, dischargeRawData);

                        // Server DB 사용(통합 관제 연동)
                        if (!ViewModelLogin.Instance.IsLocalDb())
                        {
                            // UpdateData Data 
                            var updateData = new TABLE_SYS_STS_SDC();
                            updateData.MC_CD = ViewModelDischarger.Instance.MachineCode;
                            updateData.MC_CH = Model.DischargerIndex + 1;
                            updateData.USER_ID = ViewModelLogin.Instance.Model.UserId;

                            updateData.DischargeCapacity_Ah = _capacity_Ah.ToString("F3");
                            updateData.DischargeCapacity_kWh = _capacity_kWh.ToString("F3");
                            updateData.DischargePhase = $"{PhaseIndex + 1}, {Model.PhaseDataList.Count}";

                            SqlClientStatus.UpdateData_Discharging(updateData);
                        }

                        if (Model.Mode == Enums.EDischargeMode.Preset ||
                            Model.Mode == Enums.EDischargeMode.Step)
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

                                    bool isOk = false;

                                    // 최대 세번 전송
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (!isOk)
                                        {
                                            isOk = viewModelDischarger.StartDischarger(new StartDischargerCommandParam()
                                            {
                                                DischargerName = Model.DischargerName,
                                                Voltage = Model.PhaseDataList[PhaseIndex].Voltage,
                                                Current = -Model.PhaseDataList[PhaseIndex].Current,
                                                LogFileName = _logFileName
                                            });
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (Model.Mode == Enums.EDischargeMode.Simple)
                        {
                            // 방전 시작 후 30초 이상 방전 진행 필요
                            if (startedTimeSpan.TotalSeconds > 30)
                            {
                                // Phase target voltage 값 도달했을 때
                                if (receiveVoltage <= Model.PhaseDataList[PhaseIndex].Voltage)
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

                                // 비가역구간 진입 예측 기울기 확인하여 다음 Phase로
                                if (PhaseIndex == 0 && Model.Dvdq < _dvdqAvg)
                                {
                                    PhaseIndex = 1;

                                    bool isOk = false;

                                    // 최대 세번 전송
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (!isOk)
                                        {
                                            isOk = viewModelDischarger.StartDischarger(new StartDischargerCommandParam()
                                            {
                                                DischargerName = Model.DischargerName,
                                                Voltage = Model.PhaseDataList[PhaseIndex].Voltage,
                                                Current = -Model.PhaseDataList[PhaseIndex].Current,
                                                LogFileName = _logFileName
                                            });
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
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

        private double CalcDvdqAvg(List<double> dvdqList)
        {
            List<double> temp = dvdqList.ToList();

            temp.Remove(temp.Min());
            temp.Remove(temp.Min());
            temp.Remove(temp.Min());
            temp.Remove(temp.Max());

            return temp.Average();
        }
    }
}
