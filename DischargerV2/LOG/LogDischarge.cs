using DischargerV2.MVVM.Enums;
using Ethernet.Client.Discharger;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using static DischargerV2.LOG.LogTrace;
using static DischargerV2.MVVM.Models.ModelStartDischarge;

namespace DischargerV2.LOG
{
    public class LogDischarge : Log
    {
        public enum ELogDischarge : uint
        {
            // TRACE
            [Description("동작 시작")]
            TRACE_START = 100,
            [Description("동작 일시 정지")]
            TRACE_PAUSE = 200,
            [Description("동작 정지")]
            TRACE_STOP = 300,
            [Description("에러 해제")]
            TRACE_CLEAR_ALARM = 400,
            [Description("데이터 수신")]
            TRACE_GET_DATA = 500,
            [Description("안전 조건 설정")]
            TRACE_SET_SAFETYCONDITION = 600,
            [Description("상태 변경")]
            TRACE_SET_STATE = 700,
            [Description("경광등 제어")]
            TRACE_CONTROL_LAMP = 800,

            // ERROR
            [Description("동작 시작 실패")]
            ERROR_START = 101,
            [Description("동작 일시 정지 실패")]
            ERROR_PAUSE = 201,
            [Description("동작 정지 실패")]
            ERROR_STOP = 301,
            [Description("에러 해제 실패")]
            ERROR_CLEAR_ALARM = 401,
            [Description("데이터 수신 실패")]
            ERROR_GET_DATA = 501,
            [Description("안전 조건 설정 실패")]
            ERROR_SET_SAFETYCONDITION = 601,
            [Description("상태 변경 실패")]
            ERROR_SET_STATE = 701,
            [Description("경광등 제어 실패")]
            ERROR_CONTROL_LAMP = 801,
        }

        public class DischargeConfig
        {
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

            public string ReceiveBatteryVoltage { get; set; } = string.Empty;
            public string ReceiveDischargeCurrent { get; set; } = string.Empty;
            public string ReceiveDischargeTemp { get; set; } = string.Empty;

            public uint ErrorCode { get; set; } = uint.MaxValue;
            public EChannelStatus EChannelStatus { get; set; }
            public EReturnCode EReturnCode { get; set; }
            public EDischargerState EDischargerState { get; set; }

            public double SafetyVoltageMax { set; get; } = double.NaN;
            public double SafetyVoltageMin { set; get; } = double.NaN;
            public double SafetyCurrentMax { set; get; } = double.NaN;
            public double SafetyCurrentMin { set; get; } = double.NaN;
            public double SafetyTempMax { set; get; } = double.NaN;
            public double SafetyTempMin { set; get; } = double.NaN;

            public uint LampDioValue { get; set; } = uint.MaxValue;
        }

        public static string Path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\LOG\\Discharge";
        
        public static Queue<List<string>> LogQueue = new Queue<List<string>>();
        private static readonly object WriteLock = new object();

        public static bool CheckExit(string fileName)
        {
            string fileNameAll = string.Format("{0}\\{1}.csv", Path, fileName);

            FileInfo fileInfo = new FileInfo(fileNameAll);

            return (fileInfo.Exists) ? true : false;
        }

        public LogDischarge(ELogDischarge eLogDischarge, string fileName, Exception ex)
        {
            string logParameter = string.Format("\"Exception:{0}\"", ex);

            WriteLog(eLogDischarge, fileName, logParameter);
        }

        public LogDischarge(ELogDischarge eLogDischarge, string fileName)
        {
            WriteLog(eLogDischarge, fileName);
        }

        public LogDischarge(ELogDischarge eLogDischarge, string fileName, string parameter)
        {
            try
            {
                string logParameter;

                switch (eLogDischarge)
                {
                    case ELogDischarge.ERROR_GET_DATA:
                        logParameter = string.Format(
                            "\"Packet:{0}\"", 
                            parameter);
                        break;
                }

                WriteLog(eLogDischarge, fileName, parameter);
            }
            catch
            {
                WriteLog(eLogDischarge, fileName);
            }
        }

