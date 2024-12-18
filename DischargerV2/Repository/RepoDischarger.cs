using Ethernet.Client.Discharger;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Discharger.MVVM.Repository
{
    public static class RepoDischarger
    {
        public static string SelectedDischargerName = string.Empty;

        /// <summary>
        /// Key: Discharger Name
        /// </summary>
        public static Dictionary<string, DischargerInfo> Infos = new Dictionary<string, DischargerInfo>();
        /// <summary>
        /// Key: Discharger Name
        /// </summary>
        public static Dictionary<string, EthernetClientDischarger> Clients = new Dictionary<string, EthernetClientDischarger>();

        public static void Reset()
        {
            FinalizeInstance();
            InitializeInstance();
        }

        private static void FinalizeInstance()
        {
            SelectedDischargerName = string.Empty;

            Infos.Clear();

            foreach (var client in Clients.Values)
            {
                client.Stop();
            }
            Clients.Clear();
        }

        private static void InitializeInstance()
        {
            SelectedDischargerName = string.Empty;

            List<TableDischargerInfo> infos = SqliteDischargerInfo.GetData();

            foreach (TableDischargerInfo info in infos)
            {
                if (!Infos.ContainsKey(info.DischargerName))
                {
                    InitializeDischargerInfos(info.DischargerName);
                    InitializeDischargerClients(info.DischargerName);
                }
            }
        }

        private static void InitializeDischargerInfos(string name)
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

            Infos[name] = new DischargerInfo();
            Infos[name].Name = name;
            Infos[name].Channel = info.DischargerChannel;
            Infos[name].IpAddress = IPAddress.Parse(info.IpAddress);
            Infos[name].EthernetPort = 10004;
            Infos[name].TimeOutMs = 5000;
            Infos[name].SpecVoltage = info.SpecVoltage;
            Infos[name].SpecCurrent = info.SpecCurrent;
            Infos[name].SafetyVoltageMax = model.SafetyVoltMax;
            Infos[name].SafetyVoltageMin = model.SafetyVoltMin;
            Infos[name].SafetyCurrentMax = model.SafetyCurrentMax;
            Infos[name].SafetyCurrentMin = model.SafetyCurrentMin;
            Infos[name].SafetyTempMax = model.SafetyTempMax;
            Infos[name].SafetyTempMin = model.SafetyTempMin;
        }

        private static void InitializeDischargerClients(string name)
        {
            EthernetClientDischargerStart parameters = new EthernetClientDischargerStart();
            parameters.DischargerName = Infos[name].Name;
            parameters.DischargerChannel = Infos[name].Channel;
            parameters.IpAddress = Infos[name].IpAddress;
            parameters.EthernetPort = Infos[name].EthernetPort;
            parameters.TimeOutMs = Infos[name].TimeOutMs;
            parameters.SafetyVoltageMax = Infos[name].SafetyVoltageMax;
            parameters.SafetyVoltageMin = Infos[name].SafetyVoltageMin;
            parameters.SafetyCurrentMax = Infos[name].SafetyCurrentMax;
            parameters.SafetyCurrentMin = Infos[name].SafetyCurrentMin;
            Clients[name] = new EthernetClientDischarger();
            Clients[name].Start(parameters);
        }
    }

    public class DischargerInfo
    {
        /// <summary>
        /// 방전기 정보
        /// </summary>
        public string Name { get; set; } = string.Empty;
        public short Channel { get; set; } = short.MaxValue;
        public IPAddress IpAddress { get; set; }
        public int EthernetPort { get; set; } = int.MaxValue;
        public int TimeOutMs { get; set; } = int.MaxValue;

        /// <summary>
        /// 방전기 스펙
        /// </summary>
        public double SpecVoltage { get; set; } = double.MaxValue;
        public double SpecCurrent { get; set; } = double.MaxValue;

        /// <summary>
        /// 안전 조건
        /// </summary>
        public double SafetyVoltageMax { get; set; } = double.MaxValue;
        public double SafetyVoltageMin { get; set; } = double.MaxValue;
        public double SafetyCurrentMax { get; set; } = double.MaxValue;
        public double SafetyCurrentMin { get; set; } = double.MaxValue;
        public double SafetyTempMax { get; set; } = double.MaxValue;
        public double SafetyTempMin { get; set; } = double.MaxValue;
    }


}
