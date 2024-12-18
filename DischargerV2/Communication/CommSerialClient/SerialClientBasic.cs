using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace Serial.Client.Basic
{
    public class SerialClientBasicConstant
    {
        public const int MAX_READ_BUFFER_SIZE = 2048;
        public const int MAX_WRITE_BUFFER_SIZE = 2048;
    }

    public enum ESerialClientBasicStatus : uint
    {
        SR_ERROR_OK,
        SR_ERROR_CONNECT_FAIL,
        SR_ERROR_PORT_ALREADY_EXIST,
        SR_ERROR_PORT_NOT_OPEN,
        SR_ERROR_WRITE_FAIL,
        SR_ERROR_READ_FAIL,
        SR_ERROR_BUFFER_FLUSH_FAIL,
    }

    public static class SerialClientBasic
    {
        private class SerialClientInstance
        {
            public SerialPort Port = null;
            public object DataLock = new object();
        }

        private static object CreationLock = new object();

        /// <summary>
        /// Key: Comport String (ex. "COM1")
        /// </summary>
        private static Dictionary<string, SerialClientInstance> ClientInstances = new Dictionary<string, SerialClientInstance>();

        public static bool IsConnected(string comPortStr)
        {
            if (ClientInstances.ContainsKey(comPortStr))
            {
                if (ClientInstances[comPortStr].Port != null)
                {
                    if (ClientInstances[comPortStr].Port.IsOpen)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static ESerialClientBasicStatus Connect(string comPortStr, int baudrate, int timeOutMs)
        {
            lock (CreationLock)
            {
                if (ClientInstances.ContainsKey(comPortStr))
                {
                    return ESerialClientBasicStatus.SR_ERROR_PORT_ALREADY_EXIST;
                }

                SerialPort port = new SerialPort();

                port.PortName = comPortStr;
                port.BaudRate = baudrate;
                port.DataBits = 8;
                port.StopBits = StopBits.One;
                port.Parity = Parity.None;
                port.Handshake = Handshake.None;

                port.RtsEnable = true;
                port.DtrEnable = true;

                port.ReadTimeout = timeOutMs;
                port.ReadBufferSize = SerialClientBasicConstant.MAX_READ_BUFFER_SIZE;
                port.WriteBufferSize = SerialClientBasicConstant.MAX_WRITE_BUFFER_SIZE;

                try
                {
                    port.Open();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    port.Close();
                    port.Dispose();

                    return ESerialClientBasicStatus.SR_ERROR_CONNECT_FAIL;
                }

                ClientInstances[comPortStr] = new SerialClientInstance();
                ClientInstances[comPortStr].Port = port;

                return ESerialClientBasicStatus.SR_ERROR_OK;
            }
        }

        public static void Disconnect(string comPortStr)
        {
            RemovePort(comPortStr);
        }

        private static void RemovePort(string comPortStr)
        {
            lock (CreationLock)
            {
                if (ClientInstances.ContainsKey(comPortStr))
                {
                    ClientInstances[comPortStr].Port.Close();
                    ClientInstances[comPortStr].Port.Dispose();
                    ClientInstances[comPortStr].Port = null;

                    ClientInstances[comPortStr].DataLock = null;

                    ClientInstances.Remove(comPortStr);
                }
            }
        }

        public static ESerialClientBasicStatus Write(string comPortStr, string writeData)
        {
            if (!IsConnected(comPortStr))
            {
                return ESerialClientBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (ClientInstances[comPortStr].DataLock)
                {
                    ClientInstances[comPortStr].Port.Write(writeData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return ESerialClientBasicStatus.SR_ERROR_WRITE_FAIL;
            }

            return ESerialClientBasicStatus.SR_ERROR_OK;
        }

        public static ESerialClientBasicStatus Write(string comPortStr, byte[] data, int offset, int length)
        {
            if (!IsConnected(comPortStr))
            {
                return ESerialClientBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (ClientInstances[comPortStr].DataLock)
                {
                    ClientInstances[comPortStr].Port.Write(data, offset, length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return ESerialClientBasicStatus.SR_ERROR_WRITE_FAIL;
            }

            return ESerialClientBasicStatus.SR_ERROR_OK;
        }

        public static ESerialClientBasicStatus ReadTo(string comPortStr, out string data, string breakStr)
        {
            data = null;

            if (!IsConnected(comPortStr))
            {
                return ESerialClientBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (ClientInstances[comPortStr].DataLock)
                {
                    data = ClientInstances[comPortStr].Port.ReadTo(breakStr);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return ESerialClientBasicStatus.SR_ERROR_READ_FAIL;
            }

            return ESerialClientBasicStatus.SR_ERROR_OK;
        }

        public static ESerialClientBasicStatus Read(string comPortStr, ref byte[] data, int offset, int dataLength)
        {
            if (!IsConnected(comPortStr))
            {
                return ESerialClientBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (ClientInstances[comPortStr].DataLock)
                {
                    int readByte = ClientInstances[comPortStr].Port.Read(data, offset, dataLength);
                    data = data.ResizeArray(readByte);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return ESerialClientBasicStatus.SR_ERROR_READ_FAIL;
            }

            return ESerialClientBasicStatus.SR_ERROR_OK;
        }

        public static ESerialClientBasicStatus FlushBuffer(string comPortStr)
        {
            if (!IsConnected(comPortStr))
            {
                return ESerialClientBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                ClientInstances[comPortStr].Port.BaseStream.Flush();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return ESerialClientBasicStatus.SR_ERROR_BUFFER_FLUSH_FAIL;
            }
            
            return ESerialClientBasicStatus.SR_ERROR_OK;
        }
    }
}
