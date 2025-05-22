using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        #endregion

        private System.Timers.Timer OneSecondTimer { get; set; } = null;

        /// <summary>
        /// Key: discharger name
        /// </summary>
        private Dictionary<string, EthernetClientDischarger> _clients = new Dictionary<string, EthernetClientDischarger>();

        public ObservableCollection<ModelDischarger> Model { get; set; } = new ObservableCollection<ModelDischarger>();

        private ModelDischarger _selectedModel = new ModelDischarger();
        public ModelDischarger SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                SetProperty(ref _selectedModel, value);
            }
        }

        private string _selectedDischargerName = string.Empty;
        public string SelectedDischargerName
        {
            get { return _selectedDischargerName; }
            set
            {
                SetProperty(ref _selectedDischargerName, value);
            }
        }

        private int _allChannelCount = 0;
        public int AllChannelCount
        {
            get { return _allChannelCount; }
            set
            {
                SetProperty(ref _allChannelCount, value);
            }
        }

        private int _connectedChannelCount = 0;
        public int ConnectedChannelCount
        {
            get { return _connectedChannelCount; }
            set
            {
                SetProperty(ref _connectedChannelCount, value);
            }
        }

        private int _faultChannelCount = 0;
        public int FaultChannelCount
        {
            get { return _faultChannelCount; }
            set
            {
                SetProperty(ref _faultChannelCount, value);
            }
        }

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

        private List<TableDischargerInfo> _dischargerInfos = null;

        public ViewModelDischarger()
        {
            InitializeDischarger();

            SelectDischarger(0);

            InitializeDischargerCommand = new DelegateCommand(InitializeDischarger);
            FinalizeDischargerCommand = new DelegateCommand(FinalizeDischarger);

            StartDischargerCommand = new DelegateCommand<StartDischargerCommandParam>(StartDischarger);
            StopDischargerCommand = new DelegateCommand<string>(StopDischarger);
            PauseDischargerCommand = new DelegateCommand<string>(PauseDischarger);

            OpenPopupErrorCommand = new DelegateCommand<string>(OpenPopupError);
            ReconnectDischargerCommand = new DelegateCommand<string>(ReconnectDischarger);
        }

        public void SelectDischarger(int selectedIndex)
        {
            try
            {
                string selectedDischargerName = Model[selectedIndex].DischargerName;

                SelectedDischargerName = selectedDischargerName;
                SelectedModel = Model[selectedIndex];

                ViewModelMain.Instance.Model.DischargerIndex = selectedIndex;
                ViewModelMain.Instance.Model.SelectedDischargerName = selectedDischargerName;
                ViewModelSetMode.Instance.SetDischargerName(selectedDischargerName);
            }
            catch { }
        }

        public void OpenPopupError(string dischargerName)
        {
            int index = Model.ToList().FindIndex(x => x.DischargerName == dischargerName);
            uint errorCode = Model[index].DischargerData.ErrorCode;
            
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
                dischargerName, Model[index].DischargerInfo.Channel,
                tableDischargerErrorCode.Description,
                tableDischargerErrorCode.Code.ToString("X"),
                tableDischargerErrorCode.Cause, 
                tableDischargerErrorCode.Action);

            ViewModelPopup_Error viewModelPopup_Error = new ViewModelPopup_Error()
            {
                Title = title,
                Comment = comment,
                Parameter = dischargerName,
                CallBackDelegate = ResetError,
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_Error(viewModelPopup_Error);
            viewModelMain.OpenPopup(ModelMain.EPopup.Error);
        }

        private void ResetError(string dischargerName)
        {
            _clients[dischargerName].SendCommand_ClearAlarm();

            ReconnectDischarger(dischargerName);
        }

        public void ReconnectDischarger(string dischargerName)
        {
            int index = Model.ToList().FindIndex(x => x.DischargerName == dischargerName);
            Model[index].ReconnectVisibility = Visibility.Collapsed;

            Thread thread = new Thread(
                delegate()
                {
                    _clients[dischargerName].Restart();
                });
            thread.Start();
        }

        public void StartDischarger(StartDischargerCommandParam param)
        {
            _clients[param.DischargerName].SendCommand_StartDischarge(
                EWorkMode.CcCvMode, param.Voltage, param.Current);
        }

        public void StopDischarger(string dischargerName)
        {
            _clients[dischargerName].SendCommand_StopDischarge();
        }

        public void PauseDischarger(string dischargerName)
        {
            _clients[dischargerName].SendCommand_PauseDischarge();
        }

        public void SetSafetyCondition(string dischargerName, 
            double voltageMax, double voltageMin, double currentMax, double currentMin, double tempMax, double tempMin)
        {
            _clients[dischargerName].SendCommand_SetSafetyCondition(voltageMax, voltageMin, currentMax, currentMin, tempMax, tempMin);
        }

        public void SetDischargerState(string dischargerName, EDischargerState eDischargerState)
        {
            _clients[dischargerName].ChangeDischargerState(eDischargerState);
        }

        public bool IsDischarging()
        {
            foreach (var state in Model.ToList().ConvertAll(x => x.DischargerState))
            {
                if (state == EDischargerState.Discharging || state == EDischargerState.Pause)
                {
                    return true;
                }
            }

            return false;
        }

        public void InitializeDischarger()
        {
            FinalizeDischarger();

            _dischargerInfos = SqliteDischargerInfo.GetData();
            List<TableDischargerInfo> infos = _dischargerInfos;

            Model.Clear();
            for (int index = 0; index < infos.Count; index++) 
            {
                var model = new ModelDischarger();

                model.DischargerIndex = index;
                model.No = (index + 1).ToString();
                model.DischargerName = infos[index].DischargerName;
                var dischargerInfo = InitializeDischargerInfos(infos[index].DischargerName);
                model.DischargerInfo = dischargerInfo;
                InitializeDischargerClients(dischargerInfo);

                model.PropertyChanged += Model_PropertyChanged;

                Model.Add(model);
            }

            ViewModelTempModule.Instance.InitializeTempModuleDictionary(infos);

            OneSecondTimer?.Stop();
            OneSecondTimer = new System.Timers.Timer();
            OneSecondTimer.Elapsed += CopyDataFromDischargerClientToModel;
            OneSecondTimer.Interval = 500;
            OneSecondTimer.Start();
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ModelDischarger model)
            {
                if (e.PropertyName == nameof(ModelDischarger.DischargerState))
                {
                    UpdateChannelState();

                    Thread thread = new Thread(() =>
                    {
                        UpdateMonitoringUI(model);
                    });
                    thread.IsBackground = true;
                    thread.Start();
                }
            }
        }

        private void UpdateMonitoringUI(ModelDischarger model)
        {
            model.ErrorDetailButtonVisibility = Visibility.Collapsed;

            // 버튼 UI 업데이트
            if (model.DischargerState == EDischargerState.Pause)
            {
                ViewModelMonitor_State.Instance.Model.PauseNResumeIsEnable = false;
                ViewModelMonitor_State.Instance.Model.StopIsEnable = false;
                /// 방전기가 pause를 반영하는 시간
                Thread.Sleep(3000);
                model.ResumeButtonVisibility = Visibility.Visible;
                ViewModelMonitor_State.Instance.Model.PauseNResumeIsEnable = true;
                ViewModelMonitor_State.Instance.Model.StopIsEnable = true;
            }
            else if (model.DischargerState == EDischargerState.Discharging)
            {
                ViewModelMonitor_State.Instance.Model.PauseNResumeIsEnable = false;
                ViewModelMonitor_State.Instance.Model.StopIsEnable = false;
                Thread.Sleep(3000);
                model.ResumeButtonVisibility = Visibility.Collapsed;
                ViewModelMonitor_State.Instance.Model.PauseNResumeIsEnable = true;
                ViewModelMonitor_State.Instance.Model.StopIsEnable = true;
            }
            else if (model.DischargerState == EDischargerState.SafetyOutOfRange ||
                model.DischargerState == EDischargerState.ReturnCodeError ||
                model.DischargerState == EDischargerState.ChStatusError ||
                model.DischargerState == EDischargerState.DeviceError)
            {
                model.ErrorDetailButtonVisibility = Visibility.Visible;
            }
        }

        private void UpdateChannelState()
        {
            ConnectedChannelCount = 0;
            FaultChannelCount = 0;
            foreach (var model in Model)
            {
                AllChannelCount = Model.Count;

                if (model.DischargerState == EDischargerState.Connecting ||
                    model.DischargerState == EDischargerState.Ready ||
                    model.DischargerState == EDischargerState.Discharging ||
                    model.DischargerState == EDischargerState.Pause)
                {
                    ConnectedChannelCount += 1;
                }
                else
                {
                    FaultChannelCount += 1;
                }
            }
        }

        public void FinalizeDischarger()
        {
            OneSecondTimer?.Stop();
            OneSecondTimer = null;

            foreach (var client in _clients)
            {
                client.Value.Stop();
            }
            _clients.Clear();

            foreach (var model in Model)
            {
                model.PropertyChanged -= Model_PropertyChanged;
            }
            Model.Clear();
        }

        private DischargerInfo InitializeDischargerInfos(string name)
        {
            List<TableDischargerInfo> infoTable = _dischargerInfos;
            TableDischargerInfo info = infoTable.Find(x => x.DischargerName == name);

            List<TableDischargerModel> modelTable = SqliteDischargerModel.GetData();
            modelTable = (from one in modelTable where one.Model == info.Model select one).ToList();
            modelTable = (from one in modelTable where one.Type == info.Type select one).ToList();
            modelTable = (from one in modelTable where one.Channel == info.DischargerChannel select one).ToList();
            modelTable = (from one in modelTable where one.SpecVoltage == info.SpecVoltage select one).ToList();
            modelTable = (from one in modelTable where one.SpecCurrent == info.SpecCurrent select one).ToList();
            TableDischargerModel model = modelTable.First();

            DischargerInfo dischargerInfo = new DischargerInfo();
            dischargerInfo.Model = model.Model;
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

            return dischargerInfo;
        }

        private void InitializeDischargerClients(DischargerInfo info)
        {
            EthernetClientDischargerStart parameters = new EthernetClientDischargerStart();
            parameters.DischargerModel = info.Model;
            parameters.DischargerName = info.Name;
            parameters.DischargerChannel = info.Channel;
            parameters.IpAddress = info.IpAddress;
            parameters.EthernetPort = info.EthernetPort;
            parameters.TimeOutMs = info.TimeOutMs;
            parameters.SafetyVoltageMax = info.SafetyVoltageMax;
            parameters.SafetyVoltageMin = info.SafetyVoltageMin;
            parameters.SafetyCurrentMax = info.SafetyCurrentMax;
            parameters.SafetyCurrentMin = info.SafetyCurrentMin;
            parameters.SafetyTempMax = info.SafetyTempMax;
            parameters.SafetyTempMin = info.SafetyTempMin;
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
            for (int i = 0; i < Model.Count; i++)
            {
                Model[i].DischargerData = _clients[Model[i].DischargerName].GetDatas();
                Model[i].DischargerState = _clients[Model[i].DischargerName].GetState();

                // 온도 모듈이 있을 경우 온도 모듈 데이터 사용
                try
                {
                    var dischargerInfo = _dischargerInfos.Find(x => x.DischargerName == Model[i].DischargerName);
                    if (dischargerInfo.IsTempModule)
                    {
                        int index = ViewModelTempModule.Instance.Model.TempModuleComportList.FindIndex(x => x == dischargerInfo.TempModuleComPort);
                        if (index >= 0)
                        {
                            var tempDatas = ViewModelTempModule.Instance.Model.TempDatas;
                            Model[i].DischargerData.ReceiveDischargeTemp = tempDatas[index][int.Parse(dischargerInfo.TempChannel)];
                        }
                    }
                }
                catch
                {
                    // nothing to do.
                }
            }

            SetStateColor();
            SetProgressTime();
            SetVisibility();
        }

        private void SetStateColor()
        {
            for (int index = 0; index < Model.Count; index++) 
            {
                var state = Model[index].DischargerState;

                if (state == EDischargerState.None || state == EDischargerState.Disconnected || state == EDischargerState.Connecting)
                {
                    Model[index].StateColor = ResColor.icon_disabled;
                }
                else if (state == EDischargerState.Ready || state == EDischargerState.Discharging)
                {
                    Model[index].StateColor = ResColor.icon_success;
                }
                else if (state == EDischargerState.Pause)
                {
                    Model[index].StateColor = ResColor.icon_primary;
                }
                else
                {
                    Model[index].StateColor = ResColor.icon_error;
                }
            }
        }

        private void SetProgressTime()
        {
            for (int index = 0; index < Model.Count; index++)
            {
                var state = Model[index].DischargerState;
                var dischargingStartTime = Model[index].DischargerData.DischargingStartTime;

                if (state == EDischargerState.Discharging && state == EDischargerState.Pause)
                {
                    TimeSpan diff = (DateTime.Now - dischargingStartTime);
                    string diffString =
                        diff.Hours.ToString("D2") + ":" +
                        diff.Minutes.ToString("D2") + ":" +
                        diff.Seconds.ToString("D2");
                    Model[index].ProgressTime = diffString;
                }
            }
        }

        private void SetVisibility()
        {
            for (int index = 0; index < Model.Count; index++)
            {
                var state = Model[index].DischargerState;

                if (state == EDischargerState.Disconnected)
                {
                    Model[index].ReconnectVisibility = Visibility.Visible;
                    Model[index].ErrorVisibility = Visibility.Collapsed;
                }
                else if (state == EDischargerState.SafetyOutOfRange ||
                    state == EDischargerState.ReturnCodeError ||
                    state == EDischargerState.ChStatusError ||
                    state == EDischargerState.DeviceError)
                {
                    Model[index].ReconnectVisibility = Visibility.Collapsed;
                    Model[index].ErrorVisibility = Visibility.Visible;
                }
                else
                {
                    Model[index].ReconnectVisibility = Visibility.Collapsed;
                    Model[index].ErrorVisibility = Visibility.Collapsed;
                }

                // 온도 모듈 새로 고침 visibility 설정
                if (!ViewModelTempModule.Instance.IsTempModuleUsed(Model[index].DischargerName))
                {
                    Model[index].TempReconnectVisibility = Visibility.Collapsed;
                }
                else
                {
                    if (!ViewModelTempModule.Instance.IsConnected(Model[index].DischargerName))
                    {
                        Model[index].TempReconnectVisibility = Visibility.Visible;
                    }
                    else
                    {
                        Model[index].TempReconnectVisibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }
}
