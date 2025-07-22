using Ethernet.Client.Discharger;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;

namespace DischargerV2.LOG
{
    public class LogTrace : Log
    {
        public enum ELogTrace : uint
        {
            // SYSTEM_OK
            [Description("프로그램 시작")]
            SYSTEM_OK_PROGRAM_START = 100,
            [Description("프로그램 종료")]
            SYSTEM_OK_PROGRAM_END = 110,
            [Description("로그인")]
            SYSTEM_OK_LOGIN = 120,

            [Description("방전기 연결")]
            SYSTEM_OK_CONNECT_DISCHARGER = 200,
            [Description("방전기 재 연결")]
            SYSTEM_OK_RECONNECT_DISCHARGER = 210,
            [Description("방전기 에러 해제")]
            SYSTEM_OK_CLEAR_ALARM = 220,

            [Description("온도 모듈 연결")]
            SYSTEM_OK_CONNECT_TEMPMODULE = 300,
            [Description("온도 모듈 재 연결")]
            SYSTEM_OK_RECONNECT_TEMPMODULE = 310,
            [Description("온도 모듈 데이터 수신")]
            SYSTEM_OK_RECEIVE_DATA_TEMP = 320,

            [Description("방전 동작 시작")]
            SYSTEM_OK_START_DISCHARGE = 400,
            [Description("방전 동작 일시 정지")]
            SYSTEM_OK_PAUSE_DISCHARGE = 410,
            [Description("방전 동작 재 시작")]
            SYSTEM_OK_RESTART_DISCHARGE = 420,
            [Description("방전 동작 정지")]
            SYSTEM_OK_STOP_DISCHARGE = 430,
            [Description("방전 모드 설정 돌아가기")]
            SYSTEM_OK_RETURN_SETMODE = 440,
            [Description("방전 동작 로그 파일 생성")]
            SYSTEM_OK_SAVE_LOG = 450,
            [Description("방전 안전 조건 설정")]
            SYSTEM_OK_SET_SAFETYCONDITION = 460,
            [Description("방전 상태 설정")]
            SYSTEM_OK_SET_STATE = 470,

            [Description("사용자 정보 추가")]
            SYSTEM_OK_ADD_USER = 500,
            [Description("사용자 정보 수정")]
            SYSTEM_OK_EDIT_USER = 510,
            [Description("사용자 정보 제거")]
            SYSTEM_OK_DELETE_USER = 520,

            [Description("방전기 정보 추가")]
            SYSTEM_OK_ADD_DISCHARGER = 600,
            [Description("방전기 정보 수정")]
            SYSTEM_OK_EDIT_DISCHARGER = 610,
            [Description("방전기 정보 제거")]
            SYSTEM_OK_DELETE_DISCHARGER = 620,

            [Description("방전기 모델 정보 추가")]
            SYSTEM_OK_ADD_MODEL = 700,
            [Description("방전기 모델 정보 수정")]
            SYSTEM_OK_EDIT_MODEL = 710,
            [Description("방전기 모델 정보 제거")]
            SYSTEM_OK_DELETE_MODEL = 720,

            // SYSTEM_ERROR
            [Description("프로그램 시작 실패")]
            SYSTEM_ERROR_PROGRAM_START = 101,
            [Description("프로그램 종료 실패")]
            SYSTEM_ERROR_PROGRAM_END = 111,
            [Description("로그인 실패")]
            SYSTEM_ERROR_LOGIN = 121,

            [Description("방전기 연결 실패")]
            SYSTEM_ERROR_CONNECT_DISCHARGER = 201,
            [Description("방전기 재 연결 실패")]
            SYSTEM_ERROR_RECONNECT_DISCHARGER = 211,
            [Description("방전기 에러 해제 실패")]
            SYSTEM_ERROR_CLEAR_ALARM = 221,

            [Description("온도 모듈 연결 실패")]
            SYSTEM_ERROR_CONNECT_TEMPMODULE = 301,
            [Description("온도 모듈 재 연결 실패")]
            SYSTEM_ERROR_RECONNECT_TEMPMODULE = 311,
            [Description("온도 모듈 데이터 수신 실패")]
            SYSTEM_ERROR_RECEIVE_DATA_TEMP = 321,

