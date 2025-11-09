using Serial.Client.Basic;
using Serial.Client.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Serial.Server.Common
{
    public enum ESerialServerStatus : uint
    {
        OK,

        INVALID_START_PARAMETER,
        PORT_ALREADY_EXIST,
        CONNECT_FAIL,

        FAIL_WRITE,
        FAIL_READ,
    }

    public class SerialServerStart
    {
        public string DeviceName = string.Empty;
        public string ComPort = string.Empty;
        public int BaudRate = int.MaxValue;
        public int TimeOutMs = int.MaxValue;
        public Encoding Encoding = Encoding.UTF8;
        public SerialDataReceivedEventHandler DataReceivedEventHandler = null;
    }

    public class SerialServer
    {
        private SerialServerStart Parameters = null;

        private bool IsParameterValid(SerialServerStart parameters)
        {
            if (parameters == null)
            {
                return false;
            }
            if (parameters.DeviceName == string.Empty ||
                parameters.ComPort == string.Empty ||
                parameters.BaudRate == int.MaxValue ||
                parameters.TimeOutMs == int.MaxValue ||
                parameters.DataReceivedEventHandler == null)
            {
                return false;
            }

            return true;
        }

        public bool IsConnected()
        {
            if (Parameters == null)
            {
                return false;
            }

            if (SerialServerBasic.IsConnected(Parameters.ComPort))
            {
                return true;
            }
            return false;
        }

        public ESerialServerStatus Start(SerialServerStart parameters)
        {
            /// 이미 실행 중이라면 바로 리턴
            if (IsConnected())
            {
                return ESerialServerStatus.PORT_ALREADY_EXIST;
            }

            if (!IsParameterValid(parameters))
            {
                return ESerialServerStatus.INVALID_START_PARAMETER;
            }

            Parameters = parameters;

            var result = SerialServerBasic.Start(Parameters.ComPort, Parameters.BaudRate, 
                Parameters.TimeOutMs, Parameters.DataReceivedEventHandler);
            if (result != ESerialServerBasicStatus.SR_ERROR_OK)
            {
                return ESerialServerStatus.CONNECT_FAIL;
            }

            return ESerialServerStatus.OK;
        }

        public void Stop()
        {
            if (Parameters != null)
            {
                SerialServerBasic.Stop(Parameters.ComPort);
            }
        }

        public ESerialServerStatus Write(string comPortStr, string writeStr)
        {
            ESerialServerBasicStatus result = SerialServerBasic.Write(comPortStr, writeStr);
            if (result != ESerialServerBasicStatus.SR_ERROR_OK)
            {
                return ESerialServerStatus.FAIL_WRITE;
            }

            return ESerialServerStatus.OK;
        }

        public ESerialServerStatus Read(string comPortStr, out string readStr, string breakStr)
        {
            if (breakStr == null)
            {
                byte[] readBuffer = new byte[SerialClientConstant.MAX_READ_BUFFER_SIZE];

                ESerialServerBasicStatus result = SerialServerBasic.Read(comPortStr, ref readBuffer, 0, readBuffer.Length);
                if (result != ESerialServerBasicStatus.SR_ERROR_OK)
                {
                    readStr = null;
                    return ESerialServerStatus.FAIL_READ;
                }

                readStr = ConvertByteArrayToString(readBuffer, Parameters.Encoding);
            }
            else
            {
                ESerialServerBasicStatus result = SerialServerBasic.ReadTo(comPortStr, out readStr, breakStr);
                if (result != ESerialServerBasicStatus.SR_ERROR_OK)
                {
                    readStr = null;
                    return ESerialServerStatus.FAIL_READ;
                }
            }

            return ESerialServerStatus.OK;
        }

        public bool WriteFunctionExample(string comPortStr, string writeStr)
        {
            if (writeStr == null || writeStr.Length == 0)
            {
                Debug.WriteLine("Write Error: Write String is Empty.");

                return false;
            }

            ESerialServerStatus result = Write(comPortStr, writeStr);
            if (result != ESerialServerStatus.OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                return false;
            }

            return true;
        }

        public bool ReadFunctionExample(string comPortStr, out string readStr, string breakStr)
        {
            ESerialServerStatus result = Read(comPortStr, out readStr, breakStr);
            if (result != ESerialServerStatus.OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                return false;
            }

            return true;
        }

        public bool ParseFunctionExample(string readStr)
        {
            if (readStr == null || readStr.Length == 0)
            {
                Debug.WriteLine("Read Error: Read String is Empty.");

                return false;
            }

            return true;
        }

        private string ConvertByteArrayToString(byte[] byteArray, Encoding encoding)
        {
            return encoding.GetString(byteArray).Replace("\0", String.Empty);
        }
    }
}
