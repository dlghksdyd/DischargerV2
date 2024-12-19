using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Serial.Client.Basic;

namespace Serial.Client.Common
{
    public class SerialClientConstant
    {
        public const int MAX_READ_BUFFER_SIZE = SerialClientBasicConstant.MAX_READ_BUFFER_SIZE;
        public const int MAX_WRITE_BUFFER_SIZE = SerialClientBasicConstant.MAX_WRITE_BUFFER_SIZE;
    }

    public enum ESerialClientStatus : uint
    {
        OK,

        INVALID_START_PARAMETER,
        PORT_ALREADY_EXIST,
        CONNECT_FAIL,

        FAIL_WRITE,
        FAIL_READ,
    }

    public class SerialClientStart
    {
        public string DeviceName = string.Empty;
        public string ComPortStr = string.Empty;
        public int BaudRate = int.MaxValue;
        public int TimeOutMs = int.MaxValue;
        public Encoding Encoding = Encoding.UTF8;

        public SerialClient.DelegateWrite WriteFunction = null;
        public SerialClient.DelegateRead ReadFunction = null;
        public SerialClient.DelegateParse ParseFunction = null;
    }

    public class SerialClient
    {
        private static object PacketLock = new object();

        public delegate bool DelegateWrite(string comPortStr, string writeStr);
        public delegate bool DelegateRead(string comPortStr, out string readStr, string breakStr);
        public delegate bool DelegateParse(string readStr);

        private SerialClientStart Parameters = null;

        private bool IsParameterValid(SerialClientStart parameters)
        {
            if (parameters == null)
            {
                return false;
            }
            if (parameters.DeviceName == string.Empty ||
                parameters.ComPortStr == string.Empty ||
                parameters.BaudRate == int.MaxValue ||
                parameters.TimeOutMs == int.MaxValue ||
                parameters.ReadFunction == null ||
                parameters.ParseFunction == null ||
                parameters.WriteFunction == null)
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

            if (SerialClientBasic.IsConnected(Parameters.ComPortStr))
            {
                return true;
            }
            return false;
        }

        public ESerialClientStatus Connect(SerialClientStart parameters)
        {
            if (!IsParameterValid(parameters))
            {
                return ESerialClientStatus.INVALID_START_PARAMETER;
            }

            Parameters = parameters;

            var result = SerialClientBasic.Connect(Parameters.ComPortStr, Parameters.BaudRate, Parameters.TimeOutMs);
            if (result == ESerialClientBasicStatus.SR_ERROR_PORT_ALREADY_EXIST)
            {
                return ESerialClientStatus.PORT_ALREADY_EXIST;
            }
            else if (result == ESerialClientBasicStatus.SR_ERROR_CONNECT_FAIL)
            {
                return ESerialClientStatus.CONNECT_FAIL;
            }

            Debug.WriteLine("Connect Serial Client.");

            return ESerialClientStatus.OK;
        }

        public void Disconnect()
        {
            SerialClientBasic.Disconnect(Parameters.ComPortStr);

            Debug.WriteLine("Disconnect Serial Client.");
        }

        public bool ProcessPacket(string writeString, string breakStr = null)
        {
            lock (PacketLock)
            {
                bool writeResult = Parameters.WriteFunction.Invoke(Parameters.ComPortStr, writeString);
                if (writeResult == false)
                {
                    return false;
                }

                bool readResult = Parameters.ReadFunction.Invoke(Parameters.ComPortStr, out string readStr, breakStr);
                if (readResult == false)
                {
                    return false;
                }

                bool parseResult = Parameters.ParseFunction.Invoke(readStr);
                if (parseResult == false)
                {
                    return false;
                }

                return true;
            }
        }

        public ESerialClientStatus Write(string comPortStr, string writeStr)
        {
            ESerialClientBasicStatus result = SerialClientBasic.Write(comPortStr, writeStr);
            if (result != ESerialClientBasicStatus.SR_ERROR_OK)
            {
                return ESerialClientStatus.FAIL_WRITE;
            }

            return ESerialClientStatus.OK;
        }

        public ESerialClientStatus Read(string comPortStr, out string readStr, string breakStr)
        {
            if (breakStr == null)
            {
                byte[] readBuffer = new byte[SerialClientConstant.MAX_READ_BUFFER_SIZE];

                ESerialClientBasicStatus result = SerialClientBasic.Read(comPortStr, ref readBuffer, 0, readBuffer.Length);
                if (result != ESerialClientBasicStatus.SR_ERROR_OK)
                {
                    readStr = null;
                    return ESerialClientStatus.FAIL_READ;
                }

                readStr = ConvertByteArrayToString(readBuffer, Parameters.Encoding);
            }
            else
            {
                ESerialClientBasicStatus result = SerialClientBasic.ReadTo(comPortStr, out readStr, breakStr);
                if (result != ESerialClientBasicStatus.SR_ERROR_OK)
                {
                    readStr = null;
                    return ESerialClientStatus.FAIL_READ;
                }
            }

            return ESerialClientStatus.OK;
        }

        public bool WriteFunctionExample(string comPortStr, string writeStr)
        {
            if (writeStr == null || writeStr.Length == 0)
            {
                Debug.WriteLine("Write Error: Write String is Empty.");

                return false;
            }

            ESerialClientStatus result = Write(comPortStr, writeStr);
            if (result != ESerialClientStatus.OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                return false;
            }

            return true;
        }

        public bool ReadFunctionExample(string comPortStr, out string readStr, string breakStr)
        {
            ESerialClientStatus result = Read(comPortStr, out readStr, breakStr);
            if (result != ESerialClientStatus.OK) 
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