            [Description("방전 동작 시작 실패")]
            SYSTEM_ERROR_START_DISCHARGE = 401,
            [Description("방전 동작 일시 정지 실패")]
            SYSTEM_ERROR_PAUSE_DISCHARGE = 411,
            [Description("방전 동작 재 시작 실패")]
            SYSTEM_ERROR_RESTART_DISCHARGE = 421,
            [Description("방전 동작 정지 실패")]
            SYSTEM_ERROR_STOP_DISCHARGE = 431,
            [Description("방전 모드 설정 돌아가기 실패")]
            SYSTEM_ERROR_RETURN_SETMODE = 441,
            [Description("방전 동작 로그 파일 생성 실패")]
            SYSTEM_ERROR_SAVE_LOG = 451,
            [Description("방전 안전 조건 설정 실패")]
            SYSTEM_ERROR_SET_SAFETYCONDITION = 461,
            [Description("방전 상태 설정 실패")]
            SYSTEM_ERROR_SET_STATE = 471,

            [Description("사용자 정보 추가 실패")]
            SYSTEM_ERROR_ADD_USER = 501,
            [Description("사용자 정보 수정 실패")]
            SYSTEM_ERROR_EDIT_USER = 511,
            [Description("사용자 정보 제거 실패")]
            SYSTEM_ERROR_DELETE_USER = 521,

            [Description("방전기 정보 추가 실패")]
            SYSTEM_ERROR_ADD_DISCHARGER = 601,
            [Description("방전기 정보 수정 실패")]
            SYSTEM_ERROR_EDIT_DISCHARGER = 611,
            [Description("방전기 정보 제거 실패")]
            SYSTEM_ERROR_DELETE_DISCHARGER = 621,

            [Description("방전기 모델 정보 추가 실패")]
            SYSTEM_ERROR_ADD_MODEL = 701,
            [Description("방전기 모델 정보 수정 실패")]
            SYSTEM_ERROR_EDIT_MODEL = 711,
            [Description("방전기 모델 정보 제거 실패")]
            SYSTEM_ERROR_DELETE_MODEL = 721,
        }

        public enum ELogDischarge : uint
        {
            // COMM_OK
            [Description("동작 시작")]
            COMM_OK_START = 100,
            [Description("동작 일시 정지")]
            COMM_OK_PAUSE = 200,
            [Description("동작 정지")]
            COMM_OK_STOP = 300,
            [Description("에러 해제")]
            COMM_OK_CLEAR_ALARM = 400,
            [Description("데이터 수신")]
            COMM_OK_GET_DATA = 500,
            [Description("안전 조건 설정")]
            COMM_OK_SET_SAFETYCONDITION = 600,
            [Description("상태 변경")]
            COMM_OK_SET_STATE = 700,
            [Description("경광등 제어")]
            COMM_OK_CONTROL_LAMP = 800,

            // COMM_ERROR
            [Description("동작 시작 실패")]
            COMM_ERROR_START = 101,
            [Description("동작 일시 정지 실패")]
            COMM_ERROR_PAUSE = 201,
            [Description("동작 정지 실패")]
            COMM_ERROR_STOP = 301,
            [Description("에러 해제 실패")]
            COMM_ERROR_CLEAR_ALARM = 401,
            [Description("데이터 수신 실패")]
            COMM_ERROR_GET_DATA = 501,
            [Description("안전 조건 설정 실패")]
            COMM_ERROR_SET_SAFETYCONDITION = 601,
            [Description("상태 변경 실패")]
            COMM_ERROR_SET_STATE = 701,
            [Description("경광등 제어 실패")]
            COMM_ERROR_CONTROL_LAMP = 801,
        }

        public class DischargerData
        {
            public EDischargerClientError EDischargerClientError { get; set; }

            public string Name { get; set; } = string.Empty;
            public short Channel { get; set; } = short.MaxValue;
            public EDischargerModel EDischargerModel { get; set; }
            public IPAddress IpAddress { get; set; }

