using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.LOG
{
    public enum ELogTrace : uint
    {
        // TRACE
        [Description("프로그램 시작")]
        TRACE_PROGRAM_START = 100,
        [Description("프로그램 종료")]
        TRACE_PROGRAM_END = 110,
        [Description("로그인")]
        TRACE_LOGIN = 120,

        [Description("방전기 연결")]
        TRACE_CONNECT_DISCHARGER = 200,
        [Description("방전기 재 연결")]
        TRACE_RECONNECT_DISCHARGER = 210,
        [Description("방전기 에러 해제")]
        TRACE_CLEAR_ALARM = 220,

        [Description("온도 모듈 연결")]
        TRACE_CONNECT_TEMPMODULE = 300,
        [Description("온도 모듈 재 연결")]
        TRACE_RECONNECT_TEMPMODULE = 310,
        [Description("온도 모듈 데이터 수신")]
        TRACE_RECEIVE_DATA_TEMP = 320,

        [Description("방전 동작 시작")]
        TRACE_START_DISCHARGE = 400,
        [Description("방전 동작 로그 파일 생성")]
        TRACE_SAVE_LOG = 410,
        [Description("방전 동작 일시 정지")]
        TRACE_PAUSE_DISCHARGE = 420,
        [Description("방전 동작 재 시작")]
        TRACE__RESTART_DISCHARGE = 430,
        [Description("방전 동작 정지")]
        TRACE_STOP_DISCHARGE = 440,
        [Description("방전 모드 설정 돌아가기")]
        TRACE_RETURN_SETMODE = 450,
        [Description("방전 동작 로그 파일명 설정하기")]
        TRACE_SET_LOGFILE = 460,

        [Description("사용자 정보 추가")]
        TRACE_ADD_USER = 500,
        [Description("사용자 정보 수정")]
        TRACE_EDIT_USER = 510,
        [Description("사용자 정보 제거")]
        TRACE_DELETE_USER = 520,

        [Description("방전기 정보 추가")]
        TRACE_ADD_DISCHARGER = 600,
        [Description("방전기 정보 수정")]
        TRACE_EDIT_DISCHARGER = 610,
        [Description("방전기 정보 제거")]
        TRACE_DELETE_DISCHARGER = 620,

        [Description("방전기 모델 정보 추가")]
        TRACE_ADD_MODEL = 700,
        [Description("방전기 모델 정보 수정")]
        TRACE_EDIT_MODEL = 710,
        [Description("방전기 모델 정보 제거")]
        TRACE_DELETE_MODEL = 720,

        // ERROR
        [Description("프로그램 시작 실패")]
        ERROR_PROGRAM_START = 101,
        [Description("프로그램 종료 실패")]
        ERROR_PROGRAM_END = 111,
        [Description("로그인 실패")]
        ERROR_LOGIN = 121,

        [Description("방전기 연결 실패")]
        ERROR_CONNECT_DISCHARGER = 201,
        [Description("방전기 재 연결 실패")]
        ERROR_RECONNECT_DISCHARGER = 211,
        [Description("방전기 에러 해제 실패")]
        ERROR_CLEAR_ALARM = 221,

        [Description("온도 모듈 연결 실패")]
        ERROR_CONNECT_TEMPMODULE = 301,
        [Description("온도 모듈 재 연결 실패")]
        ERROR_RECONNECT_TEMPMODULE = 311,
        [Description("온도 모듈 데이터 수신 실패")]
        ERROR_RECEIVE_DATA_TEMP = 321,

        [Description("방전 동작 시작 실패")]
        ERROR_START_DISCHARGE = 401,
        [Description("방전 동작 로그 파일 생성 실패")]
        ERROR_SAVE_LOG = 411,
        [Description("방전 동작 일시 정지 실패")]
        ERROR_PAUSE_DISCHARGE = 421,
        [Description("방전 동작 재 시작 실패")]
        ERROR__RESTART_DISCHARGE = 431,
        [Description("방전 동작 정지 실패")]
        ERROR_STOP_DISCHARGE = 441,
        [Description("방전 모드 설정 돌아가기 실패")]
        ERROR_RETURN_SETMODE = 451,
        [Description("방전 동작 로그 파일명 설정하기 실패")]
        ERROR_SET_LOGFILE = 461,

        [Description("사용자 정보 추가 실패")]
        ERROR_ADD_USER = 501,
        [Description("사용자 정보 수정 실패")]
        ERROR_EDIT_USER = 511,
        [Description("사용자 정보 제거 실패")]
        ERROR_DELETE_USER = 521,

        [Description("방전기 정보 추가 실패")]
        ERROR_ADD_DISCHARGER = 601,
        [Description("방전기 정보 수정 실패")]
        ERROR_EDIT_DISCHARGER = 611,
        [Description("방전기 정보 제거 실패")]
        ERROR_DELETE_DISCHARGER = 621,

        [Description("방전기 모델 정보 추가 실패")]
        ERROR_ADD_MODEL = 701,
        [Description("방전기 모델 정보 수정 실패")]
        ERROR_EDIT_MODEL = 711,
        [Description("방전기 모델 정보 제거 실패")]
        ERROR_DELETE_MODEL = 721,
    }

    public class DischargerComm
    {
        public string Name { get; set; } = string.Empty;
        public EDischargerModel EDischargerModel { get; set; }
        public short Channel { get; set; } = short.MaxValue;
        public IPAddress IpAddress { get; set; }
    }

    public class LogTrace : Log
    {
        public static string Path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\LOG\\Trace";
        public static List<string> ListTitle = new List<string>
        {
            "LogDateTime",
            "TraceCode", "TraceLevel", "TraceMnemonic", "TraceDescription", "TraceParameter"
        };

        public static int Code;
        public static string Level;
        public static string Mnemonic;
        public static string Description;
        public static string Parameter;

        public LogTrace(ELogTrace eLogTrace)
        {
            SaveLog(eLogTrace);
        }

        public LogTrace(ELogTrace eLogTrace, string data)
        {
            try
            {
                string logParameter = "\"";

                switch (eLogTrace)
                {
                    case ELogTrace.TRACE_LOGIN:
                    case ELogTrace.ERROR_LOGIN:
                        logParameter += string.Format("ID:{0}", data);
                        break;
                }
                logParameter += "\"";

                SaveLog(eLogTrace, logParameter);
            }
            catch
            {
                SaveLog(eLogTrace);
            }
        }

        public LogTrace(ELogTrace eLogTrace, DischargerComm dischargerComm)
        {
            try
            {
                string logParameter = "\"";

                logParameter += string.Format(
                            "Name:{0}, Model:{1}, Channel:{2}, IPAddress:{3}",
                            dischargerComm.Name,
                            dischargerComm.EDischargerModel,
                            dischargerComm.Channel,
                            dischargerComm.IpAddress);

                logParameter += "\"";

                SaveLog(eLogTrace, logParameter);
            }
            catch
            {
                SaveLog(eLogTrace);
            }
        }






        private void SaveLog(ELogTrace eLogTrace, string parameter = "")
        {
            try
            {
                SetTraceData(eLogTrace, parameter);

                List<string> listContent = new List<string>
                {
                    " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Code.ToString(), Level, Mnemonic, Description, Parameter
                };

                SaveFile(ListTitle, listContent, Path, out string contentAll);
            }
            catch
            {
                Debug.WriteLine("SAVE LOG ERROR : " + eLogTrace.ToString());
            }
        }

        private void SetTraceData(ELogTrace eLogTrace, string parameter = "")
        {
            Code = (int)eLogTrace;
            Level = eLogTrace.ToString().Split('_')[0];
            Mnemonic = eLogTrace.ToString();

            FieldInfo fi = eLogTrace.GetType().GetField(eLogTrace.ToString());
            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
                Description = attributes.First().Description;
            else
                Description = "";

            Parameter = parameter;
        }
    }
}
