using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using Microsoft.WindowsAPICodePack.Shell;
using Prism.Commands;
using Prism.Mvvm;
using Serial.Client.TempModule;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using static System.Windows.Forms.AxHost;

namespace DischargerV2.MVVM.ViewModels
{
    class StartDischargerCommandParamConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            StartDischargerCommandParam param = new StartDischargerCommandParam();
            param.DischargerName = (string)Values[0];
            param.Voltage = (double)Values[1];
            param.Current = (double)Values[2];

            return param;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StartDischargerCommandParam
    {
        public string DischargerName { get; set; } = string.Empty;
        public double Current { get; set; } = 0.0;
        public double Voltage { get; set; } = 0.0;
    }

    public class ViewModelDischarger : BindableBase
    {
        #region Command
        public DelegateCommand InitializeDischargerCommand { get; set; }
        public DelegateCommand FinalizeDischargerCommand { get; set; }

        public DelegateCommand<StartDischargerCommandParam> StartDischargerCommand { get; set; }
        public DelegateCommand<string> StopDischargerCommand { get; set; }
        public DelegateCommand<string> PauseDischargerCommand { get; set; }

        public DelegateCommand<string> OpenPopupErrorCommand { get; set; }
        public DelegateCommand<string> ReconnectDischargerCommand { get; set; }

        public DelegateCommand<string> SelectDischargerCommand { get; set; }
        #endregion

        private System.Timers.Timer OneSecondTimer { get; set; } = null;

        /// <summary>
        /// Key: discharger name
        /// </summary>
        private Dictionary<string, EthernetClientDischarger> _clients = new Dictionary<string, EthernetClientDischarger>();

        public ModelDischarger Model { get; set; } = new ModelDischarger();

        private static ViewModelDischarger _instance = null;
        public static ViewModelDischarger Instance
        {
            get
            {
                if (_instance == null )
                {
                    _instance = new ViewModelDischarger();
                }
                return _instance;
            }
        }

        public ViewModelDischarger()
        {
            InitializeDischarger();

            InitializeDischargerCommand = new DelegateCommand(InitializeDischarger);
            FinalizeDischargerCommand = new DelegateCommand(FinalizeDischarger);

            StartDischargerCommand = new DelegateCommand<StartDischargerCommandParam>(StartDischarger);
            StopDischargerCommand = new DelegateCommand<string>(StopDischarger);
            PauseDischargerCommand = new DelegateCommand<string>(PauseDischarger);

            OpenPopupErrorCommand = new DelegateCommand<string>(OpenPopupError);
            ReconnectDischargerCommand = new DelegateCommand<string>(ReconnectDischarger);

            SelectDischargerCommand = new DelegateCommand<string>(SelectDischarger);
        }

        private void SelectDischarger(string strSelectedIndex)
        {
            try
            {
                if (Int32.TryParse(strSelectedIndex, out int selectedIndex))
                {
                    string selectedDischargerName = Model.DischargerNameList[selectedIndex];

                    Model.SelectedDischargerName = selectedDischargerName;

                    ViewModelMain.Instance.Model.SelectedDischargerName = selectedDischargerName;
                    ViewModelSetMode.Instance.SetDischargerName(selectedDischargerName);
                }
            }
            catch { }
        }

