using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Multimedia;
using System.Net.NetworkInformation;

namespace MinTech.ABTClass
{
    class cls_devCREVIS
    {
        int m_tempcount = 1;
        public int TEMP_COUNT
        {
            get { return m_tempcount; }
            set
            {
                m_tempcount = value;
                m_calib = new float[m_tempcount];

                m_maxaux = m_voltcount + m_tempcount;
            }
        }
		//+ Add by KGY -250430 : system setting Calibration 값 담는 List
        public List<float> m_sysCalList = new List<float>();
		//-
        int m_voltcount = 0;
        public int VOLT_COUNT
        {
            get { return m_voltcount; }
            set
            {
                m_voltcount = value;

                m_maxaux = m_voltcount + m_tempcount;
            }
        }

        int m_maxaux = 8;
        //public int NUM_OF_AUX
        //{
        //    get { return m_maxaux; }
        //    set { m_maxaux = value; }
        //}


        public float[] m_calib = new float[1];
        

        private bool m_CtrlEnabled = false;
        public bool ControlEnabled
        {
            get { return m_CtrlEnabled; }
            set { m_CtrlEnabled = value; }
        }

        // -------------------------------------
        //
        // -------------------------------------
        private StreamWriter w_logstream = null;

        // ---------------------------------------
        //
        // ---------------------------------------
        private string m_ipaddress = "192.168.1.200";
        public string IP_Address
        {
            get { return m_ipaddress; }
            set { m_ipaddress = value; }
        }
        private int m_ipport = 502;
        public int IP_Port
        {
            get { return m_ipport; }
            set { m_ipport = value; }
        }

        // ----------------------------------------------------------
        // 
        // ----------------------------------------------------------
        int m_index = 0; 
        public int mIndex
        {
            get { return m_index; }
            set { m_index = value; }
        }

        // ------------------------------------------------------
        //
        // ------------------------------------------------------
        System.Threading.ThreadStart threadDelegate = null;
        //+ Add by YMJ - 251027 : CREVIS 통신 검사 쓰레드 추가
        Thread m_CommCheckThread;
        //-

        // ------------------------------------------------------
        //
        // ------------------------------------------------------
        ABT.Mainframe m_frmMain;

        // ------------------------------------------------------
        //
        // ------------------------------------------------------
        private string m_logpath = "";
        public string datalog_path
        {
            get { return m_logpath; }
            set { m_logpath = value; }
        }

        public List<byte[]> m_rxbuff = new List<byte[]>();
        public int m_listcount = 0;

        // ------------------------------------
        // Received buffer
        // ------------------------------------
        public const int RX_BUFF_SIZE = 2096; //4096 8192;
        public int rxcount = 0;
        private byte[] rxbuff = new byte[RX_BUFF_SIZE];    //[15360];        

        // ------------------------------------
        // 
        // ------------------------------------
        public bool isConnected = false;

        private struct mtTimerVar
        {
            public bool enable;
            public int limit;      // 1-100ms
            public int tick;       // 1-100ms

            public mtTimerVar(bool pEnable, int plimit, int pTick)
            {
                enable = pEnable;
                limit = plimit;
                tick = pTick;
            }
            public void Reset()
            {
                enable = false;
                limit = 1;
                tick = 0;
            }
        };
        private mtTimerVar t001 = new mtTimerVar(false, 1, 0);  // 1sec

        // ------------------------------------------------------
        // Connect Event
        // ------------------------------------------------------
        public delegate void RaiseConnectEventHandler(int pch);
        public RaiseConnectEventHandler OnConnected = null;

        // ------------------------------------------------------
        // Disonnect Event
        // ------------------------------------------------------
        public delegate void RaiseDisonnectEventHandler(int pch);
        public RaiseDisonnectEventHandler OnIsNotConnected = null;

        // ------------------------------------------------------
        // Read data packet
        // ------------------------------------------------------
        public delegate void ReadPacketEventHandler(int ch, float[] rvalue);
        public ReadPacketEventHandler OnReadCrevisData = null;

