using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Serial.Client.Common;

namespace Serial.Client.TempModule
{
    public enum ETempModuleState
    {
        None = 0x0,

        Disconnect = 0x10,
        Connected = 0x11,
    }

    public enum ETempModuleClientError
    {
        Ok,

        INVALID_PARAMETER,
        FAIL_TO_CONNECT,

        FAIL_PROCESS_PACKET,
    }

    public class SerialClientTempModuleStart
    {
        public string DeviceName = string.Empty;
        public string ComPort = string.Empty;
        public int BaudRate = int.MaxValue;
        public Encoding Encoding = Encoding.UTF8;
        public int TempModuleChannel = int.MaxValue;
        public int TempChannelCount = int.MaxValue;
    }

    public class TempModuleDatas
    {
        /// <summary>
        /// 수신 받은 데이터
        /// </summary>
        public List<double> TempDatas = new List<double>();
    }

    public class SerialClientTempModule
    {
        private class LogArgument
        {
            public LogArgument(string logMessage)
            {
                LogMessage = logMessage;
            }

            public string LogMessage { get; set; } = string.Empty;
            public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        }

        private SerialClient _tempModuleClient = null;

        private System.Timers.Timer _readInfoTimer = null;

        private SerialClientTempModuleStart _parameters = null;

        private TempModuleDatas _tempModuleDatas = new TempModuleDatas();

        private ETempModuleState _tempModuleState = ETempModuleState.None;

        private List<string> _traceLogs = new List<string>();
        private object _traceLogLock = new object();

        private void AddTraceLog(LogArgument logFormat)
        {
            string formattedMessage = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ");
            formattedMessage += _parameters.DeviceName + " - ";
            formattedMessage += logFormat.LogMessage + " ";

            for (int i = 0; i < logFormat.Parameters.Count; i++)
            {
                string key = logFormat.Parameters.Keys.ElementAt(i);
                string value = logFormat.Parameters.Values.ElementAt(i).ToString();

                if (i == 0)
                {
                    formattedMessage += "(";
                }

                formattedMessage += key + ": " + value;

                if (i < logFormat.Parameters.Count - 1)
                {
                    formattedMessage += ", ";
                }
                else
                {
                    formattedMessage += ")";
                }
            }

            lock (_traceLogLock)
            {
                _traceLogs.Add(formattedMessage);
            }
        }

        private bool IsParameterValid(SerialClientTempModuleStart parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            if (parameters.DeviceName == string.Empty ||
                parameters.ComPort == string.Empty ||
                parameters.BaudRate == int.MaxValue ||
                parameters.TempModuleChannel == int.MaxValue ||
                parameters.TempChannelCount == int.MaxValue)
            {
                return false;
            }

            return true;
        }

        private void ChangeTempModuleState(ETempModuleState state)
        {
            if (_tempModuleState != state)
            {
                LogArgument logArgument = new LogArgument("Enter " + state + " State.");
                AddTraceLog(logArgument);
            }

            _tempModuleState = state;
        }

        public bool IsConnected()
        {
            if (_tempModuleClient.IsConnected())
            {
                return true;
            }

            return false;
        }

