using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.CodeDom;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Prism.Common;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;

namespace Discharger.MVVM.Repository
{
    public class LogArgument
    {
        public LogArgument(string logMessage)
        {
            LogMessage = logMessage;
        }

        public string LogMessage { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    public static class RepoLog
    {
        private class LogInstance
        {
            public string LogFilePathAndName = string.Empty;
            public object DataLock = new object();
            public List<string> Logs = new List<string>();
        }

        /// <summary>
        /// Key: logKey
        /// </summary>
        private static Dictionary<string, LogInstance> _logInstance = new Dictionary<string, LogInstance>();

        public static bool IsLogKeyExist(string logKey)
        {
            if (_logInstance.ContainsKey(logKey))
            {
                return true;
            }

            return false;
        }

        public static void CreateLogInstance(string logKey, string logFilePath, string logFileName)
        {
            if (_logInstance.ContainsKey(logKey))
            {
                throw new ArgumentException("logKey 값이 중복됩니다.");
            }

            _logInstance[logKey] = new LogInstance();

            DirectoryInfo directoryInfo = new DirectoryInfo(logFilePath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            _logInstance[logKey].LogFilePathAndName = logFilePath + logKey + "_" + logFileName;

            FileStream fileStream = new FileStream(_logInstance[logKey].LogFilePathAndName, FileMode.Create);
            StreamWriter file = new StreamWriter(fileStream);
            file.Flush();
            file.Close();
        }

        public static void RemoveLogInstance(string logKey)
        {
            if (_logInstance.ContainsKey(logKey))
            {
                _logInstance[logKey].Logs.Clear();
                _logInstance[logKey].DataLock = null;
                _logInstance.Remove(logKey);
            }
        }

        private static void AppendLogFile(string logKey, string appendString)
        {
            try
            {
                FileStream fileStream = new FileStream(_logInstance[logKey].LogFilePathAndName, FileMode.Append);
                StreamWriter file = new StreamWriter(fileStream);
                file.WriteLine(appendString);
                file.Flush();
                file.Close();
            }
            catch
            {

            }
        }

        public static List<string> GetLogs(string logKey)
        {
            if (!_logInstance.ContainsKey(logKey))
            {
                return new List<string>();
            }

            lock (_logInstance[logKey].DataLock)
            {
                return _logInstance[logKey].Logs.ConvertAll(x => x);
            }
        }

        public static void AddLog(string logKey, string logString)
        {
            if (!_logInstance.ContainsKey(logKey))
            {
                return;
            }

            AppendLogFile(logKey, logString);

            lock (_logInstance[logKey].DataLock)
            {
                if (_logInstance[logKey].Logs.Count > 1000)
                {
                    _logInstance[logKey].Logs.RemoveRange(0, 200);
                }

                _logInstance[logKey].Logs.Add(logString);
            }
        }

        public static void AddLog(string logKey, LogArgument logFormat)
        {
            if (!_logInstance.ContainsKey(logKey))
            {
                return;
            }

            string formattedMessage = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ");
            formattedMessage += logFormat.LogMessage + " (";
            for (int i = 0; i < logFormat.Parameters.Count; i++)
            {
                string key = logFormat.Parameters.Keys.ElementAt(i);
                string value = logFormat.Parameters.Values.ElementAt(i).ToString();

                formattedMessage += key + ": " + value;

                if (i != logFormat.Parameters.Count - 1)
                {
                    formattedMessage += ", ";
                }
            }
            formattedMessage += ")";

            AppendLogFile(logKey, formattedMessage);

            lock (_logInstance[logKey].DataLock)
            {
                if (_logInstance[logKey].Logs.Count > 1000)
                {
                    _logInstance[logKey].Logs.RemoveRange(0, 200);
                }

                _logInstance[logKey].Logs.Add(formattedMessage);
            }
        }
    }
}
