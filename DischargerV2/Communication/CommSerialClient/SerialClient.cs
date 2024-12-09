using Ethernet.Client.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility.Common;

namespace Serial.Client.Common
{
    public class SerialClientStart
    {
        public string ClientId = "SerialClient";
        public string ComPort;
        public int BaudRate;
        public Encoding Encoding;

        public SerialClient.DelegateWrite WriteFunction = null;
        public SerialClient.DelegateRead ReadFunction = null;
        public SerialClient.DelegateReadTo ReadToFunction = null;
        public SerialClient.DelegateParse ParseFunction = null;
    }

    public class SerialClient
    {
        /// <summary>
        /// Key: ComPort String
        /// </summary>
        private static Dictionary<string, object> Lock = new Dictionary<string, object>();

        public delegate bool DelegateWrite(string comPortStr, string writeString);
        public delegate bool DelegateRead(string comPortStr, out string readString);
        public delegate bool DelegateReadTo(string comPortStr, string breakStr, out string readStr);
        public delegate bool DelegateParse(string readString);

        private DelegateWrite WriteFunction = null;
        private DelegateRead ReadFunction = null;
        private DelegateReadTo ReadToFunction = null;
        private DelegateParse ParseFunction = null;

        private SerialClientStart Params = null;

        public SerialClient(SerialClientStart clientStart)
        {
            Params = clientStart;

            if (!Lock.ContainsKey(Params.ComPort))
            {
                Lock[Params.ComPort] = new object();

                WriteFunction = Params.WriteFunction;
                ReadFunction = Params.ReadFunction;
                ReadToFunction = Params.ReadToFunction;
                ParseFunction = Params.ParseFunction;
            }
        }

        public bool IsRunning()
        {
            if (SerialClientBasic.IsConnect(Params.ComPort))
            {
                return true;
            }
            return false;
        }

        public bool Connect()
        {
            lock (Lock[Params.ComPort])
            {
                /// 이미 실행 중이라면 바로 리턴
                if (IsRunning())
                {
                    return false;
                }

                if (Params.ComPort == "")
                {
                    MessageBox.Show("Serial: ComPort를 설정해 주세요.");
                    return false;
                }

                var result = SerialClientBasic.Connect(Params.ComPort, Params.BaudRate);
                if (result != SRClientStatus.SR_ERROR_OK)
                {
                    return false;
                }
            }
            return true;
        }

        public void Disconnect()
        {
            lock (Lock[Params.ComPort])
            {
                SerialClientBasic.Disconnect(Params.ComPort);
            }
        }

        public bool ProcessPacket(string writeString)
        {
            lock (Lock[Params.ComPort])
            {
                bool writeResult = WriteFunction.Invoke(Params.ComPort, writeString);
                if (writeResult == false)
                {
                    return false;
                }

                string readStr = "";
                bool readResult = ReadFunction.Invoke(Params.ComPort, out readStr);
                if (readResult == false)
                {
                    return false;
                }

                bool parseResult = ParseFunction.Invoke(readStr);
                if (parseResult == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool ProcessPacket(string writeString, string breakStr)
        {
            lock (Lock[Params.ComPort])
            {
                bool writeResult = WriteFunction.Invoke(Params.ComPort, writeString);
                if (writeResult == false)
                {
                    return false;
                }

                string readStr = "";
                bool readResult = ReadToFunction.Invoke(Params.ComPort, breakStr, out readStr);
                if (readResult == false)
                {
                    return false;
                }

                bool parseResult = ParseFunction.Invoke(readStr);
                if (parseResult == false)
                {
                    return false;
                }
            }

            return true;
        }

        private bool WriteData(string comPortStr, string writeStr)
        {
            SRClientStatus result = SerialClientBasic.Write(comPortStr, writeStr);
            if (result != SRClientStatus.SR_ERROR_OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private bool ReadData(string comPortStr, out string readString)
        {
            readString = "";

            /// 데이터 읽기
            byte[] readBuffer = new byte[SRClientConstant.MAX_READ_BUFFER_SIZE];
            SRClientStatus result = SerialClientBasic.Read(comPortStr, readBuffer, 0, readBuffer.Length);
            if (result != SRClientStatus.SR_ERROR_OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                throw new NotImplementedException();
            }

            readString = readBuffer.FromByteArrayToString(Params.Encoding);

            throw new NotImplementedException();
        }

        private bool ReadDataTo(string comPortStr, string breakStr, out string readStr)
        {
            /// 데이터 읽기
            SRClientStatus result = SerialClientBasic.ReadTo(comPortStr, breakStr, out readStr);
            if (result != SRClientStatus.SR_ERROR_OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private bool ParseData(string readString)
        {
            throw new NotImplementedException();
        }
    }
}
