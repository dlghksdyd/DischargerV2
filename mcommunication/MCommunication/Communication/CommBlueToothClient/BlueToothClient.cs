using Bluetooth.Client.Basic;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bluetooth.Client.Common
{
    public enum EBluetoothClientStatus : uint
    {
        OK,
        INVALID_START_PARAMETER,

        FAIL_READ,
        FAIL_WRITE,

        ALREADY_CONNECT,
        FAIL_TO_CONNECT,
    }

    public class BluetoothClientConstant
    { 
        public const int MAX_DATA_LENGTH = 244;
    }

    public class BluetoothClientStart
    {
        public string DeviceName = String.Empty;
        public BluetoothAddress Address = BluetoothAddress.None;
        public Guid Guid = Guid.Empty;
        public int TimeOutMs = int.MaxValue;
        public BluetoothClient.DelegateWrite WriteFunction = null;
        public BluetoothClient.DelegateRead ReadFunction = null;
        public BluetoothClient.DelegateParse ParseFunction = null;
    }

    public class BluetoothClient
    {
        private object PacketLock = new object();

        public delegate bool DelegateWrite(int handle, byte[] writeBuffer);
        public delegate bool DelegateRead(int handle, out byte[] readBuffer);
        public delegate bool DelegateParse(byte[] readBuffer, byte[] writeBuffer = null);

        private int Handle = int.MaxValue;

        private BluetoothClientStart Parameters = null;

        private bool IsParameterValid(BluetoothClientStart parameters)
        {
            if (parameters == null)
            {
                return false;
            }
            if (parameters.DeviceName == string.Empty ||
                parameters.Address == BluetoothAddress.None ||
                parameters.Guid == Guid.Empty ||
                parameters.TimeOutMs == int.MaxValue)
            {
                return false;
            }

            return true;
        }

        public int Gethandle()
        {
            return Handle;
        }

        public bool IsConnected()
        {
            return BluetoothClientBasic.IsConnected(Handle);
        }

        public List<BluetoothDeviceInfo> DiscoverDevices()
        {
            return BluetoothClientBasic.DiscoverDevices();
        }

        public EBluetoothClientStatus Connect(BluetoothClientStart parameters)
        {
            if (!IsParameterValid(parameters))
            {
                return EBluetoothClientStatus.INVALID_START_PARAMETER;
            }

            Parameters = parameters;

            var result = BluetoothClientBasic.Connect(
                parameters.Address, parameters.Guid, parameters.TimeOutMs, out Handle);
            if (result != EBluetoothClientBasicStatus.BT_ERROR_OK)
            {
                return EBluetoothClientStatus.FAIL_TO_CONNECT;
            }

            Debug.WriteLine("Connect Bluetooth Client.");

            return EBluetoothClientStatus.OK;
        }

        public void Disconnect()
        {
            BluetoothClientBasic.Disconnect(Handle);

            Debug.WriteLine("Disconnect Bluetooth Client.");
        }

        public bool ProcessPacket(byte[] _writeBuffer)
        {
            lock (PacketLock)
            {
                bool writeResult = Parameters.WriteFunction.Invoke(Handle, _writeBuffer);
                if (writeResult == false)
                {
                    return false;
                }

                bool readResult = Parameters.ReadFunction.Invoke(Handle, out byte[] _readBuffer);
                if (readResult == false)
                {
                    return false;
                }

                bool parseResult = Parameters.ParseFunction.Invoke(_readBuffer, _writeBuffer);
                if (parseResult == false)
                {
                    return false;
                }

                return true;
            }
        }

        public EBluetoothClientStatus Write(int handle, byte[] writeBuffer)
        {
            EBluetoothClientBasicStatus result = BluetoothClientBasic.Write(handle, writeBuffer, 0, writeBuffer.Length);
            if (result != EBluetoothClientBasicStatus.BT_ERROR_OK)
            {
                return EBluetoothClientStatus.FAIL_WRITE;
            }

            return EBluetoothClientStatus.OK;
        }

        public EBluetoothClientStatus Read(int handle, out byte[] readBuffer)
        {
            readBuffer = new byte[BluetoothClientConstant.MAX_DATA_LENGTH * 2];
            EBluetoothClientBasicStatus result = BluetoothClientBasic.Read(handle, ref readBuffer, 0, readBuffer.Length);
            if (result != EBluetoothClientBasicStatus.BT_ERROR_OK)
            {
                return EBluetoothClientStatus.FAIL_READ;
            }

            return EBluetoothClientStatus.OK;
        }

        public bool WriteFunctionExample(int handle, byte[] writeBuffer)
        {
            if (writeBuffer == null || writeBuffer.Length == 0)
            {
                return true;
            }

            EBluetoothClientStatus result = Write(handle, writeBuffer);
            if (result != EBluetoothClientStatus.OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                return false;
            }

            return true;
        }

        public bool ReadFunctionExample(int handle, out byte[] readBuffer)
        {
            /// 데이터 읽기
            EBluetoothClientStatus result = Read(handle, out readBuffer);
            if (result != EBluetoothClientStatus.OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                return false;
            }

            return true;
        }

        public bool ParseFunctionExample(byte[] readBuffer, byte[] writeBuffer)
        {
            return true;
        }

        public void FlushReceiveBuffer(int handle)
        {
            BluetoothClientBasic.FlushReceiveBuffer(handle);
        }
    }
}
