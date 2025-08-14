using DischargerV2.Languages.Strings;
using DischargerV2.LOG;
using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using Prism.Commands;
using Prism.Mvvm;
using ScottPlot.Colormaps;
using SqlClient.Server;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using Utility.Common;
using static DischargerV2.LOG.LogDischarge;
using static DischargerV2.LOG.LogTrace;
using static DischargerV2.MVVM.Models.ModelStartDischarge;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelSetMode : BindableBase
    {
        #region Command
        public DelegateCommand<string> SelectModeCommand { get; set; }
        #endregion

        #region Model
        private ModelSetMode _model = new ModelSetMode();
        public ModelSetMode Model
        {
            get => _model;
            set
            {
                SetProperty(ref _model, value);
            }
        }
        #endregion

        #region Property
        private static ViewModelSetMode _instance = new ViewModelSetMode();
        public static ViewModelSetMode Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelSetMode();
                }
                return _instance;
            }
        }

        private Dictionary<string, ModelSetMode> _modelDictionary = new Dictionary<string, ModelSetMode>();
        public Dictionary<string, ModelSetMode> ModelDictionary
        {
            get
            {
                return _modelDictionary;
            }
            set
            {
                SetProperty(ref _modelDictionary, value);
            }
        }

        private Dictionary<string, ViewModelStartDischarge> _startDischargeDictionary = new Dictionary<string, ViewModelStartDischarge>();
        public Dictionary<string, ViewModelStartDischarge> StartDischargeDictionary
        {
            get
            {
                return _startDischargeDictionary;
            }
            set
            {
                SetProperty(ref _startDischargeDictionary, value);
            }
        }

        private string _selectedDischargerName = string.Empty;
        public string SelectedDischargerName
        {
            get => _selectedDischargerName;
            set
            {
                SetProperty(ref _selectedDischargerName, value);
            }
        }

        #endregion

        public ViewModelSetMode()
        {
            _instance = this;

            SelectModeCommand = new DelegateCommand<string>(SelectMode);

            InitializeModelDictionary();
            InitializeViewModelDictionary();
        }

        public void SetDischargerName(string dischargerName)
        {
            SelectedDischargerName = dischargerName;
            Model = ModelDictionary[dischargerName];

            ViewModelSetMode_Preset.Instance.SetDischargerName(dischargerName);
            ViewModelSetMode_Step.Instance.SetDischargerName(dischargerName);
            ViewModelSetMode_Simple.Instance.SetDischargerName(dischargerName);
            ViewModelSetMode_SafetyCondition.Instance.SetDischargerName(dischargerName);
            ViewModelMonitor_Graph.Instance.SetDischargerName(dischargerName);
            ViewModelMonitor_Step.Instance.UpdatePhaseData(dischargerName);
        }

        public void InitializeModelDictionary()
        {
            // 기존 값 초기화
            ViewModelSetMode.Instance.ModelDictionary.Clear();
            ViewModelSetMode_Preset.Instance.ModelDictionary.Clear();
            ViewModelSetMode_Step.Instance.ModelDictionary.Clear();
            ViewModelSetMode_Simple.Instance.ModelDictionary.Clear();
            ViewModelSetMode_SafetyCondition.Instance.ModelDictionary.Clear();

            // Discharger에서 관련 값 받아와 사용
            List<string> dischargerNameList = ViewModelDischarger.Instance.Model.ToList().ConvertAll(x => x.DischargerName);
            List<DischargerInfo> dischargerInfoList = ViewModelDischarger.Instance.Model.ToList().ConvertAll(x => x.DischargerInfo);

            for (int index = 0; index < dischargerNameList.Count; index++)
            {
                string dischargerName;
                int dischargerChannel;

                var dischargerNameListSplit = dischargerNameList[index].Split('_');
                if (dischargerNameListSplit.Length > 1)
                {
                    dischargerName = dischargerNameListSplit[0];
                    dischargerChannel = Convert.ToInt32(dischargerNameListSplit[1]);
                }
                else
                {
                    dischargerName = dischargerNameListSplit[0];
                    dischargerChannel = 1;
                }

                DischargerInfo dischargerInfo = dischargerInfoList[index];
                TableDischargerInfo tableTischargerInfo = SqliteDischargerInfo.GetData().Find(x => x.DischargerName == dischargerName);

                ModelSetMode modelSetMode = new ModelSetMode();
                modelSetMode.DischargerIndex = index;
                modelSetMode.DischargerChannel = dischargerChannel;
                modelSetMode.DischargerName = dischargerNameList[index];
                modelSetMode.TempModuleIndex = ViewModelTempModule.Instance.GetTempModuleDataIndex(tableTischargerInfo.TempModuleComPort);
                modelSetMode.TempModuleChannel = Convert.ToInt32(tableTischargerInfo.TempChannel);
                ViewModelSetMode.Instance.ModelDictionary.Add(dischargerNameList[index], modelSetMode);

                ModelSetMode_Preset modelSetMode_Preset = new ModelSetMode_Preset();
                ViewModelSetMode_Preset.Instance.ModelDictionary.Add(dischargerNameList[index], modelSetMode_Preset);

                ModelSetMode_Step modelSetMode_Step = new ModelSetMode_Step();
                modelSetMode_Step.DischargerName = dischargerNameList[index];
                modelSetMode_Step.Content.Add(new ModelSetMode_StepData());
                ViewModelSetMode_Step.Instance.ModelDictionary.Add(dischargerNameList[index], modelSetMode_Step);

                ModelSetMode_Simple modelSetMode_Simple = new ModelSetMode_Simple();
                modelSetMode_Simple.DischargerName = dischargerNameList[index];
                ViewModelSetMode_Simple.Instance.ModelDictionary.Add(dischargerNameList[index], modelSetMode_Simple);

                ModelSetMode_SafetyCondition modelSetMode_SafetyCondition = new ModelSetMode_SafetyCondition();
                modelSetMode_SafetyCondition.DischargerName = dischargerNameList[index];
                modelSetMode_SafetyCondition.VoltageMin = dischargerInfo.SafetyVoltageMin.ToString();
                modelSetMode_SafetyCondition.VoltageMax = dischargerInfo.SafetyVoltageMax.ToString();
                modelSetMode_SafetyCondition.CurrentMin = (-dischargerInfo.SafetyCurrentMax).ToString();
                modelSetMode_SafetyCondition.CurrentMax = (-dischargerInfo.SafetyCurrentMin).ToString();
                modelSetMode_SafetyCondition.TempMin = dischargerInfo.SafetyTempMin.ToString();
                modelSetMode_SafetyCondition.TempMax = dischargerInfo.SafetyTempMax.ToString();
                ViewModelSetMode_SafetyCondition.Instance.ModelDictionary.Add(dischargerNameList[index], modelSetMode_SafetyCondition);
            }
            ViewModelSetMode_Preset.Instance.SetBatteryType();
        }

        public void InitializeViewModelDictionary()
        {
            // 기존 값 초기화
            StartDischargeDictionary.Clear();

            // Discharger에서 관련 값 받아와 사용
            List<string> dischargerNameList = ViewModelDischarger.Instance.Model.ToList().ConvertAll(x => x.DischargerName);
            List<DischargerInfo> dischargerInfoList = ViewModelDischarger.Instance.Model.ToList().ConvertAll(x => x.DischargerInfo);

            for (int index = 0; index < dischargerNameList.Count; index++)
            {
                string dischargerName = dischargerNameList[index];

                ModelStartDischarge modelStartDischarge = new ModelStartDischarge();
                modelStartDischarge.DischargerName = dischargerName;

                StartDischargeDictionary.Add(dischargerName, new ViewModelStartDischarge()
                {
                    Model = modelStartDischarge
                });
            }
        }

        public void Start()
        {
            try
            {
                // 설정 값 적용
                SetDischargerName(Model.DischargerName);

                // 설정 값 확인
                if (!CheckModeNTarget()) return;
                if (!CheckNSetSafetyCondition()) return;
                if (!CalculateTarget(out ModelStartDischarge model)) return;

                // 설정 값 적용
                StartDischargeDictionary[Model.DischargerName].Model = model;
                ViewModelMonitor_Step.Instance.UpdatePhaseData(Model.DischargerName);

                // 방전 로그 파일명 설정 및 확인
                CheckLogFileName();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// CallBackDelegate is SetLogFileNameNStartDischarge
        /// </summary>
        public void CheckLogFileName()
        {
            try
            {
                var title = new DynamicString().GetDynamicString("PleaseEnterLogFileName");
                var confirmText = new DynamicString().GetDynamicString("Save");

                ViewModelPopup_SetLogFileName viewModelPopup_SetLogFileName = new ViewModelPopup_SetLogFileName()
                {
                    Title = title,
                    CallBackDelegate = SetLogFileNameNStartDischarge,
                    ConfirmText = confirmText
                };

                ViewModelMain viewModelMain = ViewModelMain.Instance;
                viewModelMain.SetViewModelPopup_SetLogFileName(viewModelPopup_SetLogFileName);
                viewModelMain.OpenPopup(ModelMain.EPopup.SetLogFileName);
            }
            catch (Exception ex)
            {
                // System Trace Log 저장 - 방전 동작 로그 파일 생성 실패
                new LogTrace(LogTrace.ELogTrace.SYSTEM_ERROR_SAVE_LOG, ex);
            }
        }

        public void SetLogFileNameNStartDischarge()
        {
            string lofFileName = ViewModelMain.Instance.GetLogFileName();

            Model.LogFileName = lofFileName;

            // System Trace Log 저장 - 방전 동작 로그 파일 생성
            var dischargerData = new LogTrace.DischargerData()
            {
                Name = Model.DischargerName,
                FileName = Model.LogFileName
            };
            new LogTrace(LogTrace.ELogTrace.SYSTEM_OK_SAVE_LOG, dischargerData);

            // Server DB 사용 (통합 관제 연동)
            if (!ViewModelLogin.Instance.IsLocalDb())
            {
                // UpdateData SetMode Data 
                var updateData = new TABLE_SYS_STS_SDC();
                updateData.MC_CD = ViewModelLogin.Instance.Model.MachineCode;
                updateData.MC_CH = Model.DischargerIndex + 1;
                updateData.USER_ID = ViewModelLogin.Instance.Model.UserId;

                updateData.DischargeMode = Model.Mode.ToString();
                updateData.DischargeTarget = Model.TargetDetail;
                updateData.LogFileName = Model.LogFileName;

                SqlClientStatus.UpdateData_Set(updateData);
            }
            
            StartDischarge();
        }

        public void StartDischarge()
        {
            if (StartDischargeDictionary.ContainsKey(Model.DischargerName))
            {
                // Graph 방전 모드 설정
                ViewModelMonitor_Graph.Instance.SetDischargeMode(Model.DischargerName, Model.Mode);

                // Discharge Log 저장 
                new LogDischarge(GetDischargeConfig(), Model.LogFileName);

                // Start 방전 모드 설정
                StartDischargeDictionary[Model.DischargerName].SetLogFileName(Model.LogFileName);

                // 방전 시작
                Thread thread = new Thread(() =>
                {
                    var title = new DynamicString().GetDynamicString("PopupWaiting_Title_Start");
                    var comment = new DynamicString().GetDynamicString("PopupWaiting_Comment");

                    ViewModelPopup_Waiting viewModelPopup_Waiting = new ViewModelPopup_Waiting()
                    {
                        Title = title,
                        Comment = $"{comment}: {Model.DischargerName}",
                    };

                    ViewModelMain viewModelMain = ViewModelMain.Instance;
                    viewModelMain.SetViewModelPopup_Waiting(viewModelPopup_Waiting);
                    viewModelMain.OpenPopup(ModelMain.EPopup.Waiting);

                    StartDischargeDictionary[Model.DischargerName].StartDischarge();

                    DateTime startTime = DateTime.Now;

                    while (ViewModelDischarger.Instance.SelectedModel.DischargerState != EDischargerState.Discharging)
                    {
                        Thread.Sleep(100);

                        // 일정시간이 지나면 방전 시작 실패로 간주
                        if (DateTime.Now - startTime > TimeSpan.FromSeconds(20))
                        {
                            // 일정 시간이 지난 후 방전 명령이 수행될 수 있음에 따름
                            StartDischargeDictionary[Model.DischargerName].StopDischarge();

                            title = new DynamicString().GetDynamicString("PopupWarning_Title");
                            comment = new DynamicString().GetDynamicString("PopupWarning_Comment_FailStart");

                            ViewModelPopup_Warning viewModelPopup_Warning = new ViewModelPopup_Warning()
                            {
                                Title = title,
                                Comment = comment,
                                CancelButtonVisibility = Visibility.Hidden
                            };

                            viewModelMain.SetViewModelPopup_Warning(viewModelPopup_Warning);
                            viewModelMain.OpenNestedPopup(ModelMain.ENestedPopup.Warning);
                            viewModelMain.OffPopup();
                            return;
                        }
                    }

                    // SetMode -> Monitor 화면 전환
                    ViewModelMain.Instance.SetIsStartedArray(true);

                    viewModelMain.OffPopup();
                });
                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                // 프로그램 오류 메세지 활성화
                ViewModelPopup_Info viewModelPopup_Info = new ViewModelPopup_Info()
                {
                    Title = "Program error",
                    Comment = "Please restart the program",
                    ConfirmText = "Ok",
                    CancelVisibility = Visibility.Collapsed,
                };

                ViewModelMain viewModelMain = ViewModelMain.Instance;
                viewModelMain.SetViewModelPopup_Info(viewModelPopup_Info);
                viewModelMain.OpenPopup(ModelMain.EPopup.Info);
                return;
            }
        }

        private void SelectMode(string mode)
        {
            foreach (EDischargeMode eMode in Enum.GetValues(typeof(EDischargeMode)))
            {
                if (mode.ToString() == eMode.ToString())
                {
                    Model.Mode = eMode;
                }
            }
        }

        /// <summary>
        /// 방전 모드 및 목표 설정 값 확인
        /// </summary>
        /// <returns></returns>
        private bool CheckModeNTarget()
        {
            // Pre-set Mode
            if (Model.Mode == EDischargeMode.Preset)
            {
                ModelSetMode_Preset modelPreset = ViewModelSetMode_Preset.Instance.Model;

                if (modelPreset.SelectedBatteryType == null || modelPreset.SelectedBatteryType == "")
                {
                    MessageBox.Show("Battery Type: 필수 정보입니다.");
                    return false;
                }

                if (modelPreset.EDischargeTarget == EDischargeTarget.Voltage)
                {
                    if (modelPreset.TargetVoltage == null || modelPreset.TargetVoltage == "")
                    {
                        MessageBox.Show("Target Voltage (V): 필수 정보입니다.");
                        return false;
                    }
                }
                else if (modelPreset.EDischargeTarget == EDischargeTarget.SoC)
                {
                    if (modelPreset.TargetSoC == null || modelPreset.TargetSoC == "")
                    {
                        MessageBox.Show("Target SoC (%): 필수 정보입니다.");
                        return false;
                    }
                }
            }
            // Step Mode
            else if (Model.Mode == EDischargeMode.Step)
            {
                StepConfigure stepConfigure = ViewModelSetMode_Step.Instance.CreateStepConfigure();

                if (stepConfigure == null)
                {
                    MessageBox.Show("각 스텝 값의 누락이 없어야 하고, 각 값은 숫자여야 합니다.");
                    return false;
                }
            }
            // Simple Mode
            else if (Model.Mode == EDischargeMode.Simple)
            {
                ModelSetMode_Simple modelSimple = ViewModelSetMode_Simple.Instance.Model;

                if (modelSimple.StandardCapacity == null || modelSimple.StandardCapacity == "")
                {
                    MessageBox.Show("Standard Capacity (A): 필수 정보입니다.");
                    return false;
                }

                if (modelSimple.EDischargeTarget == Enums.EDischargeTarget.Voltage)
                {
                    if (modelSimple.TargetVoltage == null || modelSimple.TargetVoltage == "")
                    {
                        MessageBox.Show("Target Voltage (V): 필수 정보입니다.");
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 방전 안전 조건 설정 값 확인 및 적용
        /// </summary>
        /// <returns></returns>
        private bool CheckNSetSafetyCondition()
        {
            // 설정 값 확인
            DischargerInfo dischargerInfo = ViewModelDischarger.Instance.SelectedModel.DischargerInfo;
            ModelSetMode_SafetyCondition modelSafetyCondition = ViewModelSetMode_SafetyCondition.Instance.Model;

            if (modelSafetyCondition.VoltageMin == null || modelSafetyCondition.VoltageMin == "")
            {
                MessageBox.Show("Voltage Min (V): 필수 정보입니다.");
                return false;
            }
            else if (Convert.ToDouble(modelSafetyCondition.VoltageMin) < dischargerInfo.SafetyVoltageMin)
            {
                MessageBox.Show("Voltage Min (V): 설정 값이 스펙 범위를 벗어났습니다.");
                return false;
            }
            else if (modelSafetyCondition.VoltageMax == null || modelSafetyCondition.VoltageMax == "")
            {
                MessageBox.Show("Voltage Max (V): 필수 정보입니다.");
                return false;
            }
            else if (Convert.ToDouble(modelSafetyCondition.VoltageMax) > dischargerInfo.SafetyVoltageMax)
            {
                MessageBox.Show("Voltage Max (V): 설정 값이 스펙 범위를 벗어났습니다.");
                return false;
            }
            else if (Convert.ToDouble(modelSafetyCondition.VoltageMin) >= Convert.ToDouble(modelSafetyCondition.VoltageMax))
            {
                MessageBox.Show("Voltage Min ~ Max (V): 범위 설정 값이 잘못되었습니다.");
                return false;
            }
            else if (modelSafetyCondition.CurrentMin == null || modelSafetyCondition.CurrentMin == "")
            {
                MessageBox.Show("Current Min (A): 필수 정보입니다.");
                return false;
            }
            else if (-Convert.ToDouble(modelSafetyCondition.CurrentMin) > dischargerInfo.SafetyCurrentMax)
            {
                MessageBox.Show("Current Max (A): 설정 값이 스펙 범위를 벗어났습니다.");
                return false;
            }
            else if (modelSafetyCondition.CurrentMax == null || modelSafetyCondition.CurrentMax == "")
            {
                MessageBox.Show("Current Max (A): 필수 정보입니다.");
                return false;
            }
            else if (-Convert.ToDouble(modelSafetyCondition.CurrentMax) < dischargerInfo.SafetyCurrentMin)
            {
                MessageBox.Show("Current Min (A): 설정 값이 스펙 범위를 벗어났습니다.");
                return false;
            }
            else if (-Convert.ToDouble(modelSafetyCondition.CurrentMin) <= -Convert.ToDouble(modelSafetyCondition.CurrentMax))
            {
                MessageBox.Show("Current Min ~ Max (A): 범위 설정 값이 잘못되었습니다.");
                return false;
            }
            else if (modelSafetyCondition.TempMin == null || modelSafetyCondition.TempMin == "")
            {
                MessageBox.Show("Temp Min (℃): 필수 정보입니다.");
                return false;
            }
            else if (modelSafetyCondition.TempMax == null || modelSafetyCondition.TempMax == "")
            {
                MessageBox.Show("Temp Max (℃): 필수 정보입니다.");
                return false;
            }
            else if (Convert.ToDouble(modelSafetyCondition.TempMin) >= Convert.ToDouble(modelSafetyCondition.TempMax))
            {
                MessageBox.Show("Temp (℃): 범위 설정 값이 잘못되었습니다.");
                return false;
            }

            // 설정 값 적용
            double voltageMin = Convert.ToDouble(modelSafetyCondition.VoltageMin);
            double voltageMax = Convert.ToDouble(modelSafetyCondition.VoltageMax);
            double currentMin = Convert.ToDouble(modelSafetyCondition.CurrentMin);
            double currentMax = Convert.ToDouble(modelSafetyCondition.CurrentMax);
            double tempMin = Convert.ToDouble(modelSafetyCondition.TempMin);
            double tempMax = Convert.ToDouble(modelSafetyCondition.TempMax);

            ViewModelDischarger.Instance.SetSafetyCondition(Model.DischargerName,
                voltageMax, voltageMin, currentMax, currentMin, tempMax, tempMin);

            return true;
        }

        /// <summary>
        /// 방전 목표 설정 값 계산
        /// </summary>
        /// <returns></returns>
        private bool CalculateTarget(out ModelStartDischarge model)
        {
            model = new ModelStartDischarge()
            {
                DischargerName = Model.DischargerName,
                DischargerIndex = Model.DischargerIndex,
                Mode = Model.Mode,
            };

            // Pre-set Mode
            if (Model.Mode == EDischargeMode.Preset)
            {
                ModelSetMode_Preset modelPreset = ViewModelSetMode_Preset.Instance.Model;
                string batteryType = modelPreset.SelectedBatteryType;

                model.EDischargeTarget = modelPreset.EDischargeTarget;

                // Full Discharge, 0V Discharge
                if (modelPreset.EDischargeTarget == EDischargeTarget.Full ||
                    modelPreset.EDischargeTarget == EDischargeTarget.Zero)
                {
                    model.PhaseDataList.Add(new PhaseData()
                    {
                        Voltage = OCV_Table.getPhase2Volt(batteryType),
                        Current = OCV_Table.getCapcity(batteryType)
                    });

                    model.PhaseDataList.Add(new PhaseData()
                    {
                        Voltage = 0,
                        Current = OCV_Table.getCapcity(batteryType) / 3
                    });

                    Model.TargetDetail = $"{modelPreset.EDischargeTarget}";
                }
                // Target Voltage
                else if (modelPreset.EDischargeTarget == EDischargeTarget.Voltage)
                {
                    double targetVoltage = Convert.ToDouble(modelPreset.TargetVoltage);

                    model.PhaseDataList.Add(new PhaseData()
                    {
                        Voltage = targetVoltage,
                        Current = OCV_Table.getCapcity(batteryType) / 3
                    });

                    Model.TargetDetail = $"{modelPreset.EDischargeTarget} ({targetVoltage}V)";
                }
                // Target SoC
                else if (modelPreset.EDischargeTarget == EDischargeTarget.SoC)
                {
                    int targetSoC = Convert.ToInt32(modelPreset.TargetSoC);

                    model.PhaseDataList.Add(new PhaseData()
                    {
                        Voltage = OCV_Table.getTargetVolt(batteryType, targetSoC),
                        Current = OCV_Table.getCapcity(batteryType) / 3
                    });

                    Model.TargetDetail = $"{modelPreset.EDischargeTarget} ({targetSoC}%)";
                }
            }
            // Step Mode
            else if (Model.Mode == EDischargeMode.Step)
            {
                ModelSetMode_Step modelStep = ViewModelSetMode_Step.Instance.Model;

                for (int index = 0; index < modelStep.Content.ToList().Count; index++)
                {
                    model.PhaseDataList.Add(new PhaseData()
                    {
                        No = (index + 1).ToString(),
                        Mode = "CC",
                        Voltage = Convert.ToDouble(modelStep.Content[index].Voltage),
                        Current = Convert.ToDouble(modelStep.Content[index].Current),
                        CRate = Convert.ToDouble(modelStep.Content[index].CRate),
                    });
                }

                if (modelStep.IsCompleteDischarge)
                {
                    model.EDischargeTarget = EDischargeTarget.Full;

                    int count = model.PhaseDataList.Count;
                    model.PhaseDataList.Add(new PhaseData()
                    {
                        No = (count + 1).ToString(),
                        Mode = "CCCV",
                        Voltage = Convert.ToDouble(0.0),
                        Current = Convert.ToDouble(modelStep.Content.Last().Current),
                        CRate = Convert.ToDouble(modelStep.Content.Last().CRate),
                    });

                    Model.TargetDetail = $"{model.EDischargeTarget}";
                }
                else
                {
                    model.EDischargeTarget = EDischargeTarget.Voltage;

                    Model.TargetDetail = $"{model.EDischargeTarget} ({model.PhaseDataList.Last().Voltage}V)";
                }
            }
            // Simple Mode
            else if (Model.Mode == EDischargeMode.Simple)
            {
                ModelSetMode_Simple modelSimple = ViewModelSetMode_Simple.Instance.Model;

                model.EDischargeTarget = modelSimple.EDischargeTarget;

                if (modelSimple.EDischargeTarget == EDischargeTarget.Full)
                {
                    // 공칭 전압 X
                    if (modelSimple.StandardVoltage == null || modelSimple.StandardVoltage == "")
                    {
                        // 용량 X
                        if (modelSimple.StandardCapacity == null || modelSimple.StandardCapacity == "")
                        {
                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = 0,
                                Current = 100
                            });

                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = 0,
                                Current = 33.4
                            });

                            model.Dvdq = 4;
                        }
                        // 용량 O
                        else
                        {
                            double standardCapacity = Convert.ToDouble(modelSimple.StandardCapacity);

                            if (standardCapacity > 100)
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = 100
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = 33.4
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity / 3
                                });
                            }

                            DischargerDatas dischargerData = ViewModelDischarger.Instance.SelectedModel.DischargerData;
                            double currentVoltage = dischargerData.ReceiveBatteryVoltage;

                            model.Dvdq = 3 * currentVoltage / standardCapacity;
                        }
                    }
                    // 공칭 전압 O
                    else
                    {
                        // 용량 X
                        if (modelSimple.StandardCapacity == null || modelSimple.StandardCapacity == "")
                        {
                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = 0,
                                Current = 100
                            });

                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = 0,
                                Current = 33.4
                            });

                            model.Dvdq = 4;
                        }
                        // 용량 O
                        else
                        {
                            double standardVoltage = Convert.ToDouble(modelSimple.StandardVoltage);
                            double standardCapacity = Convert.ToDouble(modelSimple.StandardCapacity);

                            if (standardCapacity > 100)
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = 100
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = 33.4
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity / 3
                                });
                            }

                            model.Dvdq = 3 * standardVoltage / standardCapacity;
                        }
                    }

                    Model.TargetDetail = $"{model.EDischargeTarget}";
                }
                else if (modelSimple.EDischargeTarget == EDischargeTarget.Zero)
                {
                    // 공칭 전압 X
                    if (modelSimple.StandardVoltage == null || modelSimple.StandardVoltage == "")
                    {
                        // 용량 X
                        if (modelSimple.StandardCapacity == null || modelSimple.StandardCapacity == "")
                        {
                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = 0,
                                Current = 100
                            });

                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = 0,
                                Current = 33.4
                            });

                            model.Dvdq = 4;
                        }
                        // 용량 O
                        else
                        {
                            double standardCapacity = Convert.ToDouble(modelSimple.StandardCapacity);

                            if (standardCapacity > 100)
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = 100
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = 33.4
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity / 3
                                });
                            }

                            DischargerDatas dischargerData = ViewModelDischarger.Instance.SelectedModel.DischargerData;
                            double currentVoltage = dischargerData.ReceiveBatteryVoltage;

                            model.Dvdq = 3 * currentVoltage / standardCapacity;
                        }
                    }
                    // 공칭 전압 O
                    else
                    {
                        // 용량 X
                        if (modelSimple.StandardCapacity == null || modelSimple.StandardCapacity == "")
                        {
                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = 0,
                                Current = 100
                            });

                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = 0,
                                Current = 33.4
                            });

                            model.Dvdq = 4;
                        }
                        // 용량 O
                        else
                        {
                            double standardVoltage = Convert.ToDouble(modelSimple.StandardVoltage);
                            double standardCapacity = Convert.ToDouble(modelSimple.StandardCapacity);

                            if (standardCapacity > 100)
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = 100
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = 33.4
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity / 3
                                });
                            }

                            model.Dvdq = 3 * standardVoltage / standardCapacity;
                        }
                    }

                    Model.TargetDetail = $"{model.EDischargeTarget}";
                }
                else if (modelSimple.EDischargeTarget == EDischargeTarget.Voltage)
                {
                    double targetVoltage = Convert.ToDouble(modelSimple.TargetVoltage);

                    // 공칭 전압 X
                    if (modelSimple.StandardVoltage == null || modelSimple.StandardVoltage == "")
                    {
                        // 용량 X
                        if (modelSimple.StandardCapacity == null || modelSimple.StandardCapacity == "")
                        {
                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = targetVoltage,
                                Current = 100
                            });

                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = targetVoltage,
                                Current = 33.4
                            });

                            model.Dvdq = 4;
                        }
                        // 용량 O
                        else
                        {
                            double standardCapacity = Convert.ToDouble(modelSimple.StandardCapacity);

                            if (standardCapacity > 100)
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = 100
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = 33.4
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = standardCapacity
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = standardCapacity / 3
                                });
                            }

                            DischargerDatas dischargerData = ViewModelDischarger.Instance.SelectedModel.DischargerData;
                            double currentVoltage = dischargerData.ReceiveBatteryVoltage;

                            model.Dvdq = 3 * currentVoltage / standardCapacity;
                        }
                    }
                    // 공칭 전압 O
                    else
                    {
                        // 용량 X
                        if (modelSimple.StandardCapacity == null || modelSimple.StandardCapacity == "")
                        {
                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = targetVoltage,
                                Current = 100
                            });

                            model.PhaseDataList.Add(new PhaseData()
                            {
                                Voltage = targetVoltage,
                                Current = 33.4
                            });

                            model.Dvdq = 4;
                        }
                        // 용량 O
                        else
                        {
                            double standardVoltage = Convert.ToDouble(modelSimple.StandardVoltage);
                            double standardCapacity = Convert.ToDouble(modelSimple.StandardCapacity);

                            if (standardCapacity > 100)
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = 100
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = 33.4
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = standardCapacity
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = standardCapacity / 3
                                });
                            }

                            model.Dvdq = 3 * standardVoltage / standardCapacity;
                        }
                    }

                    Model.TargetDetail = $"{modelSimple.EDischargeTarget} ({targetVoltage}V)";
                }
            }
            return true;
        }

        private DischargeConfig GetDischargeConfig()
        {
            DischargeConfig dischargeConfig = new DischargeConfig();

            string dischargerName;
            short dischargerChannel;

            var dischargerNameListSplit = Model.DischargerName.Split('_');
            if (dischargerNameListSplit.Length > 1)
            {
                dischargerName = dischargerNameListSplit[0];
                dischargerChannel = Convert.ToInt16(dischargerNameListSplit[1]);
            }
            else
            {
                dischargerName = dischargerNameListSplit[0];
                dischargerChannel = 1;
            }

            // Discharger
            TableDischargerInfo tableDischargerInfo = SqliteDischargerInfo.GetData(dischargerName);
            dischargeConfig.DischargerName = tableDischargerInfo.DischargerName;
            dischargeConfig.DischargerModel = tableDischargerInfo.Model;
            dischargeConfig.DischargeType = tableDischargerInfo.Type;
            dischargeConfig.DischargeChannel = dischargerChannel;
            dischargeConfig.SpecVoltage = tableDischargerInfo.SpecVoltage;
            dischargeConfig.SpecCurrent = tableDischargerInfo.SpecCurrent;
            dischargeConfig.IPAddress = tableDischargerInfo.IpAddress;
            dischargeConfig.IsTempModule = tableDischargerInfo.IsTempModule;
            dischargeConfig.TempModuleComport = tableDischargerInfo.TempModuleComPort;
            dischargeConfig.TempModuleChannel = tableDischargerInfo.TempModuleChannel;
            dischargeConfig.Tempchannel = tableDischargerInfo.TempChannel;

            // Server DB 사용 (통합 관제 연동)
            if (!ViewModelLogin.Instance.IsLocalDb())
            {
                dischargeConfig.MachineCode = ViewModelLogin.Instance.Model.MachineCode;
            }

            // Discharge Mode
            dischargeConfig.EDischargeMode = Model.Mode;

            // Pre-set Mode
            if (Model.Mode == EDischargeMode.Preset)
            {
                ModelSetMode_Preset modelPreset = ViewModelSetMode_Preset.Instance.Model;
                var batteryType = modelPreset.SelectedBatteryType;
                var currentSoC = modelPreset.CurrentSoC;
                var dischargeTarget = modelPreset.EDischargeTarget;
                var targetVoltage = modelPreset.TargetVoltage;
                var targetSoC = modelPreset.TargetSoC;

                // Discharge Mode Config
                dischargeConfig.BatteryType = batteryType;
                dischargeConfig.CurrentSoC = currentSoC;

                // Discharge Target
                dischargeConfig.EDischargeTarget = dischargeTarget;

                if (dischargeTarget == EDischargeTarget.Voltage)
                {
                    dischargeConfig.TargetValue = targetVoltage;
                }
                else if (dischargeTarget == EDischargeTarget.SoC)
                {
                    dischargeConfig.TargetValue = targetSoC;
                }
            }
            // Step Mode
            else if (Model.Mode == EDischargeMode.Step)
            {
                ModelSetMode_Step modelStep = ViewModelSetMode_Step.Instance.Model;
                var standardCapacity = modelStep.StandardCapacity;
                var isCompleteDischarge = modelStep.IsCompleteDischarge;

                List<PhaseData> phaseDataList = new List<PhaseData>();
                foreach (var phaseData in modelStep.Content)
                {
                    double voltageInFor = 0;
                    double currentInFor = 0;
                    double cRateInFor = 0;

                    try
                    {
                        voltageInFor = Convert.ToDouble(phaseData.Voltage);
                        currentInFor = Convert.ToDouble(phaseData.Current);
                        cRateInFor = Convert.ToDouble(phaseData.CRate);
                    }
                    catch (Exception ex)
                    {
                        voltageInFor = 0;
                        currentInFor = 0;
                        cRateInFor = 0;

                        MessageBox.Show(
                            $"Error 발생\n\n" +
                            $"ClassName: {this.GetType().Name}\n" +
                            $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                            $"Exception: {ex.Message}");
                    }

                    phaseDataList.Add(new PhaseData()
                    {
                        Voltage = voltageInFor,
                        Current = currentInFor,
                        CRate = cRateInFor,
                    });
                }

                // Discharge Mode Config
                dischargeConfig.StandardCapacity = standardCapacity;
                dischargeConfig.PhaseDataList = phaseDataList;

                // Discharge Target
                if (isCompleteDischarge)
                {
                    dischargeConfig.EDischargeTarget = EDischargeTarget.Full;
                }
                else
                {
                    dischargeConfig.EDischargeTarget = EDischargeTarget.Voltage;
                    dischargeConfig.TargetValue = phaseDataList.Last().Voltage.ToString();
                }
            }
            // Simple Mode
            else if (Model.Mode == EDischargeMode.Simple)
            {
                ModelSetMode_Simple modelSimple = ViewModelSetMode_Simple.Instance.Model;
                var standartVoltage = modelSimple.StandardVoltage;
                var standardCapacity = modelSimple.StandardCapacity;
                var dischargeTarget = modelSimple.EDischargeTarget;
                var targetVoltage = modelSimple.TargetVoltage;

                // Discharge Mode Config
                dischargeConfig.StandartVoltage = standartVoltage;
                dischargeConfig.StandardCapacity = standardCapacity;

                // Discharge Target
                dischargeConfig.EDischargeTarget = dischargeTarget;

                if (dischargeTarget == EDischargeTarget.Voltage)
                {
                    dischargeConfig.TargetValue = targetVoltage;
                }
            }

            // SafetyCondition
            ModelSetMode_SafetyCondition modelSafetyCondition = ViewModelSetMode_SafetyCondition.Instance.Model;

            dischargeConfig.SafetyVoltageMax = modelSafetyCondition.VoltageMax;
            dischargeConfig.SafetyVoltageMin = modelSafetyCondition.VoltageMin;
            dischargeConfig.SafetyCurrentMax = modelSafetyCondition.CurrentMax;
            dischargeConfig.SafetyCurrentMin = modelSafetyCondition.CurrentMin;
            dischargeConfig.SafetyTempMax = modelSafetyCondition.TempMax;
            dischargeConfig.SafetyTempMin = modelSafetyCondition.TempMin;

            return dischargeConfig;
        }
    }
}