            public double SafetyVoltageMax { set; get; } = double.MaxValue;
            public double SafetyVoltageMin { set; get; } = double.MaxValue;
            public double SafetyCurrentMax { set; get; } = double.MaxValue;
            public double SafetyCurrentMin { set; get; } = double.MaxValue;
            public double SafetyTempMax { set; get; } = double.MaxValue;
            public double SafetyTempMin { set; get; } = double.MaxValue;

            public string ReceiveBatteryVoltage { get; set; } = string.Empty;
            public string ReceiveDischargeCurrent { get; set; } = string.Empty;
            public string ReceiveDischargeTemp { get; set; } = string.Empty;

            public EWorkMode EWorkMode { get; set; }
            public double SetValue_Voltage { get; set; } = double.MaxValue;
            public double LimitingValue_Current { get; set; } = double.MaxValue;

            public uint ErrorCode { get; set; } = uint.MaxValue;
            public EChannelStatus EChannelStatus { get; set; }
            public EReturnCode EReturnCode { get; set; }
            public EDischargerState EDischargerState { get; set; }

            public uint LampDioValue { get; set; } = uint.MaxValue;

            public string FileName { set; get; } = string.Empty;
        }

        public class TempModuleData
        {
            public string DischargerName { get; set; } = string.Empty;
            public EDischargerModel EDischargerModel { get; set; }
            public short DischargerChannel { get; set; } = short.MaxValue;
            public string TempModuleComPort { get; set; } = string.Empty;
            public string TempModuleChannel { get; set; } = string.Empty;
            public string TempChannel { get; set; } = string.Empty;
        }

        public class UserData
        {
            public string UserId { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public bool IsAdmin { get; set; } = false;

            public string PasswordNew { get; set; } = string.Empty;
        }

        public class DeviceData
        {
            public string Name { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Channel { get; set; } = string.Empty;
            public string SpecVoltage { get; set; } = string.Empty;
            public string SpecCurrent { get; set; } = string.Empty;
            public string IpAddress { get; set; } = string.Empty;
            public bool IsTempModule { get; set; } = false;
            public string TempModuleComPort { get; set; } = string.Empty;
            public string TempModuleChannel { get; set; } = string.Empty;
            public string TempChannel { get; set; } = string.Empty;
        }

        public class ModelData
        {
            public int Id { get; set; } = int.MaxValue;
            public string Model { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Channel { get; set; } = string.Empty;
            public string SpecVoltage { get; set; } = string.Empty;
            public string SpecCurrent { get; set; } = string.Empty;
            public string SafetyVoltMax { get; set; } = string.Empty;
            public string SafetyVoltMin { get; set; } = string.Empty;
            public string SafetyCurrentMax { get; set; } = string.Empty;
            public string SafetyCurrentMin { get; set; } = string.Empty;
            public string SafetyTempMax { get; set; } = string.Empty;
            public string SafetyTempMin { get; set; } = string.Empty;
        }

        public static string Path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\LOG\\Trace";
        public static List<string> ListTitle = new List<string>
        {
            "LogDateTime",
            "TraceCode", "TraceLevel", "TraceResult", "TraceMnemonic", "TraceDescription", "TraceParameter"
        };

        public static Queue<List<string>> LogQueue = new Queue<List<string>>();
        private static readonly object WriteLock = new object();

        public static int Code;
        public static string Level;
        public static string Result;
        public static string Mnemonic;
        public static string Description;
        public static string Parameter;

        public LogTrace(ELogTrace eLogTrace, Exception ex)
        {
            string logParameter = string.Format("\"Exception:{0}\"", ex);

            WriteLog(eLogTrace, logParameter);
        }

        public LogTrace(ELogDischarge eLogDischarge, Exception ex)
        {
            string logParameter = string.Format("\"Exception:{0}\"", ex);

            WriteLog(eLogDischarge, logParameter);
        }

        public LogTrace(ELogTrace eLogTrace)
        {
            WriteLog(eLogTrace);
        }

        public LogTrace(ELogDischarge eLogDischarge)
        {
            WriteLog(eLogDischarge);
        }

