using Discharger.MVVM.Repository;
using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using Microsoft.WindowsAPICodePack.Shell;
using Prism.Commands;
using Serial.Client.TempModule;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DischargerV2.MVVM.ViewModels
{
    public class DischargerViewModel
    {
        #region Command

        public DelegateCommand InitializeDischargerCommand { get; set; }

        #endregion

        /// <summary>
        /// Key: discharger name
        /// </summary>
        private Dictionary<string, EthernetClientDischarger> Clients { get; set; } = new Dictionary<string, EthernetClientDischarger>();
        /// <summary>
        /// Key: discharger name
        /// </summary>
        public Dictionary<string, DischargerDataModel> DischargerData { get; private set; } = new Dictionary<string, DischargerDataModel>();

        public System.Timers.Timer OneSeccondTimer { get; private set; } = null;

        public DischargerViewModel()
        {
            InitializeDischargerCommand = new DelegateCommand(InitializeDischarger);
        }

        private void InitializeDischarger()
        {
            List<TableDischargerInfo> infos = SqliteDischargerInfo.GetData();

            foreach (var info in infos)
            {
                List<TableDischargerModel> models = SqliteDischargerModel.GetData();
                models = (from one in models where one.Model == info.Model select one).ToList();
                models = (from one in models where one.Type == info.Type select one).ToList();
                models = (from one in models where one.SpecCurrent == info.SpecCurrent select one).ToList();
                models = (from one in models where one.SpecVoltage == info.SpecVoltage select one).ToList();

                EthernetClientDischargerStart parameters = new EthernetClientDischargerStart();
                parameters.DischargerName = info.DischargerName;
                parameters.DischargerChannel = info.DischargerChannel;
                parameters.IpAddress = IPAddress.Parse(info.IpAddress);
                parameters.EthernetPort = 10004;
                parameters.TimeOutMs = 3000;
                parameters.SafetyVoltageMax = info.SpecVoltage + 2;
                parameters.SafetyVoltageMin = 0 - 2;
                parameters.SafetyCurrentMax = info.SpecCurrent + 2;
                parameters.SafetyCurrentMin = 0 - 2;

                Clients[info.DischargerName] = new EthernetClientDischarger();
                var result = Clients[info.DischargerName].Start(parameters);
                if (result)
                {
                    DischargerData[info.DischargerName] = new DischargerDataModel();
                }
            }

            OneSeccondTimer?.Stop();
            OneSeccondTimer = new System.Timers.Timer();
            OneSeccondTimer.Elapsed += OneSecondTimer_Elapsed;
            OneSeccondTimer.Interval = 1000;
            OneSeccondTimer.Start();
        }

        private void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var data in DischargerData)
            {
                DischargerDatas dischargerDatas = Clients[data.Key].GetDatas();
                data.Value.Voltage = dischargerDatas.ReceiveBatteryVoltage;
                data.Value.Current = dischargerDatas.ReceiveDischargeCurrent;
            }
        }
    }
}
