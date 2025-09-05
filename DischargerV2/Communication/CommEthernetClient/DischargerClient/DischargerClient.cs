using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Threading;
using DischargerV2.LOG;
using Ethernet.Client.Common;
using Sqlite.Common;
using static DischargerV2.LOG.LogTrace;

namespace Ethernet.Client.Discharger
{
    public enum EDischargerState : byte
    {
        None = 0x0,

        [Description("연결 끊김")]
        Disconnected = 0x10,
        [Description("연결 중")]
        Connecting = 0x11,
        [Description("대기 중")]
        Ready = 0x12,
        [Description("방전 중")]
        Discharging = 0x13,
        [Description("일시 정지")]
        Pause = 0x14,

        [Description("안전 조건 범위 초과 오류 발생")]
        SafetyOutOfRange = 0x20,
        [Description("반환 코드 오류 발생")]
        ReturnCodeError = 0x21,
        [Description("채널 상태 오류 발생")]
        ChStatusError = 0x22,
        [Description("장비 오류 발생")]
        DeviceError = 0x23,
    }

    public enum EDischargerClientError
    {
        Ok,
        InvalidDischargerState,
        FailProcessPacket,
        Exception
    }

    public class EthernetClientDischargerStart
    {
        public EDischargerModel DischargerModel;
        public string DischargerName = string.Empty;
        public short[] DischargerChannel;
        public bool DischargerIsTempModule;
        public IPAddress IpAddress = IPAddress.None;
        public int EthernetPort = int.MaxValue;
        public int TimeOutMs = int.MaxValue;
        public double[] SafetyVoltageMax;
        public double[] SafetyVoltageMin;
        public double[] SafetyCurrentMax;
        public double[] SafetyCurrentMin;
        public double[] SafetyTempMax;
        public double[] SafetyTempMin;
    }

    public class DischargerDatas
    {
        public EReturnCode ReturnCode { get; set; } = EReturnCode.Success;

        /// <summary>
        /// 수신 받은 데이터
        /// </summary>
        public uint ErrorCode { get; set; } = 0;
        public byte DiModuleInfo { get; set; } = 0x00;
        public EChannelStatus ChannelStatus { get; set; } = EChannelStatus.Standby0;
        public double ReceiveBatteryVoltage { get; set; } = 0;
        public double ReceiveDischargeCurrent { get; set; } = 0;
        public double ReceiveDischargeTemp { get; set; } = 0;

        /// <summary>
        /// Safety 데이터
        /// </summary>
        public double SafetyVoltageMax { get; set; } = 0.0;
        public double SafetyVoltageMin { get; set; } = 0.0;
        public double SafetyCurrentMax { get; set; } = 0.0;
        public double SafetyCurrentMin { get; set; } = 0.0;
        public double SafetyTempMax { get; set; } = 0.0;
        public double SafetyTempMin { get; set; } = 0.0;

        /// <summary>
        /// 방전시작시간
        /// </summary>
        public DateTime DischargingStartTime { get; set; } = DateTime.MinValue;

        public DischargerDatas()
        {
            ReturnCode = EReturnCode.Success;

            ErrorCode = 0;
            DiModuleInfo = 0x00;
            ChannelStatus = EChannelStatus.Standby0;
            ReceiveBatteryVoltage = 0;
            ReceiveDischargeCurrent = 0;
            ReceiveDischargeTemp = 0;

            SafetyVoltageMax = 0.0;
            SafetyVoltageMin = 0.0;
            SafetyCurrentMax = 0.0;
            SafetyCurrentMin = 0.0;
            SafetyTempMax = 0.0;
            SafetyTempMin = 0.0;

            DischargingStartTime = DateTime.MinValue;
        }
    }

    public class DischargerInfo
    {
        public string MachineCode { get; set; } = string.Empty;

        /// <summary>
        /// 방전기 정보
        /// </summary>
        public EDischargerModel Model { get; set; }
        public string Name { get; set; } = string.Empty;
        public short Channel { get; set; } = short.MaxValue;
        public bool IsTempModule { get; set; } = false;
        public IPAddress IpAddress { get; set; }
        public int EthernetPort { get; set; } = int.MaxValue;
        public int TimeOutMs { get; set; } = int.MaxValue;

        /// <summary>
        /// 방전기 스펙
        /// </summary>
        public double SpecVoltage { get; set; } = double.MaxValue;
        public double SpecCurrent { get; set; } = double.MaxValue;

        /// <summary>
        /// 안전 조건
        /// </summary>
        public double SafetyVoltageMax { get; set; } = double.MaxValue;
        public double SafetyVoltageMin { get; set; } = double.MaxValue;
        public double SafetyCurrentMax { get; set; } = double.MaxValue;
        public double SafetyCurrentMin { get; set; } = double.MaxValue;
        public double SafetyTempMax { get; set; } = double.MaxValue;
        public double SafetyTempMin { get; set; } = double.MaxValue;
    }