        public LogTrace(ELogTrace eLogTrace, string parameter)
        {
            try
            {
                string logParameter;

                switch (eLogTrace)
                {
                    case ELogTrace.SYSTEM_OK_LOGIN:
                    case ELogTrace.SYSTEM_ERROR_LOGIN:
                        logParameter = string.Format("\"ID:{0}\"", parameter);
                        break;
                    default:
                        logParameter = string.Format("\"Exception:{0}\"", parameter);
                        break;
                }

                WriteLog(eLogTrace, logParameter);
            }
            catch
            {
                WriteLog(eLogTrace);
            }
        }

        public LogTrace(ELogTrace eLogTrace, DischargerData dischargerData)
        {
            try
            {
                string logParameter = string.Empty;

                switch (eLogTrace)
                {
                    case ELogTrace.SYSTEM_OK_CONNECT_DISCHARGER:
                    case ELogTrace.SYSTEM_ERROR_CONNECT_DISCHARGER:
                    case ELogTrace.SYSTEM_OK_RECONNECT_DISCHARGER:
                    case ELogTrace.SYSTEM_ERROR_RECONNECT_DISCHARGER:
                    case ELogTrace.SYSTEM_OK_CLEAR_ALARM:
                    case ELogTrace.SYSTEM_ERROR_CLEAR_ALARM:
                        logParameter = string.Format(
                            "\"Name:{0}, Model:{1}, Channel:{2}, IPAddress:{3}\"",
                            dischargerData.Name,
                            dischargerData.EDischargerModel,
                            dischargerData.Channel,
                            dischargerData.IpAddress);
                        break;
                    case ELogTrace.SYSTEM_OK_START_DISCHARGE:
                    case ELogTrace.SYSTEM_ERROR_START_DISCHARGE:
                    case ELogTrace.SYSTEM_OK_RESTART_DISCHARGE:
                    case ELogTrace.SYSTEM_ERROR_RESTART_DISCHARGE:
                        logParameter = string.Format(
                            "\"Name:{0}, Model:{1}, Channel:{2}, IPAddress:{3}, " +
                            "Workmode:{4}, SetValue:{5}, LimitingValue:{6}\"",
                            dischargerData.Name,
                            dischargerData.EDischargerModel,
                            dischargerData.Channel,
                            dischargerData.IpAddress,
                            dischargerData.EWorkMode,
                            dischargerData.SetValue_Voltage,
                            dischargerData.LimitingValue_Current);
                        break;
                    case ELogTrace.SYSTEM_OK_RETURN_SETMODE:
                    case ELogTrace.SYSTEM_ERROR_RETURN_SETMODE:
                        logParameter = string.Format(
                            "\"Name:{0}\"",
                            dischargerData.Name);
                        break;
                    case ELogTrace.SYSTEM_OK_SAVE_LOG:
                    case ELogTrace.SYSTEM_ERROR_SAVE_LOG:
                        logParameter = string.Format(
                            "\"Name:{0}, FileName:{1}\"",
                            dischargerData.Name,
                            dischargerData.FileName);
                        break;
                    case ELogTrace.SYSTEM_OK_SET_SAFETYCONDITION:
                    case ELogTrace.SYSTEM_ERROR_SET_SAFETYCONDITION:
                        logParameter = string.Format(
                            "\"Name:{0}, Model:{1}, Channel:{2}, IPAddress:{3}, " +
                            "SafetyVoltageMax:{4}, SafetyVoltageMin:{5}, " +
                            "SafetyCurrentMax:{6}, SafetyCurrentMin:{7}, " +
                            "SafetyTempMax:{8}, SafetyTempMin:{9}\"",
                            dischargerData.Name,
                            dischargerData.EDischargerModel,
                            dischargerData.Channel,
                            dischargerData.IpAddress,
                            dischargerData.SafetyVoltageMax,
                            dischargerData.SafetyVoltageMin,
                            dischargerData.SafetyCurrentMax,
                            dischargerData.SafetyCurrentMin,
                            dischargerData.SafetyTempMax,
                            dischargerData.SafetyTempMin);
                        break;
                    case ELogTrace.SYSTEM_OK_SET_STATE:
                    case ELogTrace.SYSTEM_ERROR_SET_STATE:
                        logParameter = string.Format(
                            "\"Name:{0}, Model:{1}, Channel:{2}, IPAddress:{3}, " +
                            "State:{4}\"",
                            dischargerData.Name,
                            dischargerData.EDischargerModel,
                            dischargerData.Channel,
                            dischargerData.IpAddress,
                            dischargerData.EDischargerState);
                        break;
                    default:
                        logParameter = string.Format(
                            "\"Name:{0}, Model:{1}, Channel:{2}, IPAddress:{3}\"",
                            dischargerData.Name,
                            dischargerData.EDischargerModel,
                            dischargerData.Channel,
                            dischargerData.IpAddress);
                        break;
                }

                WriteLog(eLogTrace, logParameter);
            }
            catch
            {
                WriteLog(eLogTrace);
            }
        }

