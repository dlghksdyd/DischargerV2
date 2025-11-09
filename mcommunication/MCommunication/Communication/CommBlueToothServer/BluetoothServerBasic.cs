using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.Sdp;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility.Common;

namespace Bluetooth.Server.Basic
{
    public enum BTServerBasicStatus : uint
    {
        BT_ERROR_OK,

        BT_ERROR_GUID_ALREADY_USE,
        BT_ERROR_ACCEPT_CLIENT_FAIL,

        BT_ERROR_SERVER_NOT_WORKING,

        BT_ERROR_WRITE_FAIL,
        BT_ERROR_READ_FAIL,
    }

    public static class BluetoothServerBasic
    {
        private class BluetoothServerInstance
        {
            public Guid Guid { get; set; } = Guid.Empty;
            public BluetoothListener Listener { get; set; } = null;
            public BluetoothClient Client { get; set; } = null;
            public Stream Stream { get; set; } = null;
            public object DataLock { get; set; } = new object();
        }

        private static object CreationLock = new object();

        /// <summary>
        /// Key: Handle Number
        /// </summary>
        private static Dictionary<int, BluetoothServerInstance> ServerInstances = new Dictionary<int, BluetoothServerInstance>();

        private static int GetUnusedHandle()
        {
            /// 사용중이지 않은 핸들 리턴
            int unUsedHandle = 0;
            for (int handle = 0; handle < int.MaxValue; handle++)
            {
                if (!ServerInstances.ContainsKey(handle))
                {
                    unUsedHandle = handle;
                    break;
                }
            }
            return unUsedHandle;
        }

        public static bool IsConnected(int handle)
        {
            if (!ServerInstances.ContainsKey(handle))
            {
                return false;
            }

            if (ServerInstances[handle].Client == null)
            {
                return false;
            }

            return ServerInstances[handle].Client.Connected;
        }

        public static BTServerBasicStatus StartServer(Guid guid, out int handle)
        {
            lock (CreationLock)
            {
                /// 핸들 받아오기
                handle = GetUnusedHandle();

                /// 서버 인스턴스 생성
                ServerInstances[handle] = new BluetoothServerInstance();

                /// Guid가 이미 사용중인지 확인
                foreach (var item in ServerInstances)
                {
                    if (item.Value.Guid == guid)
                    {
                        StopServer(handle);
                        return BTServerBasicStatus.BT_ERROR_GUID_ALREADY_USE;
                    }
                }

                /// Guid 할당
                ServerInstances[handle].Guid = guid;
                /// Listener 실행
                ServerInstances[handle].Listener = new BluetoothListener(guid);
                ServerInstances[handle].Listener.Start();
            }

            try
            {
                ServerInstances[handle].Client = ServerInstances[handle].Listener.AcceptBluetoothClient();
                ServerInstances[handle].Stream = ServerInstances[handle].Client.GetStream();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return BTServerBasicStatus.BT_ERROR_ACCEPT_CLIENT_FAIL;
            }

            return BTServerBasicStatus.BT_ERROR_OK;
        }

        public static BTServerBasicStatus StopServer(int handle)
        {
            lock (CreationLock)
            {
                if (!ServerInstances.ContainsKey(handle))
                {
                    return BTServerBasicStatus.BT_ERROR_SERVER_NOT_WORKING;
                }

                ServerInstances[handle].Stream?.Close();
                ServerInstances[handle].Stream = null;
                ServerInstances[handle].Client?.Close();
                ServerInstances[handle].Client = null;
                ServerInstances[handle].Listener?.Stop();
                ServerInstances[handle].Listener = null;
                ServerInstances.Remove(handle);

                return BTServerBasicStatus.BT_ERROR_OK;
            }
        }

        public static BTServerBasicStatus Write(int handle, byte[] data, int offset, int dataLength)
        {
            if (!IsConnected(handle))
            {
                return BTServerBasicStatus.BT_ERROR_SERVER_NOT_WORKING;
            }

            lock (ServerInstances[handle].DataLock)
            {
                try
                {
                    ServerInstances[handle].Stream.Write(data, offset, dataLength);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    StopServer(handle);

                    return BTServerBasicStatus.BT_ERROR_WRITE_FAIL;
                }

                return BTServerBasicStatus.BT_ERROR_OK;
            }
        }

        public static BTServerBasicStatus Read(int handle, ref byte[] buffer, int offset, int dataLength)
        {
            if (!IsConnected(handle))
            {
                return BTServerBasicStatus.BT_ERROR_SERVER_NOT_WORKING;
            }

            lock (ServerInstances[handle].DataLock)
            {
                try
                {
                    int readByte = ServerInstances[handle].Stream.Read(buffer, offset, dataLength);
                    buffer.ResizeArray(readByte);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);

                    StopServer(handle);

                    return BTServerBasicStatus.BT_ERROR_READ_FAIL;
                }

                return BTServerBasicStatus.BT_ERROR_OK;
            }
        }

        public static BTServerBasicStatus FlushReceiveBuffer(int handle)
        {
            if (!IsConnected(handle))
            {
                return BTServerBasicStatus.BT_ERROR_SERVER_NOT_WORKING;
            }

            ServerInstances[handle].Stream.Flush();

            return BTServerBasicStatus.BT_ERROR_OK;
        }
    }
}
