using Ethernet.Server.Basic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ethernet.Server.Common
{
    public struct EthernetHandle
    {
        public int Port;
        public int ClientId;
    }

    public enum EEthernetServerStatus : uint
    {
        OK,
        INVALID_START_PARAMETER,
        FAIL_START_SERVER,
        FAIL_WRITE,
        FAIL_READ,
    }

    public class EthernetServerConstant
    {
        public const int MAX_DATA_LENGTH = 1380;
    }

    public class EthernetServerStart
    {
        public IPAddress IpAddress = IPAddress.None;
        public int EthernetPort = int.MaxValue;

        public EthernetServer.DelegateRead ReadFunction = null;
        public EthernetServer.DelegateParse ParseFunction = null;
        public EthernetServer.DelegateWrite WriteFunction = null;
    }

    public static class EthernetServerUtility
    {
        public static EthernetHandleBasic ToHandleBasic(this EthernetHandle handle)
        {
            return new EthernetHandleBasic() { Port = handle.Port, ClientId = handle.ClientId };
        }

        public static EthernetHandle ToHandle(this EthernetHandleBasic handleBasic)
        {
            return new EthernetHandle() { Port = handleBasic.Port, ClientId = handleBasic.ClientId };
        }
    }

    public class EthernetServer
    {
        public delegate bool DelegateRead(EthernetHandle handle, out byte[] readBuffer);
        public delegate bool DelegateParse(EthernetHandle handle, byte[] readBuffer, out byte[] writeBuffer);
        public delegate bool DelegateWrite(EthernetHandle handle, byte[] writeBuffer);

        private Thread ClientWaitThread = null;

        /// <summary>
        /// Key: Client ID
        /// </summary>
        private Dictionary<int, Thread> CommunicationThreads = new Dictionary<int, Thread>();

        private EthernetServerStart Parameters = null;

        public EthernetServer()
        {
            /// nothing to do
        }

        private bool IsParameterValid(EthernetServerStart parameters)
        {
            if (parameters == null)
            {
                return false;
            }
            if (parameters.IpAddress == IPAddress.None ||
                parameters.EthernetPort == int.MaxValue ||
                parameters.ReadFunction == null ||
                parameters.ParseFunction == null ||
                parameters.WriteFunction == null)
            {
                return false;
            }

            return true;
        }

        public EEthernetServerStatus Start(EthernetServerStart parameters)
        {
            if (!IsParameterValid(parameters))
            {
                return EEthernetServerStatus.INVALID_START_PARAMETER;
            }

            Parameters = parameters;

            EEthernetServerBasicStatus result = EthernetServerBasic.StartServer(
                Parameters.IpAddress, Parameters.EthernetPort);
            if (result != EEthernetServerBasicStatus.EN_ERROR_OK)
            {
                Debug.WriteLine("서버 시작 실패.");

                return EEthernetServerStatus.FAIL_START_SERVER;
            }

            /// 클라이언트 대기 쓰레드 시작
            ClientWaitThread = new Thread(WaitForClients);
            ClientWaitThread.Start();

            Debug.WriteLine("이더넷 서버 시작 완료");

            return EEthernetServerStatus.OK;
        }

        private void RemoveAllThread()
        {
            /// 커뮤니케이션 쓰레드 모두 종료
            foreach (var oneThread in CommunicationThreads)
            {
                oneThread.Value.Abort();
            }
            CommunicationThreads.Clear();

            /// 클라이언트 대기 쓰레드 종료
            if (ClientWaitThread != null)
            {
                ClientWaitThread.Interrupt();
                ClientWaitThread = null;
                Thread.Sleep(50);
            }
        }

        public void ChangeReadFunction(DelegateRead readFunction)
        {
            if (readFunction != null)
            {
                Parameters.ReadFunction = readFunction;
            }
        }

        public void ChangeParseFunction(DelegateParse parseFunction)
        {
            if (parseFunction != null)
            {
                Parameters.ParseFunction = parseFunction;
            }
        }

        public void ChangeWriteFunction(DelegateWrite writeFunction)
        {
            if (writeFunction != null)
            {
                Parameters.WriteFunction = writeFunction;
            }
        }

        public void Stop()
        {
            RemoveAllThread();

            EthernetServerBasic.StopServer(Parameters.EthernetPort);

            Debug.WriteLine("이더넷 서버 중지.");
        }

        public bool IsConnected(EthernetHandle handle)
        {
            return EthernetServerBasic.IsConnected(handle.ToHandleBasic());
        }

        private void WaitForClients()
        {
            while (true)
            {
                EEthernetServerBasicStatus result = EthernetServerBasic.WaitForClient(
                    Parameters.EthernetPort, out EthernetHandleBasic handleBasic);
                if (result != EEthernetServerBasicStatus.EN_ERROR_OK)
                {
                    Debug.WriteLine("에러 발생으로 인한 서버 중지.");
                    break;
                }

                Debug.WriteLine("클라이언트 연결");

                CommunicationThreads[handleBasic.ClientId] = new Thread(ProcessCommunication);
                CommunicationThreads[handleBasic.ClientId].Start(handleBasic);
            }
        }

        private void ProcessCommunication(object inputHandleBasic)
        {
            byte[] writeBuffer = new byte[0];
            EthernetHandleBasic handleBasic = (EthernetHandleBasic)inputHandleBasic;
            EthernetHandle handle = handleBasic.ToHandle();

            while (true)
            {
                bool readResult = Parameters.ReadFunction.Invoke(handle, out byte[] readBuffer);
                if (readResult == false)
                {
                    EthernetServerBasic.RemoveClient(handleBasic);
                    break;
                }

                bool parseResult = Parameters.ParseFunction.Invoke(handle, readBuffer, out writeBuffer);
                if (parseResult == false)
                {
                    EthernetServerBasic.RemoveClient(handleBasic);
                    break;
                }

                bool writeResult = Parameters.WriteFunction.Invoke(handle, writeBuffer);
                if (writeResult == false)
                {
                    EthernetServerBasic.RemoveClient(handleBasic);
                    break;
                }
            }
        }

        public EEthernetServerStatus Write(EthernetHandle handle, byte[] writeBuffer)
        {
            EthernetHandleBasic handleBasic = handle.ToHandleBasic();

            EEthernetServerBasicStatus result = EthernetServerBasic.Write(handleBasic, writeBuffer, 0, writeBuffer.Length);
            if (result != EEthernetServerBasicStatus.EN_ERROR_OK)
            {
                return EEthernetServerStatus.FAIL_WRITE;
            }

            return EEthernetServerStatus.OK;
        }

        public EEthernetServerStatus Read(EthernetHandle handle, out byte[] readBuffer)
        {
            EthernetHandleBasic handleBasic = handle.ToHandleBasic();

            readBuffer = new byte[EthernetServerConstant.MAX_DATA_LENGTH];
            EEthernetServerBasicStatus result = EthernetServerBasic.Read(handleBasic, readBuffer, 0, readBuffer.Length);
            if (result != EEthernetServerBasicStatus.EN_ERROR_OK)
            {
                return EEthernetServerStatus.FAIL_READ;
            }

            return EEthernetServerStatus.OK;
        }

        public bool ReadFunctionExample(EthernetHandle handle, out byte[] readBuffer)
        {
            /// 데이터 읽기
            EEthernetServerStatus result = this.Read(handle, out readBuffer);
            if (result != EEthernetServerStatus.OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                return false;
            }

            return true;
        }

        public bool ParseFunctionExample(EthernetHandle handle, byte[] readBuffer, out byte[] writeBuffer)
        {
            writeBuffer = readBuffer.ToArray();

            return true;
        }

        public bool WriteFunctionExample(EthernetHandle handle, byte[] writeBuffer)
        {
            if (writeBuffer == null || writeBuffer.Length == 0)
            {
                return true;
            }

            EEthernetServerStatus result = this.Write(handle, writeBuffer);
            if (result != EEthernetServerStatus.OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                return false;
            }

            return true;
        }
    }
}


