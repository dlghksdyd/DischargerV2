using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using Microsoft.WindowsAPICodePack.Shell;
using Prism.Commands;
using Prism.Mvvm;
using Serial.Client.TempModule;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelDischarger
    {
        #region Command

        public DelegateCommand InitializeDischargerCommand { get; set; }
        public DelegateCommand FinalizeDischargerCommand { get; set; }

        #endregion

        private System.Timers.Timer OneSecondTimer { get; set; } = null;

        /// <summary>
        /// Key: discharger name
        /// </summary>
        private Dictionary<string, EthernetClientDischarger> _clients = new Dictionary<string, EthernetClientDischarger>();

        private static ViewModelDischarger _instance = null;

        public ModelDischarger Model { get; set; } = new ModelDischarger();

        public static ViewModelDischarger Instance()
        {
            return _instance;
        }

        public ViewModelDischarger()
        {
            _instance = this;

            InitializeDischarger();

            InitializeDischargerCommand = new DelegateCommand(InitializeDischarger);
            FinalizeDischargerCommand = new DelegateCommand(FinalizeDischarger);
        }

        private void InitializeDischarger()
        {
            FinalizeDischarger();

            List<TableDischargerInfo> infos = SqliteDischargerInfo.GetData();

            foreach (var info in infos)
            {
                Model.DischargerDatas.Add(new DischargerDatas());
                Model.DischargerStates.Add(EDischargerState.None);

                var dischargerInfo = InitializeDischargerInfos(info.DischargerName);
                InitializeDischargerClients(dischargerInfo);

                Model.DischargerNameList.Add(info.DischargerName);
            }

            OneSecondTimer?.Stop();
            OneSecondTimer = new System.Timers.Timer();
            OneSecondTimer.Elapsed += CopyDataFromDischargerClientToModel;
            OneSecondTimer.Interval = 1000;
            OneSecondTimer.Start();
        }

        private void FinalizeDischarger()
        {
            OneSecondTimer?.Stop();
            OneSecondTimer = null;

            foreach (var client in _clients)
            {
                client.Value.Stop();
            }
            _clients.Clear();

            Model.DischargerNameList.Clear();
            Model.SelectedDischargerName = string.Empty;
            Model.DischargerDatas.Clear();
            Model.DischargerInfos.Clear();
            Model.DischargerStates.Clear();
        }

        private DischargerInfo InitializeDischargerInfos(string name)
        {
            List<TableDischargerInfo> infoTable = SqliteDischargerInfo.GetData();
            TableDischargerInfo info = infoTable.Find(x => x.DischargerName == name);

            List<TableDischargerModel> modelTable = SqliteDischargerModel.GetData();
            modelTable = (from one in modelTable where one.Model == info.Model select one).ToList();
            modelTable = (from one in modelTable where one.Type == info.Type select one).ToList();
            modelTable = (from one in modelTable where one.Channel == info.DischargerChannel select one).ToList();
            modelTable = (from one in modelTable where one.SpecVoltage == info.SpecVoltage select one).ToList();
            modelTable = (from one in modelTable where one.SpecCurrent == info.SpecCurrent select one).ToList();
            TableDischargerModel model = modelTable.First();

            DischargerInfo dischargerInfo = new DischargerInfo();
            dischargerInfo.Name = name;
            dischargerInfo.Channel = info.DischargerChannel;
            dischargerInfo.IpAddress = IPAddress.Parse(info.IpAddress);
            dischargerInfo.EthernetPort = 10004;
            dischargerInfo.TimeOutMs = 5000;
            dischargerInfo.SpecVoltage = info.SpecVoltage;
            dischargerInfo.SpecCurrent = info.SpecCurrent;
            dischargerInfo.SafetyVoltageMax = model.SafetyVoltMax;
            dischargerInfo.SafetyVoltageMin = model.SafetyVoltMin;
            dischargerInfo.SafetyCurrentMax = model.SafetyCurrentMax;
            dischargerInfo.SafetyCurrentMin = model.SafetyCurrentMin;
            dischargerInfo.SafetyTempMax = model.SafetyTempMax;
            dischargerInfo.SafetyTempMin = model.SafetyTempMin;
            Model.DischargerInfos.Add(dischargerInfo);

            return dischargerInfo;
        }

        private void InitializeDischargerClients(DischargerInfo info)
        {
            EthernetClientDischargerStart parameters = new EthernetClientDischargerStart();
            parameters.DischargerName = info.Name;
            parameters.DischargerChannel = info.Channel;
            parameters.IpAddress = info.IpAddress;
            parameters.EthernetPort = info.EthernetPort;
            parameters.TimeOutMs = info.TimeOutMs;
            parameters.SafetyVoltageMax = info.SafetyVoltageMax;
            parameters.SafetyVoltageMin = info.SafetyVoltageMin;
            parameters.SafetyCurrentMax = info.SafetyCurrentMax;
            parameters.SafetyCurrentMin = info.SafetyCurrentMin;
            _clients[info.Name] = new EthernetClientDischarger();
            _clients[info.Name].Start(parameters);
        }

        private void CopyDataFromDischargerClientToModel(object sender, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < Model.DischargerNameList.Count; i++)
            {
                Model.DischargerDatas[i] = _clients[Model.DischargerNameList[i]].GetDatas();
                Model.DischargerStates[i] = _clients[Model.DischargerNameList[i]].GetState();
            }
        }
    }
}
