using Ethernet.Client.Basic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Ethernet.Client.Common
{
    public enum EthernetClientStatus : uint
    {
        OK,
        INVALID_START_PARAMETER,

        FAIL_READ,
        FAIL_WRITE,

        ALREADY_CONNECT,
        FAIL_TO_CONNECT,
    }

    public class EthernetClientConstant
    {
        public const int MAX_DATA_LENGTH = 1460;
    }

    public class EthernetClientStart
    {
        public string DeviceName = String.Empty;
        public IPAddress IpAddress = IPAddress.None;
        public int EthernetPort = int.MaxValue;
        public int TimeOutMs = int.MaxValue;
        public EthernetClient.DelegateWrite WriteFunction = null;
        public EthernetClient.DelegateRead ReadFunction = null;
        public EthernetClient.DelegateParse ParseFunction = null;
    }

    public class EthernetClient
    {
        private object PacketLock = new object();

        public delegate bool DelegateWrite(int handle, byte[] writeBuffer);
        public delegate bool DelegateRead(int handle, out byte[] readBuffer);
        public delegate bool DelegateParse(byte[] readBuffer);

        private int Handle = int.MaxValue;

        private EthernetClientStart Parameters = null;

        public EthernetClient()
        {
            /// nothing to do
        }

        private bool IsParameterValid(EthernetClientStart parameters)
        {
            if (parameters == null)
            {
                return false;
            }
            if (parameters.DeviceName == string.Empty ||
                parameters.IpAddress == IPAddress.None ||
                parameters.EthernetPort == int.MaxValue ||
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
            return EthernetClientBasic.IsConnected(Handle);
        }

        public EthernetClientStatus Connect(EthernetClientStart parameters)
        {
            if (!IsParameterValid(parameters))
            {
                return EthernetClientStatus.INVALID_START_PARAMETER;
            }

            Parameters = parameters;

            var result = EthernetClientBasic.Connect(
                parameters.IpAddress, parameters.EthernetPort, parameters.TimeOutMs, out Handle);
            if (result != EthernetClientBasicStatus.EN_ERROR_OK)
            {
                return EthernetClientStatus.FAIL_TO_CONNECT;
            }

            Debug.WriteLine("Connect Ethernet Client.");

            return EthernetClientStatus.OK;
        }

        public void Disconnect()
        {
            EthernetClientBasic.Disconnect(Handle);

            Debug.WriteLine("Disconnect Ethernet Client.");
        }

        public bool ProcessPacket(byte[] _writeBuffer)
        {
            lock (PacketLock)
            {
                byte[] readBuffer = new byte[0];

                bool writeResult = Parameters.WriteFunction.Invoke(Handle, _writeBuffer);
                if (writeResult == false)
                {
                    return false;
                }

                bool readResult = Parameters.ReadFunction.Invoke(Handle, out readBuffer);
                if (readResult == false)
                {
                    return false;
                }

                bool parseResult = Parameters.ParseFunction.Invoke(readBuffer);
                if (parseResult == false)
                {
                    return false;
                }

                return true;
            }
        }

        public EthernetClientStatus Write(int handle, byte[] writeBuffer)
        {
            EthernetClientBasicStatus result = EthernetClientBasic.Write(handle, writeBuffer, 0, writeBuffer.Length);
            if (result != EthernetClientBasicStatus.EN_ERROR_OK)
            {
                return EthernetClientStatus.FAIL_WRITE;
            }

            return EthernetClientStatus.OK;
        }

        public EthernetClientStatus Read(int handle, out byte[] readBuffer)
        {
            readBuffer = new byte[EthernetClientConstant.MAX_DATA_LENGTH];
            EthernetClientBasicStatus result = EthernetClientBasic.Read(handle, ref readBuffer, 0, readBuffer.Length);
            if (result != EthernetClientBasicStatus.EN_ERROR_OK)
            {
                return EthernetClientStatus.FAIL_READ;
            }

            return EthernetClientStatus.OK;
        }

        public bool WriteFunctionExample(int handle, byte[] writeBuffer)
        {
            if (writeBuffer == null || writeBuffer.Length == 0)
            {
                Debug.WriteLine("WriteFunctionExample Write Error: Write Buffer is Empty.");

                return false;
            }

            EthernetClientStatus result = Write(handle, writeBuffer);
            if (result != EthernetClientStatus.OK)
            {
                Debug.WriteLine("WriteFunctionExample Write Error: " + result.ToString());

                return false;
            }

            return true;
        }

        public bool ReadFunctionExample(int handle, out byte[] readBuffer)
        {
            /// 데이터 읽기
            EthernetClientStatus result = Read(handle, out readBuffer);
            if (result != EthernetClientStatus.OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                return false;
            }

            return true;
        }

        public bool ParseFunctionExample(byte[] readBuffer)
        {
            if (readBuffer == null || readBuffer.Length == 0)
            {
                Debug.WriteLine("Read Error: Read Buffer is Empty.");

                return false;
            }

            return true;
        }
    }
}
