using DischargerV2.LOG;
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
using static DischargerV2.LOG.LogTrace;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

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

        public string LogFileName { get; set; } = string.Empty;

        public bool? IsRestart { get; set; } = null;
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

            string title = string.Empty;
            string comment = string.Empty;
            if (tableDischargerErrorCode == null)
            {
                title = "Unknown error.";
                comment = string.Format(
                "{0} (Channel: {1})\n\n" +
                "{2} 오류입니다.\n" +
                "(Error Code: 0x{3})\n\n" +
                "원인: \n{4}\n\n" +
                "해결 방법: \n{5}",
                dischargerName, Model[index].DischargerInfo.Channel,
                "알 수 없는",
                errorCode.ToString("X"),
                "알 수 없음",
                "알 수 없음");
            }
            else
            {
                title = tableDischargerErrorCode.Title;
                comment = string.Format(
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
            }

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

        /// <summary>
        /// 방전기 에러 해제 
        /// </summary>
        /// <param name="dischargerName"></param>
        private void ResetError(string dischargerName)
        {
            try
            {
                // 방전기 에러 해제
                bool isOk = _clients[dischargerName].SendCommand_ClearAlarm();

                // 방전기 에러 해제 Trace Log 저장
                DischargerData dischargerComm = _clients[dischargerName].GetLogSystemDischargerData();

                if (isOk)
                {
                    new LogTrace(ELogTrace.TRACE_CLEAR_ALARM, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_CLEAR_ALARM, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_CLEAR_ALARM, ex);
            }
            
            ReconnectDischarger(dischargerName);
        }

        /// <summary>
        /// 방전기 재 연결
        /// </summary>
        /// <param name="dischargerName"></param>
        public void ReconnectDischarger(string dischargerName)
        {
            int index = Model.ToList().FindIndex(x => x.DischargerName == dischargerName);
            Model[index].ReconnectVisibility = Visibility.Collapsed;

            try
            {
                Thread thread = new Thread(
                    delegate ()
                    {
                        // 방전기 재 연결
                        bool isOk = _clients[dischargerName].Restart();

                        // 방전기 재 연결 Trace Log 저장
                        DischargerData dischargerComm = _clients[dischargerName].GetLogSystemDischargerData();

                        if (isOk)
                        {
                            new LogTrace(ELogTrace.TRACE_RECONNECT_DISCHARGER, dischargerComm);
                        }
                        else
                        {
                            new LogTrace(ELogTrace.ERROR_RECONNECT_DISCHARGER, dischargerComm);
                        }
                    });

                thread.Start();
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_RECONNECT_DISCHARGER, ex);
            }
        }

        public void StartDischarger(StartDischargerCommandParam param)
        {
            bool? isRestart = param.IsRestart;

            try
            {
                // 방전 동작 로그 파일명 설정
                _clients[param.DischargerName].SetLogFileName(param.LogFileName);

                // 방전 동작 시작
                var isOk = _clients[param.DischargerName].SendCommand_StartDischarge(
                    EWorkMode.CcCvMode, param.Voltage, param.Current);

                // 방전 동작 시작 Trace Log 저장
                LogTrace.DischargerData dischargerComm = _clients[param.DischargerName].GetLogSystemDischargerData();
                dischargerComm.EWorkMode = EWorkMode.CcCvMode;
                dischargerComm.SetValue_Voltage = param.Voltage;
                dischargerComm.LimitingValue_Current = param.Current;

                if (isOk == EDischargerClientError.Ok)
                {
                    if (isRestart != null)
                    {
                        if (isRestart == false)
                        {
                            new LogTrace(ELogTrace.TRACE_START_DISCHARGE, dischargerComm);
                        }
                        else
                        {
                            new LogTrace(ELogTrace.TRACE_RESTART_DISCHARGE, dischargerComm);
                        }
                    }
                }
                else
                {
                    if (isRestart != null)
                    {
                        if (isRestart == false)
                        {
                            new LogTrace(ELogTrace.ERROR_START_DISCHARGE, dischargerComm);
                        }
                        else
                        {
                            new LogTrace(ELogTrace.ERROR_RESTART_DISCHARGE, dischargerComm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (isRestart != null)
                {
                    if (isRestart == false)
                    {
                        new LogTrace(ELogTrace.ERROR_START_DISCHARGE, ex);
                    }
                    else
                    {
                        new LogTrace(ELogTrace.ERROR_RESTART_DISCHARGE, ex);
                    }
                }
            }
        }

        public void StopDischarger(string dischargerName)
        {
            try
            {
                // 방전 동작 정지
                var isOk = _clients[dischargerName].SendCommand_StopDischarge();

                // 방전 동작 정지 Trace Log 저장
                DischargerData dischargerComm = _clients[dischargerName].GetLogSystemDischargerData();

                if (isOk == EDischargerClientError.Ok)
                {
                    new LogTrace(ELogTrace.TRACE_STOP_DISCHARGE, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_STOP_DISCHARGE, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_STOP_DISCHARGE, ex);
            }
        }

        public void PauseDischarger(string dischargerName)
        {
            try
            {
                // 방전 동작 일시 정지
                var isOk = _clients[dischargerName].SendCommand_PauseDischarge();

                // 방전 동작 일시 정지 Trace Log 저장
                LogTrace.DischargerData dischargerComm = _clients[dischargerName].GetLogSystemDischargerData();

                if (isOk == EDischargerClientError.Ok)
                {
                    new LogTrace(ELogTrace.TRACE_PAUSE_DISCHARGE, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_PAUSE_DISCHARGE, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_PAUSE_DISCHARGE, ex);
            }
        }

        public void SetSafetyCondition(string dischargerName, 
            double voltageMax, double voltageMin, double currentMax, double currentMin, double tempMax, double tempMin)
        {
            try
            {
                // 방전 안전 조건 설정
                bool isOk = _clients[dischargerName].SendCommand_SetSafetyCondition(voltageMax, voltageMin, currentMax, currentMin, tempMax, tempMin);

                // 방전기 안전 조건 설정 Trace Log 저장
                LogTrace.DischargerData dischargerComm = _clients[dischargerName].GetLogSystemDischargerData();

                if (isOk)
                {
                    new LogTrace(ELogTrace.TRACE_SET_SAFETYCONDITION, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_SET_SAFETYCONDITION, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_SET_SAFETYCONDITION, ex);
            }
        }

        public void SetDischargerState(string dischargerName, EDischargerState eDischargerState)
        {
            try
            {
                // 방전기 상태 설정
                bool isOk = _clients[dischargerName].ChangeDischargerState(eDischargerState);

                // 방전기 상태 설정 Trace Log 저장
                LogTrace.DischargerData dischargerComm = _clients[dischargerName].GetLogSystemDischargerData();
                dischargerComm.EDischargerState = eDischargerState;

                if (isOk)
                {
                    new LogTrace(ELogTrace.TRACE_SET_STATE, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_SET_STATE, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_SET_STATE, ex);
            }
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
                model.ResumeButtonVisibility = Visibility.Visible;
                ViewModelMonitor_State.Instance.Model.PauseNResumeIsEnable = true;
                ViewModelMonitor_State.Instance.Model.StopIsEnable = true;
            }
            else if (model.DischargerState == EDischargerState.Discharging)
            {
                model.ResumeButtonVisibility = Visibility.Collapsed;
                ViewModelMonitor_State.Instance.Model.PauseNResumeIsEnable = true;
                ViewModelMonitor_State.Instance.Model.StopIsEnable = true;
            }
            else if (model.DischargerState == EDischargerState.Ready)
            {
                ViewModelMonitor_State.Instance.Model.PauseNResumeIsEnable = false;
                ViewModelMonitor_State.Instance.Model.StopIsEnable = false;
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
            try
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
                        // 방전기 연결
                        bool isOk = _clients[info.Name].Start(parameters);

                        // 방전기 연결 Trace Log 저장
                        LogTrace.DischargerData dischargerComm = new LogTrace.DischargerData()
                        {
                            Name = info.Name,
                            EDischargerModel = info.Model,
                            Channel = info.Channel,
                            IpAddress = info.IpAddress,
                        };

                        if (isOk)
                        {
                            new LogTrace(ELogTrace.TRACE_CONNECT_DISCHARGER, dischargerComm);
                        }
                        else
                        {
                            new LogTrace(ELogTrace.ERROR_CONNECT_DISCHARGER, dischargerComm);
                        }
                    });

                thread.Start();
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_CONNECT_DISCHARGER, ex);
            }
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

                if (Model[i].DischargerState != EDischargerState.Discharging)
                {
                    Model[i].ShortAvailableVisibility = Visibility.Hidden;
                }
                else
                {
                    // 만약 전압이 1V 미만이고, 전류가 10A 미만이면 short available 아이콘 표시
                    if (Model[i].DischargerData.ReceiveBatteryVoltage <= 1.0 && Model[i].DischargerData.ReceiveDischargeCurrent >= -10.0)
                    {
                        Model[i].ShortAvailableVisibility = Visibility.Visible;
                    }
                    else
                    {
                        Model[i].ShortAvailableVisibility = Visibility.Hidden;
                    }
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

                if (state == EDischargerState.Discharging || state == EDischargerState.Pause)
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