        public ETempModuleClientError Start(SerialClientTempModuleStart parameters)
        {
            if (!IsParameterValid(parameters))
            {
                return ETempModuleClientError.INVALID_PARAMETER;
            }

            _parameters = parameters;

            SerialClientStart clientStart = new SerialClientStart();
            clientStart.DeviceName = _parameters.DeviceName;
            clientStart.ComPortStr = _parameters.ComPort;
            clientStart.BaudRate = _parameters.BaudRate;
            clientStart.Encoding = _parameters.Encoding;
            clientStart.WriteFunction = WriteData;
            clientStart.ReadFunction = ReadData;
            clientStart.ParseFunction = ParseData;
            _tempModuleClient = new SerialClient();

            var result = _tempModuleClient.Connect(clientStart);
            if (result != ESerialClientStatus.OK)
            {
                LogArgument logArgument = new LogArgument("Fail to connect.");
                AddTraceLog(logArgument);

                ChangeTempModuleState(ETempModuleState.Disconnect);

                return ETempModuleClientError.FAIL_TO_CONNECT;
            }

            /// 온도 데이터 리스트 초기화
            _tempModuleDatas.TempDatas = new List<double>();
            for (int i = 0; i < _parameters.TempChannelCount; i++)
            {
                _tempModuleDatas.TempDatas.Add(0.0);
            }

            _readInfoTimer?.Stop();
            _readInfoTimer = null;
            _readInfoTimer = new System.Timers.Timer();
            _readInfoTimer.Interval = 1000;
            _readInfoTimer.Elapsed += OneSecondTimer_Elapsed;
            _readInfoTimer.Start();

            ChangeTempModuleState(ETempModuleState.Connected);

            return ETempModuleClientError.Ok;
        }

        public TempModuleDatas GetDatas()
        {
            TempModuleDatas temp = new TempModuleDatas();
            temp.TempDatas = _tempModuleDatas.TempDatas.ConvertAll(x => x);

            return temp;
        }

        public ETempModuleState GetState()
        {
            return _tempModuleState;
        }

        public List<string> GetTraceLogs()
        {
            lock (_traceLogLock)
            {
                List<string> temp = _traceLogs.ConvertAll(x => x);
                _traceLogs.Clear();

                return temp;
            }
        }

        public void Stop()
        {
            _tempModuleClient?.Disconnect();
            _tempModuleClient = null;

            _readInfoTimer?.Stop();
            _readInfoTimer = null;

            ChangeTempModuleState(ETempModuleState.Disconnect);
        }

        private void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_tempModuleClient.IsConnected())
            {
                ChangeTempModuleState(ETempModuleState.Disconnect);

                return;
            }

            SendCommand_RequestTemperature();
        }

        private ETempModuleClientError SendCommand_RequestTemperature()
        {
            string writeString = "#" + _parameters.TempModuleChannel.ToString("D2") + "\r\n";

            bool result = _tempModuleClient.ProcessPacket(writeString, "\r");
            if (result == false)
            {
                LogArgument logArgument = new LogArgument("Fail to process request temperature command.");
                AddTraceLog(logArgument);

                return ETempModuleClientError.FAIL_PROCESS_PACKET;
            }

            return ETempModuleClientError.Ok;
        }

        private bool ReadData(string comPortStr, out string readStr, string breakStr)
        {
            ESerialClientStatus result = _tempModuleClient.Read(comPortStr, out readStr, breakStr);
            if (result != ESerialClientStatus.OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                LogArgument logArgument = new LogArgument("Fail to read data.");
                AddTraceLog(logArgument);

                return false;
            }

            return true;
        }

        private bool WriteData(string comPortStr, string writeData)
        {
            if (writeData == null || writeData.Length == 0)
            {
                Debug.WriteLine("Write Error: Write String is Empty.");

                LogArgument logArgument = new LogArgument("Write data is empty.");
                AddTraceLog(logArgument);

                return false;
            }

            ESerialClientStatus result = _tempModuleClient.Write(comPortStr, writeData);
            if (result != ESerialClientStatus.OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                LogArgument logArgument = new LogArgument("Fail to write data.");
                AddTraceLog(logArgument);

                return false;
            }

            return true;
        }

        private bool ParseData(string readStr)
        {
            LogArgument logArgument = new LogArgument("Read Temperature Data.");
            logArgument.Parameters["RawData"] = readStr;
            AddTraceLog(logArgument);

            // 데이터 read (ex>">+025.12+020.45+012.78+018.97+003.24+015.35+008.07+014.79")
            readStr = readStr.Replace(">", "");

            for (int i = 0; i < _parameters.TempChannelCount; i++)
            {
                double tempData = double.Parse(readStr.Substring(i * 7, 7));

                _tempModuleDatas.TempDatas[i] = tempData;
            }

            return true;
        }
    }
}
