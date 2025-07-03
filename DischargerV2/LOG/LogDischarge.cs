using DischargerV2.MVVM.Enums;
using Ethernet.Client.Discharger;
using Sqlite.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using static DischargerV2.MVVM.Models.ModelStartDischarge;

namespace DischargerV2.LOG
{
    public class LogDischarge : Log
    {
        public class DischargeConfig
        {
            // Server DB 연동
            public string MachineCode { get; set; }

            // Discharger
            public string DischargerName { get; set; }
            public EDischargerModel DischargerModel { get; set; }
            public EDischargeType DischargeType { get; set; }
            public short DischargeChannel { get; set; }
            public double SpecVoltage { get; set; }
            public double SpecCurrent { get; set; }
            public string IPAddress { get; set; }
            public bool IsTempModule { get; set; }
            public string TempModuleComport { get; set; }
            public string TempModuleChannel { get; set; }
            public string Tempchannel { get; set; }

            // Discharge Mode
            public EDischargeMode EDischargeMode { get; set; }

            // Discharge Mode Config
            public string BatteryType { get; set; } = string.Empty;
            public string CurrentSoC { get; set; } = string.Empty;
            public string StandardCapacity { get; set; } = string.Empty;
            public string StandartVoltage { get; set; } = string.Empty;
            public List<PhaseData> PhaseDataList { get; set; } = new List<PhaseData>();

            // Discharge Target
            public EDischargeTarget EDischargeTarget { get; set; }
            public string TargetValue { get; set; } = string.Empty;

            // SafetyCondition
            public string SafetyVoltageMax { set; get; } = string.Empty;
            public string SafetyVoltageMin { set; get; } = string.Empty;
            public string SafetyCurrentMax { set; get; } = string.Empty;
            public string SafetyCurrentMin { set; get; } = string.Empty;
            public string SafetyTempMax { set; get; } = string.Empty;
            public string SafetyTempMin { set; get; } = string.Empty;
        }

        public class DischargerData
        {
            public EDischargerClientError EDischargerClientError { get; set; }

            public EWorkMode EWorkMode { get; set; }
            public double SetValue_Voltage { get; set; } = double.NaN;
            public double LimitingValue_Current { get; set; } = double.NaN;

            public double SafetyVoltageMax { set; get; } = double.NaN;
            public double SafetyVoltageMin { set; get; } = double.NaN;
            public double SafetyCurrentMax { set; get; } = double.NaN;
            public double SafetyCurrentMin { set; get; } = double.NaN;
            public double SafetyTempMax { set; get; } = double.NaN;
            public double SafetyTempMin { set; get; } = double.NaN;
        }

        public class DischargeRawData
        {
            public string Time { set; get; } = string.Empty;
            public string Current { set; get; } = string.Empty;
            public string Voltage { set; get; } = string.Empty;
            public string Temp { set; get; } = string.Empty;
            public string Capacity { set; get; } = string.Empty;
            public string dv { set; get; } = string.Empty;
            public string dq { set; get; } = string.Empty;
            public string dvdq { set; get; } = string.Empty;
            public string destDvdq { set; get; } = string.Empty;
            public string phase2 { set; get; } = string.Empty;
            public string dvdqAvg { set; get; } = string.Empty;

            public string phase { set; get; } = string.Empty;
        }

        public class LogData
        {
            public string FileName { set; get; } = string.Empty;
            public List<string> Datas { set; get; } = new List<string>();
        }

        public static string Path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\LOG\\Discharge";

        public static ConcurrentQueue<LogData> LogDataQueue = new ConcurrentQueue<LogData>();

        public static ConcurrentQueue<List<string>> LogQueue = new ConcurrentQueue<List<string>>();
        private static readonly object WriteLock = new object();

        public static bool CheckExit(string fileName)
        {
            string fileNameAll = string.Format("{0}\\{1}.csv", Path, fileName);

            FileInfo fileInfo = new FileInfo(fileNameAll);

            return (fileInfo.Exists) ? true : false;
        }

        public LogDischarge(DischargeConfig dischargeConfig, string fileName)
        {
            List<string> listContent = new List<string>();

            // Config
            listContent.Add("[Config]");

            // Discharger
            listContent.Add("Discharger");
            listContent.Add("Name, Model, Type, Channel, SpecVoltage, SpecCurrent, IpAddress, IsTempModule, TempModuleComport, TempModuleChannel, Tempchannel");
            listContent.Add(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                    dischargeConfig.DischargerName,
                    dischargeConfig.DischargerModel,
                    dischargeConfig.DischargeType,
                    dischargeConfig.DischargeChannel,
                    dischargeConfig.SpecVoltage,
                    dischargeConfig.SpecCurrent,
                    dischargeConfig.IPAddress,
                    dischargeConfig.IsTempModule,
                    dischargeConfig.TempModuleComport,
                    dischargeConfig.TempModuleChannel,
                    dischargeConfig.Tempchannel));
            listContent.Add("");

