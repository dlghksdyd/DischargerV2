using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial.Server.Common
{
    public class SerialServerBasicConstant
    {
        public const int MAX_READ_BUFFER_SIZE = 2048;
        public const int MAX_WRITE_BUFFER_SIZE = 2048;
    }

    public enum ESerialServerBasicStatus : uint
    {
        SR_ERROR_OK,
        SR_ERROR_CONNECT_FAIL,
        SR_ERROR_PORT_ALREADY_EXIST,
        SR_ERROR_PORT_NOT_EXIST,
        SR_ERROR_PORT_NOT_OPEN,
        SR_ERROR_WRITE_FAIL,
        SR_ERROR_READ_FAIL,
    }

    public static class SerialServerBasic
    {
        private static object CreationLock = new object();

        /// <summary>
        /// Key: Comport String
        /// </summary>
        private static Dictionary<string, SerialPort> Ports = new Dictionary<string, SerialPort>();
        /// <summary>
        /// Key: Comport String
        /// </summary>
        private static Dictionary<string, object> DataLocks = new Dictionary<string, object>();

        public static bool IsConnected(string comPortStr)
        {
            if (Ports.ContainsKey(comPortStr))
            {
                if (Ports[comPortStr].IsOpen)
                {
                    return true;
                }
            }

            return false;
        }

        public static ESerialServerBasicStatus Start(string comPortStr, int baudrate, int timeOutMs, SerialDataReceivedEventHandler serialDataReceivedEventHandler)
        {
            if (Ports.ContainsKey(comPortStr))
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_ALREADY_EXIST;
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
            port.ReadBufferSize = SerialServerBasicConstant.MAX_READ_BUFFER_SIZE;
            port.WriteBufferSize = SerialServerBasicConstant.MAX_WRITE_BUFFER_SIZE;

            port.DataReceived += new SerialDataReceivedEventHandler(serialDataReceivedEventHandler);

            try
            {
                port.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                port.Close();
                port.Dispose();

                return ESerialServerBasicStatus.SR_ERROR_CONNECT_FAIL;
            }

            Ports[comPortStr] = port;
            DataLocks[comPortStr] = new object();            

            return ESerialServerBasicStatus.SR_ERROR_OK;
        }

        public static void Stop(string comPortStr)
        {
            RemovePort(comPortStr);
        }

        private static void RemovePort(string comPortStr)
        {
            lock (CreationLock)
            {
                if (Ports.ContainsKey(comPortStr))
                {
                    Ports[comPortStr].Close();
                    Ports[comPortStr].Dispose();
                    Ports.Remove(comPortStr);

                    DataLocks.Remove(comPortStr);
                }
            }
        }

        public static ESerialServerBasicStatus Write(string comPortStr, string writeData)
        {
            if (!Ports.ContainsKey(comPortStr))
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (DataLocks[comPortStr])
                {
                    Ports[comPortStr].Write(writeData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return ESerialServerBasicStatus.SR_ERROR_WRITE_FAIL;
            }

            return ESerialServerBasicStatus.SR_ERROR_OK;
        }

        public static ESerialServerBasicStatus Write(string comPortStr, byte[] buffer, int offset, int length)
        {
            if (!Ports.ContainsKey(comPortStr))
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (DataLocks[comPortStr])
                {
                    Ports[comPortStr].Write(buffer, offset, length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return ESerialServerBasicStatus.SR_ERROR_WRITE_FAIL;
            }

            return ESerialServerBasicStatus.SR_ERROR_OK;
        }

        public static ESerialServerBasicStatus ReadTo(string comPortStr, out string readStr, string breakStr)
        {
            readStr = null;

            if (!Ports.ContainsKey(comPortStr))
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (DataLocks[comPortStr])
                {
                    readStr = Ports[comPortStr].ReadTo(breakStr);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return ESerialServerBasicStatus.SR_ERROR_READ_FAIL;
            }

            return ESerialServerBasicStatus.SR_ERROR_OK;
        }

        public static ESerialServerBasicStatus Read(string comPortStr, ref byte[] buffer, int offset, int dataLength)
        {
            if (!Ports.ContainsKey(comPortStr))
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (DataLocks[comPortStr])
                {
                    int readByte = Ports[comPortStr].Read(buffer, offset, dataLength);
                    buffer = buffer.ResizeArray(readByte);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return ESerialServerBasicStatus.SR_ERROR_READ_FAIL;
            }

            return ESerialServerBasicStatus.SR_ERROR_OK;
        }

        public static ESerialServerBasicStatus FlushBuffer(string comPortStr)
        {
            if (!Ports.ContainsKey(comPortStr))
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return ESerialServerBasicStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            Ports[comPortStr].BaseStream.Flush();

            return ESerialServerBasicStatus.SR_ERROR_OK;
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