        public LogDischarge(ELogDischarge eLogDischarge, string fileName, DischargerData dischargerData)
        {
            try
            {
                string logParameter = string.Empty;

                switch (eLogDischarge)
                {
                    case ELogDischarge.TRACE_START:
                        logParameter = string.Format(
                            "\"Workmode:{0}, SetValue:{1}, LimitingValue:{2}\"",
                            dischargerData.EWorkMode,
                            dischargerData.SetValue_Voltage,
                            dischargerData.LimitingValue_Current);
                        break;
                    case ELogDischarge.ERROR_START:
                        logParameter = string.Format(
                            "\"Workmode:{0}, SetValue:{1}, LimitingValue:{2}, " +
                            "DischargerClientError:{3}\"",
                            dischargerData.EWorkMode,
                            dischargerData.SetValue_Voltage,
                            dischargerData.LimitingValue_Current,
                            dischargerData.EDischargerClientError);
                        break;
                    case ELogDischarge.TRACE_GET_DATA:
                        logParameter = string.Format(
                            "\"ReceiveBatteryVoltage:{0}, " +
                            "ReceiveDischargeCurrent:{1}, " +
                            "ReceiveDischargeTemp:{2}, " +
                            "ErrorCode:{3}, ReturnCode:{4}, ChannelStatus:{5}\"",
                            dischargerData.ReceiveBatteryVoltage,
                            dischargerData.ReceiveDischargeCurrent,
                            dischargerData.ReceiveDischargeTemp,
                            dischargerData.ErrorCode,
                            dischargerData.EReturnCode,
                            dischargerData.EChannelStatus);
                        break;
                    case ELogDischarge.TRACE_SET_SAFETYCONDITION:
                    case ELogDischarge.ERROR_SET_SAFETYCONDITION:
                        logParameter = string.Format(
                            "\"SafetyVoltageMax:{0}, SafetyVoltageMin:{1}, " +
                            "SafetyCurrentMax:{2}, SafetyCurrentMin:{3}, " +
                            "SafetyTempMax:{4}, SafetyTempMin:{5}\"",
                            dischargerData.SafetyVoltageMax,
                            dischargerData.SafetyVoltageMin,
                            dischargerData.SafetyCurrentMax,
                            dischargerData.SafetyCurrentMin,
                            dischargerData.SafetyTempMax,
                            dischargerData.SafetyTempMin);
                        break;
                    case ELogDischarge.TRACE_SET_STATE:
                    case ELogDischarge.ERROR_SET_STATE:
                        logParameter = string.Format(
                            "\"ReceiveBatteryVoltage:{0}, " +
                            "ReceiveDischargeCurrent:{1}, " +
                            "ReceiveDischargeTemp:{2}, " +
                            "ErrorCode:{3}, ReturnCode:{4}, DischargerState:{5}\"",
                            dischargerData.ReceiveBatteryVoltage,
                            dischargerData.ReceiveDischargeCurrent,
                            dischargerData.ReceiveDischargeTemp,
                            dischargerData.ErrorCode,
                            dischargerData.EReturnCode,
                            dischargerData.EDischargerState);
                        break;
                    case ELogDischarge.TRACE_CONTROL_LAMP:
                    case ELogDischarge.ERROR_CONTROL_LAMP:
                        logParameter = string.Format(
                            "\"LampDioValue:{0}\"",
                            dischargerData.LampDioValue);
                        break;
                }

                WriteLog(eLogDischarge, fileName, logParameter);
            }
            catch
            {
                WriteLog(eLogDischarge, fileName);
            }
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
            listContent.Add("LogDateTime, TraceCode, TraceMnemonic, TraceDescription, TraceParameter");

            SaveFile_DischargeConfig(listContent, Path, fileName);
        }

        private static void WriteLog(ELogDischarge eLogDischarge, string fileName, string parameter = "")
        {
            if (fileName == null || fileName == string.Empty) return;

            SetTraceData(eLogDischarge, parameter);

            List<string> listContent = new List<string>
            {
                " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Code.ToString(), Level, Mnemonic, Description, Parameter
            };

            LogQueue.Enqueue(listContent);

            lock (WriteLock)
            {
                while (LogQueue.Count > 0)
                {
                    SaveFile_Discharge(LogQueue.Dequeue(), Path, fileName);
                }
            }
        }

        private static void SetTraceData(ELogDischarge eLogDischarge, string parameter = "")
        {
            Code = (int)eLogDischarge;
            Level = eLogDischarge.ToString().Split('_')[0];
            Mnemonic = eLogDischarge.ToString();

            FieldInfo fi = eLogDischarge.GetType().GetField(eLogDischarge.ToString());
            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
                Description = attributes.First().Description;
            else
                Description = "";

            Parameter = parameter;
        }
    }
}
