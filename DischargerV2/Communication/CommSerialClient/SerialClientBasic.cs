using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial.Client.Common
{
    public class SRClientConstant
    {
        public const int MAX_READ_BUFFER_SIZE = 2048;
        public const int MAX_WRITE_BUFFER_SIZE = 2048;
    }

    public enum SRClientStatus : uint
    {
        SR_ERROR_OK,
        SR_ERROR_CONNECT_FAIL,
        SR_ERROR_PORT_ALREADY_EXIST,
        SR_ERROR_PORT_NOT_EXIST,
        SR_ERROR_PORT_NOT_OPEN,
        SR_ERROR_WRITE_FAIL,
        SR_ERROR_READ_FAIL,
    }

    public static class SerialClientBasic
    {
        private static object RemoveLock = new object();
        private static object ErrorLock = new object();

        /// <summary>
        /// Key: Comport String
        /// </summary>
        private static Dictionary<string, SerialPort> Ports = new Dictionary<string, SerialPort>();
        /// <summary>
        /// Key: Comport String
        /// </summary>
        private static Dictionary<string, object> RWLock = new Dictionary<string, object>();

        public static bool IsConnect(string comPortStr)
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

        public static SRClientStatus Connect(string comPortStr, int baudrate)
        {
            lock (RemoveLock)
            {
                if (Ports.ContainsKey(comPortStr))
                {
                    return SRClientStatus.SR_ERROR_PORT_ALREADY_EXIST;
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

                port.ReadTimeout = 3000;
                port.ReadBufferSize = SRClientConstant.MAX_READ_BUFFER_SIZE;
                port.WriteBufferSize = SRClientConstant.MAX_WRITE_BUFFER_SIZE;

                try
                {
                    port.Open();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    port.Close();
                    port.Dispose();

                    return SRClientStatus.SR_ERROR_CONNECT_FAIL;
                }

                port.ErrorReceived += new SerialErrorReceivedEventHandler(EventErrorReceived);

                Ports[comPortStr] = port;
                RWLock[comPortStr] = new object();
            }

            return SRClientStatus.SR_ERROR_OK;
        }

        public static void Disconnect(string comPortStr)
        {
            RemovePort(comPortStr);
        }

        private static void RemovePort(string comPortStr)
        {
            lock (RemoveLock)
            {
                if (Ports.ContainsKey(comPortStr))
                {
                    Ports[comPortStr].Close();
                    Ports[comPortStr].Dispose();
                    Ports.Remove(comPortStr);

                    RWLock.Remove(comPortStr);
                }
            }
        }

        public static SRClientStatus Write(string comPortStr, string writeData)
        {
            if (!Ports.ContainsKey(comPortStr))
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (RWLock[comPortStr])
                {
                    Ports[comPortStr].Write(writeData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return SRClientStatus.SR_ERROR_WRITE_FAIL;
            }

            return SRClientStatus.SR_ERROR_OK;
        }

        public static SRClientStatus Write(string comPortStr, byte[] buffer, int offset, int length)
        {
            if (!Ports.ContainsKey(comPortStr))
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (RWLock[comPortStr])
                {
                    Ports[comPortStr].Write(buffer, offset, length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return SRClientStatus.SR_ERROR_WRITE_FAIL;
            }

            return SRClientStatus.SR_ERROR_OK;
        }

        public static SRClientStatus ReadTo(string comPortStr, string breakStr, out string readStr)
        {
            readStr = "";

            if (!Ports.ContainsKey(comPortStr))
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (RWLock[comPortStr])
                {
                    readStr = Ports[comPortStr].ReadTo(breakStr);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return SRClientStatus.SR_ERROR_READ_FAIL;
            }

            return SRClientStatus.SR_ERROR_OK;
        }

        public static SRClientStatus Read(string comPortStr, byte[] buffer, int offset, int dataLength)
        {
            if (!Ports.ContainsKey(comPortStr))
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            try
            {
                lock (RWLock[comPortStr])
                {
                    Ports[comPortStr].Read(buffer, offset, dataLength);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);

                RemovePort(comPortStr);

                return SRClientStatus.SR_ERROR_READ_FAIL;
            }

            return SRClientStatus.SR_ERROR_OK;
        }

        public static SRClientStatus FlushBuffer(string comPortStr)
        {
            if (!Ports.ContainsKey(comPortStr))
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_EXIST;
            }

            if (Ports[comPortStr].IsOpen == false)
            {
                return SRClientStatus.SR_ERROR_PORT_NOT_OPEN;
            }

            Ports[comPortStr].BaseStream.Flush();

            return SRClientStatus.SR_ERROR_OK;
        }

        private static void EventErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            lock (ErrorLock)
            {
                SerialError err = e.EventType;
                switch (err)
                {
                    case SerialError.Frame: Debug.WriteLine("[Serial Error] Hardware framing Error"); break;
                    case SerialError.Overrun: Debug.WriteLine("[Serial Error] Characters buffer over run"); break;
                    case SerialError.RXOver: Debug.WriteLine("[Serial Error] Input buffer overrun"); break;
                    case SerialError.RXParity: Debug.WriteLine("[Serial Error] Parity Error"); break;
                    case SerialError.TXFull: Debug.WriteLine("[Serial Error] Write buffer was fulled"); break;
                    default: break;
                }
            }
        }
    }
}
