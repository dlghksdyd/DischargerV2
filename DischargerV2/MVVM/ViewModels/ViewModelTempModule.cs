using DischargerV2.MVVM.Models;
using Prism.Commands;
using Serial.Client.TempModule;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelTempModule
    {
        #region Command


        #endregion

        public ModelTempModule Model { get; set; } = new ModelTempModule();

        private System.Timers.Timer OneSecondTimer { get; set; } = null;

        /// <summary>
        /// Key: ComPortString (e.g. COM3)
        /// </summary>
        private Dictionary<string, SerialClientTempModule> _clients = new Dictionary<string, SerialClientTempModule>();

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
        }

        public bool IsConnected(string comPortStr)
        {
            if (!_clients.ContainsKey(comPortStr))
            {
                return false;
            }

            return _clients[comPortStr].IsConnected();
        }

        public int GetTempModuleDataIndex(string comPortStr)
        {
            return Model.TempModuleComportList.FindIndex(x => x == comPortStr);
        }

        private void InitializeTempModule()
        {
            FinalizeTempModule();

            List<TableDischargerInfo> infos = SqliteDischargerInfo.GetData();

            foreach (var info in infos)
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
                _clients[info.TempModuleComPort].Start(parameters);
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
            foreach (var client in _clients)
            {
                if (!client.Value.IsConnected())
                {
                    continue;
                }

                string comPortStr = client.Key.ToString();
                int index = Model.TempModuleComportList.FindIndex(x => x == comPortStr);

                for (int i = 0; i < Model.TempDatas[index].Count; i++)
                {
                    Model.TempDatas[index][i] = _clients[comPortStr].GetDatas().TempDatas[i];
                }
            }
        }
    }
}
