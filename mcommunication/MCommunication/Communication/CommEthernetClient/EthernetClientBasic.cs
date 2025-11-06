using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace Ethernet.Client.Basic
{
    public enum EthernetClientBasicStatus : uint
    {
        EN_ERROR_OK,

        EN_ERROR_CONNECT_FAIL,

        EN_ERROR_CLIENT_NOT_EXIST,

        EN_ERROR_CLIENT_NOT_CONNECTED,

        EN_ERROR_WRITE_FAIL,
        EN_ERROR_READ_FAIL,
    }

    public static class EthernetClientBasic
    {
        private class EthernetClientInstance
        {
            public TcpClient Client { get; set; } = null;
            public Stream Stream { get; set; } = null;
            public object DataLock { get; set; } = null;
        }

        private static object CreationLock = new object();

        /// <summary>
        /// Key: handle.
        /// </summary>
        private static Dictionary<int, EthernetClientInstance> ClientInstances = new Dictionary<int, EthernetClientInstance>();

        private static int GetUnusedHandle()
        {
            int unUsedHandle = 0;
            for (int handle = 0; handle < Int32.MaxValue; handle++)
            {
                if (!ClientInstances.ContainsKey(handle))
                {
                    unUsedHandle = handle;
                    break;
                }
            }

            return unUsedHandle;
        }

        public static bool IsConnected(int handle)
        {
            if (!ClientInstances.ContainsKey(handle))
            {
                return false;
            }

            return ClientInstances[handle].Client.Connected;
        }

        public static EthernetClientBasicStatus Disconnect(int handle)
        {
            lock (CreationLock)
            {
                if (!ClientInstances.ContainsKey(handle))
                {
                    return EthernetClientBasicStatus.EN_ERROR_CLIENT_NOT_EXIST;
                }

                ClientInstances[handle].Stream?.Close();
                ClientInstances[handle].Client?.Close();
                ClientInstances[handle].DataLock = null;
                ClientInstances.Remove(handle);

                return EthernetClientBasicStatus.EN_ERROR_OK;
            }
        }

        public static EthernetClientBasicStatus Connect(IPAddress serverIpAddress, int port, int timeOutMs, out int handle)
        {
            lock (CreationLock)
            {
                handle = GetUnusedHandle();

                ClientInstances[handle] = new EthernetClientInstance();
                ClientInstances[handle].Client = new TcpClient();

                try
                {
                    ClientInstances[handle].Client.Connect(serverIpAddress.ToString(), port);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);

                    Disconnect(handle);

                    return EthernetClientBasicStatus.EN_ERROR_CONNECT_FAIL;
                }

                ClientInstances[handle].Client.ReceiveTimeout = timeOutMs;
                ClientInstances[handle].Stream = ClientInstances[handle].Client.GetStream();
                ClientInstances[handle].DataLock = new object();

                return EthernetClientBasicStatus.EN_ERROR_OK;
            }
        }

        public static EthernetClientBasicStatus Write(int handle, byte[] data, int offset, int dataLength)
        {
            if (!IsConnected(handle))
            {
                return EthernetClientBasicStatus.EN_ERROR_CLIENT_NOT_CONNECTED;
            }

            lock (ClientInstances[handle].DataLock)
            {
                try
                {
                    ClientInstances[handle].Stream.Write(data, offset, dataLength);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);

                    Disconnect(handle);

                    return EthernetClientBasicStatus.EN_ERROR_WRITE_FAIL;
                }

                return EthernetClientBasicStatus.EN_ERROR_OK;
            }
        }

        public static EthernetClientBasicStatus Read(int handle, ref byte[] data, int offset, int dataLength)
        {
            if (!IsConnected(handle))
            {
                return EthernetClientBasicStatus.EN_ERROR_CLIENT_NOT_CONNECTED;
            }

            lock (ClientInstances[handle].DataLock)
            {
                try
                {
                    int readByte = ClientInstances[handle].Stream.Read(data, offset, dataLength);
                    data = data.ResizeArray(readByte);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);

                    Disconnect(handle);

                    return EthernetClientBasicStatus.EN_ERROR_READ_FAIL;
                }

                return EthernetClientBasicStatus.EN_ERROR_OK;
            }
        }

        public static EthernetClientBasicStatus FlushReceiveBuffer(int handle)
        {
            if (!IsConnected(handle))
            {
                return EthernetClientBasicStatus.EN_ERROR_CLIENT_NOT_CONNECTED;
            }

            ClientInstances[handle].Stream.Flush();

            return EthernetClientBasicStatus.EN_ERROR_OK;
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
