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
        /// <summary>
        /// 수신 받은 데이터
        /// </summary>
        public uint ErrorCode { get; set; } = 0;
        public EReturnCode ReturnCode { get; set; } = EReturnCode.Success;
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
    }

    public class DischargerInfo
    {
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

        private DischargerDatas _dischargerData = new DischargerDatas();
       
        private EDischargerState _dischargerState = EDischargerState.None;
        private uint _dioValue = uint.MaxValue;

        private string _logFileName = string.Empty;

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

        public void SetLogFileName(string logFileName)
        {
            _logFileName = logFileName;
        }

        public bool IsConnected()
        {
            if (_dischargerClient == null)
            {
                return false;
            }

            return _dischargerClient.IsConnected();
        }

        public bool ChangeDischargerState(EDischargerState dischargerState)
        {
            try
            {
                if (dischargerState == EDischargerState.SafetyOutOfRange)
                {
                    _dischargerData.ErrorCode = 0xF0000001;
                }
                else if (dischargerState == EDischargerState.ReturnCodeError)
                {
                    _dischargerData.ErrorCode = 0xF0000002;
                }
                else if (dischargerState == EDischargerState.ChStatusError)
                {
                    _dischargerData.ErrorCode = 0xF0000003;
                }

                if (_dischargerState != dischargerState)
                {
                    // 방전 Trace Log 저장 - 상태 변경
                    var dischargerData = new LogDischarge.DischargerData()
                    {
                        ReceiveBatteryVoltage = _dischargerData.ReceiveBatteryVoltage.ToString("F1"),
                        ReceiveDischargeCurrent = _dischargerData.ReceiveDischargeCurrent.ToString("F1"),
                        ReceiveDischargeTemp = _dischargerData.ReceiveDischargeTemp.ToString("F1"),
                        
                        ErrorCode = _dischargerData.ErrorCode,
                        EReturnCode = _dischargerData.ReturnCode,
                        EDischargerState = dischargerState,
                    };
                    new LogDischarge(ELogDischarge.TRACE_SET_STATE, _logFileName, dischargerData);
                }

                _dischargerState = dischargerState;

                return true;
            }
            catch (Exception ex) 
            {
                // 방전 Trace Log 저장 - 상태 변경 실패
                new LogDischarge(ELogDischarge.ERROR_SET_STATE, _logFileName, ex);

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

            Thread.Sleep(500);

            /// 이전 에러 상태 초기화
            bool clearAlarmResult = SendCommand_ClearAlarm();
            if (clearAlarmResult == false)
            {
                ChangeDischargerState(EDischargerState.Disconnected);
                return false;
            }

            /// 경광등 초기화
            bool lampControlResult = SendCommand_LampControl(EDioControl.TowerLampYellow, false);
            if (lampControlResult == false)
            {
                ChangeDischargerState(EDischargerState.Disconnected);
                return false;
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

        public DischargerDatas GetDatas()
        {
            /// Deep Copy
            DischargerDatas temp = new DischargerDatas();

            try
            {
                /// receive data
                temp.ErrorCode = _dischargerData.ErrorCode;
                temp.ChannelStatus = _dischargerData.ChannelStatus;
                temp.ReceiveBatteryVoltage = double.Parse(_dischargerData.ReceiveBatteryVoltage.ToString("F1"));
                temp.ReceiveDischargeCurrent = double.Parse(_dischargerData.ReceiveDischargeCurrent.ToString("F1"));
                temp.ReceiveDischargeTemp = double.Parse(_dischargerData.ReceiveDischargeTemp.ToString("F1"));

                /// safety
                temp.SafetyCurrentMin = _dischargerData.SafetyCurrentMin;
                temp.SafetyCurrentMax = _dischargerData.SafetyCurrentMax;
                temp.SafetyVoltageMin = _dischargerData.SafetyVoltageMin;
                temp.SafetyVoltageMax = _dischargerData.SafetyVoltageMax;
                temp.SafetyTempMin = _dischargerData.SafetyTempMin;
                temp.SafetyTempMax = _dischargerData.SafetyTempMax;

                /// start time
                temp.DischargingStartTime = _dischargerData.DischargingStartTime;

                return temp;
            }
            catch
            {
                return temp;
            }
        }

        public void SetReceiveTemp(double temp)
        {
            _dischargerData.ReceiveDischargeTemp = temp;
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

        private void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!IsConnected())
            {
                ChangeDischargerState(EDischargerState.Disconnected);
                return;
            }

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

            SendCommand_RequestChannelInfo();

            if (_dischargerState == EDischargerState.Discharging)
            {
                SendCommand_LampControl(EDioControl.TowerLampGreen, false);

                if (_dischargerData.ReceiveBatteryVoltage < 1 && _dischargerData.ReceiveDischargeCurrent < 0.1)
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

        public EDischargerClientError SendCommand_StartDischarge(EWorkMode workMode, double setValue, double limitingValue)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogDischarge.DischargerData();

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
                                _dischargerData.DischargingStartTime = DateTime.Now;
                            }

                            // 방전 Trace Log 저장 - 동작 시작
                            dischargerData = new LogDischarge.DischargerData()
                            {
                                EWorkMode = workMode,
                                SetValue_Voltage = setValue,
                                LimitingValue_Current = limitingValue
                            };
                            new LogDischarge(ELogDischarge.TRACE_START, _logFileName, dischargerData);

                            return EDischargerClientError.Ok;
                        }
                        else
                        {
                            // 방전 Trace Log 저장 - 동작 시작 실패
                            dischargerData = new LogDischarge.DischargerData()
                            {
                                EDischargerClientError = EDischargerClientError.FailProcessPacket,
                                EWorkMode = workMode,
                                SetValue_Voltage = setValue,
                                LimitingValue_Current = limitingValue
                            };
                            new LogDischarge(ELogDischarge.ERROR_START, _logFileName, dischargerData);

                            return EDischargerClientError.FailProcessPacket;
                        }
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 동작 시작 실패
                        dischargerData = new LogDischarge.DischargerData()
                        {
                            EDischargerClientError = EDischargerClientError.InvalidDischargerState,
                            EWorkMode = workMode,
                            SetValue_Voltage = setValue,
                            LimitingValue_Current = limitingValue
                        };
                        new LogDischarge(ELogDischarge.ERROR_START, _logFileName, dischargerData);

                        return EDischargerClientError.InvalidDischargerState;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 동작 시작 실패
                new LogDischarge(ELogDischarge.ERROR_START, _logFileName, ex);

                return EDischargerClientError.Exception;
            }
        }

        public bool SendCommand_SetSafetyCondition(double voltageMax, double voltageMin, double currentMax, double currentMin, double tempMax, double tempMin)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogDischarge.DischargerData();

                    LogArgument logArgument = new LogArgument("Set Safety Condition.");
                    logArgument.Parameters["VoltMax"] = voltageMax;
                    logArgument.Parameters["VoltMin"] = voltageMin;
                    logArgument.Parameters["CurrentMax"] = currentMax;
                    logArgument.Parameters["CurrentMin"] = currentMin;
                    logArgument.Parameters["TempMax"] = tempMax;
                    logArgument.Parameters["TempMin"] = tempMin;

                    byte[] writeBuffer = CreateSetSafetyConditionCommand(
                        _parameters.DischargerChannel,
                        voltageMax + SafetyMarginVoltage, voltageMin - SafetyMarginVoltage,
                        currentMax, currentMin);

                    // 안전 조건 설정
                    bool isOk = _dischargerClient.ProcessPacket(writeBuffer);

                    if (isOk)
                    {
                        _dischargerData.SafetyVoltageMax = voltageMax + SafetyMarginVoltage;
                        _dischargerData.SafetyVoltageMin = voltageMin - SafetyMarginVoltage;
                        _dischargerData.SafetyCurrentMax = currentMax + SafetyMarginCurrent;
                        _dischargerData.SafetyCurrentMin = currentMin - SafetyMarginCurrent;
                        _dischargerData.SafetyTempMax = tempMax;
                        _dischargerData.SafetyTempMin = tempMin;

                        // 방전 Trace Log 저장 - 안전 조건 설정
                        dischargerData = new LogDischarge.DischargerData()
                        {
                            SafetyVoltageMax = voltageMax,
                            SafetyVoltageMin = voltageMin,
                            SafetyCurrentMax = currentMax,
                            SafetyCurrentMin = currentMin,
                            SafetyTempMax = tempMax,
                            SafetyTempMin = tempMin,
                        };
                        new LogDischarge(ELogDischarge.TRACE_SET_SAFETYCONDITION, _logFileName, dischargerData);
                        
                        return true;
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 안전 조건 설정 실패
                        dischargerData = new LogDischarge.DischargerData()
                        {
                            SafetyVoltageMax = voltageMax,
                            SafetyVoltageMin = voltageMin,
                            SafetyCurrentMax = currentMax,
                            SafetyCurrentMin = currentMin,
                            SafetyTempMax = tempMax,
                            SafetyTempMin = tempMin,
                        };
                        new LogDischarge(ELogDischarge.ERROR_SET_SAFETYCONDITION, _logFileName, dischargerData);
                        
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 안전 조건 설정 실패
                new LogDischarge(ELogDischarge.ERROR_SET_SAFETYCONDITION, _logFileName, ex);
                return false;
            }
        }

        public EDischargerClientError SendCommand_StopDischarge()
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogDischarge.DischargerData();

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
                            new LogDischarge(ELogDischarge.TRACE_STOP, _logFileName);
                            
                            return EDischargerClientError.Ok;
                        }
                        else
                        {
                            // 방전 Trace Log 저장 - 동작 정지 실패
                            dischargerData = new LogDischarge.DischargerData()
                            {
                                EDischargerClientError = EDischargerClientError.FailProcessPacket,
                            };
                            new LogDischarge(ELogDischarge.ERROR_STOP, _logFileName, dischargerData);

                            return EDischargerClientError.FailProcessPacket;
                        }
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 동작 정지 실패
                        dischargerData = new LogDischarge.DischargerData()
                        {
                            EDischargerClientError = EDischargerClientError.InvalidDischargerState,
                        };
                        new LogDischarge(ELogDischarge.ERROR_STOP, _logFileName, dischargerData);

                        return EDischargerClientError.InvalidDischargerState;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 동작 정지 실패
                new LogDischarge(ELogDischarge.ERROR_STOP, _logFileName, ex);

                return EDischargerClientError.Exception;
            }
        }

        public EDischargerClientError SendCommand_PauseDischarge()
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogDischarge.DischargerData();

                    if (_dischargerState == EDischargerState.Discharging)
                    {
                        byte[] writeBuffer = CreateStopDischargeCommand(_parameters.DischargerChannel);

                        bool isOk = _dischargerClient.ProcessPacket(writeBuffer);

                        if (isOk)
                        {
                            ChangeDischargerState(EDischargerState.Pause);

                            // 방전 Trace Log 저장 - 동작 일시 정지
                            new LogDischarge(ELogDischarge.TRACE_PAUSE, _logFileName);

                            return EDischargerClientError.Ok;
                        }
                        else
                        {
                            // 방전 Trace Log 저장 - 동작 일시 정지 실패
                            dischargerData = new LogDischarge.DischargerData()
                            {
                                EDischargerClientError = EDischargerClientError.FailProcessPacket,
                            };
                            new LogDischarge(ELogDischarge.ERROR_PAUSE, _logFileName, dischargerData);

                            return EDischargerClientError.FailProcessPacket;
                        }
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 동작 일시 정지 실패
                        dischargerData = new LogDischarge.DischargerData()
                        {
                            EDischargerClientError = EDischargerClientError.InvalidDischargerState,
                        };
                        new LogDischarge(ELogDischarge.ERROR_PAUSE, _logFileName, dischargerData);

                        return EDischargerClientError.InvalidDischargerState;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 동작 일시 정지 실패
                new LogDischarge(ELogDischarge.ERROR_PAUSE, _logFileName, ex);

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
                        new LogDischarge(ELogDischarge.TRACE_CLEAR_ALARM, _logFileName);

                        return true;
                    }
                    else
                    {
                        // 방전 Trace Log 저장 - 에러 해제 실패
                        new LogDischarge(ELogDischarge.ERROR_CLEAR_ALARM, _logFileName);

                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 에러 해제 실패
                new LogDischarge(ELogDischarge.ERROR_CLEAR_ALARM, _logFileName, ex);

                return false;
            }
        }

        public bool SendCommand_LampControl(EDioControl dioControl, bool isBuzzer = false)
        {
            try
            {
                lock (_packetLock)
                {
                    var dischargerData = new LogDischarge.DischargerData();

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
                            dischargerData = new LogDischarge.DischargerData()
                            {
                                LampDioValue = dioValue,
                            };
                            new LogDischarge(ELogDischarge.TRACE_CONTROL_LAMP, _logFileName, dischargerData);
                        }

                        _dioValue = dioValue;

                        return true;
                    }
                    else
                    {
                        if (_dioValue != dioValue)
                        {
                            // 방전 Trace Log 저장 - 경광등 제어 실패
                            dischargerData = new LogDischarge.DischargerData()
                            {
                                LampDioValue = dioValue,
                            };
                            new LogDischarge(ELogDischarge.ERROR_CONTROL_LAMP, _logFileName, dischargerData);
                        }

                        _dioValue = dioValue;

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // 방전 Trace Log 저장 - 경광등 제어 실패
                new LogDischarge(ELogDischarge.ERROR_CONTROL_LAMP, _logFileName, ex);

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
                    short length = (short)Marshal.SizeOf(typeof(ChannelInfo.Reply));
                    dataByteArray = readBuffer.ExtractSubArray(DCCPacketConstant.PACKET_HEADER_SIZE, length);
                    ChannelInfo.Reply channelInfo = dataByteArray.FromByteArrayToPacket<ChannelInfo.Reply>();

                    /// 채널 상태 업데이트
                    _dischargerData.ErrorCode = channelInfo.ErrorCode;
                    _dischargerData.ReturnCode = channelInfo.ReturnCode;
                    _dischargerData.ChannelStatus = channelInfo.ChannelStatus;

                    /// 전압, 전류, 온도 값 업데이트
                    _dischargerData.ReceiveBatteryVoltage = channelInfo.BatteryVoltage;
                    _dischargerData.ReceiveDischargeCurrent = -channelInfo.BatteryCurrent;

                    if (!_parameters.DischargerIsTempModule)
                    {
                        _dischargerData.ReceiveDischargeTemp = channelInfo.AuxTemp1;
                    }

                    if (_parameters.DischargerIsTempModule == true && 
                        (channelInfo.BatteryVoltage < _dischargerData.SafetyVoltageMin ||
                        channelInfo.BatteryVoltage > _dischargerData.SafetyVoltageMax ||
                        channelInfo.BatteryCurrent < _dischargerData.SafetyCurrentMin ||
                        channelInfo.BatteryCurrent > _dischargerData.SafetyCurrentMax))
                    {
                        ChangeDischargerState(EDischargerState.SafetyOutOfRange);
                    }
                    else if (_parameters.DischargerIsTempModule == false && 
                        (channelInfo.BatteryVoltage < _dischargerData.SafetyVoltageMin ||
                        channelInfo.BatteryVoltage > _dischargerData.SafetyVoltageMax ||
                        channelInfo.BatteryCurrent < _dischargerData.SafetyCurrentMin ||
                        channelInfo.BatteryCurrent > _dischargerData.SafetyCurrentMax ||
                        channelInfo.AuxTemp1 < _dischargerData.SafetyTempMin ||
                        channelInfo.AuxTemp1 > _dischargerData.SafetyTempMax))
                    {
                        ChangeDischargerState(EDischargerState.SafetyOutOfRange);
                    }
                    else if (channelInfo.ErrorCode != 0) /// 에러코드 검사
                    {
                        ChangeDischargerState(EDischargerState.DeviceError);
                    }
                    else if (channelInfo.ReturnCode != EReturnCode.Success) /// 리턴 코드 검사
                    {
                        ChangeDischargerState(EDischargerState.ReturnCodeError);
                    }
                    else if (channelInfo.ChannelStatus == EChannelStatus.Error) /// 채널 상태 검사
                    {
                        ChangeDischargerState(EDischargerState.ChStatusError);
                    }
                    else if (channelInfo.ChannelStatus == EChannelStatus.Standby0 || channelInfo.ChannelStatus == EChannelStatus.Standby5)
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

                if (_dischargerState == EDischargerState.Discharging ||
                    _dischargerState == EDischargerState.Pause)
                {
                    // 방전 Trace Log 저장 - 데이터 수신
                    var dischargerData = new LogDischarge.DischargerData()
                    {
                        ReceiveBatteryVoltage = _dischargerData.ReceiveBatteryVoltage.ToString("F1"),
                        ReceiveDischargeCurrent = _dischargerData.ReceiveDischargeCurrent.ToString("F1"),
                        ReceiveDischargeTemp = _dischargerData.ReceiveDischargeTemp.ToString("F1"),

                        ErrorCode = _dischargerData.ErrorCode,
                        EReturnCode = _dischargerData.ReturnCode,
                        EChannelStatus = _dischargerData.ChannelStatus,
                    };
                    new LogDischarge(ELogDischarge.TRACE_GET_DATA, _logFileName, dischargerData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                // 방전 Trace Log 저장 - 데이터 수신 실패
                new LogDischarge(ELogDischarge.ERROR_GET_DATA, _logFileName, readBuffer.GetRawDataHexString());

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
    }
}
