using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using Prism.Commands;
using Serial.Client.TempModule;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using static DischargerV2.LOG.LogTrace;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelTempModule
    {
        #region Command
        public DelegateCommand<string> ReconnectTempModuleCommand { get; set; }
        #endregion

        public ModelTempModule Model { get; set; } = new ModelTempModule();

        private System.Timers.Timer OneSecondTimer { get; set; } = null;

        /// <summary>
        /// Key: ComPortString (e.g. COM3)
        /// </summary>
        private Dictionary<string, SerialClientTempModule> _clients = new Dictionary<string, SerialClientTempModule>();

        /// <summary>
        /// Key: ComPortString (e.g. COM3)
        /// </summary>
        private Dictionary<string, TempModuleData> _logParameter = new Dictionary<string, TempModuleData>();

        private int _tempChannelCount = 8;

        private static ViewModelTempModule _instance = null;
        public static ViewModelTempModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelTempModule();
                }
                return _instance;
            }
        }

        public ViewModelTempModule()
        {
            InitializeTempModule();

            ReconnectTempModuleCommand = new DelegateCommand<string>(ReconnectTempModule);
        }

        public void InitializeTempModuleDictionary(List<TableDischargerInfo> infos) 
        {
            Model.TempModuleDictionary.Clear();

            foreach (var info in infos)
            {
                if (info.IsTempModule == true)
                {
                    Model.TempModuleDictionary.Add(info.DischargerName, new TempModule()
                    {
                        ComportIndex = GetTempModuleDataIndex(info.TempModuleComPort),
                        Comport = info.TempModuleComPort,
                        Channel = info.TempChannel
                    });
                }
            }
        }

        public bool IsTempModuleUsed(string dischargerName)
        {
            if (!Model.TempModuleDictionary.ContainsKey(dischargerName))
            {
                return false;
            }

            return true;
        }

        public bool IsConnected(string dischargerName)
        {
            if (!Model.TempModuleDictionary.ContainsKey(dischargerName))
            {
                return false;
            }

            string comport = Model.TempModuleDictionary[dischargerName].Comport;

            if (!_clients.ContainsKey(comport))
            {
                return false;
            }

            return _clients[comport].IsConnected();
        }

        public int GetTempModuleDataIndex(string comPortStr)
        {
            return Model.TempModuleComportList.FindIndex(x => x == comPortStr);
        }

        public double GetTempData(string dischargerName)
        {
            try
            {
                int tempModuleIndex = Model.TempModuleDictionary[dischargerName].ComportIndex;
                int tempModuleChannel = Convert.ToInt32(Model.TempModuleDictionary[dischargerName].Channel);

                return Model.TempDatas[tempModuleIndex][tempModuleChannel];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");

                return 0;
            }
        }

        public void ReconnectTempModule(string dischargerName)
        {
            try
            {
                if (!Model.TempModuleDictionary.ContainsKey(dischargerName))
                {
                    return;
                }

                string comport = Model.TempModuleDictionary[dischargerName].Comport;

                Thread thread = new Thread(delegate ()
                {
                    // 온도 모듈 재 연결
                    var isOk = _clients[comport].Restart();

                    // 온도 모듈 재 연결 Trace Log 저장
                    if (isOk == ETempModuleClientError.Ok)
                    {
                        new LogTrace(ELogTrace.TRACE_RECONNECT_TEMPMODULE, _logParameter[comport]);
                    }
                    else
                    {
                        new LogTrace(ELogTrace.ERROR_RECONNECT_TEMPMODULE, _logParameter[comport]);
                    }
                });

                thread.Start();
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_CONNECT_TEMPMODULE, ex);

                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");
            }
        }

        private void InitializeTempModule()
        {
            FinalizeTempModule();

            List<TableDischargerInfo> infos = SqliteDischargerInfo.GetData();

            foreach (var info in infos)
            {
                try
                {
                    if (Model.TempModuleComportList.Contains(info.TempModuleComPort))
                    {
                        continue;
                    }

                    Model.TempModuleComportList.Add(info.TempModuleComPort);
                    Model.TempDatas.Add(new ObservableCollection<double>());

                    for (int i = 0; i < _tempChannelCount; i++)
                    {
                        Model.TempDatas.Last().Add(0.0);
                    }

                    SerialClientTempModuleStart parameters = new SerialClientTempModuleStart();
                    parameters.DeviceName = "TempModule";
                    parameters.ComPort = info.TempModuleComPort;
                    parameters.BaudRate = 9600;
                    parameters.TimeOutMs = 2000;
                    parameters.Encoding = Encoding.UTF8;
                    parameters.TempModuleChannel = Convert.ToInt32(info.TempModuleChannel);
                    parameters.TempChannelCount = _tempChannelCount;

                    _clients[info.TempModuleComPort] = new SerialClientTempModule();

                    _logParameter[info.TempModuleComPort] = new TempModuleData()
                    {
                        DischargerName = info.DischargerName,
                        EDischargerModel = info.Model,
                        DischargerChannel = info.DischargerChannel,
                        TempModuleComPort = info.TempModuleComPort,
                        TempModuleChannel = info.TempModuleChannel,
                        TempChannel = info.TempChannel,
                    };

                    if (info.IsTempModule)
                    {
                        // 온도 모듈 연결
                        var isOk = _clients[info.TempModuleComPort].Start(parameters);

                        // 온도 모듈 연결 Trace Log 저장
                        if (isOk == ETempModuleClientError.Ok)
                        {
                            new LogTrace(ELogTrace.TRACE_CONNECT_TEMPMODULE, _logParameter[info.TempModuleComPort]);
                        }
                        else
                        {
                            new LogTrace(ELogTrace.ERROR_CONNECT_TEMPMODULE, _logParameter[info.TempModuleComPort]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    new LogTrace(ELogTrace.ERROR_CONNECT_TEMPMODULE, ex);

                    MessageBox.Show(
                        $"Error 발생\n\n" +
                        $"ClassName: {this.GetType().Name}\n" +
                        $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                        $"Exception: {ex.Message}");
                }
            }

            OneSecondTimer?.Stop();
            OneSecondTimer = new System.Timers.Timer();
            OneSecondTimer.Elapsed += CopyDataFromTempModuleClientToModel;
            OneSecondTimer.Interval = 1000;
            OneSecondTimer.Start();
        }

        private void FinalizeTempModule()
        {
            OneSecondTimer?.Stop();
            OneSecondTimer = null;

            /// Client 초기화
            foreach (var client in _clients)
            {
                client.Value.Stop();
            }
            _clients.Clear();

            /// Model 초기화
            Model.TempModuleComportList.Clear();
            foreach (var datas in Model.TempDatas)
            {
                datas.Clear();
            }
            Model.TempDatas.Clear();
        }

        private void CopyDataFromTempModuleClientToModel(object sender, System.Timers.ElapsedEventArgs e)
        {
            // CollectionChanged Event 발생 목적 변경
            ObservableCollection<ObservableCollection<double>> tempDatas = new ObservableCollection<ObservableCollection<double>>();

            for (int i = 0; i < Model.TempModuleComportList.Count; i++)
            {
                tempDatas.Add(new ObservableCollection<double>());

                for (int j = 0; j < _tempChannelCount; j++)
                {
                    tempDatas.Last().Add(0.0);
                }
            }

            foreach (var client in _clients)
            {
                if (!client.Value.IsConnected()) continue;

                string comPortStr = client.Key.ToString();
                int index = Model.TempModuleComportList.FindIndex(x => x == comPortStr);

                for (int i = 0; i < Model.TempDatas[index].Count; i++)
                {
                    tempDatas[index][i] = _clients[comPortStr].GetDatas().TempDatas[i];
                }
            }

            Model.TempDatas = tempDatas;
        }
    }
}
