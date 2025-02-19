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
using static DischargerV2.MVVM.Models.ModelStartDischargeConfig;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelStartDischarge_Preset : BindableBase
    {
        #region Command
        #endregion

        #region Model
        public ModelStartDischargeConfig Model { get; set; } = new ModelStartDischargeConfig();
        #endregion

        #region Property
        private System.Timers.Timer DischargeTimer = null;
        #endregion

        public ViewModelStartDischarge_Preset()
        {

        }

        public void StartDischarge()
        {
            ViewModelDischarger viewModelDischarger = ViewModelDischarger.Instance;

            // 초기화
            Model.PhaseNo = 0;
            Model.IsEnterLastPhase = false;

            viewModelDischarger.StartDischarger(new StartDischargerCommandParam()
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

        private void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ViewModelDischarger viewModelDischarger = ViewModelDischarger.Instance;

            EDischargerState receiveState = viewModelDischarger.Model.DischargerStates[Model.DischargerIndex];
            double receiveVoltage = viewModelDischarger.Model.DischargerDatas[Model.DischargerIndex].ReceiveBatteryVoltage;
            double receiveCurrent = viewModelDischarger.Model.DischargerDatas[Model.DischargerIndex].ReceiveDischargeCurrent;

            if (Model.IsEnterLastPhase == false)
            {
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
