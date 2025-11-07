using static DischargerV2.LOG.LogTrace;
using static DischargerV2.Ini.IniDischarge;
using MExpress.Mex;
using DischargerV2.LOG;
using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using Sqlite.Common;
using SqlClient.Server;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Diagnostics;
using DischargerV2.Languages.Strings;

namespace DischargerV2.MVVM.ViewModels
{
    public class StartDischargerCommandParam
    {
        public string DischargerName { get; set; } = string.Empty;
        public double Current { get; set; } = 0.0;
        public double Voltage { get; set; } = 0.0;

        public EDischargeTarget EDischargeTarget { get; set; }

        public string LogFileName { get; set; } = string.Empty;

        public bool? IsRestart { get; set; } = null;
    }

    public class ViewModelDischarger : BindableBase
    {
        private System.Timers.Timer OneSecondTimer { get; set; } = null;

        /// <summary>
        /// Key: discharger name
        /// </summary>
        private Dictionary<string, EthernetClientDischarger> _clients = new Dictionary<string, EthernetClientDischarger>();
        private List<TableDischargerInfo> _dischargerInfos = null;

        public ObservableCollection<ModelDischarger> Model { get; set; } = new ObservableCollection<ModelDischarger>();
        public string MachineCode = string.Empty;

        private ModelDischarger _selectedModel = new ModelDischarger();
        public ModelDischarger SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                try { SetProperty(ref _selectedModel, value); } catch (Exception ex) { Debug.WriteLine(ex); }
            }
        }

        private string _selectedDischargerName = string.Empty;
        public string SelectedDischargerName
        {
            get { return _selectedDischargerName; }
            set
            {
                try { SetProperty(ref _selectedDischargerName, value); } catch (Exception ex) { Debug.WriteLine(ex); }
            }
        }

        private int _allChannelCount = 0;
        public int AllChannelCount
        {
            get { return _allChannelCount; }
            set
            {
                try { SetProperty(ref _allChannelCount, value); } catch (Exception ex) { Debug.WriteLine(ex); }
            }
        }

        private int _connectedChannelCount = 0;
        public int ConnectedChannelCount
        {
            get { return _connectedChannelCount; }
            set
            {
                try { SetProperty(ref _connectedChannelCount, value); } catch (Exception ex) { Debug.WriteLine(ex); }
            }
        }

        private int _faultChannelCount = 0;
        public int FaultChannelCount
        {
            get { return _faultChannelCount; }
            set
            {
                try { SetProperty(ref _faultChannelCount, value); } catch (Exception ex) { Debug.WriteLine(ex); }
            }
        }

        private static ViewModelDischarger _instance = new ViewModelDischarger();
        public static ViewModelDischarger Instance
        {
            get
            {
                return _instance;
            }
        }

        public ViewModelDischarger()
        {
            try { InitializeModel(); } catch (Exception ex) { Debug.WriteLine(ex); }
        }

        public void Initialize()
        {
            try
            {
                InitializeModel();
                InitializeDischarger();
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        public void InitializeModel()
        {
            try
            {
                int No = 1;

                FinalizeModel();

                _dischargerInfos = SqliteDischargerInfo.GetData();
                List<TableDischargerInfo> infos = _dischargerInfos;

                Model.Clear();

                for (int index = 0; index < infos.Count; index++)
                {
                    var dischargerInfo = InitializeDischargerInfos(infos[index].DischargerName);

                    // 자체 제작 방전기의 경우, Voltage 안전 조건 마진 적용 필요
                    dischargerInfo.SafetyVoltageMin -= EthernetClientDischarger.SafetyMarginVoltage;
                    dischargerInfo.SafetyVoltageMax += EthernetClientDischarger.SafetyMarginVoltage;

                    int channel = dischargerInfo.Channel;

                    for (int i = 0; i < channel; i++)
                    {
                        try
                        {
                            var model = new ModelDischarger();

                            model.DischargerInfo = dischargerInfo;
                            model.DischargerIndex = index;
                            model.No = No.ToString();
                            No++;

                            if (channel > 1)
                            {
                                model.DischargerInfo.Channel = (short)(i + 1);
                                model.DischargerName = $"{infos[index].DischargerName}_{i + 1}";
                            }
                            else
                            {
                                model.DischargerName = $"{infos[index].DischargerName}";
                            }

                            model.DischargerChannel = i + 1;
                            model.PropertyChanged += Model_PropertyChanged;

                            Model.Add(model);
                        }
                        catch (Exception ex) { Debug.WriteLine(ex); }
                    }
                }

                ViewModelTempModule.Instance.InitializeTempModuleDictionary(infos);

                Thread thread = new Thread(() =>
                {
                    try
                    {
                        Thread.Sleep(1000);
                        SelectDischarger(0);
                    }
                    catch (Exception ex) { Debug.WriteLine(ex); }
                });
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void InitializeDischarger()
        {
            try
            {
                FinalizeDischarger();

                _dischargerInfos = SqliteDischargerInfo.GetData();
                List<TableDischargerInfo> infos = _dischargerInfos;

                for (int index = 0; index < infos.Count; index++)
                {
                    try
                    {
                        var dischargerInfo = InitializeDischargerInfos(infos[index].DischargerName);
                        InitializeDischargerClients(dischargerInfo);
                    }
                    catch (Exception exItem) { Debug.WriteLine(exItem); }
                }

                OneSecondTimer?.Stop();
                OneSecondTimer = new System.Timers.Timer();
                OneSecondTimer.Elapsed += CopyDataFromDischargerClientToModel;
                OneSecondTimer.Interval = 1000;
                OneSecondTimer.Start();
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        public void FinalizeModel()
        {
            try
            {
                foreach (var model in Model)
                {
                    try { model.PropertyChanged -= Model_PropertyChanged; } catch (Exception exSub) { Debug.WriteLine(exSub); }
                }
                Model.Clear();
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        public void FinalizeDischarger()
        {
            try
            {
                OneSecondTimer?.Stop();
                OneSecondTimer = null;

                foreach (var client in _clients)
                {
                    try { client.Value.Stop(); } catch (Exception exStop) { Debug.WriteLine(exStop); }
                }
                _clients.Clear();
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (sender is ModelDischarger modelDischarger)
                {
                    if (e.PropertyName == nameof(ModelDischarger.DischargerState))
                    {
                        UpdateChannelState(modelDischarger);
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        public bool StartDischarger(StartDischargerCommandParam param)
        {
            bool? isRestart = param.IsRestart;
            bool returnValue = false;

            try
            {
                string[] discharger = param.DischargerName.Split('_');
                string dischargerName = discharger[0];
                short channel = (discharger.Length > 1) ?
                    Convert.ToInt16(discharger[1]) : (short)1;

                var isOk = _clients[dischargerName].SendCommand_StartDischarge(
                    channel, EWorkMode.CcCvMode, param.Voltage, param.Current);

                var dischargerComm = _clients[dischargerName].GetLogSystemDischargerData(channel);
                dischargerComm.EWorkMode = EWorkMode.CcCvMode;
                dischargerComm.SetValue_Voltage = param.Voltage;
                dischargerComm.LimitingValue_Current = param.Current;

                if (isOk == EDischargerClientError.Ok)
                {
                    if (isRestart != null)
                    {
                        if (isRestart == false)
                        {
                            new LogTrace(ELogTrace.SYSTEM_OK_START_DISCHARGE, dischargerComm);
                        }
                        else
                        {
                            new LogTrace(ELogTrace.SYSTEM_OK_RESTART_DISCHARGE, dischargerComm);
                        }
                    }

                    returnValue = true;
                }
                else
                {
                    if (isRestart != null)
                    {
                        if (isRestart == false)
                        {
                            new LogTrace(ELogTrace.SYSTEM_ERROR_START_DISCHARGE, dischargerComm);
                        }
                        else
                        {
                            new LogTrace(ELogTrace.SYSTEM_ERROR_RESTART_DISCHARGE, dischargerComm);
                        }
                    }
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                try
                {
                    if (isRestart != null)
                    {
                        if (isRestart == false)
                        {
                            new LogTrace(ELogTrace.SYSTEM_ERROR_START_DISCHARGE, ex);
                        }
                        else
                        {
                            new LogTrace(ELogTrace.SYSTEM_ERROR_RESTART_DISCHARGE, ex);
                        }
                    }
                }
                catch (Exception ex2) { Debug.WriteLine(ex2); }

                Debug.WriteLine(ex);
                return returnValue;
            }
        }

        public void StopDischarger(string getDischargerName)
        {
            try
            {
                string[] discharger = getDischargerName.Split('_');
                string dischargerName = discharger[0];
                short channel = (discharger.Length > 1) ?
                    Convert.ToInt16(discharger[1]) : (short)1;

                var isOk = _clients[dischargerName].SendCommand_StopDischarge(channel);

                LogTrace.DischargerData dischargerComm = _clients[dischargerName].GetLogSystemDischargerData(channel);

                if (isOk == EDischargerClientError.Ok)
                {
                    new LogTrace(ELogTrace.SYSTEM_OK_STOP_DISCHARGE, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.SYSTEM_ERROR_STOP_DISCHARGE, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                try
                {
                    new LogTrace(ELogTrace.SYSTEM_ERROR_STOP_DISCHARGE, ex);
                }
                catch (Exception ex2) { Debug.WriteLine(ex2); }
            }
        }

        public void PauseDischarger(string getDischargerName)
        {
            try
            {
                string[] discharger = getDischargerName.Split('_');
                string dischargerName = discharger[0];
                short channel = (discharger.Length > 1) ?
                    Convert.ToInt16(discharger[1]) : (short)1;

                var isOk = _clients[dischargerName].SendCommand_PauseDischarge(channel);

                var dischargerComm = _clients[dischargerName].GetLogSystemDischargerData(channel);

                if (isOk == EDischargerClientError.Ok)
                {
                    new LogTrace(ELogTrace.SYSTEM_OK_PAUSE_DISCHARGE, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.SYSTEM_ERROR_PAUSE_DISCHARGE, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                try { new LogTrace(ELogTrace.SYSTEM_ERROR_PAUSE_DISCHARGE, ex); } catch (Exception ex2) { Debug.WriteLine(ex2); }
            }
        }

        public void SetIsPaused(string getDischargerName, bool isPaused)
        {
            try
            {
                string[] discharger = getDischargerName.Split('_');
                string dischargerName = discharger[0];
                short channel = (discharger.Length > 1) ?
                    Convert.ToInt16(discharger[1]) : (short)1;

                _clients[dischargerName].SetIsPaused(channel, isPaused);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void SetSafetyCondition(string getDischargerName,
            double voltageMax, double voltageMin, double currentMax, double currentMin, double tempMax, double tempMin)
        {
            try
            {
                string[] discharger = getDischargerName.Split('_');
                string dischargerName = discharger[0];
                short channel = (discharger.Length > 1) ?
                    Convert.ToInt16(discharger[1]) : (short)1;

                bool isOk = _clients[dischargerName].SendCommand_SetSafetyCondition(channel, voltageMax, voltageMin, currentMax, currentMin, tempMax, tempMin);

                var dischargerComm = _clients[dischargerName].GetLogSystemDischargerData(channel);

                if (isOk)
                {
                    new LogTrace(ELogTrace.SYSTEM_OK_SET_SAFETYCONDITION, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.SYSTEM_ERROR_SET_SAFETYCONDITION, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                try { new LogTrace(ELogTrace.SYSTEM_ERROR_SET_SAFETYCONDITION, ex); } catch (Exception ex2) { Debug.WriteLine(ex2); }
            }
        }

        public void SetDischargerState(string getDischargerName, EDischargerState eDischargerState)
        {
            try
            {
                string[] discharger = getDischargerName.Split('_');
                string dischargerName = discharger[0];
                short channel = (discharger.Length > 1) ?
                    Convert.ToInt16(discharger[1]) : (short)1;

                bool isOk = _clients[dischargerName].ChangeDischargerState(eDischargerState, channel);

                var dischargerComm = _clients[dischargerName].GetLogSystemDischargerData(channel);
                dischargerComm.EDischargerState = eDischargerState;

                if (isOk)
                {
                    new LogTrace(ELogTrace.SYSTEM_OK_SET_STATE, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.SYSTEM_ERROR_SET_STATE, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                try { new LogTrace(ELogTrace.SYSTEM_ERROR_SET_STATE, ex); } catch (Exception ex2) { Debug.WriteLine(ex2); }
            }
        }

        public bool IsDischarging()
        {
            try
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
            catch (Exception ex) { Debug.WriteLine(ex); return false; }
        }

        private DischargerInfo InitializeDischargerInfos(string name)
        {
            try
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
                dischargerInfo.IsTempModule = info.IsTempModule;
                dischargerInfo.IpAddress = IPAddress.Parse(info.IpAddress);
                dischargerInfo.EthernetPort = 10004;
                dischargerInfo.TimeOutMs = 2000;
                dischargerInfo.SpecVoltage = info.SpecVoltage;
                dischargerInfo.SpecCurrent = info.SpecCurrent;
                dischargerInfo.SafetyVoltageMax = model.SafetyVoltMax;
                dischargerInfo.SafetyVoltageMin = model.SafetyVoltMin;
                dischargerInfo.SafetyCurrentMax = model.SafetyCurrentMax;
                dischargerInfo.SafetyCurrentMin = model.SafetyCurrentMin;
                dischargerInfo.SafetyTempMax = model.SafetyTempMax;
                dischargerInfo.SafetyTempMin = model.SafetyTempMin;

                if (!ViewModelLogin.Instance.IsLocalDb())
                {
                    try
                    {
                        string machineCode = GetIniData<string>(EIniData.MachineCode);
                        MachineCode = machineCode;

                        for (int i = 0; i < info.DischargerChannel; i++)
                        {
                            var insertData = new TABLE_SYS_STS_SDC();
                            insertData.MC_CD = machineCode;
                            insertData.MC_CH = i + 1;
                            insertData.DischargerName = $"{name}_{i + 1}";
                            SqlClientStatus.InsertData_Init(insertData);
                        }
                    }
                    catch (Exception exDb) { Debug.WriteLine(exDb); }
                }

                return dischargerInfo;
            }
            catch (Exception ex) { Debug.WriteLine(ex); return new DischargerInfo(); }
        }

        private void InitializeDischargerClients(DischargerInfo info)
        {
            try
            {
                short channel = info.Channel;

                short[] channelArray = new short[channel];
                double[] safetyVoltageMaxArray = new double[channel];
                double[] safetyVoltageMinArray = new double[channel];
                double[] safetyCurrentMaxArray = new double[channel];
                double[] safetyCurrentMinArray = new double[channel];
                double[] safetyTempMaxArray = new double[channel];
                double[] safetyTempMinArray = new double[channel];

                for (int i = 0; i < info.Channel; i++)
                {
                    channelArray[i] = (short)(i + 1);
                    safetyVoltageMaxArray[i] = info.SafetyVoltageMax;
                    safetyVoltageMinArray[i] = info.SafetyVoltageMin;
                    safetyCurrentMaxArray[i] = info.SafetyCurrentMax;
                    safetyCurrentMinArray[i] = info.SafetyCurrentMin;
                    safetyTempMaxArray[i] = info.SafetyTempMax;
                    safetyTempMinArray[i] = info.SafetyTempMin;
                }

                EthernetClientDischargerStart parameters = new EthernetClientDischargerStart();
                parameters.DischargerModel = info.Model;
                parameters.DischargerName = info.Name;
                parameters.DischargerChannel = channelArray;
                parameters.DischargerIsTempModule = info.IsTempModule;
                parameters.IpAddress = info.IpAddress;
                parameters.EthernetPort = info.EthernetPort;
                parameters.TimeOutMs = info.TimeOutMs;
                parameters.SafetyVoltageMax = safetyVoltageMaxArray;
                parameters.SafetyVoltageMin = safetyVoltageMinArray;
                parameters.SafetyCurrentMax = safetyCurrentMaxArray;
                parameters.SafetyCurrentMin = safetyCurrentMinArray;
                parameters.SafetyTempMax = safetyTempMaxArray;
                parameters.SafetyTempMin = safetyTempMinArray;
                _clients[info.Name] = new EthernetClientDischarger();

                Thread thread = new Thread(
                    delegate ()
                    {
                        try
                        {
                            bool isOk = _clients[info.Name].Start(parameters);
                            LogTrace.DischargerData dischargerComm = new LogTrace.DischargerData()
                            {
                                Name = info.Name,
                                EDischargerModel = info.Model,
                                Channel = info.Channel,
                                IpAddress = info.IpAddress,
                            };

                            if (isOk)
                            {
                                new LogTrace(ELogTrace.SYSTEM_OK_CONNECT_DISCHARGER, dischargerComm);
                            }
                            else
                            {
                                new LogTrace(ELogTrace.SYSTEM_ERROR_CONNECT_DISCHARGER, dischargerComm);
                            }
                        }
                        catch (Exception exThread) { Debug.WriteLine(exThread); }
                    });
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                try { new LogTrace(ELogTrace.SYSTEM_ERROR_CONNECT_DISCHARGER, ex); } catch (Exception ex2) { Debug.WriteLine(ex2); }
            }
        }

        private void CopyDataFromDischargerClientToModel(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                for (int i = 0; i < Model.Count; i++)
                {
                    string dischargerName;
                    int dischargerChannel;

                    var dischargerNameSplit = Model[i].DischargerName.Split('_');
                    if (dischargerNameSplit.Length > 1)
                    {
                        dischargerName = dischargerNameSplit[0];
                        dischargerChannel = Convert.ToInt32(dischargerNameSplit[1]);
                    }
                    else
                    {
                        dischargerName = dischargerNameSplit[0];
                        dischargerChannel = 1;
                    }

                    EDischargerState state = EDischargerState.None;
                    TimeSpan diff = new TimeSpan(0);

                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                Model[i].DischargerData = _clients[dischargerName].GetDatas(dischargerChannel);
                                Model[i].DischargerState = _clients[dischargerName].GetState(dischargerChannel);

                                var dischargerInfo = _dischargerInfos.Find(x => x.DischargerName == dischargerName);
                                if (dischargerInfo != null && dischargerInfo.IsTempModule)
                                {
                                    int index = ViewModelTempModule.Instance.Model.TempModuleComportList.FindIndex(x => x == dischargerInfo.TempModuleComPort);
                                    if (index >= 0)
                                    {
                                        var tempDatas = ViewModelTempModule.Instance.Model.TempDatas;
                                        var temp = tempDatas[index][int.Parse(dischargerInfo.TempChannel)];

                                        Model[i].DischargerData.ReceiveDischargeTemp = temp;
                                        _clients[dischargerName].SetReceiveTemp(temp, dischargerChannel);
                                    }
                                }
                                else
                                {
                                    var table = _dischargerInfos.Find(x => x.DischargerName == dischargerName);
                                    if (table != null && !table.IsTempModule)
                                    {
                                        int deviceIndex = 0;
                                        int baseTempChannel = 0;
                                        int channelCount = table.DischargerChannel;
                                        try { deviceIndex = int.Parse(table.TempModuleChannel ?? "0"); } catch { deviceIndex = 0; }
                                        try { baseTempChannel = int.Parse(table.TempChannel ?? "0"); } catch { baseTempChannel = 0; }

                                        int zeroBasedChannelOrder = Model[i].DischargerChannel - 1;
                                        int effectiveTempChannel = baseTempChannel + zeroBasedChannelOrder;

                                        double tempVal = ViewModelMain.Instance.GetCrevisTemperature(deviceIndex, effectiveTempChannel);
                                        if (!double.IsNaN(tempVal))
                                        {
                                            Model[i].DischargerData.ReceiveDischargeTemp = tempVal;
                                            _clients[dischargerName].SetReceiveTemp(tempVal, dischargerChannel);
                                        }
                                    }
                                }

                                state = Model[i].DischargerState;
                                SetStateColor(i);
                                SetProgressTime(i, out diff);
                                SetVisibility(i);
                            }
                            catch (Exception exInner) { Debug.WriteLine(exInner); }
                        });
                    }
                    catch (Exception exDisp) { Debug.WriteLine(exDisp); }

                    try
                    {
                        if (!ViewModelLogin.Instance.IsLocalDb())
                        {
                            var updateData = new TABLE_SYS_STS_SDC();
                            updateData.MC_CD = MachineCode;
                            updateData.MC_CH = Model[i].DischargerChannel;
                            updateData.USER_ID = ViewModelLogin.Instance.Model.UserId;

                            updateData.DischargerVoltage = Model[i].DischargerData.ReceiveBatteryVoltage.ToString("F3");
                            updateData.DischargerCurrent = Model[i].DischargerData.ReceiveDischargeCurrent.ToString("F3");
                            updateData.DischargerTemp = Model[i].DischargerData.ReceiveDischargeTemp.ToString("F3");

                            updateData.DischargerState = state.ToString();
                            updateData.ProgressTime = $"{diff.Hours.ToString("D2")}:{diff.Minutes.ToString("D2")}:{diff.Seconds.ToString("D2")}";

                            SqlClientStatus.UpdateData_Monitoring(updateData);
                            SqlClientStatus.UpdateData_StateNTime(updateData);

                            if (state == EDischargerState.SafetyOutOfRange ||
                                state == EDischargerState.ReturnCodeError ||
                                state == EDischargerState.ChStatusError ||
                                state == EDischargerState.DeviceError)
                            {
                                try
                                {
                                    TableDischargerErrorCode tableDischargerErrorCode = SqliteDischargerErrorCode.GetData().Find(x => x.Code == Model[i].DischargerData.ErrorCode);

                                    var alarmData = new TABLE_QLT_HISTORY_ALARM();
                                    alarmData.MC_CD = MachineCode;
                                    alarmData.CH_NO = Model[i].DischargerChannel;
                                    alarmData.Alarm_Time = DateTime.Now;
                                    alarmData.Alarm_Code = (int)Model[i].DischargerData.ErrorCode;
                                    alarmData.Alarm_Desc = (tableDischargerErrorCode != null) ? tableDischargerErrorCode.Description : string.Empty;

                                    SqlClientStatus.UpdateData_Alarm(alarmData);
                                }
                                catch (Exception exAlarm) { Debug.WriteLine(exAlarm); }
                            }
                        }
                    }
                    catch (Exception exServer) { Debug.WriteLine(exServer); }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        public void SelectDischarger(int selectedIndex, bool IsSetDischargerName = true)
        {
            try
            {
                string selectedDischargerName = Model[selectedIndex].DischargerName;

                SelectedDischargerName = selectedDischargerName;
                SelectedModel = Model[selectedIndex];

                ViewModelMain.Instance.Model.DischargerIndex = selectedIndex;
                ViewModelMain.Instance.Model.SelectedDischargerName = selectedDischargerName;

                if (IsSetDischargerName)
                {
                    ViewModelSetMode.Instance.SetDischargerName(selectedDischargerName);
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        public void OpenPopupError(string dischargerName)
        {
            try
            {
                int index = Model.ToList().FindIndex(x => x.DischargerName == dischargerName);
                uint errorCode = Model[index].DischargerData.ErrorCode;

                List<TableDischargerErrorCode> tableDischargerErrorCodeList = SqliteDischargerErrorCode.GetData();
                TableDischargerErrorCode tableDischargerErrorCode = tableDischargerErrorCodeList.Find(x => x.Code == errorCode);

                string title = string.Empty;
                string comment = string.Empty;

                string _channel = new DynamicString().GetDynamicString("Channel");
                string _unknownError = new DynamicString().GetDynamicString("PopupError_Comment_UnknownError");
                string _description = new DynamicString().GetDynamicString("PopupError_Comment_Description");
                string _errorCode = new DynamicString().GetDynamicString("PopupError_Comment_ErrorCode");
                string _cause = new DynamicString().GetDynamicString("PopupError_Comment_Cause");
                string _solution = new DynamicString().GetDynamicString("PopupError_Comment_Solution");
                string _unknown = new DynamicString().GetDynamicString("PopupError_Comment_Unknown");

                if (tableDischargerErrorCode == null)
                {
                    title = new DynamicString().GetDynamicString("PopupError_Title_Unknown");
                    comment =
                        $"{dischargerName} ({_channel}: {Model[index].DischargerInfo.Channel})\n\n" +
                        $"{_unknownError}\n" +
                        $"({_errorCode}: 0x{errorCode.ToString("X")})\n\n" +
                        $"{_cause}: \n" +
                        $"{_unknown}\n\n" +
                        $"{_solution}: \n" +
                        $"{_unknown}";
                }
                else if (tableDischargerErrorCode.Code == 0xA00000FF)
                {
                    title = tableDischargerErrorCode.Title;
                    comment =
                        $"{dischargerName} ({_channel}: {Model[index].DischargerInfo.Channel})\n\n" +
                        $"{tableDischargerErrorCode.Description} {_description}\n" +
                        $"({_errorCode}: 0x{tableDischargerErrorCode.Code.ToString("X")} (0x{Model[index].DischargerData.DiModuleInfo.ToString("X2")}))\n\n" +
                        $"{_cause}: \n" +
                        $"{tableDischargerErrorCode.Cause}\n\n" +
                        $"{_solution}: \n" +
                        $"{tableDischargerErrorCode.Action}";
                }
                else
                {
                    title = tableDischargerErrorCode.Title;
                    comment =
                        $"{dischargerName} ({_channel}: {Model[index].DischargerInfo.Channel})\n\n" +
                        $"{tableDischargerErrorCode.Description} {_description}\n" +
                        $"({_errorCode}: 0x{tableDischargerErrorCode.Code.ToString("X")})\n\n" +
                        $"{_cause}: \n" +
                        $"{tableDischargerErrorCode.Cause}\n\n" +
                        $"{_solution}: \n" +
                        $"{tableDischargerErrorCode.Action}";
                }

                ViewModelPopup_Error viewModelPopup_Error = new ViewModelPopup_Error()
                {
                    Title = title,
                    Comment = comment,
                    Parameter = dischargerName,
                    CallBackDelegate = ResetError,
                };

                try
                {
                    ViewModelMain viewModelMain = ViewModelMain.Instance;
                    viewModelMain.SetViewModelPopup_Error(viewModelPopup_Error);
                    viewModelMain.OpenPopup(ModelMain.EPopup.Error);
                }
                catch (Exception exPopup) { Debug.WriteLine(exPopup); }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        public void ResetError(string getDischargerName)
        {
            try
            {
                string[] discharger = getDischargerName.Split('_');
                string dischargerName = discharger[0];
                short channel = (discharger.Length > 1) ?
                    Convert.ToInt16(discharger[1]) : (short)1;

                bool isOk = _clients[dischargerName].SendCommand_ClearAlarm(channel);

                DischargerData dischargerComm = _clients[dischargerName].GetLogSystemDischargerData(channel);

                if (isOk)
                {
                    new LogTrace(ELogTrace.SYSTEM_OK_CLEAR_ALARM, dischargerComm);
                }
                else
                {
                    new LogTrace(ELogTrace.SYSTEM_ERROR_CLEAR_ALARM, dischargerComm);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                try { new LogTrace(ELogTrace.SYSTEM_ERROR_CLEAR_ALARM, ex); } catch (Exception ex2) { Debug.WriteLine(ex2); }
            }
        }

        public void ReconnectDischarger(string getDischargerName)
        {
            try
            {
                int index = Model.ToList().FindIndex(x => x.DischargerName == getDischargerName);
                if (index >= 0)
                {
                    try { Model[index].ReconnectVisibility = Visibility.Collapsed; } catch (Exception exVis) { Debug.WriteLine(exVis); }
                }

                string[] discharger = getDischargerName.Split('_');
                string dischargerName = discharger[0];
                short channel = (discharger.Length > 1) ?
                    Convert.ToInt16(discharger[1]) : (short)1;

                Thread thread = new Thread(() =>
                {
                    try
                    {
                        bool isOk = _clients[dischargerName].Restart();
                        LogTrace.DischargerData dischargerComm = _clients[dischargerName].GetLogSystemDischargerData(channel);

                        if (isOk)
                        {
                            new LogTrace(ELogTrace.SYSTEM_OK_RECONNECT_DISCHARGER, dischargerComm);
                        }
                        else
                        {
                            new LogTrace(ELogTrace.SYSTEM_ERROR_RECONNECT_DISCHARGER, dischargerComm);
                        }
                    }
                    catch (Exception exThread) { Debug.WriteLine(exThread); }
                });
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                try { new LogTrace(ELogTrace.SYSTEM_ERROR_RECONNECT_DISCHARGER, ex); } catch (Exception ex2) { Debug.WriteLine(ex2); }
            }
        }

        private object _channelStateLock = new object();
        private void UpdateChannelState(ModelDischarger modelDischarger)
        {
            try
            {
                lock (_channelStateLock)
                {
                    try
                    {
                        ConnectedChannelCount = 0;
                        FaultChannelCount = 0;

                        foreach (var model in Model)
                        {
                            try { AllChannelCount = Model.Count; } catch (Exception exCount) { Debug.WriteLine(exCount); }

                            if (model.DischargerState == EDischargerState.Ready ||
                                model.DischargerState == EDischargerState.Discharging ||
                                model.DischargerState == EDischargerState.Pause)
                            {
                                if (model == modelDischarger)
                                {
                                    try { ViewModelSetMode_Preset.Instance.GetCurrentSoc(model); } catch (Exception exSoc) { Debug.WriteLine(exSoc); }
                                }

                                ConnectedChannelCount += 1;
                            }
                            else
                            {
                                FaultChannelCount += 1;
                            }
                        }
                    }
                    catch (Exception exIter) { Debug.WriteLine(exIter); }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private void SetStateColor(int index)
        {
            try
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
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private void SetProgressTime(int index, out TimeSpan diff)
        {
            diff = new TimeSpan(0);
            try
            {
                var state = Model[index].DischargerState;
                var dischargingStartTime = Model[index].DischargerData.DischargingStartTime;

                if (state == EDischargerState.Discharging || state == EDischargerState.Pause)
                {
                    diff = DateTime.Now - dischargingStartTime;

                    var hours = diff.Hours.ToString("D2");
                    var mins = diff.Minutes.ToString("D2");
                    var seconds = diff.Seconds.ToString("D2");

                    Model[index].ProgressTime = $"{hours}:{mins}:{seconds}";
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }

        private void SetVisibility(int index)
        {
            try
            {
                var state = Model[index].DischargerState;

                // ShortAvailable 표시 설정
                if (state != EDischargerState.Discharging)
                {
                    Model[index].ShortAvailableVisibility = Visibility.Hidden;
                }
                else
                {
                    // 만약 전압이 1V 이하이고, 전류가 10A 이하이면 short available 아이콘 표시
                    if (Model[index].DischargerData.ReceiveBatteryVoltage <= 1.0 &&
                        Model[index].DischargerData.ReceiveDischargeCurrent <= 10.0)
                    {
                        Model[index].ShortAvailableVisibility = Visibility.Visible;
                    }
                    else
                    {
                        Model[index].ShortAvailableVisibility = Visibility.Hidden;
                    }
                }

                // Reconnect, ErrorAlarm 표시 설정
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

                // 온도 모듈 Reconnect 표시 설정
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
            catch (Exception ex) { Debug.WriteLine(ex); }
        }
    }
}
