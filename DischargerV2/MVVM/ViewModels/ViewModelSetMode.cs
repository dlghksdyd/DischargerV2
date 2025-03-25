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
using static DischargerV2.MVVM.Models.ModelStartDischarge;

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

        private Dictionary<string, ViewModelControlDischarge> _viewModelDictionary = new Dictionary<string, ViewModelControlDischarge>();
        public Dictionary<string, ViewModelControlDischarge> ViewModelDictionary
        {
            get
            {
                return _viewModelDictionary;
            }
            set
            {
                SetProperty(ref _viewModelDictionary, value);
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
            Model = ModelDictionary[dischargerName];

            ViewModelSetMode_Preset.Instance.SetDischargerName(dischargerName);
            ViewModelSetMode_Step.Instance.SetDischargerName(dischargerName);
            ViewModelSetMode_Simple.Instance.SetDischargerName(dischargerName);
            ViewModelSetMode_SafetyCondition.Instance.SetDischargerName(dischargerName);
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
            List<string> dischargerNameList = ViewModelDischarger.Instance.Model.DischargerNameList.ToList();
            List<DischargerInfo> dischargerInfoList = ViewModelDischarger.Instance.Model.DischargerInfos.ToList();

            for (int index = 0; index < dischargerNameList.Count; index++)
            {
                string dischargerName = dischargerNameList[index];
                DischargerInfo dischargerInfo = dischargerInfoList[index];
                TableDischargerInfo tableTischargerInfo = SqliteDischargerInfo.GetData().Find(x => x.DischargerName == dischargerName);

                ModelSetMode modelSetMode = new ModelSetMode();
                modelSetMode.DischargerIndex = index;
                modelSetMode.DischargerName = dischargerName;
                modelSetMode.TempModuleIndex = ViewModelTempModule.Instance.GetTempModuleDataIndex(tableTischargerInfo.TempModuleComPort);
                modelSetMode.TempModuleChannel = Convert.ToInt32(tableTischargerInfo.TempChannel);
                ViewModelSetMode.Instance.ModelDictionary.Add(dischargerName, modelSetMode);

                ModelSetMode_Preset modelSetMode_Preset = new ModelSetMode_Preset();
                ViewModelSetMode_Preset.Instance.ModelDictionary.Add(dischargerName, modelSetMode_Preset);

                ModelSetMode_Step modelSetMode_Step = new ModelSetMode_Step();
                modelSetMode_Step.DischargerName = dischargerName;
                modelSetMode_Step.Content.Add(new ModelSetMode_StepData());
                ViewModelSetMode_Step.Instance.ModelDictionary.Add(dischargerName, modelSetMode_Step);

                ModelSetMode_Simple modelSetMode_Simple = new ModelSetMode_Simple();
                modelSetMode_Simple.DischargerName = dischargerName;
                ViewModelSetMode_Simple.Instance.ModelDictionary.Add(dischargerName, modelSetMode_Simple);

                ModelSetMode_SafetyCondition modelSetMode_SafetyCondition = new ModelSetMode_SafetyCondition();
                modelSetMode_SafetyCondition.DischargerName = dischargerName;
                modelSetMode_SafetyCondition.VoltageMin = dischargerInfo.SafetyVoltageMin.ToString();
                modelSetMode_SafetyCondition.VoltageMax = dischargerInfo.SafetyVoltageMax.ToString();
                modelSetMode_SafetyCondition.CurrentMin = dischargerInfo.SafetyCurrentMin.ToString();
                modelSetMode_SafetyCondition.CurrentMax = dischargerInfo.SafetyCurrentMax.ToString();
                modelSetMode_SafetyCondition.TempMin = dischargerInfo.SafetyTempMin.ToString();
                modelSetMode_SafetyCondition.TempMax = dischargerInfo.SafetyTempMax.ToString();
                ViewModelSetMode_SafetyCondition.Instance.ModelDictionary.Add(dischargerName, modelSetMode_SafetyCondition);
            }
            ViewModelSetMode_Preset.Instance.SetBatteryType();
        }

        private void InitializeViewModelDictionary()
        {
            // 기존 값 초기화
            ViewModelDictionary.Clear();

            // Discharger에서 관련 값 받아와 사용
            List<string> dischargerNameList = ViewModelDischarger.Instance.Model.DischargerNameList.ToList();
            List<DischargerInfo> dischargerInfoList = ViewModelDischarger.Instance.Model.DischargerInfos.ToList();

            for (int index = 0; index < dischargerNameList.Count; index++)
            {
                string dischargerName = dischargerNameList[index];

                ModelStartDischarge modelStartDischarge = new ModelStartDischarge();
                modelStartDischarge.DischargerName = dischargerName;

                ViewModelDictionary.Add(dischargerName, new ViewModelControlDischarge()
                {
                    Model = modelStartDischarge
                });
            }
        }

        public void StartDischarge()
        {
            // 설정 값 적용
            SetDischargerName(Model.DischargerName);

            if (!CheckModeNTarget()) return;
            if (!CheckNSetSafetyCondition()) return;
            if (!CalculateTarget(out ModelStartDischarge model)) return;

            // 방전 모드 및 목표 설정 후 방전 시작
            if (ViewModelDictionary.ContainsKey(Model.DischargerName))
            {
                ViewModelDictionary[Model.DischargerName].Model = model;
                ViewModelDictionary[Model.DischargerName].StartDischarge();

                // SetMode -> Monitor 화면 전환
                ViewModelMain.Instance.SetIsStartedArray(true);
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

                if (modelPreset.EDischargeType == Enums.EDischargeTarget.Voltage)
                {
                    if (modelPreset.TargetVoltage == null || modelPreset.TargetVoltage == "")
                    {
                        MessageBox.Show("Target Voltage (V): 필수 정보입니다.");
                        return false;
                    }
                }
                else if (modelPreset.EDischargeType == Enums.EDischargeTarget.SoC)
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

                if (modelSimple.EDischargeType == Enums.EDischargeTarget.Voltage)
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
            DischargerInfo dischargerInfo = ViewModelDischarger.Instance.Model.DischargerInfos[Model.DischargerIndex];
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
            else if (Convert.ToDouble(modelSafetyCondition.CurrentMin) < dischargerInfo.SafetyCurrentMin)
            {
                MessageBox.Show("Current Min (A): 설정 값이 스펙 범위를 벗어났습니다.");
                return false;
            }
            else if (modelSafetyCondition.CurrentMax == null || modelSafetyCondition.CurrentMax == "")
            {
                MessageBox.Show("Current Max (A): 필수 정보입니다.");
                return false;
            }
            else if (Convert.ToDouble(modelSafetyCondition.CurrentMax) > dischargerInfo.SafetyCurrentMax)
            {
                MessageBox.Show("Current Max (A): 설정 값이 스펙 범위를 벗어났습니다.");
                return false;
            }
            else if (Convert.ToDouble(modelSafetyCondition.CurrentMin) >= Convert.ToDouble(modelSafetyCondition.CurrentMax))
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

                model.Target = modelPreset.EDischargeType;

                // Full Discharge, 0V Discharge
                if (modelPreset.EDischargeType == EDischargeTarget.Full ||
                    modelPreset.EDischargeType == EDischargeTarget.Zero)
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
                }
                // Target Voltage
                else if (modelPreset.EDischargeType == EDischargeTarget.Voltage)
                {
                    int tagetVoltage = Convert.ToInt32(modelPreset.TargetVoltage);

                    model.PhaseDataList.Add(new PhaseData()
                    {
                        Voltage = OCV_Table.getPhase2Volt(batteryType),
                        Current = OCV_Table.getCapcity(batteryType) / 3
                    });

                    model.PhaseDataList.Add(new PhaseData()
                    {
                        Voltage = tagetVoltage,
                        Current = OCV_Table.getCapcity(batteryType) / 3
                    });
                }
                // Target SoC
                else if (modelPreset.EDischargeType == EDischargeTarget.SoC)
                {
                    int targetSoC = Convert.ToInt32(modelPreset.TargetSoC);

                    model.PhaseDataList.Add(new PhaseData()
                    {
                        Voltage = OCV_Table.getTargetVolt(batteryType, targetSoC),
                        Current = OCV_Table.getCapcity(batteryType) / 3
                    });
                }
            }
            // Step Mode
            else if (Model.Mode == EDischargeMode.Step)
            {
                ModelSetMode_Step modelStep = ViewModelSetMode_Step.Instance.Model;

                if (modelStep.IsCompleteDischarge)
                {
                    model.Target = EDischargeTarget.Zero;
                }
                else
                {
                    model.Target = EDischargeTarget.Voltage;
                }

                foreach (var stepData in modelStep.Content)
                {
                    if (stepData == null) continue;

                    model.PhaseDataList.Add(new PhaseData()
                    {
                        Voltage = Convert.ToDouble(stepData.Voltage),
                        Current = Convert.ToDouble(stepData.Current)
                    });
                }
            }
            // Simple Mode
            else if (Model.Mode == EDischargeMode.Simple)
            {
                ModelSetMode_Simple modelSimple = ViewModelSetMode_Simple.Instance.Model;

                model.Target = modelSimple.EDischargeType;

                if (modelSimple.EDischargeType == EDischargeTarget.Full)
                {
                    // 공칭 전압 X
                    if (modelSimple.StandardVoltage == null || modelSimple.StandardVoltage == "")
                    {
                        // 용량 X
                        if (modelSimple.StandardCapacity == null ||  modelSimple.StandardCapacity == "")
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
                                    Current = 33.4
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity / 3
                                });
                            }

                            DischargerDatas dischargerData = ViewModelDischarger.Instance.Model.DischargerDatas[Model.DischargerIndex];
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
                                    Voltage = standardVoltage,
                                    Current = 100
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = 0,
                                    Current = standardCapacity / 3
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = standardVoltage,
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
                }
                else if (modelSimple.EDischargeType == EDischargeTarget.Zero)
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
                                    Current = standardCapacity / 3
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

                            DischargerDatas dischargerData = ViewModelDischarger.Instance.Model.DischargerDatas[Model.DischargerIndex];
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
                                    Voltage = standardVoltage,
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
                                    Voltage = standardVoltage,
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
                }
                else if (modelSimple.EDischargeType == EDischargeTarget.Voltage)
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
                                    Current = 33.4
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = standardCapacity / 3
                                });
                            }

                            DischargerDatas dischargerData = ViewModelDischarger.Instance.Model.DischargerDatas[Model.DischargerIndex];
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
                                    Current = 33.4
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = standardVoltage,
                                    Current = 33.4
                                });
                            }
                            else
                            {
                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = targetVoltage,
                                    Current = standardCapacity / 3
                                });

                                model.PhaseDataList.Add(new PhaseData()
                                {
                                    Voltage = standardVoltage,
                                    Current = standardCapacity / 3
                                });
                            }

                            model.Dvdq = 3 * standardVoltage / standardCapacity;
                        }
                    }
                }
            }
            return true;
        }
    }
}
