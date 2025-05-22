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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;

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

                    ViewModelMain.Instance.Model.DischargerIndex = selectedIndex;
                    ViewModelMain.Instance.Model.SelectedDischargerName = selectedDischargerName;
                    ViewModelSetMode.Instance.SetDischargerName(selectedDischargerName);
                }
            }
            catch { }
        }

        public void OpenPopupError(string dischargerName)
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
                DischargerData dischargerComm = _clients[dischargerName].GetDischargerComm();

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
        private void ReconnectDischarger(string dischargerName)
        {
            try
            {
                int index = Model.DischargerNameList.ToList().FindIndex(x => x == dischargerName);

                Model.ReconnectVisibility[index] = Visibility.Collapsed;

                Thread thread = new Thread(
                    delegate ()
                    {
                        // 방전기 재 연결
                        bool isOk = _clients[dischargerName].Restart();

                        // 방전기 재 연결 Trace Log 저장
                        DischargerData dischargerComm = _clients[dischargerName].GetDischargerComm();

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
                // 방전 동작 시작
                var isOk = _clients[param.DischargerName].SendCommand_StartDischarge(
                    EWorkMode.CcCvMode, param.Voltage, param.Current);

                // 방전 동작 시작 Trace Log 저장
                DischargerData dischargerComm = _clients[param.DischargerName].GetDischargerComm();
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
                DischargerData dischargerComm = _clients[dischargerName].GetDischargerComm();

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
                DischargerData dischargerComm = _clients[dischargerName].GetDischargerComm();

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
                DischargerData dischargerComm = _clients[dischargerName].GetDischargerComm();

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
                DischargerData dischargerComm = _clients[dischargerName].GetDischargerComm();
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
            foreach (var state in Model.DischargerStates)
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

            ViewModelTempModule.Instance.InitializeTempModuleDictionary(infos);

            OneSecondTimer?.Stop();
            OneSecondTimer = new System.Timers.Timer();
            OneSecondTimer.Elapsed += CopyDataFromDischargerClientToModel;
            OneSecondTimer.Interval = 500;
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
            Model.DischargerInfos.Add(dischargerInfo);

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
                        DischargerData dischargerComm = new DischargerData()
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
            // CollectionChanged Event 발생 목적 변경
            ObservableCollection<DischargerDatas> dischargerDatas = new ObservableCollection<DischargerDatas>();
            ObservableCollection<EDischargerState> dischargerStates = new ObservableCollection<EDischargerState>();

            for (int i = 0; i < Model.DischargerNameList.Count; i++)
            {
                dischargerDatas.Add(_clients[Model.DischargerNameList[i]].GetDatas());
                dischargerStates.Add(_clients[Model.DischargerNameList[i]].GetState());
            }

            Model.DischargerDatas = dischargerDatas;
            Model.DischargerStates = dischargerStates;

            SetStateColor(Model.DischargerStates);
            SetProgressTime(Model.DischargerStates, Model.DischargerDatas);
            SetVisibility(Model.DischargerStates);
        }

        private void SetStateColor(ObservableCollection<EDischargerState> dischargerStates)
        {
            for (int index = 0; index < dischargerStates.Count; index++) 
            {
                var state = dischargerStates[index];

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

        private void SetProgressTime(ObservableCollection<EDischargerState> dischargerStates, ObservableCollection<DischargerDatas> dischargerDatas)
        {
            for (int index = 0; index < dischargerStates.Count; index++)
            {
                var state = dischargerStates[index];
                var dischargingStartTime = dischargerDatas[index].DischargingStartTime;

                if (state == EDischargerState.Discharging && state == EDischargerState.Pause)
                {
                    TimeSpan diff = (DateTime.Now - dischargingStartTime);
                    string diffString =
                        diff.Hours.ToString("D2") + ":" +
                        diff.Minutes.ToString("D2") + ":" +
                        diff.Seconds.ToString("D2");
                    Model.ProgressTime[index] = diffString;
                }
            }
        }

        private void SetVisibility(ObservableCollection<EDischargerState> dischargerStates)
        {
            for (int index = 0; index < dischargerStates.Count; index++)
            {
                var state = dischargerStates[index];

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
        }
    }
}
