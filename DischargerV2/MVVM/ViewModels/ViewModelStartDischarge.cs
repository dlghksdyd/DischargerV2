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

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelControlDischarge : BindableBase
    {
        #region Command
        #endregion

        #region Model
        public ModelStartDischarge Model { get; set; } = new ModelStartDischarge();
        #endregion

        #region Property
        private System.Timers.Timer DischargeTimer = null;

        private static ViewModelControlDischarge _instance;
        public static ViewModelControlDischarge Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelControlDischarge();
                }
                return _instance;
            }
        }
        #endregion

        public ViewModelControlDischarge()
        {
            _instance = this;
        }

        public void StartDischarge()
        {
            // 초기화
            Model.PhaseNo = 0;
            Model.IsEnterLastPhase = false;

            ViewModelDischarger.Instance.StartDischarger(new StartDischargerCommandParam()
            {
                DischargerName = Model.DischargerName,
                Voltage = Model.PhaseDataList[Model.PhaseNo].Voltage,
                Current = Model.PhaseDataList[Model.PhaseNo].Current,
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
            ViewModelDischarger.Instance.PauseDischarger(Model.DischargerName);
        }

        public void ResumeDischarge()
        {
            ViewModelDischarger.Instance.StartDischarger(new StartDischargerCommandParam()
            {
                DischargerName = Model.DischargerName,
                Voltage = Model.PhaseDataList[Model.PhaseNo].Voltage,
                Current = Model.PhaseDataList[Model.PhaseNo].Current,
            });

            DischargeTimer?.Stop();
            DischargeTimer = null;
            DischargeTimer = new System.Timers.Timer();
            DischargeTimer.Interval = 1000;
            DischargeTimer.Elapsed += OneSecondTimer_Elapsed;
            DischargeTimer.Start();
        }

        public void StopDischarge()
        {
            ViewModelDischarger.Instance.StopDischarger(Model.DischargerName);
        }

        private void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ViewModelDischarger viewModelDischarger = ViewModelDischarger.Instance;

            EDischargerModel model = viewModelDischarger.Model.DischargerInfos[Model.DischargerIndex].Model;
            EDischargerState receiveState = viewModelDischarger.Model.DischargerStates[Model.DischargerIndex];
            
            double receiveVoltage = viewModelDischarger.Model.DischargerDatas[Model.DischargerIndex].ReceiveBatteryVoltage;
            double receiveCurrent = viewModelDischarger.Model.DischargerDatas[Model.DischargerIndex].ReceiveDischargeCurrent;

            double safetyTempMin = viewModelDischarger.Model.DischargerDatas[Model.DischargerIndex].SafetyTempMin;
            double safetyTempMax = viewModelDischarger.Model.DischargerDatas[Model.DischargerIndex].SafetyTempMax;

            if (Model.IsEnterLastPhase == false)
            {
                // 모델별 온도 안전 조건 확인하는 부분
                if (model == EDischargerModel.MBDC)
                {
                    double receiveTemp = ViewModelTempModule.Instance.GetTempData(Model.DischargerName);

                    if (receiveTemp < safetyTempMin || receiveTemp > safetyTempMax)
                    {
                        viewModelDischarger.SetDischargerState(Model.DischargerName, EDischargerState.SafetyOutOfRange);
                        return;
                    }
                }

                // 타겟 전압에 도달했을 경우 Phase 상승
                if (receiveVoltage <= Model.PhaseDataList[Model.PhaseNo].Voltage)
                {
                    Model.PhaseNo++;

                    // 모든 Phase 끝났을 때
                    if (Model.PhaseNo == Model.PhaseDataList.Count)
                    {
                        viewModelDischarger.StopDischarger(Model.DischargerName);

                        Model.IsEnterLastPhase = true;
                    }
                    else
                    {
                        viewModelDischarger.StartDischarger(new StartDischargerCommandParam()
                        {
                            DischargerName = Model.DischargerName,
                            Voltage = Model.PhaseDataList[Model.PhaseNo].Voltage,
                            Current = Model.PhaseDataList[Model.PhaseNo].Current,
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
                        viewModelDischarger.StopDischarger(Model.DischargerName);
                    }
                }
            }
        }
    }
}
