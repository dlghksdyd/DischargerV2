using Bluetooth.Server.Basic;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.Sdp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Bluetooth.Server.Common
{
    public enum BTServerStatus : uint
    {
        OK,
        INVALID_START_PARAMETER,
        ALREADY_SERVER_START,
        FAIL_TO_START_SERVER,
        FAIL_READ,
        FAIL_WRITE,
    }

    public class BluetoothServerConstant
    {
        public const int MAX_DATA_LENGTH = 244;
    }

    public class BluetoothServerStart
    {
        public string DeviceName = string.Empty;
        public Guid Guid { get; set; } = Guid.Empty;
        public BluetoothServer.DelegateRead ReadFunction { get; set; } = null;
        public BluetoothServer.DelegateParse ParseFunction { get; set; } = null;
        public BluetoothServer.DelegateWrite WriteFunction { get; set; } = null;
    }

    public class BluetoothServer
    {
        public delegate bool DelegateRead(out byte[] readBuffer);
        public delegate bool DelegateParse(byte[] readBuffer, out byte[] writeBuffer);
        public delegate bool DelegateWrite(byte[] writeBuffer);

        private Thread ProcessPacketThread = null;

        private BluetoothServerStart Parameters = null;

        private int Handle = int.MaxValue;

        public BluetoothServer()
        {
            /// nothing to do.
        }

        private bool IsParameterValid(BluetoothServerStart parameters)
        {
            if (parameters == null)
            {
                return false;
            }
            if (parameters.DeviceName == string.Empty ||
                parameters.Guid == Guid.Empty ||
                parameters.ReadFunction == null ||
                parameters.WriteFunction == null ||
                parameters.ParseFunction == null)
            {
                return false;
            }

            return true;
        }

        public BTServerStatus Start(BluetoothServerStart parameters)
        {
            if (!IsParameterValid(parameters))
            {
                return BTServerStatus.INVALID_START_PARAMETER;
            }

            if (ProcessPacketThread != null)
            {
                return BTServerStatus.ALREADY_SERVER_START;
            }

            Parameters = parameters;

            ProcessPacketThread = new Thread(ProcessPacket);
            ProcessPacketThread.IsBackground = true;
            ProcessPacketThread.Start();

            Debug.WriteLine("Start bluetooth communication.");

            return BTServerStatus.OK;
        }

        public void Stop()
        {
            BluetoothServerBasic.StopServer(Handle);

            if (ProcessPacketThread != null)
            {
                ProcessPacketThread.Abort();
                ProcessPacketThread = null;
            }

            Debug.WriteLine("Stop bluetooth server");
        }

        public bool IsConnected()
        {
            return BluetoothServerBasic.IsConnected(Handle);
        }

        private void ProcessPacket()
        {
            while (true)
            {
                /// 클라이언트 대기
                Debug.WriteLine("Wait for client.");
                var result = BluetoothServerBasic.StartServer(Parameters.Guid, out Handle);
                if (result != BTServerBasicStatus.BT_ERROR_OK)
                {
                    BluetoothServerBasic.StopServer(Handle);
                    Debug.WriteLine("Fail to start server.");
                    return;
                }
                Debug.WriteLine("Connect client.");

                byte[] readBuffer;
                byte[] writeBuffer = new byte[0];

                while (true)
                {
                    bool readResult = Parameters.ReadFunction.Invoke(out readBuffer);
                    if (readResult == false)
                    {
                        BluetoothServerBasic.StopServer(Handle);
                        Debug.WriteLine("Fail to read packet");
                        break;
                    }

                    bool parseResult = Parameters.ParseFunction.Invoke(readBuffer, out writeBuffer);
                    if (parseResult == false)
                    {
                        BluetoothServerBasic.StopServer(Handle);
                        Debug.WriteLine("Fail to parse packet");
                        break;
                    }

                    bool writeResult = Parameters.WriteFunction.Invoke(writeBuffer);
                    if (writeResult == false)
                    {
                        BluetoothServerBasic.StopServer(Handle);
                        Debug.WriteLine("Fail to write packet");
                        break;
                    }
                }
            }
        }

        public void ChangeReadFunction(DelegateRead delegateRead)
        {
            if (delegateRead != null) Parameters.ReadFunction = delegateRead;
        }

        public void ChangeParseFunction(DelegateParse delegateParse)
        {
            if (delegateParse != null) Parameters.ParseFunction = delegateParse;
        }

        public void ChangeWriteFunction(DelegateWrite delegateWrite)
        {
            if (delegateWrite != null) Parameters.WriteFunction = delegateWrite;
        }

        public BTServerStatus Read(out byte[] readBuffer)
        {
            readBuffer = new byte[BluetoothServerConstant.MAX_DATA_LENGTH];
            BTServerBasicStatus result = BluetoothServerBasic.Read(Handle, ref readBuffer, 0, readBuffer.Length);
            if (result != BTServerBasicStatus.BT_ERROR_OK)
            {
                return BTServerStatus.FAIL_READ;
            }
            
            return BTServerStatus.OK;
        }

        public BTServerStatus Write(byte[] writeBuffer)
        {
            var result = BluetoothServerBasic.Write(Handle, writeBuffer, 0, writeBuffer.Length);
            if (result != BTServerBasicStatus.BT_ERROR_OK)
            {
                return BTServerStatus.FAIL_WRITE;
            }

            return BTServerStatus.OK;
        }

        public bool ReadFunctionExample(out byte[] readBuffer)
        {
            /// 데이터 읽기
            readBuffer = new byte[BluetoothServerConstant.MAX_DATA_LENGTH];
            BTServerBasicStatus result = BluetoothServerBasic.Read(Handle, ref readBuffer, 0, readBuffer.Length);
            if (result != BTServerBasicStatus.BT_ERROR_OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                return false;
            }

            return true;
        }

        public bool ParseFunctionExample(byte[] readBuffer, out byte[] writeBuffer)
        {
            writeBuffer = readBuffer;

            return true;
        }

        public bool WriteFunctionExample(byte[] writeBuffer)
        {
            if (writeBuffer == null || writeBuffer.Length == 0)
            {
                return true;
            }

            BTServerBasicStatus result = BluetoothServerBasic.Write(Handle, writeBuffer, 0, writeBuffer.Length);
            if (result != BTServerBasicStatus.BT_ERROR_OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                return false;
            }

            return true;
        }
    }
}
