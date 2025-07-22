using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Discharger.MVVM.Repository;
using DischargerV2.LOG;
using Ethernet.Client.Common;
using Microsoft.Xaml.Behaviors.Media;
using Sqlite.Common;
using static DischargerV2.LOG.LogDischarge;
using static DischargerV2.LOG.LogTrace;
using static Ethernet.Client.Discharger.ChannelInfo;

namespace Ethernet.Client.Discharger
{
    public enum EDischargerState : byte
    {
        None = 0x0,

        Disconnected = 0x10,
        Connecting = 0x11,
        Ready = 0x12,
        Discharging = 0x13,
        Pause = 0x14,

        SafetyOutOfRange = 0x20,
        ReturnCodeError = 0x21,
        ChStatusError = 0x22,
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
        public short DischargerChannel = short.MaxValue;
        public bool DischargerIsTempModule { get; set; } = false;
        public IPAddress IpAddress = IPAddress.None;
        public int EthernetPort = int.MaxValue;
        public int TimeOutMs = int.MaxValue;
        public double SafetyVoltageMax = double.MaxValue;
        public double SafetyVoltageMin = double.MaxValue;
        public double SafetyCurrentMax = double.MaxValue;
        public double SafetyCurrentMin = double.MaxValue;
        public double SafetyTempMax = double.MaxValue;
        public double SafetyTempMin = double.MaxValue;
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

        private EthernetClientDischargerStart _parameters = null;
        private EthernetClient _dischargerClient = null;

        private System.Timers.Timer ReadInfoTimer = null;

        private DischargerDatas[] _dischargerDataArray = null;
       
        private EDischargerState _dischargerState = EDischargerState.None;
        private uint _dioValue = uint.MaxValue;

        public static double SafetyMarginVoltage = 15;
        public static double SafetyMarginCurrent = 2;

        public static bool IsLampBuzzerUsed = true;

