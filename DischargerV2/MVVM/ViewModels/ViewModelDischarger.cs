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

            InitializeDischargerCommand = new DelegateCommand(InitializeDischarger);
            FinalizeDischargerCommand = new DelegateCommand(FinalizeDischarger);
        }

        private void InitializeDischarger()
        {
            FinalizeDischarger();

            List<TableDischargerInfo> infos = SqliteDischargerInfo.GetData();

            foreach (var info in infos)
            {
                Model.DischargerDatas[info.DischargerName] = new DischargerDatas();
                Model.DischargerStates[info.DischargerName] = EDischargerState.None;
                InitializeDischargerInfos(info.DischargerName);
                InitializeDischargerClients(info.DischargerName);
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

            Model.SelectedDischargerName = string.Empty;
            Model.DischargerDatas.Clear();
            Model.DischargerInfos.Clear();
            Model.DischargerStates.Clear();
        }

        private void InitializeDischargerInfos(string name)
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

            Model.DischargerInfos[name] = new DischargerInfo();
            Model.DischargerInfos[name].Name = name;
            Model.DischargerInfos[name].Channel = info.DischargerChannel;
            Model.DischargerInfos[name].IpAddress = IPAddress.Parse(info.IpAddress);
            Model.DischargerInfos[name].EthernetPort = 10004;
            Model.DischargerInfos[name].TimeOutMs = 5000;
            Model.DischargerInfos[name].SpecVoltage = info.SpecVoltage;
            Model.DischargerInfos[name].SpecCurrent = info.SpecCurrent;
            Model.DischargerInfos[name].SafetyVoltageMax = model.SafetyVoltMax;
            Model.DischargerInfos[name].SafetyVoltageMin = model.SafetyVoltMin;
            Model.DischargerInfos[name].SafetyCurrentMax = model.SafetyCurrentMax;
            Model.DischargerInfos[name].SafetyCurrentMin = model.SafetyCurrentMin;
            Model.DischargerInfos[name].SafetyTempMax = model.SafetyTempMax;
            Model.DischargerInfos[name].SafetyTempMin = model.SafetyTempMin;
        }

        private void InitializeDischargerClients(string name)
        {
            EthernetClientDischargerStart parameters = new EthernetClientDischargerStart();
            parameters.DischargerName = Model.DischargerInfos[name].Name;
            parameters.DischargerChannel = Model.DischargerInfos[name].Channel;
            parameters.IpAddress = Model.DischargerInfos[name].IpAddress;
            parameters.EthernetPort = Model.DischargerInfos[name].EthernetPort;
            parameters.TimeOutMs = Model.DischargerInfos[name].TimeOutMs;
            parameters.SafetyVoltageMax = Model.DischargerInfos[name].SafetyVoltageMax;
            parameters.SafetyVoltageMin = Model.DischargerInfos[name].SafetyVoltageMin;
            parameters.SafetyCurrentMax = Model.DischargerInfos[name].SafetyCurrentMax;
            parameters.SafetyCurrentMin = Model.DischargerInfos[name].SafetyCurrentMin;
            _clients[name] = new EthernetClientDischarger();
            _clients[name].Start(parameters);
        }

        private void CopyDataFromDischargerClientToModel(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<string> dischargerNameList = new List<string>();
            foreach (var name in Model.DischargerDatas.Keys)
            {
                dischargerNameList.Add(name);
            }

            foreach (var name in dischargerNameList)
            {
                Model.DischargerDatas[name] = _clients[name].GetDatas();
                Model.DischargerStates[name] = _clients[name].GetState();
            }
        }
    }
}