        // ------------------------------------------------------
        // for TCP/IP
        // 231102 BGH
        // ------------------------------------------------------
        Thread m_ReadThread = null;
        private delegate void ReadDelegateHandler();
        private ReadDelegateHandler m_ReadDelegate = null;

        public bool TCPStop = false;
        public Socket socket;

        // ------------------------------------------------------
        // Constructor, Distructor
        // ------------------------------------------------------
        public cls_devCREVIS(int mch, ABT.Mainframe mfrm)
        {
            m_frmMain = mfrm;
            m_index = mch;

            m_ReadDelegate = new ReadDelegateHandler(ReadSocket);

            mfrm.OnTick100ms += TimerEvent100;

            //+ Add by YMJ - 251027 : CREVIS 통신 검사 쓰레드 추가
            m_CommCheckThread = new Thread(new ThreadStart(StartCommChk));
            m_CommCheckThread.IsBackground = true;
            m_CommCheckThread.Start();
            //-
        }
        ~cls_devCREVIS()
        {
            TCPStop = true;

            try
            {
                if (m_ReadThread != null)
                {
                    m_ReadThread.Abort();
                    m_ReadThread.Join();
                    m_ReadThread = null;
                }
            }
            catch { }

            try
            {
                if (this.socket != null)
                {
                    if (this.socket.Connected)
                    {
                        socket.Close();
                    }
                }
                //-
            }
            catch { }


        }

        //+ Add by YMJ - 251027 : CREVIS 통신 검사 쓰레드 추가
        int retryCommCnt = 0;

        private void StartCommChk()
        {
            while(true)
            {
                Thread.Sleep(1000);
                if (socket != null && ControlEnabled)
                {
                    if (!IsSocketConnected())
                    {
                        if (StartTCP())
                        {
                            m_frmMain.WriteSystemLog("// CREVIS #" + (mIndex + 1).ToString() + " Re-Connect // Connectivity=" + isConnected.ToString() + " // Start " + IP_Address + ":" + IP_Port);
                        }
                        else
                        {
                            retryCommCnt++;
                            if (retryCommCnt > 5)
                            {
                                //+ Revision by YMJ - 251028 : CREVIS 통신 끊김 시 알람 or 0 데이터 플래그 추가
                                if (m_frmMain.m_CrevisCommAlarm)
                                {
                                    m_frmMain.AddAlarm(0, ABT.TypeOfSafetyViolation.CREVIS_COMMErr, "[CRE001]Crevis Comm. Error.");
                                }
                                else
                                {
                                    //+ Revision by YMJ - 251027 : 크레비스 통신 끊겼을 경우 0으로 표기
                                    float[] rvalue = new float[m_maxaux];
                                    for (int i = 0; i < m_maxaux; i++)
                                    {
                                        rvalue[i] = 0;
                                    }
                                    OnReadCrevisData?.Invoke(m_index, rvalue);
                                    //-
                                }
                                //-
                            }
                        }
                    }
                    else
                    {
                        retryCommCnt = 0;
                    }
                }
            }
        }
        //-