    public class EthernetClientDischarger
    {
        /// <summary>
        /// key: IP Address
        /// </summary>
        private static Dictionary<string, byte> _serialNumbers = new Dictionary<string, byte>();
        /// <summary>
        /// key: IP Address
        /// </summary>
        private static Dictionary<string, object> _serialNumberDataLock = new Dictionary<string, object>();

        private static object _serialNumberLock = new object();

        private object _packetLock = new object();

        private int _handle = int.MaxValue;
        private EthernetClient _dischargerClient = null;
        private EthernetClientDischargerStart _parameters = null;

        private DischargerDatas[] _dischargerDataArray = null;

        private EDischargerState[] _dischargerState = null;
        private bool[] _isPaused = null;

        private uint _dioValue = uint.MaxValue;

        private System.Timers.Timer ReadInfoTimer = null;

        public static double SafetyMarginVoltage = 15;
        public static double SafetyMarginCurrent = 2;

        public static bool IsLampBuzzerUsed = true;

        public LogTrace.DischargerData GetLogSystemDischargerData(short channel)
        {
            int index = channel - 1;

            var dischargerData = new LogTrace.DischargerData()
            {
                Name = _parameters.DischargerName,
                EDischargerModel = _parameters.DischargerModel,
                Channel = channel,
                IpAddress = _parameters.IpAddress,

                SafetyVoltageMax = _parameters.SafetyVoltageMax[index],
                SafetyVoltageMin = _parameters.SafetyVoltageMin[index],
                SafetyCurrentMax = _parameters.SafetyCurrentMax[index],
                SafetyCurrentMin = _parameters.SafetyCurrentMin[index],
                SafetyTempMax = _parameters.SafetyTempMax[index],
                SafetyTempMin = _parameters.SafetyTempMin[index],
            };

            return dischargerData;
        }

        public bool IsConnected()
        {
            if (_dischargerClient == null)
            {
                return false;
            }

            return _dischargerClient.IsConnected();
        }

        public void SetIsPaused(int channel, bool isPaused)
        {
            int index = channel - 1;

            _isPaused[index] = isPaused;
        }

        public bool ChangeDischargerState(EDischargerState dischargerState, params short[] channel)
        {
            try
            {
                for (int i = 0; i < channel.Length; i++)
                {
                    int index = channel[i] - 1;

                    if (dischargerState == EDischargerState.SafetyOutOfRange)
                    {
                        _dischargerDataArray[index].ErrorCode = 0xF0000001;
                    }
                    else if (dischargerState == EDischargerState.ReturnCodeError)
                    {
                        _dischargerDataArray[index].ErrorCode = 0xF0000002;
                    }
                    else if (dischargerState == EDischargerState.ChStatusError)
                    {
                        _dischargerDataArray[index].ErrorCode = 0xF0000003;
                    }

                    if (_dischargerState[index] != dischargerState)
                    {
                        // 방전 Trace Log 저장 - 상태 변경
                        var dischargerData = new LogTrace.DischargerData()
                        {
                            Name = _parameters.DischargerName,
                            Channel = _parameters.DischargerChannel[index],

                            ReceiveBatteryVoltage = _dischargerDataArray[index].ReceiveBatteryVoltage.ToString("F1"),
                            ReceiveDischargeCurrent = _dischargerDataArray[index].ReceiveDischargeCurrent.ToString("F1"),
                            ReceiveDischargeTemp = _dischargerDataArray[index].ReceiveDischargeTemp.ToString("F1"),

                            ErrorCode = _dischargerDataArray[index].ErrorCode,
                            EReturnCode = _dischargerDataArray[index].ReturnCode,
                            EDischargerState = dischargerState,
                        };
                        new LogTrace(ELogDischarge.COMM_OK_SET_STATE, dischargerData);
                    }

                    _dischargerState[index] = dischargerState;
                }

                return true;
            }
            catch (Exception ex) 
            {
                // 방전 Trace Log 저장 - 상태 변경 실패
                new LogTrace(ELogDischarge.COMM_ERROR_SET_STATE, ex);

                return false;
            }
        }

        private bool IsParameterValid(EthernetClientDischargerStart parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            if (parameters.DischargerName == string.Empty ||
                parameters.IpAddress == IPAddress.None ||
                parameters.EthernetPort == int.MaxValue ||
                parameters.TimeOutMs == int.MaxValue)
            {
                return false;
            }

            int channel = parameters.DischargerChannel.Length;

            if (parameters.SafetyVoltageMax.Length != channel ||
                parameters.SafetyVoltageMin.Length != channel ||
                parameters.SafetyCurrentMax.Length != channel ||
                parameters.SafetyCurrentMin.Length != channel ||
                parameters.SafetyTempMax.Length != channel ||
                parameters.SafetyTempMin.Length != channel)
            {
                return false;
            }

            return true;
        }

        public bool Restart()
        {
            Stop();

            // 연결 안정화 시간 추가
            Thread.Sleep(3000);

            return Start(_parameters);
        }

