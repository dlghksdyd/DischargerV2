using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ethernet.Client.Discharger;
using Serial.Client.Common;
using Utility.Common;

namespace Serial.Client.TempModule
{
    public class SerialClientTempModuleStart
    {
        public string ComPort;
        public int BaudRate;
        public Encoding Encoding;
        public int TempModuleChannel;
        public int TempChannelCount;
    }

    public enum ETempModuleState
    {
        Disconnect,
        Connecting,
        Connected,
    }

    public partial class SerialClientTempModule
    {
        private SerialClient TempModuleClient = null;

        private System.Timers.Timer ReadInfoTimer = null;

        private SerialClientTempModuleStart Param = null;

        private List<double> TempDatas; // 수신 받은 온도 데이터

        /// <summary>
        /// 온도 모듈 상태
        /// </summary>
        public ETempModuleState TempModuleState = ETempModuleState.Disconnect;

        public SerialClientTempModule(SerialClientTempModuleStart clientStart)
        {
            Param = clientStart;

            TempDatas = new List<double>();
            for (int i = 0; i < Param.TempChannelCount; i++)
            {
                TempDatas.Add(0);
            }
        }

        public bool Start()
        {
            SerialClientStart clientStart = new SerialClientStart();
            clientStart.ClientId = "TempModule";
            clientStart.ComPort = Param.ComPort;
            clientStart.BaudRate = Param.BaudRate;
            clientStart.Encoding = Param.Encoding;
            clientStart.WriteFunction = WriteData;
            clientStart.ReadToFunction = ReadDataTo;
            clientStart.ParseFunction = ParseData;
            TempModuleClient = new SerialClient(clientStart);

            if (!TempModuleClient.Connect())
            {
                return false;
            }

            ReadInfoTimer = new System.Timers.Timer();
            ReadInfoTimer.Interval = 1000;
            ReadInfoTimer.Elapsed += ReadInfoTimer_Elapsed;
            ReadInfoTimer.Start();

            return true;
        }

        public List<double> GetDatas()
        {
            return TempDatas.ConvertAll(x => x);  // deep copy.
        }

        public ETempModuleState GetState()
        {
            return TempModuleState;
        }

        public void Dispose()
        {
            TempModuleClient?.Disconnect();

            ReadInfoTimer?.Stop();
            ReadInfoTimer = null;
        }

        private void ReadInfoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (TempModuleClient.IsRunning())
            {
                ProcessPacket();
            }
        }

        private bool ProcessPacket()
        {
            string writeString = "#" + Param.TempModuleChannel.ToString("D2") + "\r\n";
            bool result = TempModuleClient.ProcessPacket(writeString, "\r");
            if (result == false)
            {
                return false;
            }

            return true;
        }

        private bool ReadDataTo(string comPortStr, string breakStr, out string readStr)
        {
            /// 데이터 읽기
            SRClientStatus result = SerialClientBasic.ReadTo(comPortStr, breakStr, out readStr);
            if (result != SRClientStatus.SR_ERROR_OK)
            {
                Debug.WriteLine("Read Error: " + result.ToString());

                return false;
            }

            return true;
        }

        private bool WriteData(string comPortStr, string writeData)
        {
            SRClientStatus result = SerialClientBasic.Write(comPortStr, writeData);
            if (result != SRClientStatus.SR_ERROR_OK)
            {
                Debug.WriteLine("Write Error: " + result.ToString());

                return false;
            }

            return true;
        }

        private bool ParseData(string readStr)
        {
            // 데이터 read (ex>">+025.12+020.45+012.78+018.97+003.24+015.35+008.07+014.79")
            readStr = readStr.Replace(">", "");

            for (int i = 0; i < Param.TempChannelCount; i++)
            {
                double tempData = double.Parse(readStr.Substring(i * 7, 7));

                TempDatas[i] = tempData;
            }
            return true;
        }
    }
}