        // ------------------------------------------------------------------------
        // LOG
        // ------------------------------------------------------------------------
        // ------------------------------------------------
        // 
        // ------------------------------------------------
        private void OpenLog()
        {
            if (w_logstream != null)
            {
                w_logstream.Close();
                w_logstream = null;
            }

            string fldpath = Application.StartupPath + "\\device\\CrevisAUX\\" + DateTime.Now.ToString("yyyy_MM_dd");
            try
            {
                DirectoryInfo di = new DirectoryInfo(fldpath);
                if (di.Exists == false)
                {
                    di.Create();
                }
            }
            catch { }

            // ghbaik :  cvs --> cyc
            //+ Revision by YMJ - 250718 : CREVIS 디바이스 단위로 로깅
            fldpath = fldpath + "\\Crevis_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_Device" + (m_index + 1).ToString("D2") + ".log";
            //-
            w_logstream = new StreamWriter(fldpath, true, System.Text.Encoding.UTF8);

            WriteLog("Created at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            WriteLog("-------------------------------------------------");
        }

        // ------------------------------------------------
        // 
        // ------------------------------------------------
        private void CloseLog()
        {
            if (w_logstream != null)
            {
                try
                {
                    w_logstream.Close();
                    w_logstream = null;
                }
                catch { }
            }
        }

        //// ------------------------------------------------
        //// 
        //// ------------------------------------------------
        private void WriteLog(string m_logstring)
        {
            if (w_logstream != null)
            {
                w_logstream.WriteLine(m_logstring);
                w_logstream.Flush();
            }
        }


        // -------------------
        //
        // -------------------
        private void SetCalibFactor()
        {
            for (int i = 0; i < m_calib.Length; i++) m_calib[i] = 0;
            if (!ControlEnabled) { return; }

            try
            {
				//+ Revision by KGY -250430 : Calibration 값 적용방식 변경
                //cls_IniUtil inifile = new cls_IniUtil(Application.StartupPath + "\\ABTProV2.ini");

                //string strcal = "CALIBRATION_";
                for (int i = 0; i < m_calib.Length; i++)
                {
					//string eble = inifile.GetIniValue("CREVIS_DEVICE_" + (m_index + 1).ToString(), strcal + (i+1).ToString(), "0");
                    //float fble = 0;
                    //if (float.TryParse(eble, out fble))
                    if (i < m_sysCalList.Count)
                    {
                        float fble = m_sysCalList[i];
                        m_calib[i] = fble;
                    }
				//-
                }
            }
            catch { }
            
        }

        // ------------------------
        // Timer
        // ------------------------
        //public void TimerEvent500(object source, EventArgs e)
        public void TimerEvent10()
        {
            // 10ms timer
        }

        public void TimerEvent100()
        {
            // 100ms Timer
            if (t001.enable)
            {
                t001.tick++;
                if (t001.tick >= t001.limit)
                {
                    t001.tick = 0;
                    SetCalibFactor();
                    SendRequest();
                }

            }

        }


        public void SetTimer(int nid, int intv)
        {
            intv = (int)((double)intv / 100.0); // 100ms --> 1 tick
            if (intv < 1) intv = 1;

            switch (nid)
            {
                case 1: t001 = new mtTimerVar(true, intv, 0); break;
            }
        }

        // ----------------------------------------
        //
        // ----------------------------------------
        public void KillTimer(int nid)
        {
            switch (nid)
            {
                case 1: t001.Reset(); break;
            }
        }

       

        public void SendRequest()
        {
            if (isConnected) // if (m_serial.isConnected)
            {
                byte[] readreq = new byte[12];
                for (int i = 0; i < readreq.Length; i++) readreq[i] = 0x00;
                readreq[7] = 0x04;
                readreq[9] = 0x00;
                readreq[11] = (byte)m_maxaux;
                socket.Send(readreq);
            }
        }

        // -------------------------------------------------------------
        // TCP/IP
        // 231102, BGH
        // -------------------------------------------------------------
        public bool ServerAlive(string hsname, int ipaddr)
        {
            try
            {
                var ping = new System.Net.NetworkInformation.Ping();
                var reply = ping.Send(hsname, 10);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //+ Add by YMJ - 251027 : Socket 연결 상태 검사 추가
        public bool IsSocketConnected()
        {
            try
            {
                if (socket == null)
                    return false;
                if (!socket.Connected)
                    return false;

                // OS 차원의 연결 종료 감지
                bool part1 = socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0;

                // 오류 상태 확인
                bool part2 = socket.Poll(1, SelectMode.SelectError);

                if (part1 || part2)
                    return false;

                // 직접 송신 테스트 (활성 감지)
                byte[] test = Array.Empty<byte>();
                socket.Send(test, 0, 0, SocketFlags.None); // zero-length send (안전)
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }
        //-

        public bool StartTCP() // (string IPAddr, string PortNumber)
        {
            IPAddress ServerIP = IPAddress.Parse(IP_Address);

            int iPort = IP_Port;
            IPEndPoint serverEndPoint = new IPEndPoint(ServerIP, iPort);

            KillTimer(1);
            if (ServerAlive(IP_Address, iPort))
            {
                //+ Revision by YMJ - 251027 : 재연결 시 socket 닫고 새로 생성
                if (socket != null)
                {
                    socket.Close();
                }
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //-

                try
                {
                    if (!socket.Connected)
                    {
                        socket.Connect(serverEndPoint);
                    }

                }
                catch (SocketException serr)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss (fff)") + " :: Connection Err = " + serr.Message);
                }

                if (!socket.Connected)
                {
                    isConnected = false;
                    return (false);
                }

                try
                {
                    if (threadDelegate == null)
                    {
                        threadDelegate = new System.Threading.ThreadStart(this.TCPReadThreadFunc);
                        m_ReadThread = new System.Threading.Thread(threadDelegate);
                        m_ReadThread.IsBackground = true;
                    }

                    if (!m_ReadThread.IsAlive)
                    {
                        m_ReadThread.Start();
                    }

                    SetTimer(1, 1000);
                }
                catch (Exception err)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss (fff)") + " :: Thread Error [" + err.Message + "]");
                }

                rxcount = 0;
                OpenLog();

                isConnected = true;
            }
            else
            {
                isConnected = false;
            }


            return (isConnected);
        }

        public void StopTCP()
        {

            KillTimer(1);

            if (socket == null) return;
            
            CloseLog();
            isConnected = false;
        }

        private void TCPReadThreadFunc()
        {
            if (!socket.Connected)
            {
                return;
            }

            // While this mode is selected
            while (!TCPStop)
            {
                m_ReadDelegate.Invoke();
                Thread.Sleep(0);
            }
        }

        //public void SendData(byte[] txPkt)
        //{
        //    socket.Send(txPkt);
        //}

        //+ Revision by YMJ - 251027 : Socket 데이터 리드 방식 개선
        DateTime beforeTime = DateTime.MinValue;
        private void ReadSocket()
        {
            DateTime nowTime = DateTime.Now;
            if((nowTime - beforeTime).TotalMilliseconds > 990)
            {
                bool RxDone = false;
                int rsize = 0;

                try
                {
                    if (IsSocketConnected())
                    {
                        rsize = socket.Receive(rxbuff, 0, rxbuff.Length, SocketFlags.None);

                        if (rsize == 0)
                        {
                            socket.Close();
                        }
                    }
                    if (rsize > 0)
                    {
                        rxcount += rsize;
                        if (rxcount > rxbuff.Length) { rxcount = rxbuff.Length; }
                    }
                    else
                    {
                        rxcount = 0;
                    }
                }
                catch
                {
                    rsize = 0;
                    socket.Close();
                }

                //+ Revision by YMJ - 250718 : CREVIS 데이터 로깅 추가
                string msg = "";
                if (rxcount >= (m_maxaux * 2 + 9))
                {
                    try
                    {
                        if (rxbuff[5] > (m_maxaux * 2))
                        {
                            Int16 m_read = 0;
                            float[] rvalue = new float[m_maxaux];
                            int tempindex = 0;
                            for (int i = 0; i < m_maxaux; i++)
                            {
                                // 231213
                                m_read = (Int16)((Int16)((Int16)rxbuff[9 + i * 2] << 8) + (Int16)rxbuff[9 + i * 2 + 1]);

                                if (i >= m_voltcount)
                                {
                                    rvalue[i] = ((float)m_read / 10.0f) + m_calib[tempindex];
                                    tempindex++;
                                }
                                else
                                {
                                    rvalue[i] = ((float)m_read / 10.0f);
                                }
                                msg += " " + rvalue[i].ToString();
                            }
                            beforeTime = DateTime.Now;
                            WriteLog("  >> R, " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]" + msg);
                            OnReadCrevisData?.Invoke(m_index, rvalue);
                        }
                    }
                    catch { }
                    //-
                }
            }
        }
        //-
    }
}