        public bool Start(EthernetClientDischargerStart parameters)
        {
            _parameters = parameters;

            int channel = _parameters.DischargerChannel.Length;
            
            _dischargerDataArray = new DischargerDatas[channel];
            _dischargerState = new EDischargerState[channel];
            _isPaused = new bool[channel];

            for (int i = 0; i < channel; i++)
            {
                _dischargerDataArray[i] = new DischargerDatas();
                _dischargerState[i] = EDischargerState.None;
                _isPaused[i] = false;
            }

            ChangeDischargerState(EDischargerState.Connecting, _parameters.DischargerChannel);

            if (!IsParameterValid(_parameters))
            {
                ChangeDischargerState(EDischargerState.Disconnected, _parameters.DischargerChannel);
                return false;
            }

            lock (_serialNumberLock)
            {
                if (!_serialNumbers.ContainsKey(_parameters.IpAddress.ToString()))
                {
                    _serialNumbers.Add(_parameters.IpAddress.ToString(), 0);
                }

                if (!_serialNumberDataLock.ContainsKey(_parameters.IpAddress.ToString()))
                {
                    _serialNumberDataLock.Add(_parameters.IpAddress.ToString(), new object());
                }
            }

            EthernetClientStart clientStart = new EthernetClientStart();
            clientStart.DeviceName = _parameters.DischargerName;
            clientStart.IpAddress = _parameters.IpAddress;
            clientStart.EthernetPort = _parameters.EthernetPort;
            clientStart.TimeOutMs = _parameters.TimeOutMs;
            clientStart.CheckFunction = false;
            _dischargerClient = new EthernetClient();

            var result = _dischargerClient.Connect(clientStart);
            if (result != EthernetClientStatus.OK)
            {
                ChangeDischargerState(EDischargerState.Disconnected, _parameters.DischargerChannel);
                return false;
            }
            else
            {
                _handle = _dischargerClient.GetHandle();
            }

            Thread.Sleep(2000);

            ReadInfoTimer?.Stop();
            ReadInfoTimer = null;
            ReadInfoTimer = new System.Timers.Timer();
            ReadInfoTimer.Interval = 500;
            ReadInfoTimer.Elapsed += OneSecondTimer_Elapsed;
            ReadInfoTimer.Start();

            ChangeDischargerState(EDischargerState.Ready, _parameters.DischargerChannel);

            return true;
        }

        public void Stop()
        {
            _dischargerClient?.Disconnect();
            _dischargerClient = null;

            ReadInfoTimer?.Stop();
            ReadInfoTimer = null;

            ChangeDischargerState(EDischargerState.Disconnected, _parameters.DischargerChannel);
        }

        public DischargerDatas GetDatas(int channel = 1)
        {
            /// Deep Copy
            DischargerDatas temp = new DischargerDatas();

            try
            {
                int index = channel - 1;

                /// receive data
                temp.ErrorCode = _dischargerDataArray[index].ErrorCode;
                temp.DiModuleInfo = _dischargerDataArray[index].DiModuleInfo;
                temp.ChannelStatus = _dischargerDataArray[index].ChannelStatus;
                temp.ReceiveBatteryVoltage = double.Parse(_dischargerDataArray[index].ReceiveBatteryVoltage.ToString("F3"));
                temp.ReceiveDischargeCurrent = double.Parse(_dischargerDataArray[index].ReceiveDischargeCurrent.ToString("F3"));
                temp.ReceiveDischargeTemp = double.Parse(_dischargerDataArray[index].ReceiveDischargeTemp.ToString("F1"));

                /// safety
                temp.SafetyCurrentMin = _dischargerDataArray[index].SafetyCurrentMin;
                temp.SafetyCurrentMax = _dischargerDataArray[index].SafetyCurrentMax;
                temp.SafetyVoltageMin = _dischargerDataArray[index].SafetyVoltageMin;
                temp.SafetyVoltageMax = _dischargerDataArray[index].SafetyVoltageMax;
                temp.SafetyTempMin = _dischargerDataArray[index].SafetyTempMin;
                temp.SafetyTempMax = _dischargerDataArray[index].SafetyTempMax;

                /// start time
                temp.DischargingStartTime = _dischargerDataArray[index].DischargingStartTime;

                return temp;
            }
            catch
            {
                return temp;
            }
        }

        public void SetReceiveTemp(double temp, int channel = 0)
        {
            _dischargerDataArray[channel].ReceiveDischargeTemp = temp;
        }

        public EDischargerState GetState(int channel = 1)
        {
            int index = channel - 1;

            return _dischargerState[index];
        }

        private byte GetPacketSerialNumber()
        {
            lock (_serialNumberDataLock[_parameters.IpAddress.ToString()])
            {
                return _serialNumbers[_parameters.IpAddress.ToString()] += 2;
            }
        }

