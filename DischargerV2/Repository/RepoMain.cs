using Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ethernet.Client.Discharger;
using Serial.Client.TempModule;
using Sqlite.Common;
using System.Net;
using Ethernet.Client.Common;
using System.Windows.Controls;
using System.Threading;
using Repository.Structures;

namespace Repository.Common
{
    public class RepoMain
    {
        /// <summary>
        /// Key: DischargerName
        /// </summary>
        public static Dictionary<string, DischargerConf> Dischargers = new Dictionary<string, DischargerConf>();

        /// <summary>
        /// Key: DischargerName
        /// </summary>
        public static Dictionary<string, OpPresetMode> PresetModes = new Dictionary<string, OpPresetMode>();
        /// <summary>
        /// Key: DischargerName
        /// </summary>
        public static Dictionary<string, OpStepMode> StepModes = new Dictionary<string, OpStepMode>();
        /// <summary>
        /// Key: DischargerName
        /// </summary>
        public static Dictionary<string, OpSimpleMode> SimpleModes = new Dictionary<string, OpSimpleMode>();

        /// <summary>
        /// Key: DischargerName
        /// </summary>
        public static Dictionary<string, EthernetClientDischarger> DischargerClients = new Dictionary<string, EthernetClientDischarger>();

        /// <summary>
        /// Key: TempModule COM port string
        /// </summary>
        public static Dictionary<string, SerialClientTempModule> TempModuleClients = new Dictionary<string, SerialClientTempModule>();
        
        /// <summary>
        /// Repository 업데이트
        /// </summary>
        public static void UpdateDataRepository()
        {
            List<TableDischargerInfo> infos = SqliteDischargerInfo.GetData();

            foreach (TableDischargerInfo info in infos)
            {
                if (!Dischargers.ContainsKey(info.DischargerName))
                {
                    InitRepoDischargerInfo(info.DischargerName);

                    PresetModes[info.DischargerName] = new OpPresetMode();
                    StepModes[info.DischargerName] = new OpStepMode();
                    SimpleModes[info.DischargerName] = new OpSimpleMode();

                    EthernetClientDischargerStart dischargerClientStart = new EthernetClientDischargerStart();
                    dischargerClientStart.DischargerName = info.DischargerName;
                    dischargerClientStart.DischargerChannel = info.DischargerChannel;
                    dischargerClientStart.IpAddress = IPAddress.Parse(info.IpAddress);
                    dischargerClientStart.EthernetPort = 10004;
                    dischargerClientStart.TimeOutMs = 6000;
                    DischargerClients[info.DischargerName] = new EthernetClientDischarger(dischargerClientStart);
                }

                if (!TempModuleClients.ContainsKey(info.TempModuleComPort))
                {
                    SerialClientTempModuleStart tempModuleClientStart = new SerialClientTempModuleStart();
                    tempModuleClientStart.ComPort = info.TempModuleComPort;
                    tempModuleClientStart.BaudRate = 9600;
                    tempModuleClientStart.Encoding = Encoding.UTF8;
                    tempModuleClientStart.TempModuleChannel = info.TempModuleChannel;
                    tempModuleClientStart.TempChannelCount = 8;
                    TempModuleClients[info.TempModuleComPort] = new SerialClientTempModule(tempModuleClientStart);
                }
            }
        }

        private static void InitRepoDischargerInfo(string name)
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

            Dischargers[name] = new DischargerConf(name);
            Dischargers[name].DischargerName = name;
            Dischargers[name].DischargerModel = info.Model;
            Dischargers[name].DischargeType = info.Type;
            Dischargers[name].DischargerChannel = info.DischargerChannel;
            Dischargers[name].IpAddress = info.IpAddress;
            Dischargers[name].TempModuleComPort = info.TempModuleComPort;
            Dischargers[name].TempModuleChannel = info.TempModuleChannel;
            Dischargers[name].TempChannel = info.TempChannel;
            Dischargers[name].SpecVoltage = info.SpecVoltage;
            Dischargers[name].SpecCurrent = info.SpecCurrent;
            Dischargers[name].SafetyVoltageMax = model.SafetyVoltMax;
            Dischargers[name].SafetyVoltageMin = model.SafetyVoltMin;
            Dischargers[name].SafetyCurrentMax = model.SafetyCurrentMax;
            Dischargers[name].SafetyCurrentMin = model.SafetyCurrentMin;
            Dischargers[name].SafetyTempMax = model.SafetyTempMax;
            Dischargers[name].SafetyTempMin = model.SafetyTempMin;
        }

        /// <summary>
        /// Repository 초기화
        /// </summary>
        public static void ResetDataRepository()
        {
            Dischargers.Clear();
            PresetModes.Clear();
            StepModes.Clear();
            SimpleModes.Clear();

            foreach (var item in DischargerClients)
            {
                item.Value.Dispose();
            }
            DischargerClients.Clear();

            foreach (var item in TempModuleClients)
            {
                item.Value.Dispose();
            }
            TempModuleClients.Clear();
        }
    }
}