        public LogTrace(ELogTrace eLogTrace, TempModuleData tempModuleComm)
        {
            try
            {
                string logParameter = string.Format(
                            "\"Name:{0}, Model:{1}, " +
                            "Channel:{2}, TempModuleComPort:{3}, " +
                            "TempModuleChannel:{4}, TempChannel:{5}\"",
                            tempModuleComm.DischargerName,
                            tempModuleComm.EDischargerModel,
                            tempModuleComm.DischargerChannel,
                            tempModuleComm.TempModuleComPort,
                            tempModuleComm.TempModuleChannel,
                            tempModuleComm.TempChannel);

                WriteLog(eLogTrace, logParameter);
            }
            catch
            {
                WriteLog(eLogTrace);
            }
        }

        public LogTrace(ELogTrace eLogTrace, UserData userData)
        {
            try
            {
                string logParameter;

                switch (eLogTrace)
                {
                    case ELogTrace.SYSTEM_OK_EDIT_USER:
                    case ELogTrace.SYSTEM_ERROR_EDIT_USER:
                        logParameter = string.Format(
                            "\"UserId:{0}, " +
                            "PasswordBefore:{1}, PasswordAfter:{2}, " +
                            "UserName:{3}, IsAdmin:{4}\"",
                            userData.UserId,
                            userData.Password,
                            userData.PasswordNew,
                            userData.UserName,
                            userData.IsAdmin); 
                        break;
                    default:
                        logParameter = string.Format(
                            "\"UserId:{0}, Password:{1}, " +
                            "UserName:{2}, IsAdmin:{3}\"",
                            userData.UserId,
                            userData.Password,
                            userData.UserName,
                            userData.IsAdmin);
                        break;
                }

                WriteLog(eLogTrace, logParameter);
            }
            catch
            {
                WriteLog(eLogTrace);
            }
        }

        public LogTrace(ELogTrace eLogTrace, DeviceData deviceData)
        {
            try
            {
                string logParameter = string.Format(
                            "\"Name:{0}, Model:{1}, " +
                            "Type:{2}, Channel:{3}, " +
                            "SpecVoltage:{4}, SpecCurrent:{5}, " +
                            "IpAddress:{6}, IsTempModule:{7}, " +
                            "TempModuleComPort:{8}, TempModuleChannel:{9}, " +
                            "TempChannel:{10}\"",
                            deviceData.Name,
                            deviceData.Model,
                            deviceData.Type,
                            deviceData.Channel,
                            deviceData.SpecVoltage,
                            deviceData.SpecCurrent,
                            deviceData.IpAddress,
                            deviceData.IsTempModule,
                            deviceData.TempModuleComPort,
                            deviceData.TempModuleChannel,
                            deviceData.TempChannel);

                WriteLog(eLogTrace, logParameter);
            }
            catch
            {
                WriteLog(eLogTrace);
            }
        }