        private object _timerLock = new object();
        private void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_timerLock)
            {
                if (!IsConnected())
                {
                    ChangeDischargerState(EDischargerState.Disconnected, _parameters.DischargerChannel);
                    return;
                }

                SendCommand_RequestChannelInfo();

                // 자체 제작 방전기 신규 버전의 경우, 경광등 제어 필요
                if (_parameters.DischargerModel == EDischargerModel.MBDC_A2)
                {
                    EDioControl eDioControl = EDioControl.TowerLampGreen;

                    // 다채널 방전기 경광등 제어 동작 확인 후 반영 필요
                    for (int i = 0; i < _parameters.DischargerChannel.Length; i++)
                    {
                        short channel = _parameters.DischargerChannel[i];
                        int index = channel - 1;

                        if (_dischargerState[index] == EDischargerState.SafetyOutOfRange ||
                            _dischargerState[index] == EDischargerState.ReturnCodeError ||
                            _dischargerState[index] == EDischargerState.ChStatusError ||
                            _dischargerState[index] == EDischargerState.DeviceError)
                        {
                            if (eDioControl > EDioControl.TowerLampRed)
                            {
                                eDioControl = EDioControl.TowerLampRed;
                            }
                        }
                        else
                        {
                            if (_dischargerState[index] == EDischargerState.Discharging)
                            {
                                if (eDioControl > EDioControl.TowerLampGreen)
                                {
                                    eDioControl = EDioControl.TowerLampGreen;
                                }
                            }
                            else if (_dischargerState[index] == EDischargerState.Pause)
                            {
                                if (eDioControl > EDioControl.TowerLampGreen)
                                {
                                    eDioControl = EDioControl.TowerLampGreen;
                                }
                            }
                            else if (_dischargerState[index] == EDischargerState.None ||
                                     _dischargerState[index] == EDischargerState.Ready)
                            {
                                if (eDioControl > EDioControl.TowerLampYellow)
                                {
                                    eDioControl = EDioControl.TowerLampYellow;
                                }
                            }
                        }
                        SendCommand_LampControl(eDioControl, IsLampBuzzerUsed);
                    }
                }
            }
        }

        public bool SendCommand_SetSafetyCondition(short channel, double voltageMax, double voltageMin, double currentMax, double currentMin, double tempMax, double tempMin)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogTrace.DischargerData();
                    int index = channel - 1;

                    // Sinexcel 방전기의 경우, Current 안전 조건 Min 값은 -Max 값으로 들어가도록 함
                    if (_parameters.DischargerModel >= EDischargerModel.MBDC_S1)
                    {
                        currentMin = -currentMax;
                    }

                    // 안전 조건 설정
                    var parameter = new Parameter.SetSafetyCondition()
                    {
                        ChannelNumber = channel,
                        VoltageUpperLimitValue = voltageMax,
                        VoltageLowerLimitValue = voltageMin,
                        CurrentUpperLimitValue = currentMax,
                        CurrentLowerLimitValue = currentMin,
                    };
                    var result = Process(ECommandCode.SetSafetyCondition, parameter);

                    bool isOk = (result == EResultCode.Success) ? true : false;

                    if (isOk)
                    {
                        // 자체 제작 방전기의 경우, Current 안전 조건 내부 마진 적용 필요
                        if (_parameters.DischargerModel <= EDischargerModel.MBDC_A2)
                        {
                            currentMax += SafetyMarginCurrent;
                            currentMin -= SafetyMarginCurrent;
                        }

                        _dischargerDataArray[index].SafetyVoltageMax = voltageMax;
                        _dischargerDataArray[index].SafetyVoltageMin = voltageMin;
                        _dischargerDataArray[index].SafetyCurrentMax = currentMax;
                        _dischargerDataArray[index].SafetyCurrentMin = currentMin;
                        _dischargerDataArray[index].SafetyTempMax = tempMax;
                        _dischargerDataArray[index].SafetyTempMin = tempMin;

                        // 방전 Trace Log 저장 - 안전 조건 설정
                        dischargerData = new LogTrace.DischargerData()
                        {
                            Name = _parameters.DischargerName,
                            Channel = channel,
                            SafetyVoltageMax = voltageMax,
                            SafetyVoltageMin = voltageMin,
                            SafetyCurrentMax = currentMax,
                            SafetyCurrentMin = currentMin,
                            SafetyTempMax = tempMax,
                            SafetyTempMin = tempMin,
                        };
                        new LogTrace(ELogDischarge.COMM_OK_SET_SAFETYCONDITION, dischargerData);

                        return true;
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 안전 조건 설정 실패
                        dischargerData = new LogTrace.DischargerData()
                        {
                            Name = _parameters.DischargerName,
                            Channel = channel,
                            SafetyVoltageMax = voltageMax,
                            SafetyVoltageMin = voltageMin,
                            SafetyCurrentMax = currentMax,
                            SafetyCurrentMin = currentMin,
                            SafetyTempMax = tempMax,
                            SafetyTempMin = tempMin,
                        };
                        new LogTrace(ELogDischarge.COMM_ERROR_SET_SAFETYCONDITION, dischargerData);

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 안전 조건 설정 실패
                new LogTrace(ELogDischarge.COMM_ERROR_SET_SAFETYCONDITION, ex);
                return false;
            }
        }

        public EDischargerClientError SendCommand_StartDischarge(short channel, EWorkMode workMode, double setValue, double limitingValue)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogTrace.DischargerData();
                    int index = channel - 1;

                    if (_dischargerState[index] == EDischargerState.Discharging ||
                        _dischargerState[index] == EDischargerState.Ready ||
                        _dischargerState[index] == EDischargerState.Pause)
                    {
                        // Sinexcel 방전기의 경우, SetValue(Voltage) 값이 0이어서는 안되고 음수로 들어가야 함
                        if (_parameters.DischargerModel >= EDischargerModel.MBDC_S1)
                        {
                            if (setValue == 0)
                            {
                                setValue = 0.001;
                            }
                            setValue = -setValue;
                        }

                        // 동작 시작
                        var parameter = new Parameter.StartDischarge()
                        {
                            ChannelNumber = channel,
                            WorkMode = (double)workMode,
                            SetValue = setValue,
                            LimitingValue = limitingValue,
                        };
                        var result = Process(ECommandCode.StartDischarge, parameter);

                        bool isOk = (result == EResultCode.Success) ? true : false;

                        if (isOk)
                        {
                            if (_dischargerState[index] == EDischargerState.Ready)
                            {
                                _dischargerDataArray[index].DischargingStartTime = DateTime.Now;
                            }

                            // 방전 Trace Log 저장 - 동작 시작
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,
                                Channel = channel,
                                EWorkMode = workMode,
                                SetValue_Voltage = setValue,
                                LimitingValue_Current = limitingValue
                            };
                            new LogTrace(ELogDischarge.COMM_OK_START, dischargerData);

                            return EDischargerClientError.Ok;
                        }
                        else
                        {
                            // 방전 Trace Log 저장 - 동작 시작 실패
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,
                                Channel = channel,
                                EDischargerClientError = EDischargerClientError.FailProcessPacket,
                                EWorkMode = workMode,
                                SetValue_Voltage = setValue,
                                LimitingValue_Current = limitingValue
                            };
                            new LogTrace(ELogDischarge.COMM_ERROR_START, dischargerData);

                            return EDischargerClientError.FailProcessPacket;
                        }
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 동작 시작 실패
                        dischargerData = new LogTrace.DischargerData()
                        {
                            Name = _parameters.DischargerName,
                            Channel = channel,
                            EDischargerClientError = EDischargerClientError.InvalidDischargerState,
                            EWorkMode = workMode,
                            SetValue_Voltage = setValue,
                            LimitingValue_Current = limitingValue
                        };
                        new LogTrace(ELogDischarge.COMM_ERROR_START, dischargerData);

                        return EDischargerClientError.InvalidDischargerState;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 동작 시작 실패
                new LogTrace(ELogDischarge.COMM_ERROR_START, ex);

                return EDischargerClientError.Exception;
            }
        }

        public EDischargerClientError SendCommand_StopDischarge(short channel)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogTrace.DischargerData();
                    int index = channel - 1;

                    if (_dischargerState[index] == EDischargerState.Discharging ||
                        _dischargerState[index] == EDischargerState.Pause ||
                        _dischargerState[index] == EDischargerState.SafetyOutOfRange ||
                        _dischargerState[index] == EDischargerState.ReturnCodeError ||
                        _dischargerState[index] == EDischargerState.ChStatusError ||
                        _dischargerState[index] == EDischargerState.DeviceError)
                    {
                        // 동작 정지
                        var parameter = new Parameter.StopDischarge()
                        {
                            ChannelNumber = channel,
                        };
                        var result = Process(ECommandCode.StopDischarge, parameter);

                        bool isOk = (result == EResultCode.Success) ? true : false;

                        if (isOk)
                        {
                            _isPaused[index] = false;

                            // 방전 Trace Log 저장 - 동작 정지
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,
                                Channel = channel,
                            };
                            new LogTrace(ELogDischarge.COMM_OK_STOP, dischargerData);
                            
                            return EDischargerClientError.Ok;
                        }
                        else
                        {
                            // 방전 Trace Log 저장 - 동작 정지 실패
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,
                                Channel = channel,
                                EDischargerClientError = EDischargerClientError.FailProcessPacket,
                            };
                            new LogTrace(ELogDischarge.COMM_ERROR_STOP, dischargerData);

                            return EDischargerClientError.FailProcessPacket;
                        }
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 동작 정지 실패
                        dischargerData = new LogTrace.DischargerData()
                        {
                            Name = _parameters.DischargerName,
                            Channel = channel,
                            EDischargerClientError = EDischargerClientError.InvalidDischargerState,
                        };
                        new LogTrace(ELogDischarge.COMM_ERROR_STOP, dischargerData);

                        return EDischargerClientError.InvalidDischargerState;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 동작 정지 실패
                new LogTrace(ELogDischarge.COMM_ERROR_STOP, ex);

                return EDischargerClientError.Exception;
            }
        }

        public EDischargerClientError SendCommand_PauseDischarge(short channel)
        {
            try
            {
                lock (_packetLock)
                {
                    int index = channel - 1;
                    var dischargerData = new LogTrace.DischargerData();

                    if (_dischargerState[index] == EDischargerState.Discharging)
                    {
                        // 동작 일시 정지
                        var parameter = new Parameter.StopDischarge()
                        {
                            ChannelNumber = channel,
                        };
                        var result = Process(ECommandCode.StopDischarge, parameter);

                        bool isOk = (result == EResultCode.Success) ? true : false;

                        if (isOk)
                        {
                            ChangeDischargerState(EDischargerState.Pause, channel);

                            _isPaused[index] = true;

                            // 방전 Trace Log 저장 - 동작 일시 정지
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,
                                Channel = channel,
                            };
                            new LogTrace(ELogDischarge.COMM_OK_PAUSE, dischargerData);

                            return EDischargerClientError.Ok;
                        }
                        else
                        {
                            // 방전 Trace Log 저장 - 동작 일시 정지 실패
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,
                                Channel = channel,
                                EDischargerClientError = EDischargerClientError.FailProcessPacket,
                            };
                            new LogTrace(ELogDischarge.COMM_ERROR_PAUSE, dischargerData);

                            return EDischargerClientError.FailProcessPacket;
                        }
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 동작 일시 정지 실패
                        dischargerData = new LogTrace.DischargerData()
                        {
                            Name = _parameters.DischargerName,
                            Channel = channel,
                            EDischargerClientError = EDischargerClientError.InvalidDischargerState,
                        };
                        new LogTrace(ELogDischarge.COMM_ERROR_PAUSE, dischargerData);

                        return EDischargerClientError.InvalidDischargerState;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 동작 일시 정지 실패
                new LogTrace(ELogDischarge.COMM_ERROR_PAUSE, ex);

                return EDischargerClientError.Exception;
            }
        }

        public bool SendCommand_ClearAlarm(short channel)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogTrace.DischargerData();

                    // 에러 해제
                    var parameter = new Parameter.ClearAlarm()
                    {
                        ChannelNumber = channel,
                    };
                    var result = Process(ECommandCode.ClearAlarm, parameter);

                    bool isOk = (result == EResultCode.Success) ? true : false;

                    if (isOk)
                    {
                        // 방전 Trace Log 저장 - 에러 해제
                        dischargerData = new LogTrace.DischargerData()
                        {
                            Name = _parameters.DischargerName,
                            Channel = channel,
                        };
                        new LogTrace(ELogDischarge.COMM_OK_CLEAR_ALARM, dischargerData);
                        return true;
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 에러 해제 실패
                        dischargerData = new LogTrace.DischargerData()
                        {
                            Name = _parameters.DischargerName,
                            Channel = channel,
                        };
                        new LogTrace(ELogDischarge.COMM_ERROR_CLEAR_ALARM, dischargerData);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 에러 해제 실패
                new LogTrace(ELogDischarge.COMM_ERROR_CLEAR_ALARM, ex);

                return false;
            }
        }

        public bool SendCommand_LampControl(EDioControl dioControl, bool isBuzzer = false)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogTrace.DischargerData();

                    uint dioValue = (uint)dioControl;
                    if (isBuzzer)
                    {
                        dioValue |= (uint)EDioControl.TowerLampBuzzer;
                    }

                    // 경광등 제어
                    var parameter = new Parameter.LampControl()
                    {
                        DioValue = dioValue,
                    };
                    var result = Process(ECommandCode.LampControl, parameter);

                    bool isOk = (result == EResultCode.Success) ? true : false;

                    if (isOk)
                    {
                        if (_dioValue != dioValue)
                        {
                            // 방전 Trace Log 저장 - 경광등 제어
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,
                                LampDioValue = dioValue,
                            };
                            new LogTrace(ELogDischarge.COMM_OK_CONTROL_LAMP, dischargerData);
                        }

                        _dioValue = dioValue;

                        return true;
                    }
                    else
                    {
                        if (_dioValue != dioValue)
                        {
                            // 방전 Trace Log 저장 - 경광등 제어 실패
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,
                                LampDioValue = dioValue,
                            };
                            new LogTrace(ELogDischarge.COMM_ERROR_CONTROL_LAMP, dischargerData);
                        }

                        _dioValue = dioValue;

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 경광등 제어 실패
                new LogTrace(ELogDischarge.COMM_ERROR_CONTROL_LAMP, ex);

                return false;
            }
        }

        public bool SendCommand_RequestChannelInfo()
        {
            lock (_packetLock)
            {
                // 모니터링
                var result = Process(ECommandCode.GetChannelInfo);

                bool isOk = (result == EResultCode.Success) ? true : false;
                return isOk;
            }
        }

        private EResultCode Process(ECommandCode eCommandCode, object obj = null)
        {
            lock (_packetLock)
            {
                // 커맨드 전송
                var writeResult = WriteCommand(eCommandCode, obj);
                if (writeResult != EthernetClientStatus.OK)
                {
                    return EResultCode.FailWriteCommand;
                }

                Thread.Sleep(50);

                // 데이터 가져오기
                var readResult = ReadCommand(out byte[] readBuffer);
                if (readResult != EthernetClientStatus.OK)
                {
                    return EResultCode.FailReadCommand;
                }

                // 데이터 파싱
                var result = ParseData(eCommandCode, readBuffer);
                return result;
            }
        }

        private EthernetClientStatus WriteCommand(ECommandCode eCommandCode, object obj = null)
        {
            try
            {
                var packetGenerator = new DischargerPacketGenerator();
                byte[] writeBuffer = null;

                if (eCommandCode == ECommandCode.SetSafetyCondition)
                {
                    if (obj is Parameter.SetSafetyCondition parameter)
                    {
                        packetGenerator.Command(parameter.CommandCode, GetPacketSerialNumber());
                        packetGenerator.Channel(parameter.ChannelNumber);
                        packetGenerator.Parameter(parameter.Index1, parameter.VoltageUpperLimitValue);
                        packetGenerator.Parameter(parameter.Index2, parameter.VoltageLowerLimitValue);
                        packetGenerator.Parameter(parameter.Index3, parameter.CurrentUpperLimitValue);
                        packetGenerator.Parameter(parameter.Index4, parameter.CurrentLowerLimitValue);
                        packetGenerator.Parameter(parameter.Index5, parameter.FixedValue);
                    }
                }
                else if (eCommandCode == ECommandCode.StartDischarge)
                {
                    if (obj is Parameter.StartDischarge parameter)
                    {
                        packetGenerator.Command(parameter.CommandCode, GetPacketSerialNumber());
                        packetGenerator.Channel(parameter.ChannelNumber);
                        packetGenerator.Parameter(parameter.Index1, parameter.WorkMode);
                        packetGenerator.Parameter(parameter.Index2, parameter.SetValue);
                        packetGenerator.Parameter(parameter.Index3, parameter.LimitingValue);
                        packetGenerator.Parameter(parameter.Index4, parameter.FixedValue);
                    }
                }
                else if (eCommandCode == ECommandCode.StopDischarge)
                {
                    if (obj is Parameter.StopDischarge parameter)
                    {
                        packetGenerator.Command(parameter.CommandCode, GetPacketSerialNumber());
                        packetGenerator.Channel(parameter.ChannelNumber);
                        packetGenerator.Parameter(parameter.Index1, parameter.WorkMode);
                        packetGenerator.Parameter(parameter.Index2, parameter.FixedValue);
                    }
                }
                else if (eCommandCode == ECommandCode.ClearAlarm)
                {
                    if (obj is Parameter.ClearAlarm parameter)
                    {
                        packetGenerator.Command(parameter.CommandCode, GetPacketSerialNumber());
                        packetGenerator.Channel(parameter.ChannelNumber);
                        packetGenerator.Parameter(parameter.Index1, parameter.FixedValue);
                        packetGenerator.Parameter(parameter.Index2, parameter.FixedValue);
                    }
                }
                else if (eCommandCode == ECommandCode.LampControl)
                {
                    if (obj is Parameter.LampControl parameter)
                    {
                        packetGenerator.Command(parameter.CommandCode, GetPacketSerialNumber());
                        packetGenerator.Channel(parameter.ChannelNumber);
                        packetGenerator.Parameter(parameter.Index1, parameter.DioValue);
                    }
                }
                else if (eCommandCode == ECommandCode.GetChannelInfo)
                {
                    packetGenerator.Command(ECommandCode.GetChannelInfo, GetPacketSerialNumber());
                    packetGenerator.Channel(_parameters.DischargerChannel);
                }

                writeBuffer = packetGenerator.GeneratePacket();
                var writeResult = _dischargerClient.Write(_handle, writeBuffer);

                return writeResult;
            }
            catch
            {
                return EthernetClientStatus.FAIL_WRITE;
            }
        }

        private EthernetClientStatus ReadCommand(out byte[] dataBuffer)
        {
            dataBuffer = new byte[0];

            try
            {
                var readResult = _dischargerClient.Read(_handle, out byte[] readBuffer);
                if (readResult != EthernetClientStatus.OK)
                {
                    return readResult;
                }

                // 데이터 추출
                var extractDataResult = ExtractDataBuffer(readBuffer, out dataBuffer);
                if (extractDataResult != EResultCode.Success)
                {
                    return EthernetClientStatus.FAIL_READ;
                }

                return readResult;
            }
            catch
            {
                return EthernetClientStatus.FAIL_READ;
            }
        }

        private EResultCode ExtractDataBuffer(byte[] readBuffer, out byte[] dataBuffer)
        {
            dataBuffer = new byte[0];

            try
            {
                int dataLength = readBuffer.Length - PacketConstant.PACKET_HEADER_SIZE;

                dataBuffer = new byte[dataLength];

                Array.Copy(readBuffer, PacketConstant.PACKET_HEADER_SIZE, dataBuffer, 0, dataLength);

                return EResultCode.Success;
            }
            catch
            {
                return EResultCode.FailReadData;
            }
        }

        private EResultCode ParseData(ECommandCode eCommandCode, byte[] readBuffer)
        {
            try
            {
                if (eCommandCode <= ECommandCode.SetParameter)
                {
                    var value = new Parameter.Reply().Parse(readBuffer);

                    if (value.ReturnCode != EReturnCode.Success)
                    {
                        ChangeDischargerState(EDischargerState.ReturnCodeError, _parameters.DischargerChannel);
                    }
                }
                else if (eCommandCode == ECommandCode.GetChannelInfo)
                {
                    object value = null;

                    if (_parameters.DischargerModel <= EDischargerModel.MBDC_S1)
                    {
                        value = new ChannelInfo.Reply().Parse(readBuffer, _parameters.DischargerChannel.Last());
                    }
                    else
                    {
                        value = new ChannelInfo.Reply_v34().Parse(readBuffer, _parameters.DischargerChannel.Last());
                    }

                    UpdateChannelInfoData(value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                // 방전 Trace Log 저장 - 데이터 수신 실패
                new LogTrace(ELogDischarge.COMM_ERROR_GET_DATA, readBuffer.GetRawDataHexString());

                return EResultCode.FailParseData;
            }

            return EResultCode.Success;
        }

        private void UpdateChannelInfoData(object channelInfo)
        {
            int numberOfChannel = _parameters.DischargerChannel.Length;

            object obj = null;
            uint[] errorCodeArray = new uint[numberOfChannel];
            byte[] dIModuleInfoArray = new byte[numberOfChannel];
            EChannelStatus[] channelStatusArray = new EChannelStatus[numberOfChannel];
            double[] batteryVoltageArray = new double[numberOfChannel];
            double[] batteryCurrentArray = new double[numberOfChannel];
            double[] auxTemp1Array = new double[numberOfChannel];
            EReturnCode eReturnCode = EReturnCode.Success;

            if (channelInfo is ChannelInfo.Reply reply)
            {
                obj = reply.DataArray;
                eReturnCode = reply.ReturnCode;
            }
            else if (channelInfo is ChannelInfo.Reply_v34 reply_v34)
            {
                obj = reply_v34.DataArray;
                eReturnCode = reply_v34.ReturnCode;
            }

            if (obj == null) return;
            else if (obj is ChannelInfo.Data_v34[] dataArray_v34)
            {
                for (int i = 0; i < numberOfChannel; i++)
                {
                    errorCodeArray[i] = dataArray_v34[i].ErrorCode;
                    dIModuleInfoArray[i] = dataArray_v34[i].DIModuleInfo;
                    channelStatusArray[i] = dataArray_v34[i].ChannelStatus;
                    batteryVoltageArray[i] = dataArray_v34[i].BatteryVoltage;
                    batteryCurrentArray[i] = dataArray_v34[i].BatteryCurrent;
                    auxTemp1Array[i] = dataArray_v34[i].AuxTemp1;
                }
            }
            else if (obj is ChannelInfo.Data[] dataArray)
            {
                for (int i = 0; i < numberOfChannel; i++)
                {
                    errorCodeArray[i] = dataArray[i].ErrorCode;
                    dIModuleInfoArray[i] = dataArray[i].DIModuleInfo;
                    channelStatusArray[i] = dataArray[i].ChannelStatus;
                    batteryVoltageArray[i] = dataArray[i].BatteryVoltage;
                    batteryCurrentArray[i] = dataArray[i].BatteryCurrent;
                    auxTemp1Array[i] = dataArray[i].AuxTemp1;
                }
            }

            for (int i = 0; i < _parameters.DischargerChannel.Length; i++)
            {
                short channel = _parameters.DischargerChannel[i];

                /// 채널 상태 업데이트
                _dischargerDataArray[i].ErrorCode = errorCodeArray[i];
                _dischargerDataArray[i].DiModuleInfo = dIModuleInfoArray[i];
                _dischargerDataArray[i].ReturnCode = eReturnCode;
                _dischargerDataArray[i].ChannelStatus = channelStatusArray[i];

                /// 전압, 전류, 온도 값 업데이트
                _dischargerDataArray[i].ReceiveBatteryVoltage = batteryVoltageArray[i];
                _dischargerDataArray[i].ReceiveDischargeCurrent = -batteryCurrentArray[i];

                if (!_parameters.DischargerIsTempModule)
                {
                    _dischargerDataArray[i].ReceiveDischargeTemp = auxTemp1Array[i];
                }

                if (errorCodeArray[i] != 0) /// 에러코드 검사
                {
                    ChangeDischargerState(EDischargerState.DeviceError, channel);
                }
                else if (eReturnCode != EReturnCode.Success) /// 리턴 코드 검사
                {
                    ChangeDischargerState(EDischargerState.ReturnCodeError, channel);
                }
                else if (channelStatusArray[i] == EChannelStatus.Error) /// 채널 상태 검사
                {
                    ChangeDischargerState(EDischargerState.ChStatusError, channel);
                }
                else if (channelStatusArray[i] == EChannelStatus.Standby0 ||
                         channelStatusArray[i] == EChannelStatus.Standby5)
                {
                    if (_dischargerState[i] != EDischargerState.Pause)
                    {
                        ChangeDischargerState(EDischargerState.Ready, channel);
                    }
                }
                else
                {
                    if (!_isPaused[i])
                    {
                        ChangeDischargerState(EDischargerState.Discharging, channel);
                    }
                }
            }
        }
    }
}