        public LogTrace.DischargerData GetLogSystemDischargerData()
        {
            var dischargerData = new LogTrace.DischargerData()
            {
                Name = _parameters.DischargerName,
                EDischargerModel = _parameters.DischargerModel,
                Channel = _parameters.DischargerChannel,
                IpAddress = _parameters.IpAddress,

                SafetyVoltageMax = _parameters.SafetyVoltageMax,
                SafetyVoltageMin = _parameters.SafetyVoltageMin,
                SafetyCurrentMax = _parameters.SafetyCurrentMax,
                SafetyCurrentMin = _parameters.SafetyCurrentMin,
                SafetyTempMax = _parameters.SafetyTempMax,
                SafetyTempMin = _parameters.SafetyTempMin,
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

        public bool ChangeDischargerState(EDischargerState dischargerState, int channel = 0)
        {
            try
            {
                if (dischargerState == EDischargerState.SafetyOutOfRange)
                {
                    _dischargerDataArray[channel].ErrorCode = 0xF0000001;
                }
                else if (dischargerState == EDischargerState.ReturnCodeError)
                {
                    _dischargerDataArray[channel].ErrorCode = 0xF0000002;
                }
                else if (dischargerState == EDischargerState.ChStatusError)
                {
                    _dischargerDataArray[channel].ErrorCode = 0xF0000003;
                }

                if (_dischargerState != dischargerState)
                {
                    // 방전 Trace Log 저장 - 상태 변경
                    var dischargerData = new LogTrace.DischargerData()
                    {
                        Name = _parameters.DischargerName,

                        ReceiveBatteryVoltage = _dischargerDataArray[channel].ReceiveBatteryVoltage.ToString("F1"),
                        ReceiveDischargeCurrent = _dischargerDataArray[channel].ReceiveDischargeCurrent.ToString("F1"),
                        ReceiveDischargeTemp = _dischargerDataArray[channel].ReceiveDischargeTemp.ToString("F1"),
                        
                        ErrorCode = _dischargerDataArray[channel].ErrorCode,
                        EReturnCode = _dischargerDataArray[channel].ReturnCode,
                        EDischargerState = dischargerState,
                    };
                    new LogTrace(ELogDischarge.COMM_OK_SET_STATE, dischargerData);
                }

                _dischargerState = dischargerState;

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
                parameters.DischargerChannel == short.MaxValue ||
                parameters.IpAddress == IPAddress.None ||
                parameters.EthernetPort == int.MaxValue ||
                parameters.TimeOutMs == int.MaxValue ||
                parameters.SafetyVoltageMax == double.MaxValue ||
                parameters.SafetyVoltageMin == double.MaxValue ||
                parameters.SafetyCurrentMax == double.MaxValue ||
                parameters.SafetyCurrentMin == double.MaxValue)
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

            int channel = _parameters.DischargerChannel;
            
            _dischargerDataArray = new DischargerDatas[channel];
            
            for (int i = 0; i < channel; i++)
            {
                _dischargerDataArray[i] = new DischargerDatas();
            }

            ChangeDischargerState(EDischargerState.Connecting);

            if (!IsParameterValid(_parameters))
            {
                ChangeDischargerState(EDischargerState.Disconnected);
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
            clientStart.WriteFunction = WriteData;
            clientStart.ReadFunction = ReadData;
            clientStart.ParseFunction = ParseData;
            _dischargerClient = new EthernetClient();

            var result = _dischargerClient.Connect(clientStart);
            if (result != EthernetClientStatus.OK)
            {
                ChangeDischargerState(EDischargerState.Disconnected);
                return false;
            }

            Thread.Sleep(2000);

            /// 이전 에러 상태 초기화
            bool clearAlarmResult = SendCommand_ClearAlarm();
            if (clearAlarmResult == false)
            {
                ChangeDischargerState(EDischargerState.Disconnected);
                return false;
            }

            /// 경광등 초기화
            if (_parameters.DischargerModel == EDischargerModel.MBDC)
            {
                bool lampControlResult = SendCommand_LampControl(EDioControl.TowerLampYellow, false);
                if (lampControlResult == false)
                {
                    ChangeDischargerState(EDischargerState.Disconnected);
                    return false;
                }
            }

            /// Safety Condition 설정
            bool safetyConditionResult = SendCommand_SetSafetyCondition(
                _parameters.SafetyVoltageMax, _parameters.SafetyVoltageMin,
                _parameters.SafetyCurrentMax, _parameters.SafetyCurrentMin,
                _parameters.SafetyTempMax, _parameters.SafetyTempMin);
            if (safetyConditionResult == false)
            {
                ChangeDischargerState(EDischargerState.Disconnected);
                return false;
            }

            ReadInfoTimer?.Stop();
            ReadInfoTimer = null;
            ReadInfoTimer = new System.Timers.Timer();
            ReadInfoTimer.Interval = 1000;
            ReadInfoTimer.Elapsed += OneSecondTimer_Elapsed;
            ReadInfoTimer.Start();

            ChangeDischargerState(EDischargerState.Ready);

            return true;
        }

        public void Stop()
        {
            _dischargerClient?.Disconnect();
            _dischargerClient = null;

            ReadInfoTimer?.Stop();
            ReadInfoTimer = null;

            ChangeDischargerState(EDischargerState.Disconnected);
        }

        public DischargerDatas GetDatas(int channel = 0)
        {
            /// Deep Copy
            DischargerDatas temp = new DischargerDatas();

            try
            {
                /// receive data
                temp.ErrorCode = _dischargerDataArray[channel].ErrorCode;
                temp.DiModuleInfo = _dischargerDataArray[channel].DiModuleInfo;
                temp.ChannelStatus = _dischargerDataArray[channel].ChannelStatus;
                temp.ReceiveBatteryVoltage = double.Parse(_dischargerDataArray[channel].ReceiveBatteryVoltage.ToString("F3"));
                temp.ReceiveDischargeCurrent = double.Parse(_dischargerDataArray[channel].ReceiveDischargeCurrent.ToString("F3"));
                temp.ReceiveDischargeTemp = double.Parse(_dischargerDataArray[channel].ReceiveDischargeTemp.ToString("F1"));

                /// safety
                temp.SafetyCurrentMin = _dischargerDataArray[channel].SafetyCurrentMin;
                temp.SafetyCurrentMax = _dischargerDataArray[channel].SafetyCurrentMax;
                temp.SafetyVoltageMin = _dischargerDataArray[channel].SafetyVoltageMin;
                temp.SafetyVoltageMax = _dischargerDataArray[channel].SafetyVoltageMax;
                temp.SafetyTempMin = _dischargerDataArray[channel].SafetyTempMin;
                temp.SafetyTempMax = _dischargerDataArray[channel].SafetyTempMax;

                /// start time
                temp.DischargingStartTime = _dischargerDataArray[channel].DischargingStartTime;

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

        public EDischargerState GetState()
        {
            return _dischargerState;
        }

        private byte GetPacketSerialNumber()
        {
            lock (_serialNumberDataLock[_parameters.IpAddress.ToString()])
            {
                return _serialNumbers[_parameters.IpAddress.ToString()]++;
            }
        }

        private object _timerLock = new object();
        private void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_timerLock)
            {
                if (!IsConnected())
                {
                    ChangeDischargerState(EDischargerState.Disconnected);
                    return;
                }

                SendCommand_RequestChannelInfo();

                if (_dischargerState == EDischargerState.SafetyOutOfRange ||
                    _dischargerState == EDischargerState.ReturnCodeError ||
                    _dischargerState == EDischargerState.ChStatusError ||
                    _dischargerState == EDischargerState.DeviceError)
                {
                    if (IsLampBuzzerUsed)
                    {
                        SendCommand_LampControl(EDioControl.TowerLampRed, true);
                    }
                    else
                    {
                        SendCommand_LampControl(EDioControl.TowerLampRed, false);
                    }

                    SendCommand_StopDischarge();
                    return;
                }
                
                if (_dischargerState == EDischargerState.Discharging)
                {
                    SendCommand_LampControl(EDioControl.TowerLampGreen, false);

                    int channel = _parameters.DischargerChannel - 1;

                    if (_dischargerDataArray[channel].ReceiveBatteryVoltage < 1 && 
                        _dischargerDataArray[channel].ReceiveDischargeCurrent < 0.1)
                    {
                        SendCommand_StopDischarge();
                    }
                }
                else if (_dischargerState == EDischargerState.Pause)
                {
                    SendCommand_LampControl(EDioControl.TowerLampGreen, false);
                }
                else if (_dischargerState == EDischargerState.None || _dischargerState == EDischargerState.Ready)
                {
                    SendCommand_LampControl(EDioControl.TowerLampYellow, false);
                }
            }
        }

        public EDischargerClientError SendCommand_StartDischarge(EWorkMode workMode, double setValue, double limitingValue)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogTrace.DischargerData();

                    if (_dischargerState == EDischargerState.Discharging ||
                        _dischargerState == EDischargerState.Ready ||
                        _dischargerState == EDischargerState.Pause)
                    {
                        byte[] writeBuffer = CreateStartDischargeCommand(
                            _parameters.DischargerChannel, workMode, setValue, limitingValue);

                        // 동작 시작
                        bool isOk = _dischargerClient.ProcessPacket(writeBuffer);

                        if (isOk)
                        {
                            if (_dischargerState == EDischargerState.Ready)
                            {
                                int channel = _parameters.DischargerChannel - 1;

                                _dischargerDataArray[channel].DischargingStartTime = DateTime.Now;
                            }

                            // 방전 Trace Log 저장 - 동작 시작
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,

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

        public bool SendCommand_SetSafetyCondition(double voltageMax, double voltageMin, double currentMax, double currentMin, double tempMax, double tempMin)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogTrace.DischargerData();
                    int channel = _parameters.DischargerChannel - 1;

                    LogArgument logArgument = new LogArgument("Set Safety Condition.");
                    logArgument.Parameters["VoltMax"] = voltageMax;
                    logArgument.Parameters["VoltMin"] = voltageMin;
                    logArgument.Parameters["CurrentMax"] = currentMax;
                    logArgument.Parameters["CurrentMin"] = currentMin;
                    logArgument.Parameters["TempMax"] = tempMax;
                    logArgument.Parameters["TempMin"] = tempMin;

                    byte[] writeBuffer = CreateSetSafetyConditionCommand(
                        _parameters.DischargerChannel,
                        voltageMax, voltageMin,
                        currentMax, currentMin);

                    // 안전 조건 설정
                    bool isOk = _dischargerClient.ProcessPacket(writeBuffer);

                    if (isOk)
                    {
                        _dischargerDataArray[channel].SafetyVoltageMax = voltageMax;
                        _dischargerDataArray[channel].SafetyVoltageMin = voltageMin;
                        _dischargerDataArray[channel].SafetyCurrentMax = currentMax + SafetyMarginCurrent;
                        _dischargerDataArray[channel].SafetyCurrentMin = currentMin - SafetyMarginCurrent;
                        _dischargerDataArray[channel].SafetyTempMax = tempMax;
                        _dischargerDataArray[channel].SafetyTempMin = tempMin;

                        // 방전 Trace Log 저장 - 안전 조건 설정
                        dischargerData = new LogTrace.DischargerData()
                        {
                            Name = _parameters.DischargerName,

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

        public EDischargerClientError SendCommand_StopDischarge()
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogTrace.DischargerData();

                    if (_dischargerState == EDischargerState.Discharging ||
                        _dischargerState == EDischargerState.Pause ||
                        _dischargerState == EDischargerState.SafetyOutOfRange ||
                        _dischargerState == EDischargerState.ReturnCodeError ||
                        _dischargerState == EDischargerState.ChStatusError ||
                        _dischargerState == EDischargerState.DeviceError)
                    {
                        byte[] writeBuffer = CreateStopDischargeCommand(_parameters.DischargerChannel);

                        // 동작 정지
                        bool isOk = _dischargerClient.ProcessPacket(writeBuffer);

                        if (isOk)
                        {
                            if (_dischargerState == EDischargerState.Pause)
                            {
                                ChangeDischargerState(EDischargerState.Ready);
                            }

                            // 방전 Trace Log 저장 - 동작 정지
                            new LogTrace(ELogDischarge.COMM_OK_STOP);
                            
                            return EDischargerClientError.Ok;
                        }
                        else
                        {
                            // 방전 Trace Log 저장 - 동작 정지 실패
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,

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

        public EDischargerClientError SendCommand_PauseDischarge()
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogTrace.DischargerData();

                    if (_dischargerState == EDischargerState.Discharging)
                    {
                        byte[] writeBuffer = CreateStopDischargeCommand(_parameters.DischargerChannel);

                        bool isOk = _dischargerClient.ProcessPacket(writeBuffer);

                        if (isOk)
                        {
                            ChangeDischargerState(EDischargerState.Pause);

                            // 방전 Trace Log 저장 - 동작 일시 정지
                            new LogTrace(ELogDischarge.COMM_OK_PAUSE);

                            return EDischargerClientError.Ok;
                        }
                        else
                        {
                            // 방전 Trace Log 저장 - 동작 일시 정지 실패
                            dischargerData = new LogTrace.DischargerData()
                            {
                                Name = _parameters.DischargerName,

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

        public bool SendCommand_ClearAlarm()
        {
            try
            {
                lock (_packetLock)
                {
                    byte[] writeBuffer = CreateClearAlarmCommand(_parameters.DischargerChannel);

                    // 에러 해제
                    bool isOk = _dischargerClient.ProcessPacket(writeBuffer);

                    if (isOk)
                    {
                        // 방전 Trace Log 저장 - 에러 해제
                        new LogTrace(ELogDischarge.COMM_OK_CLEAR_ALARM);

                        return true;
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 에러 해제 실패
                        new LogTrace(ELogDischarge.COMM_ERROR_CLEAR_ALARM);

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

                    byte[] writeBuffer = CreateLampControlCommand(dioValue);

                    // 경광등 제어
                    bool isOk = _dischargerClient.ProcessPacket(writeBuffer);

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
                byte[] writeBuffer = CreateChannelInfoCommand(_parameters.DischargerChannel);

                bool result = _dischargerClient.ProcessPacket(writeBuffer);
                if (result != true)
                {
                    return false;
                }

                return true;
            }
        }

        private bool ReadData(int handle, out byte[] readBuffer)
        {
            EthernetClientStatus result = _dischargerClient.Read(handle, out readBuffer);
            if (result != EthernetClientStatus.OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                return false;
            }

            return true;
        }

        private bool WriteData(int handle, byte[] writeBuffer)
        {
            if (writeBuffer == null || writeBuffer.Length == 0)
            {
                return true;
            }

            EthernetClientStatus result = _dischargerClient.Write(handle, writeBuffer);
            if (result != EthernetClientStatus.OK)
            {
                Debug.WriteLine("WriteData Write Error: " + result.ToString());

                return false;
            }

            return true;
        }

        private bool ParseData(byte[] readBuffer)
        {
            try
            {
                if (readBuffer == null || readBuffer.Length == 0)
                {
                    return true;
                }

                /// 커맨드 코드 가져오기
                byte[] dataByteArray = readBuffer.ExtractSubArray(DCCPacketConstant.PACKET_HEADER_SIZE, 6);
                SetSafetyCondition.Reply reply = dataByteArray.FromByteArrayToPacket<SetSafetyCondition.Reply>();

                if (reply.CommandCode == ECommandCode.RequestCommand)
                {
                    /// 리턴 코드 검사
                    if (reply.ReturnCode != EReturnCode.Success)
                    {
                        ChangeDischargerState(EDischargerState.ReturnCodeError);
                    }
                }
                else if (reply.CommandCode == ECommandCode.ChannelInfo)
                {
                    bool isSingle = (_parameters.DischargerChannel == 1) ? true : false;

                    short length = isSingle ?
                            (short)Marshal.SizeOf(typeof(ChannelInfo.Reply_Channel1)) :
                            (short)Marshal.SizeOf(typeof(ChannelInfo.Reply_Channel2));

                    dataByteArray = readBuffer.ExtractSubArray(DCCPacketConstant.PACKET_HEADER_SIZE, length);

                    if (isSingle)
                    {
                        var packetData = dataByteArray.FromByteArrayToPacket<ChannelInfo.Reply_Channel1>();
                        UpdateChannelInfoData(packetData);
                    }
                    else
                    {
                        var packetData = dataByteArray.FromByteArrayToPacket<ChannelInfo.Reply_Channel2>();
                        UpdateChannelInfoData(packetData);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                // 방전 Trace Log 저장 - 데이터 수신 실패
                new LogTrace(ELogDischarge.COMM_ERROR_GET_DATA, readBuffer.GetRawDataHexString());

                return false;
            }

            return true;
        }

        private byte[] CreateChannelInfoCommand(short channel)
        {
            int length = Marshal.SizeOf(typeof(ChannelInfo.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            ChannelInfo.Request data = new ChannelInfo.Request();
            data.ChannelNumber = channel;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private byte[] CreateSetSafetyConditionCommand(short channel, double voltageMax, double voltageMin, double currentMax, double currentMin)
        {
            int length = Marshal.SizeOf(typeof(SetSafetyCondition.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            SetSafetyCondition.Request data = new SetSafetyCondition.Request();
            data.ChannelNumber = channel;
            data.VoltageUpperLimitValue = voltageMax;
            data.VoltageLowerLimitValue = voltageMin;
            data.CurrentUpperLimitValue = currentMax;
            data.CurrentLowerLimitValue = currentMin;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private byte[] CreateStartDischargeCommand(short channel, EWorkMode workMode, double setValue, double limitingValue)
        {
            int length = Marshal.SizeOf(typeof(StartDischarge.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            StartDischarge.Request data = new StartDischarge.Request();
            data.ChannelNumber = channel;
            data.WorkMode = (double)workMode;
            data.SetValue = setValue;
            data.LimitingValue = limitingValue;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private byte[] CreateStopDischargeCommand(short channel)
        {
            int length = Marshal.SizeOf(typeof(StopDischarge.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            StopDischarge.Request data = new StopDischarge.Request();
            data.ChannelNumber = channel;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private byte[] CreateClearAlarmCommand(short channel)
        {
            int length = Marshal.SizeOf(typeof(ClearAlarm.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            ClearAlarm.Request data = new ClearAlarm.Request();
            data.ChannelNumber = channel;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private byte[] CreateLampControlCommand(uint value)
        {
            int length = Marshal.SizeOf(typeof(LampControl.Request)) + 6;  // 4 is header, 2 is tail

            /// 헤더 생성
            DCCPacketHeader header = new DCCPacketHeader();
            header.SerialNumber = GetPacketSerialNumber();
            header.Length = (short)length;
            byte[] headerByteArray = header.FromPacketToByteArray();

            /// 데이터 생성
            LampControl.Request data = new LampControl.Request();
            data.ChannelNumber = 999; // 방전기가 다 채널일 경우에도 경광등은 하나라서 채널 번호는 999로 고정
            data.DioValue = (double)value;
            byte[] dataByteArray = data.FromPacketToByteArray();

            /// 테일 생성
            DCCPacketTail tail = new DCCPacketTail();
            byte[] tailByteArray = tail.FromPacketToByteArray();

            /// 각 생성된 데이터들 병합
            byte[] byteArraySum = new byte[0];
            byteArraySum = byteArraySum.AppendByteArray(headerByteArray);
            byteArraySum = byteArraySum.AppendByteArray(dataByteArray);
            byteArraySum = byteArraySum.AppendByteArray(tailByteArray);

            return byteArraySum;
        }

        private void UpdateChannelInfoData(ChannelInfo.Reply_Channel1 channelInfo)
        {
            for (int i = 0; i < _parameters.DischargerChannel; i++)
            {
                /// 채널 상태 업데이트
                _dischargerDataArray[i].ErrorCode = channelInfo.ReplyArray[i].ErrorCode;
                _dischargerDataArray[i].DiModuleInfo = channelInfo.ReplyArray[i].DIModuleInfo;
                _dischargerDataArray[i].ReturnCode = channelInfo.ReturnCode;
                _dischargerDataArray[i].ChannelStatus = channelInfo.ReplyArray[i].ChannelStatus;

                /// 전압, 전류, 온도 값 업데이트
                _dischargerDataArray[i].ReceiveBatteryVoltage = channelInfo.ReplyArray[i].BatteryVoltage;
                _dischargerDataArray[i].ReceiveDischargeCurrent = -channelInfo.ReplyArray[i].BatteryCurrent;

                if (!_parameters.DischargerIsTempModule)
                {
                    _dischargerDataArray[i].ReceiveDischargeTemp = channelInfo.ReplyArray[i].AuxTemp1;
                }

                if (_parameters.DischargerIsTempModule == true &&
                    (channelInfo.ReplyArray[i].BatteryVoltage < _dischargerDataArray[i].SafetyVoltageMin ||
                    channelInfo.ReplyArray[i].BatteryVoltage > _dischargerDataArray[i].SafetyVoltageMax ||
                    channelInfo.ReplyArray[i].BatteryCurrent < _dischargerDataArray[i].SafetyCurrentMin ||
                    channelInfo.ReplyArray[i].BatteryCurrent > _dischargerDataArray[i].SafetyCurrentMax))
                {
                    ChangeDischargerState(EDischargerState.SafetyOutOfRange);
                }
                else if (_parameters.DischargerIsTempModule == false &&
                    (channelInfo.ReplyArray[i].BatteryVoltage < _dischargerDataArray[i].SafetyVoltageMin ||
                    channelInfo.ReplyArray[i].BatteryVoltage > _dischargerDataArray[i].SafetyVoltageMax ||
                    channelInfo.ReplyArray[i].BatteryCurrent < _dischargerDataArray[i].SafetyCurrentMin ||
                    channelInfo.ReplyArray[i].BatteryCurrent > _dischargerDataArray[i].SafetyCurrentMax ||
                    channelInfo.ReplyArray[i].AuxTemp1 < _dischargerDataArray[i].SafetyTempMin ||
                    channelInfo.ReplyArray[i].AuxTemp1 > _dischargerDataArray[i].SafetyTempMax))
                {
                    ChangeDischargerState(EDischargerState.SafetyOutOfRange);
                }
                else if (channelInfo.ReplyArray[i].ErrorCode != 0) /// 에러코드 검사
                {
                    ChangeDischargerState(EDischargerState.DeviceError);
                }
                else if (channelInfo.ReturnCode != EReturnCode.Success) /// 리턴 코드 검사
                {
                    ChangeDischargerState(EDischargerState.ReturnCodeError);
                }
                else if (channelInfo.ReplyArray[i].ChannelStatus == EChannelStatus.Error) /// 채널 상태 검사
                {
                    ChangeDischargerState(EDischargerState.ChStatusError);
                }
                else if (channelInfo.ReplyArray[i].ChannelStatus == EChannelStatus.Standby0 ||
                         channelInfo.ReplyArray[i].ChannelStatus == EChannelStatus.Standby5)
                {
                    if (_dischargerState != EDischargerState.Pause)
                    {
                        ChangeDischargerState(EDischargerState.Ready);
                    }
                }
                else
                {
                    ChangeDischargerState(EDischargerState.Discharging);
                }
            }
        }

        private void UpdateChannelInfoData(ChannelInfo.Reply_Channel2 channelInfo)
        {
            for (int i = 0; i < _parameters.DischargerChannel; i++)
            {
                /// 채널 상태 업데이트
                _dischargerDataArray[i].ErrorCode = channelInfo.ReplyArray[i].ErrorCode;
                _dischargerDataArray[i].DiModuleInfo = channelInfo.ReplyArray[i].DIModuleInfo;
                _dischargerDataArray[i].ReturnCode = channelInfo.ReturnCode;
                _dischargerDataArray[i].ChannelStatus = channelInfo.ReplyArray[i].ChannelStatus;

                /// 전압, 전류, 온도 값 업데이트
                _dischargerDataArray[i].ReceiveBatteryVoltage = channelInfo.ReplyArray[i].BatteryVoltage;
                _dischargerDataArray[i].ReceiveDischargeCurrent = -channelInfo.ReplyArray[i].BatteryCurrent;

                if (!_parameters.DischargerIsTempModule)
                {
                    _dischargerDataArray[i].ReceiveDischargeTemp = channelInfo.ReplyArray[i].AuxTemp1;
                }

                if (_parameters.DischargerIsTempModule == true &&
                    (channelInfo.ReplyArray[i].BatteryVoltage < _dischargerDataArray[i].SafetyVoltageMin ||
                    channelInfo.ReplyArray[i].BatteryVoltage > _dischargerDataArray[i].SafetyVoltageMax ||
                    channelInfo.ReplyArray[i].BatteryCurrent < _dischargerDataArray[i].SafetyCurrentMin ||
                    channelInfo.ReplyArray[i].BatteryCurrent > _dischargerDataArray[i].SafetyCurrentMax))
                {
                    ChangeDischargerState(EDischargerState.SafetyOutOfRange);
                }
                else if (_parameters.DischargerIsTempModule == false &&
                    (channelInfo.ReplyArray[i].BatteryVoltage < _dischargerDataArray[i].SafetyVoltageMin ||
                    channelInfo.ReplyArray[i].BatteryVoltage > _dischargerDataArray[i].SafetyVoltageMax ||
                    channelInfo.ReplyArray[i].BatteryCurrent < _dischargerDataArray[i].SafetyCurrentMin ||
                    channelInfo.ReplyArray[i].BatteryCurrent > _dischargerDataArray[i].SafetyCurrentMax ||
                    channelInfo.ReplyArray[i].AuxTemp1 < _dischargerDataArray[i].SafetyTempMin ||
                    channelInfo.ReplyArray[i].AuxTemp1 > _dischargerDataArray[i].SafetyTempMax))
                {
                    ChangeDischargerState(EDischargerState.SafetyOutOfRange);
                }
                else if (channelInfo.ReplyArray[i].ErrorCode != 0) /// 에러코드 검사
                {
                    ChangeDischargerState(EDischargerState.DeviceError);
                }
                else if (channelInfo.ReturnCode != EReturnCode.Success) /// 리턴 코드 검사
                {
                    ChangeDischargerState(EDischargerState.ReturnCodeError);
                }
                else if (channelInfo.ReplyArray[i].ChannelStatus == EChannelStatus.Error) /// 채널 상태 검사
                {
                    ChangeDischargerState(EDischargerState.ChStatusError);
                }
                else if (channelInfo.ReplyArray[i].ChannelStatus == EChannelStatus.Standby0 ||
                         channelInfo.ReplyArray[i].ChannelStatus == EChannelStatus.Standby5)
                {
                    if (_dischargerState != EDischargerState.Pause)
                    {
                        ChangeDischargerState(EDischargerState.Ready);
                    }
                }
                else
                {
                    ChangeDischargerState(EDischargerState.Discharging);
                }
            }
        }
    }
}