            // Discharge Mode
            listContent.Add("Discharge Mode");
            listContent.Add(dischargeConfig.EDischargeMode.ToString());
            listContent.Add("");

            // Discharge Mode Config
            if (dischargeConfig.EDischargeMode == EDischargeMode.Preset)
            {
                listContent.Add("Battery Type");
                listContent.Add(dischargeConfig.BatteryType);
                listContent.Add("");

                listContent.Add("Current SoC (%)");
                listContent.Add(dischargeConfig.CurrentSoC);
                listContent.Add("");
            }
            else if (dischargeConfig.EDischargeMode == EDischargeMode.Step)
            {
                if (dischargeConfig.StandardCapacity == null ||
                    dischargeConfig.StandardCapacity == string.Empty ||
                    dischargeConfig.StandardCapacity == "")
                {
                    listContent.Add("Step Data List");
                    listContent.Add("No, Voltage, Current");

                    for (int index = 0; index < dischargeConfig.PhaseDataList.Count; index++)
                    {
                        var phaseData = dischargeConfig.PhaseDataList[index];

                        listContent.Add(string.Format("{0}, {1}, {2}",
                            index + 1,
                            phaseData.Voltage,
                            phaseData.Current));
                    }
                }
                else
                {
                    listContent.Add("Standard Capacity");
                    listContent.Add(dischargeConfig.StandardCapacity);
                    listContent.Add("");

                    listContent.Add("Step Data List");
                    listContent.Add("No, Voltage, Current, C-Rate");

                    for (int index = 0; index < dischargeConfig.PhaseDataList.Count; index++)
                    {
                        var phaseData = dischargeConfig.PhaseDataList[index];

                        listContent.Add(string.Format("{0}, {1}, {2}, {3}",
                            index + 1,
                            phaseData.Voltage,
                            phaseData.Current,
                            phaseData.CRate));
                    }
                }
                listContent.Add("");
            }
            else if (dischargeConfig.EDischargeMode == EDischargeMode.Simple)
            {
                listContent.Add("Standard Voltage (V)");
                listContent.Add(dischargeConfig.StandartVoltage);
                listContent.Add("");

                listContent.Add("Standard Capacity (A)");
                listContent.Add(dischargeConfig.StandardCapacity);
                listContent.Add("");
            }

            // Discharge Target
            listContent.Add("Discharge Target");
            if (dischargeConfig.EDischargeTarget == EDischargeTarget.Voltage ||
                dischargeConfig.EDischargeTarget == EDischargeTarget.SoC)
            {
                listContent.Add(string.Format("Target {0} ({1})",
                    dischargeConfig.EDischargeTarget,
                    dischargeConfig.TargetValue));
                listContent.Add("");
            }
            else
            {
                listContent.Add(string.Format("{0} Discharge",
                    dischargeConfig.EDischargeTarget));
                listContent.Add("");
            }

            // SafetyCondition
            listContent.Add("Safety Condition");
            listContent.Add("Voltage Max, Voltage Min, Current Max, Current Min, Temp Max, Temp Min");
            listContent.Add(string.Format("{0}, {1}, {2}, {3}, {4}, {5}",
                    dischargeConfig.SafetyVoltageMax,
                    dischargeConfig.SafetyVoltageMin,
                    dischargeConfig.SafetyCurrentMax,
                    dischargeConfig.SafetyCurrentMin,
                    dischargeConfig.SafetyTempMax,
                    dischargeConfig.SafetyTempMin));
            listContent.Add("");

            // Data
            listContent.Add("[Data]");
            listContent.Add("Time(s), Current(A), Voltage(V), Capacity(Ah), dv, dq, dvdq, destDvdq, phase2, dvdqAvg, Temp(℃), phase");

            SaveFile_DischargeConfig(listContent, Path, fileName);
        }

        public LogDischarge(string fileName, DischargeRawData dischargeRawData)
        {
            try
            {
                LogDataQueue.Enqueue(new LogData()
                {
                    FileName = fileName, 
                    Datas = new List<string>()
                    {
                        dischargeRawData.Time,
                        dischargeRawData.Current,
                        dischargeRawData.Voltage,
                        dischargeRawData.Capacity,
                        dischargeRawData.dv,
                        dischargeRawData.dq,
                        dischargeRawData.dvdq,
                        dischargeRawData.destDvdq,
                        dischargeRawData.phase2,
                        dischargeRawData.dvdqAvg,
                        dischargeRawData.Temp,
                        dischargeRawData.phase,
                    }
                });

                lock (WriteLock)
                {
                    while (LogDataQueue.Count > 0)
                    {
                        if (LogDataQueue.TryDequeue(out LogData logData))
                        {
                            if (logData == null) continue;

                            SaveFile_Discharge(logData.Datas, Path, logData.FileName);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
