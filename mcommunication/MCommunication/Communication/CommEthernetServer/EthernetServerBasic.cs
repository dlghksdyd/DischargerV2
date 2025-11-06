using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ethernet.Server.Basic
{
    public struct EthernetHandleBasic
    {
        public int Port;
        public int ClientId;
    }

    public enum EEthernetServerBasicStatus : uint
    {
        EN_ERROR_OK,

        EN_ERROR_SERVER_ALREADY_EXIST,

        EN_ERROR_FAIL_START,
        EN_ERROR_FAIL_GET_CLIENT,

        EN_ERROR_CLIENT_NOT_CONNECTED,
        EN_ERROR_WRITE_FAIL,
        EN_ERROR_READ_FAIL,
    }

    public static class EthernetServerBasic
    {
        private class EthernetServerInstance
        {
            public TcpListener TcpListener = null;
            public IPAddress IpAddress = null;

            /// <summary>
            /// Key: Client ID
            /// </summary>
            public Dictionary<int, EthernetClientInstance> Clients = new Dictionary<int, EthernetClientInstance>();
        }

        private class EthernetClientInstance
        {
            public TcpClient TcpClient { get; set; } = null;
            public Stream Stream { get; set; } = null;
            public object DataLock { get; set; } = null;
        }

        /// <summary>
        /// Key: port number
        /// </summary>
        private static Dictionary<int, EthernetServerInstance> Servers = new Dictionary<int, EthernetServerInstance>();

        private static object CreationLock = new object();

        private static EthernetHandleBasic GetUnusedHandle(int port)
        {
            EthernetHandleBasic handle = new EthernetHandleBasic();
            handle.Port = port;

            for (int i = 0; i < int.MaxValue; i++)
            {
                if (!Servers[port].Clients.ContainsKey(i))
                {
                    /// key 할당을 위해 추가된 코드
                    Servers[port].Clients[i] = null;

                    handle.ClientId = i;
                    break;
                }
            }

            return handle;
        }

        public static bool IsConnected(EthernetHandleBasic handle)
        {
            if (!Servers.ContainsKey(handle.Port))
            {
                return false;
            }

            if (!Servers[handle.Port].Clients.ContainsKey(handle.ClientId))
            {
                return false;
            }

            if (!Servers[handle.Port].Clients[handle.ClientId].TcpClient.Connected)
            {
                return false;
            }

            return true;
        }

        public static EEthernetServerBasicStatus StartServer(IPAddress ipAddress, int port)
        {
            lock (CreationLock)
            {
                if (!Servers.ContainsKey(port))
                {
                    Servers[port] = new EthernetServerInstance();
                    Servers[port].TcpListener = new TcpListener(ipAddress, port);
                    Servers[port].IpAddress = ipAddress;

                    try
                    {
                        Servers[port].TcpListener.Start();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);

                        Servers[port].IpAddress = null;
                        Servers[port].TcpListener.Stop();
                        Servers[port].TcpListener = null;
                        Servers.Remove(port);

                        return EEthernetServerBasicStatus.EN_ERROR_FAIL_START;
                    }
                }
                else
                {
                    return EEthernetServerBasicStatus.EN_ERROR_SERVER_ALREADY_EXIST;
                }

                return EEthernetServerBasicStatus.EN_ERROR_OK;
            }
        }

        public static EEthernetServerBasicStatus WaitForClient(int port, out EthernetHandleBasic handleBasic)
        {
            lock (CreationLock)
            {
                handleBasic = GetUnusedHandle(port);
                Servers[handleBasic.Port].Clients[handleBasic.ClientId] = new EthernetClientInstance();
            }

            try
            {
                EthernetClientInstance client = Servers[handleBasic.Port].Clients[handleBasic.ClientId];

                client.TcpClient = Servers[handleBasic.Port].TcpListener.AcceptTcpClient();
                client.TcpClient.NoDelay = true;
                client.Stream = client.TcpClient.GetStream();
                client.DataLock = new object();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                RemoveClient(handleBasic);

                return EEthernetServerBasicStatus.EN_ERROR_FAIL_GET_CLIENT;
            }

            return EEthernetServerBasicStatus.EN_ERROR_OK;
        }

        public static void StopServer(int port)
        {
            RemoveServer(port);
        }

        private static void RemoveServer(int port)
        {
            lock (CreationLock)
            {
                if (Servers.ContainsKey(port))
                {
                    /// 모든 클라이언트 삭제
                    foreach (var clientDict in Servers[port].Clients)
                    {
                        clientDict.Value.DataLock = null;
                        clientDict.Value.Stream?.Close();
                        clientDict.Value.Stream = null;
                        clientDict.Value.TcpClient?.Close();
                        clientDict.Value.TcpClient = null;
                    }
                    Servers[port].Clients.Clear();

                    /// 서버 삭제
                    Servers[port].IpAddress = IPAddress.None;
                    Servers[port].TcpListener.Stop();
                    Servers[port].TcpListener = null;
                    Servers.Remove(port);
                }

                Thread.Sleep(50);
            }
        }

        public static void RemoveClient(EthernetHandleBasic handle)
        { 
            lock (CreationLock)
            {
                int port = handle.Port;
                int clientId = handle.ClientId;

                if (Servers.ContainsKey(port))
                {
                    if (Servers[port].Clients.ContainsKey(clientId))
                    {
                        var client = Servers[port].Clients[clientId];

                        client.DataLock = null;
                        client?.Stream.Close();
                        client.Stream = null;
                        client?.TcpClient.Close();
                        client.TcpClient = null;

                        Servers[port].Clients.Remove(clientId);
                    }
                }

                Thread.Sleep(50);
            }
        }

        public static EEthernetServerBasicStatus Write(EthernetHandleBasic handle, byte[] data, int offset, int dataLength)
        {
            if (!IsConnected(handle))
            {
                return EEthernetServerBasicStatus.EN_ERROR_CLIENT_NOT_CONNECTED;
            }

            lock (Servers[handle.Port].Clients[handle.ClientId].DataLock)
            {
                try
                {
                    Servers[handle.Port].Clients[handle.ClientId].Stream.Write(data, offset, dataLength);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    RemoveClient(handle);

                    return EEthernetServerBasicStatus.EN_ERROR_WRITE_FAIL;
                }

                return EEthernetServerBasicStatus.EN_ERROR_OK;
            }
        }

        public static EEthernetServerBasicStatus Read(EthernetHandleBasic handle, byte[] data, int offset, int dataLength)
        {
            if (!IsConnected(handle))
            {
                return EEthernetServerBasicStatus.EN_ERROR_CLIENT_NOT_CONNECTED;
            }

            lock (Servers[handle.Port].Clients[handle.ClientId].DataLock)
            {
                try
                {
                    int actualLength = Servers[handle.Port].Clients[handle.ClientId].Stream.Read(data, offset, dataLength);
                    data = data.ResizeArray(actualLength);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    RemoveClient(handle);

                    return EEthernetServerBasicStatus.EN_ERROR_READ_FAIL;
                }

                return EEthernetServerBasicStatus.EN_ERROR_OK;
            }
        }

        public static EEthernetServerBasicStatus FlushReceiveBuffer(EthernetHandleBasic handle)
        {
            if (!IsConnected(handle))
            {
                return EEthernetServerBasicStatus.EN_ERROR_CLIENT_NOT_CONNECTED;
            }

            Servers[handle.Port].Clients[handle.ClientId].Stream.Flush();

            return EEthernetServerBasicStatus.EN_ERROR_OK;
        }

        private static T[] ResizeArray<T>(this T[] oldArray, int length)
        {
            T[] newArray = new T[oldArray.Length];

            Array.Copy(oldArray, newArray, oldArray.Length);

            Array.Resize(ref newArray, length);

            return newArray;
        }
    }
}