        public LogTrace(ELogTrace eLogTrace, ModelData modelData)
        {
            try
            {
                string logParameter = string.Format(
                            "\"Id:{0}, Model:{1}, " +
                            "Type:{2}, Channel:{3}, " +
                            "SpecVoltage:{4}, SpecCurrent:{5}, " +
                            "SafetyVoltMax:{6}, SafetyVoltMin:{7}, " +
                            "SafetyCurrentMax:{8}, SafetyCurrentMin:{9}, " +
                            "SafetyTempMax:{10}, SafetyTempMin:{11}\"",
                            modelData.Id,
                            modelData.Model,
                            modelData.Type,
                            modelData.Channel,
                            modelData.SpecVoltage,
                            modelData.SpecCurrent,
                            modelData.SafetyVoltMax,
                            modelData.SafetyVoltMin,
                            modelData.SafetyCurrentMax,
                            modelData.SafetyCurrentMin,
                            modelData.SafetyTempMax,
                            modelData.SafetyTempMin);

                WriteLog(eLogTrace, logParameter);
            }
            catch
            {
                WriteLog(eLogTrace);
            }
        }

        public LogTrace(ELogDischarge eLogDischarge, string parameter)
        {
            try
            {
                string logParameter;

                switch (eLogDischarge)
                {
                    case ELogDischarge.COMM_ERROR_GET_DATA:
                        logParameter = string.Format(
                            "\"Packet:{0}\"",
                            parameter);
                        break;
                }

                WriteLog(eLogDischarge, parameter);
            }
            catch
            {
                WriteLog(eLogDischarge);
            }
        }

        public LogTrace(ELogDischarge eLogDischarge, DischargerData dischargerData)
        {
            try
            {
                string logParameter = string.Empty;

                switch (eLogDischarge)
                {
                    case ELogDischarge.COMM_OK_START:
                        logParameter = string.Format(
                            "\"Name:{0}, Channel:{1}, " +
                            "Workmode:{2}, SetValue:{3}, LimitingValue:{4}\"",
                            dischargerData.Name,
                            dischargerData.Channel,
                            dischargerData.EWorkMode,
                            dischargerData.SetValue_Voltage,
                            dischargerData.LimitingValue_Current);
                        break;
                    case ELogDischarge.COMM_ERROR_START:
                        logParameter = string.Format(
                            "\"Name:{0}, Channel:{1}, " +
                            "Workmode:{2}, SetValue:{3}, LimitingValue:{4}, " +
                            "DischargerClientError:{5}\"",
                            dischargerData.Name,
                            dischargerData.Channel,
                            dischargerData.EWorkMode,
                            dischargerData.SetValue_Voltage,
                            dischargerData.LimitingValue_Current,
                            dischargerData.EDischargerClientError);
                        break;
                    case ELogDischarge.COMM_OK_STOP:
                        logParameter = string.Format(
                            "\"Name:{0}, Channel:{1}\"",
                            dischargerData.Name,
                            dischargerData.Channel);
                        break;
                    case ELogDischarge.COMM_ERROR_STOP:
                        logParameter = string.Format(
                            "\"Name:{0}, Channel:{1}, " +
                            "DischargerClientError:{2}\"",
                            dischargerData.Name,
                            dischargerData.Channel,
                            dischargerData.EDischargerClientError);
                        break;
                    case ELogDischarge.COMM_OK_PAUSE:
                        logParameter = string.Format(
                            "\"Name:{0}, Channel:{1}\"",
                            dischargerData.Name,
                            dischargerData.Channel);
                        break;
                    case ELogDischarge.COMM_ERROR_PAUSE:
                        logParameter = string.Format(
                            "\"Name:{0}, Channel:{1}, " +
                            "DischargerClientError:{2}\"",
                            dischargerData.Name,
                            dischargerData.Channel,
                            dischargerData.EDischargerClientError);
                        break;
                    case ELogDischarge.COMM_OK_CLEAR_ALARM:
                    case ELogDischarge.COMM_ERROR_CLEAR_ALARM:
                        logParameter = string.Format(
                            "\"Name:{0}, Channel:{1}\"",
                            dischargerData.Name,
                            dischargerData.Channel);
                        break;
                    case ELogDischarge.COMM_OK_SET_SAFETYCONDITION:
                    case ELogDischarge.COMM_ERROR_SET_SAFETYCONDITION:
                        logParameter = string.Format(
                            "\"Name:{0}, Channel:{1}, " +
                            "SafetyVoltageMax:{2}, SafetyVoltageMin:{3}, " +
                            "SafetyCurrentMax:{4}, SafetyCurrentMin:{5}, " +
                            "SafetyTempMax:{6}, SafetyTempMin:{7}\"",
                            dischargerData.Name,
                            dischargerData.Channel,
                            dischargerData.SafetyVoltageMax,
                            dischargerData.SafetyVoltageMin,
                            dischargerData.SafetyCurrentMax,
                            dischargerData.SafetyCurrentMin,
                            dischargerData.SafetyTempMax,
                            dischargerData.SafetyTempMin);
                        break;
                    case ELogDischarge.COMM_OK_SET_STATE:
                    case ELogDischarge.COMM_ERROR_SET_STATE:
                        logParameter = string.Format(
                            "\"Name:{0}, Channel:{1}, " +
                            "ReceiveBatteryVoltage:{2}, " +
                            "ReceiveDischargeCurrent:{3}, " +
                            "ReceiveDischargeTemp:{4}, " +
                            "ErrorCode:{5}, ReturnCode:{6}, DischargerState:{7}\"",
                            dischargerData.Name,
                            dischargerData.Channel,
                            dischargerData.ReceiveBatteryVoltage,
                            dischargerData.ReceiveDischargeCurrent,
                            dischargerData.ReceiveDischargeTemp,
                            dischargerData.ErrorCode,
                            dischargerData.EReturnCode,
                            dischargerData.EDischargerState);
                        break;
                    case ELogDischarge.COMM_OK_CONTROL_LAMP:
                    case ELogDischarge.COMM_ERROR_CONTROL_LAMP:
                        logParameter = string.Format(
                            "\"Name:{0}, " +
                            "LampDioValue:{1}\"",
                            dischargerData.Name,
                            dischargerData.LampDioValue);
                        break;
                }

                WriteLog(eLogDischarge, logParameter);
            }
            catch
            {
                WriteLog(eLogDischarge);
            }
        }