        private void OpenPopupError(string dischargerName)
        {
            int index = Model.DischargerNameList.ToList().FindIndex(x => x == dischargerName);
            uint errorCode = Model.DischargerDatas[index].ErrorCode;
            
            List<TableDischargerErrorCode> tableDischargerErrorCodeList = SqliteDischargerErrorCode.GetData();
            TableDischargerErrorCode tableDischargerErrorCode = tableDischargerErrorCodeList.Find(x => x.Code == errorCode);

            if (tableDischargerErrorCode == null) return;

            string title = tableDischargerErrorCode.Title;
            string comment = string.Format(
                "{0} (Channel: {1})\n\n" +
                "{2} 오류입니다.\n" +
                "(Error Code: 0x{3})\n\n" +
                "원인: \n{4}\n\n" +
                "해결 방법: \n{5}",
                dischargerName, Model.DischargerInfos[index].Channel,
                tableDischargerErrorCode.Description,
                tableDischargerErrorCode.Code.ToString("X"),
                tableDischargerErrorCode.Cause, 
                tableDischargerErrorCode.Action);

            ViewModelPopup_Error viewModelPopup_Error = new ViewModelPopup_Error()
            {
                Title = title,
                Comment = comment,
                CallBackDelegate = ResetError,
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_Error(viewModelPopup_Error);
            viewModelMain.OpenPopup(ModelMain.EPopup.Error);
        }

        private void ResetError(string dischargerName)
        {
            _clients[dischargerName].SendCommand_ClearAlarm();
        }

        private void ReconnectDischarger(string dischargerName)
        {
            int index = Model.DischargerNameList.ToList().FindIndex(x => x == dischargerName);
            Model.ReconnectVisibility[index] = Visibility.Collapsed;

            Thread thread = new Thread(
                delegate()
                {
                    _clients[dischargerName].Restart();
                });
            thread.Start();
        }

        private void StartDischarger(StartDischargerCommandParam param)
        {
            _clients[param.DischargerName].SendCommand_StartDischarge(
                EWorkMode.CcCvMode, param.Voltage, param.Current);
        }

        private void StopDischarger(string dischargerName)
        {
            _clients[dischargerName].SendCommand_StopDischarge();
        }

        private void PauseDischarger(string dischargerName)
        {
            _clients[dischargerName].SendCommand_PauseDischarge();
        }

        private void InitializeDischarger()
        {
            FinalizeDischarger();

            List<TableDischargerInfo> infos = SqliteDischargerInfo.GetData();

            for (int index = 0; index < infos.Count; index++) 
            {
                Model.DischargerDatas.Add(new DischargerDatas());
                Model.DischargerStates.Add(EDischargerState.None);
                Model.StateColor.Add(ResColor.icon_disabled);
                Model.ProgressTime.Add("00:00:00");
                Model.ReconnectVisibility.Add(Visibility.Collapsed);
                Model.ErrorVisibility.Add(Visibility.Collapsed);

                var dischargerInfo = InitializeDischargerInfos(infos[index].DischargerName);
                InitializeDischargerClients(dischargerInfo);

                Model.DischargerNameList.Add(infos[index].DischargerName);
            }

            OneSecondTimer?.Stop();
            OneSecondTimer = new System.Timers.Timer();
            OneSecondTimer.Elapsed += CopyDataFromDischargerClientToModel;
            OneSecondTimer.Interval = 1000;
            OneSecondTimer.Start();
        }

        private void FinalizeDischarger()
        {
            OneSecondTimer?.Stop();
            OneSecondTimer = null;

            foreach (var client in _clients)
            {
                client.Value.Stop();
            }
            _clients.Clear();

            Model.DischargerNameList.Clear();
            Model.SelectedDischargerName = string.Empty;
            Model.DischargerDatas.Clear();
            Model.DischargerInfos.Clear();
            Model.DischargerStates.Clear();
            Model.StateColor.Clear();
            Model.ProgressTime.Clear();
            Model.ReconnectVisibility.Clear();
            Model.ErrorVisibility.Clear();
        }

        private DischargerInfo InitializeDischargerInfos(string name)
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

            DischargerInfo dischargerInfo = new DischargerInfo();
            dischargerInfo.Name = name;
            dischargerInfo.Channel = info.DischargerChannel;
            dischargerInfo.IpAddress = IPAddress.Parse(info.IpAddress);
            dischargerInfo.EthernetPort = 10004;
            dischargerInfo.TimeOutMs = 1000;
            dischargerInfo.SpecVoltage = info.SpecVoltage;
            dischargerInfo.SpecCurrent = info.SpecCurrent;
            dischargerInfo.SafetyVoltageMax = model.SafetyVoltMax;
            dischargerInfo.SafetyVoltageMin = model.SafetyVoltMin;
            dischargerInfo.SafetyCurrentMax = model.SafetyCurrentMax;
            dischargerInfo.SafetyCurrentMin = model.SafetyCurrentMin;
            dischargerInfo.SafetyTempMax = model.SafetyTempMax;
            dischargerInfo.SafetyTempMin = model.SafetyTempMin;
            Model.DischargerInfos.Add(dischargerInfo);

            return dischargerInfo;
        }

