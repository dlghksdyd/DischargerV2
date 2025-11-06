using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bluetooth.Server.Basic;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace Bluetooth.Client.Basic
{
    public enum EBluetoothClientBasicStatus : uint
    {
        BT_ERROR_OK,
        BT_ERROR_CONNECTION_FAIL,
        BT_ERROR_NOT_CONNECTED,
        BT_ERROR_WRITE_FAIL,
        BT_ERROR_READ_FAIL,
    }

    public static class BluetoothClientBasic
    {
        private class BluetoothClientInstance
        {
            public Guid Guid = Guid.Empty;
            public BluetoothClient Client = null;
            public Stream Stream { get; set; } = null;
            public object DataLock { get; set; } = new object();
        }

        private static object CreationLock = new object();

        /// <summary>
        /// Key: Handle Number
        /// </summary>
        private static Dictionary<int, BluetoothClientInstance> ClientInstances = new Dictionary<int, BluetoothClientInstance>();

        private static int GetUnusedHandle()
        {
            /// 사용중이지 않은 핸들 리턴
            int unUsedHandle = 0;
            for (int handle = 0; handle < int.MaxValue; handle++)
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
            try
            {
                if (!ClientInstances.ContainsKey(handle))
                {
                    return false;
                }

                if (ClientInstances[handle].Client == null)
                {
                    return false;
                }

                return ClientInstances[handle].Client.Connected;
            }
            catch
            {
                return false;
            }
        }

        public static EBluetoothClientBasicStatus Connect(BluetoothAddress address, Guid guid, int timeOutMs, out int handle)
        {
            lock (CreationLock)
            {
                handle = GetUnusedHandle();

                /// 클라이언트 인스턴스 생성
                ClientInstances[handle] = new BluetoothClientInstance();

                try
                {
                    BluetoothEndPoint serverEndPoint = new BluetoothEndPoint(address, guid);
                    ClientInstances[handle].Guid = guid;
                    ClientInstances[handle].Client = new BluetoothClient();
                    ClientInstances[handle].Client.Connect(serverEndPoint);
                    ClientInstances[handle].Client.Client.ReceiveTimeout = timeOutMs;
                    ClientInstances[handle].Stream = ClientInstances[handle].Client.GetStream();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);

                    Disconnect(handle);

                    return EBluetoothClientBasicStatus.BT_ERROR_CONNECTION_FAIL;
                }

                return EBluetoothClientBasicStatus.BT_ERROR_OK;
            }
        }

        public static EBluetoothClientBasicStatus Disconnect(int handle)
        {
            lock (CreationLock)
            {
                if (!ClientInstances.ContainsKey(handle))
                {
                    return EBluetoothClientBasicStatus.BT_ERROR_NOT_CONNECTED;
                }

                ClientInstances[handle].Stream?.Close();
                ClientInstances[handle].Stream = null;
                ClientInstances[handle].Client?.Close();
                ClientInstances[handle].Client = null;
                ClientInstances.Remove(handle);

                return EBluetoothClientBasicStatus.BT_ERROR_OK;
            }
        }

        public static List<BluetoothDeviceInfo> DiscoverDevices()
        {
            var btClient = new InTheHand.Net.Sockets.BluetoothClient();
            var btDeviceInfoList = btClient.DiscoverDevices();

            List<BluetoothDeviceInfo> devInfoList = new List<BluetoothDeviceInfo>();

            devInfoList.Clear();
            foreach (var btDeviceInfo in btDeviceInfoList)
            {
                devInfoList.Add(btDeviceInfo);
            }

            return devInfoList;
        }

        public static EBluetoothClientBasicStatus Write(int handle, byte[] data, int offset, int dataLength)
        {
            if (!IsConnected(handle))
            {
                return EBluetoothClientBasicStatus.BT_ERROR_NOT_CONNECTED;
            }

            lock (ClientInstances[handle].DataLock)
            {
                try
                {
                    ClientInstances[handle].Stream.Write(data, offset, dataLength);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);

                    Disconnect(handle);

                    return EBluetoothClientBasicStatus.BT_ERROR_WRITE_FAIL;
                }

                return EBluetoothClientBasicStatus.BT_ERROR_OK;
            }
        }

        public static EBluetoothClientBasicStatus Read(int handle, ref byte[] buffer, int offset, int dataLength)
        {
            if (!IsConnected(handle))
            {
                return EBluetoothClientBasicStatus.BT_ERROR_NOT_CONNECTED;
            }

            lock (ClientInstances[handle].DataLock)
            {
                try
                {
                    int readSize = ClientInstances[handle].Stream.Read(buffer, offset, dataLength);
                    buffer = buffer.ResizeArray(readSize);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);

                    Disconnect(handle);

                    return EBluetoothClientBasicStatus.BT_ERROR_READ_FAIL;
                }

                return EBluetoothClientBasicStatus.BT_ERROR_OK;
            }
        }

        public static EBluetoothClientBasicStatus FlushReceiveBuffer(int handle)
        {
            if (!IsConnected(handle))
            {
                return EBluetoothClientBasicStatus.BT_ERROR_NOT_CONNECTED;
            }

            ClientInstances[handle].Stream.Flush();

            return EBluetoothClientBasicStatus.BT_ERROR_OK;
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