        private static void WriteLog(ELogTrace eLogTrace, string parameter = "")
        {
            SetTraceData(eLogTrace, parameter);

            List<string> listContent = new List<string>
            {
                " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Code.ToString(), Level, Result, Mnemonic, Description, Parameter
            };

            LogQueue.Enqueue(listContent);

            lock (WriteLock)
            {
                while (LogQueue.Count > 0)
                {
                    SaveFile_Hour(ListTitle, LogQueue.Dequeue(), Path);
                }
            }
        }

        private static void WriteLog(ELogDischarge eLogDischarge, string parameter = "")
        {
            SetTraceData(eLogDischarge, parameter);

            LogQueue.Enqueue(new List<string>
            {
                " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Code.ToString(), Level, Result, Mnemonic, Description, Parameter
            });

            lock (WriteLock)
            {
                while (LogQueue.Count > 0)
                {
                    SaveFile_Hour(ListTitle, LogQueue.Dequeue(), Path);
                }
            }
        }

        private static void SetTraceData(ELogTrace eLogTrace, string parameter = "")
        {
            Code = (int)eLogTrace;
            Level = $"{eLogTrace.ToString().Split('_')[0]}";
            Result = $"{eLogTrace.ToString().Split('_')[1]}";
            Mnemonic = eLogTrace.ToString().Replace($"{eLogTrace.ToString().Split('_')[0]}_{eLogTrace.ToString().Split('_')[1]}_", "");

            FieldInfo fi = eLogTrace.GetType().GetField(eLogTrace.ToString());
            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
                Description = attributes.First().Description;
            else
                Description = "";

            Parameter = parameter;
        }

        private static void SetTraceData(ELogDischarge eLogDischarge, string parameter = "")
        {
            Code = (int)eLogDischarge;
            Level = $"{eLogDischarge.ToString().Split('_')[0]}";
            Result = $"{eLogDischarge.ToString().Split('_')[1]}";
            Mnemonic = eLogDischarge.ToString().Replace($"{eLogDischarge.ToString().Split('_')[0]}_{eLogDischarge.ToString().Split('_')[1]}_", "");
            
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