        private void InitializeDischargerClients(DischargerInfo info)
        {
            EthernetClientDischargerStart parameters = new EthernetClientDischargerStart();
            parameters.DischargerName = info.Name;
            parameters.DischargerChannel = info.Channel;
            parameters.IpAddress = info.IpAddress;
            parameters.EthernetPort = info.EthernetPort;
            parameters.TimeOutMs = info.TimeOutMs;
            parameters.SafetyVoltageMax = info.SafetyVoltageMax;
            parameters.SafetyVoltageMin = info.SafetyVoltageMin;
            parameters.SafetyCurrentMax = info.SafetyCurrentMax;
            parameters.SafetyCurrentMin = info.SafetyCurrentMin;
            _clients[info.Name] = new EthernetClientDischarger();

            Thread thread = new Thread(
                delegate ()
                {
                    _clients[info.Name].Start(parameters);
                });
            thread.Start();
        }

        private void CopyDataFromDischargerClientToModel(object sender, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < Model.DischargerNameList.Count; i++)
            {
                Model.DischargerDatas[i] = _clients[Model.DischargerNameList[i]].GetDatas();
                Model.DischargerStates[i] = _clients[Model.DischargerNameList[i]].GetState();
                SetStateColor(Model.DischargerStates[i], i);
                SetProgressTime(Model.DischargerStates[i], Model.DischargerDatas[i].DischargingStartTime, i);
                SetVisibility(Model.DischargerStates[i], i);
            }
        }

        private void SetVisibility(EDischargerState state, int index)
        {
            if (state == EDischargerState.Disconnected)
            {
                Model.ReconnectVisibility[index] = Visibility.Visible;
                Model.ErrorVisibility[index] = Visibility.Collapsed;
            }
            else if (state == EDischargerState.SafetyOutOfRange ||
                state == EDischargerState.ReturnCodeError ||
                state == EDischargerState.ChStatusError ||
                state == EDischargerState.DeviceError)
            {
                Model.ReconnectVisibility[index] = Visibility.Collapsed;
                Model.ErrorVisibility[index] = Visibility.Visible;
            }
            else
            {
                Model.ReconnectVisibility[index] = Visibility.Collapsed;
                Model.ErrorVisibility[index] = Visibility.Collapsed;
            }
        }

        private void SetProgressTime(EDischargerState state, DateTime dischargingStartTime, int index)
        {
            if (state == EDischargerState.Discharging &&
                state == EDischargerState.Pause)
            {
                TimeSpan diff = (DateTime.Now - dischargingStartTime);
                string diffString =
                    diff.Hours.ToString("D2") + ":" +
                    diff.Minutes.ToString("D2") + ":" +
                    diff.Seconds.ToString("D2");
                Model.ProgressTime[index] = diffString;
            }
        }

        private void SetStateColor(EDischargerState state, int index)
        {
            if (state == EDischargerState.None || state == EDischargerState.Disconnected || state == EDischargerState.Connecting)
            {
                Model.StateColor[index] = ResColor.icon_disabled;
            }
            else if (state == EDischargerState.Ready || state == EDischargerState.Discharging)
            {
                Model.StateColor[index] = ResColor.icon_success;
            }
            else if (state == EDischargerState.Pause)
            {
                Model.StateColor[index] = ResColor.icon_primary;
            }
            else
            {
                Model.StateColor[index] = ResColor.icon_error;
            }
        }
    }
}
