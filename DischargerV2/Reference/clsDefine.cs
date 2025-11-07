using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABT
{
    //+ Add by LBG - 230330 : 언어 정의 위치 변경
    public enum LanguageType { KOREAN = 0, ENGLISH = 1, CHINESE = 2 };

    public enum ContainerType{ FORM = 0, UserControl = 1 };
    //-

    public enum e_CyclerChStatusType { STANDBY = 0, CV = 1, CC = 2, CP = 3, CR = 4, STANDBY_2nd = 5, CCCV = 6, ALARM = 15 };

    // ------------------------------------
    // OVP --> Over Voltage Protection
    // ------------------------------------
    public enum TypeOfSafetyViolation
    {
        NONE = 0,
        // DiffVolt : Max Cell 전압과 Min Cell 전압 차에 의한 Safety
        // LowVolt  : CCCV에서 방전시 목표 전압보다 배터리 전압이 낮을경우
        // HighVolt : CCCV에서 충전시 목표 전압보다 배터리 전압이 높을경우
        OverVolt            = 0x00000001,
        UnderVolt           = 0x00000002,
        OverVolt_Cell       = 0x00000004,
        UnderVolt_Cell      = 0x00000008,
        DiffVolt            = 0x00000010,
        LowVolt             = 0x00000020,
        HighVolt            = 0x00000040,
        OverCurr            = 0x00000100,
        UnderCurr           = 0x00000200,
        NoCurrent           = 0x00000400,
        OverTemp            = 0x00001000,
        UnderTemp           = 0x00002000,
        
        //+ Add by YMJ - 240429 : 충방전기 리턴값 알람 추가
        OverQ               = 0x00010000,
        UnderQ              = 0x00020000,
        Q_OverFlow          = 0x00040000,
        CyclerReturnAlarm   = 0x00080000,
        //-

        ChamberAlarm        = 0x00100000,
        ChillerAlarm        = 0x00200000,
        //+ Revisioin by YMJ - 250213 : MxZ 및 MBT 알람 코드 추가
        InsulationTesterAlarm = 0x00300000,
        MBT_COMMErr         = 0x00400000,
        //+ Add by YMJ - 251027 : CREVIS 통신 알람 코드 추가
        MxZ_COMMErr = 0x00500000,
        //-
        MBT_Err             = 0x00800000,
        MxZ_Err = 0x00900000,
        //-

        EPO_COMMErr         = 0x01000000,
        AUX_COMMErr         = 0x02000000,
        CREVIS_COMMErr      = 0x03000000,
        CAN_COMMErr         = 0x04000000,
        CHAMBER_COMMErr     = 0x08000000,
        CHILLER_COMMErr     = 0x10000000,
        DAU_COMMErr         = 0x20000000,
        MTVD_Err            = 0x30000000,  
        ETC                 = 0x40000000
    };

    public static class SysSet_Table
    {
        public const string m_tb_Name_Use = "Set_Use";
        public const string m_tb_Name_BD = "Set_BD";
        public const string m_tb_Name_DAQ = "Set_DAQ";
        public const string m_tb_Name_DMM = "Set_DMM";
        public const string m_tb_Name_Ins = "Set_Insulate";
        public const string m_tb_Name_DAU = "Set_DAU";
        public const string m_tb_Name_BMS = "Set_BMS";
        public const string m_tb_Name_MCZ = "Set_MCZ";
        public const string m_tb_Name_MBT = "Set_MBT";
        public const string m_tb_Name_MUX = "Set_MUX";
        public const string m_tb_Name_Chamber = "Set_Chamber";
        public const string m_tb_Name_Chiller = "Set_Chiller";
        //+add by KGY - 20240820 : SYS stting 항목 추가
        public const string m_tb_Name_AUX = "Set_AUX";
        public const string m_tb_Name_AUX_Detail = "Set_AUX_Detail";
        public const string m_tb_Name_CAN = "Set_CAN";
        public const string m_tb_Name_MTVD = "Set_MTVD";
        public const string m_tb_Name_DIMS = "Set_DIMS";
        public const string m_tb_Name_BALANCER = "Set_BALANCER";
        public const string m_tb_Name_POWERSUPPLY = "Set_POWERSUPPLY";
        public const string m_tb_Name_MBI = "Set_MBI";
        public const string m_tb_Name_CREVIS = "Set_CREVIS";
        public const string m_tb_Name_CREVIS_Cal = "Set_CREVIS_CAL";
        public const string m_tb_Name_UDP = "Set_UDP";
        //-
        //+ Add by YMJ - 250819 : 자동병렬 기능 테이블 추가
        public const string m_tb_Name_Parallel = "Set_Parallel";
        //-

        public const string m_Col_No_DAQ = "DAQ_No";
        public const string m_Col_No_DMM = "DMM_No";
        public const string m_Col_No_Ins = "Ins_No";
        public const string m_Col_No_DAU = "DAU_No";
        public const string m_Col_No_BMS = "BMS_No";
        public const string m_Col_No_MCZ = "MCZ_No";
        public const string m_Col_No_MBT = "MBT_No";
        public const string m_Col_No_MUX = "MUX_No";
        public const string m_Col_No_Chamber = "Chamber_No";
        public const string m_Col_No_Chiller = "Chiller_No";
        //+add by KGY - 20240820 : SYS stting 항목 추가
        public const string m_Col_No_AUX = "AUX_No";
        public const string m_Col_No_CAN = "CAN_No";
        public const string m_Col_No_MTVD = "MTVD_No";
        public const string m_Col_No_DIMS = "DIMS_No";
        //-
    }

    public struct Sys_Set_BD
    {
        public bool     BD_Use;
		public int      BD_No;
		public int      BD_Type;
        public int      BD_CH_Cnt;
		public string   BD_IP;
		public int      BD_TCP_Port;
		public string   BD_COM_Port;
    }

    public struct Sys_Set_DAQ
    {
        public bool     DAQ_Use;
		public int      DAQ_No;
		public int      DAQ_ComType;
		public string   DAQ_IP;
		public int      DAQ_TCP_Port;
		public string   DAQ_COM_Port;
    }

    public struct Sys_Set_DMM
    {
        public bool     DMM_Use;
		public int      DMM_No;
		public int      DMM_ComType;
		public string   DMM_IP;
		public int      DMM_TCP_Port;
		public string   DMM_COM_Port;
    }

    //+ Revision by KGY - 20240820 : 절연저항기 SYS Setting 항목 변경
    public struct Sys_Set_Insulate
    {
        public bool     Ins_Use;
		public int      Ins_No;
		public string   Ins_COM_Port;
        public string   Ins_Mode;
        public string   Ins_Rly_COM_Port;
        public int      Ins_Voltage;
        public int      Ins_Time;
    }
    //-

    //+ Revision by KGY - 20240820: DAU SYS Setting 항목 변경
    public struct Sys_Set_DAU
    {
        public bool     DAU_Use;
		public int      DAU_No;
        public string   DAU_IP;
        public string   DAU_NTC;
        public int      DAU_VModule_Count;
        public int      DAU_TModule_Count;
        public int      DAU_Voltage_Count;
        public int      DAU_Temp_Count;
        /// <summary>
        /// 0 : NTC , 1 : K , 2 : BOTH
        /// </summary>
        public int      DAU_Temp_Type;
        public bool     DAU_Auto_Logging;
        public bool     DAU_Multi_Use;
        public int      DAU_Temp_MultiCnt;
    }

    public struct Sys_Set_BMS
    {
        public bool     BMS_Use;
		public int      BMS_No;
        public string   BMS_CAN_Id;
        public int      BMS_Voltage_Count;
        public int      BMS_Temp_Count;
    }

    public struct Sys_Set_MCZ
    {
        public bool     MCZ_Use;
		public int      MCZ_No;
        public string   MCZ_IP;
        public int      MCZ_Port;
    }

    public struct Sys_Set_MBT
    {
        public bool     MBT_Use;
		public int      MBT_No;
		public string   MBT_IP;
		public int      MBT_TCP_Port;
    }

    public struct Sys_Set_MUX
    {
        public bool MUX_Use;
        public int MUX_No;
        public string MUX_IP;
        public int MUX_TCP_Port;
        public int MUX_CH_Cnt;
    }
    //+ Revision by KGY - 20240820 : 챔버 SYS Setting 항목 변경
    public struct Sys_Set_Chamber
    {
        public bool     Chamber_Use;
		public int      Chamber_No;
		public string   Chamber_Model;
        public string   Chamber_COM_Port;
		public string   Chamber_Baudrate;
        public bool     Chamber_Checksum;
        public bool     Chamber_Alarm_Auto_Stop;
        public string   ChamberDIO_Device;
        public int      ChamberDIO_Module_No;
        public string   ChamberDIO_COM_Port;
        public string   ChamberDIO_Baudrate;
    }
    //-
    //+ Revision by KGY - 20240820 : 칠러 SYS Setting 항목 변경
    public struct Sys_Set_Chiller
    {
        public bool     Chiller_Use;
        public int      Chiller_No;
        public string   Chiller_Model;
        public string   Chiller_COM_Port;
        public string   Chiller_Baudrate;
        public bool     Chiller_Checksum;

    }
    //-
    //+ Revision by KGY - 20240820 : SYS Setting 항목 추가
    
    public struct Sys_Set_AUX
    {
        public bool      AUX_Use;
        public int       AUX_No;
        public string    AUX_COM_Port;
        public string    AUX_Baudrate;
        public int       AUX_VModule_Count;
        public int       AUX_TModule_Count;
        public int       AUX_Voltage_Count;
        public int       AUX_Temp_Count;
        public int       AUX_Applied_ModuleNo;

    }
    public struct Sys_Set_AUX_Detail
    {
        public int AUX_No;
        public List<int> AUX_VList;
        public List<int> AUX_TList;
    }
    public struct Sys_Set_CAN
    {
        public bool     CAN_Use;
        public int      CAN_No;
        public string   CAN_Type;
        public string   CAN_MDBC;
        public string   CAN_ID;
        //+ Add by YMJ - 250725 : CAN 채널 추가
        public string   CAN_CH;
        //-
        public string   CAN_HS_Bitrate;
        public string   CAN_FD_ClockFrequency;
        public string   CAN_FD_BitRate;
        public string   CAN_FD_DataBitRate;
    }
    public struct Sys_Set_MTVD
    {
        public bool     MTVD_Use;
        public int      MTVD_No;
        public string   MTVD_Model;
        public string   MTVD_VOC;
        public string   MTVD_IP;
        public string   MTVD_Port;
    }
    public struct Sys_Set_DIMS
    {
        public bool     DIMS_Use;
        public int      DIMS_No;
        public string   DIMS_IP;
        public string   DIMS_Port;
        public int      DIMS_Send_Interval;
    }
    public struct Sys_Set_BALANCER
    {
        public bool     BALANCER_Use;
        public int      BALANCER_No;
        public string   BALANCER_COM_Port;
        public string BALANCER_Baudrate;
    }
    public struct Sys_Set_POWERSUPPLY
    {
        public bool POWERSUPPLY_Use;
        public string POWERSUPPLY_COM_Port;
        public string POWERSUPPLY_Baudrate;
    }
    public struct Sys_Set_MBI
    {
        public bool MBI_Use;
        public int MBI_No;
        public string MBI_CAN_ID;
        public int MBI_VOLT;
        public int MBI_TEMP;
    }
    public struct Sys_Set_CREVIS
    {
        public bool CREVIS_Use;
        public int CREVIS_No;
        public string CREVIS_IP;
        public int CREVIS_VOLT;
        public int CREVIS_TEMP;
        public List<float> CREVIS_CALIBRATION;
    }

    //+ Add by YMJ - 250819 : 자동병렬 기능 장비 정보 추가
    public struct Sys_Set_Parallel
    {
        public bool Parallel_Use;
        public int Parallel_No;
        public string Parallel_IP;
    }
    //-

    public struct Sys_Set_UDP
    {
        public bool     UDP_Enable;
        public string   UDP_Server_IP;
        public string   UDP_Server_Port;
        public int      UDP_State_Send_Interval;
    }
    //-
    public struct Sys_Link
    {
        public bool Use;
        public int  No;
        public int  BD_No;
        public int  BD_CH;
        public int  DAQ_No;
        public int  DMM_No;
        public int  Ins_No;
		public int  DAU_No;
		public int  BMS_No;
		public int  MCZ_No;
        public int  MBT_No;
        public int  MUX_No;
        public int  MUX_CH;

        //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
        public int Chamber_No;
        public int Chiller_No;
        //-
        //+add by KGY - 20240820 : SYS stting 항목 추가
        public int AUX_No;
        public int CAN_No;
        public int MTVD_No;
        //-
    }

    public class Def_Menu
    {
        //+ Revision By YMJ - 241217 : 미사용 메뉴 제거
        //+ Menu - File
        public const string File                    = "0";
        public const string File_Captuer            = "0-0";
        public const string File_Quit               = "0-1";
        //-

        //+ Menu - Edit
        public const string Edit                    = "1";
        //-
        //-

        //+ Menu - Controls
        public const string Control                 = "2";
        public const string Control_Start           = "2-0";
        public const string Control_Stop            = "2-1";
        public const string Control_Reserve         = "2-2";
        public const string Control_Pause           = "2-3";
        public const string Control_Pause_Pause     = "2-3-0";
        public const string Control_Pause_StepC     = "2-3-1";
        public const string Control_Pause_CycC      = "2-3-2";
        public const string Control_Pause_IoopC     = "2-3-3";
        public const string Control_Pause_ETC       = "2-3-4";
        public const string Control_Pause_Cancel    = "2-3-5";
        public const string Control_Resume          = "2-4";
        public const string Control_Next            = "2-5";
        public const string Control_Move            = "2-6";
        public const string Control_Reset           = "2-7";
        public const string Control_Schedule        = "2-8";
        public const string Control_Graph_Cycler    = "2-9";
        public const string Control_Graph_ACIA      = "2-10";
        //-

        //+ Menu - Setting
        public const string Set                     = "3";
        public const string Set_Sys                 = "3-0";
        public const string Set_SysLink             = "3-1";
        public const string Set_UserOpt             = "3-2";
        public const string Set_ChCode              = "3-3";
        public const string Set_Parallel            = "3-4";
        public const string Set_AUX                 = "3-5";
        public const string Set_DAU                 = "3-6";
        //-

        //+ Menu - Schedule
        public const string Sche                    = "4";
        public const string Sche_Manager            = "4-0";
        public const string Sche_Data               = "4-1";
        public const string Sche_LosData            = "4-2";
        public const string Sche_Network            = "4-3";
        //-

        //+ Menu - View
        public const string View                    = "5";
        public const string View_Data               = "5-0";
        public const string View_Schedule           = "5-1";
        public const string View_Work               = "5-2";
        public const string View_System             = "5-3";
        public const string View_Device             = "5-4";
        public const string View_Reserve            = "5-5";
        public const string View_Connection         = "5-6";
        //-

        //+ Menu - Language
        public const string Lang                    = "6";
        public const string Lang_Kor                = "6-0";
        public const string Lang_Eng                = "6-1";
        //-

        //+ Menu - Help
        public const string Help                    = "7";
        public const string Help_Folder             = "7-0";
        public const string Help_About              = "7-1";
        //-
    }

    public struct Display_Board_Status
    {
        public int      bd_s_No;
        public bool     bd_s_Conected;
        public int      bd_s_CH_Cnt;
        public int      bd_s_CH_Wait_Cnt;
        public int      bd_s_CH_Work_Cnt;
        public string   bd_s_IP_Addr;
        public string   bd_s_Spec;
    }

    // ghbaik
    // public enum eNUM_CutOFF_Type { NONE = 0, VOLTAGE, CURRENT, CAPACITY, ENERGY, TIME, DOD, SOC, deltaVoltage, ELSE };
    // public enum eNUM_CutOFF_Type { NONE = 0, VOLTAGE, CURRENT, CAPACITY, ENERGY, TIME, DOD, SOC, deltaVoltage, AUX_VOLTAGE, AUX_TEMP, CAN, TEMPERATURE, SAFETY, ELSE };
    //public enum eNUM_CutOFF_Type { NONE = 0, VOLTAGE, CURRENT, CAPACITY, ENERGY, TIME, DOD, SOC, POWER, deltaVoltage, AUX_VOLTAGE, AUX_TEMP, CAN, TEMPERATURE, SAFETY, ELSE };
    // 231126, BGH
    //public enum eNUM_CutOFF_Type { NONE = 0, VOLTAGE, CURRENT, CAPACITY, ENERGY, TIME, DOD, SOC, POWER, deltaVoltage, AUX_VOLTAGE, AUX_TEMP, CAN, TEMPERATURE, SAFETY, Balancing, EIS, USER, ELSE };
    public enum eNUM_CutOFF_Type { NONE = 0, VOLTAGE, CURRENT, CAPACITY, ENERGY, TIME, DOD, SOC, POWER, deltaVoltage, deltaCurrent, deltaTemp, AUX_VOLTAGE, AUX_TEMP, CAN, TEMPERATURE, SAFETY, Balancing, EIS, USER, ELSE };

    public struct Display_Channel_Status
    {
        public bool SafetyWarmingUp; // Safety 조건 설정하는 시간. 초기 전류 구동모드 전환 후 첫번째 스케쥴의 Step 번호 생길때까지... (230923) BGH

        public bool isTesting;
        public bool isChamberWarming;

        //+ Add by YMJ - 250218 : 병렬 출력 모드 관련 이관 (TP코드 -> Main코드)
        public bool ch_s_parause;
        public bool ch_s_ismaster;
        public int ch_s_master_ch;  // master면 0, slave면 master 채널 번호(base 0)
        // -

        public int ch_s_CH_No;
        public int ch_s_Step_No;
        public STEP_TYPE ch_s_Step_Type;
        //+ Add By LBG - 22.11.18 : 채널 상태창에 Mode 추가
        public STEP_MODE ch_s_Step_Mode;
        //-
        //public int          ch_s_Code;  // 상태 Code 추가
        public string ch_s_Code;  // 상태 Code 추가

        public DateTime ch_s_Total_Time;  // ghbaik
        public DateTime ch_s_Step_Time;  // ghbaik

        public TimeSpan ch_s_TotSpent_Time;  // ghnnn
        public TimeSpan ch_s_Spent_Time;  // ghnnn
		public TimeSpan ch_s_CV_Time; // Add by BGH

        public double ch_s_Volt;
        public double ch_s_Curr;
        public double ch_s_Capa;

        public double ch_s_Power;     // Power 추가
        public double ch_s_Temp;      // 온도 추가

        public double ch_s_Energy;    // Energy 추가        
                                      
        //public bool ch_s_MUX_Status;
        public bool ch_s_Warmup;     // 전류 올라오기전 대기표시 위해, 230902, BGH

        public string ch_s_Sche_Name;
        public string ch_s_Work_Name;
        
        public eNUM_CutOFF_Type ch_s_CutOffType;  // ghbaik

        //+ Add By LBG - 22.11.18 : 채널 상태창에 알람 표시를 위한 Flag 추가
        public bool         ch_s_Exist_Alarm;

        public bool ch_s_isPaused;
        //public bool ch_s_Backup_Alarm;
        //-

        //+ Add by LBG - 23.10.10 : 시작 시간 기록을 위하여 추가
        public DateTime ch_Start_DateTime;
        //-

        public void Reset()
        {
            SafetyWarmingUp = false;

            isTesting = false;
            isChamberWarming = false;

            //+ Add by YMJ - 250218 : 병렬 출력 모드 관련 이관 (TP코드 -> Main코드)
            ch_s_parause = false;
            ch_s_ismaster = false;
            ch_s_master_ch = 0;
            //-

            ch_s_CH_No              = 0;
            ch_s_Step_No            = 0;
            ch_s_Step_Type          = STEP_TYPE.NONE;
            ch_s_Step_Mode          = STEP_MODE.NONE;
            ch_s_Code = ""; // 0;  // 상태 Code 추가

            ch_s_Total_Time         = DateTime.Now;  // ghbaik
            ch_s_Step_Time          = DateTime.Now;  // ghbaik
            
            ch_s_TotSpent_Time      = new TimeSpan(0, 0, 0, 0, 0);
            ch_s_Spent_Time         = new TimeSpan(0, 0, 0, 0, 0);
			ch_s_CV_Time            = new TimeSpan(0, 0, 0, 0, 0);

			ch_s_Volt               = 0;
            ch_s_Curr               = 0;
            ch_s_Capa               = 0;
            ch_s_Power              = 0;  // Power 추가
            ch_s_Temp               = 999.99; // -50;   // 온도 추가
            ch_s_Energy             = 0;    // Energy 추가

            //ch_s_MUX_Status         = false;
            ch_s_Warmup             = false;

            ch_s_Sche_Name          = "";
            ch_s_Work_Name          = "";

            // ghbaik
            ch_s_CutOffType         = eNUM_CutOFF_Type.NONE;

            ch_s_Exist_Alarm        = false;
            ch_s_isPaused = false;
            //ch_s_Backup_Alarm = false;

            //+ Add by LBG - 23.10.10 : 시작 시간 기록을 위하여 추가
            ch_Start_DateTime = DateTime.MinValue;
            //-
        }
    }

    //+ Add by LBG - 230110 : 메인화면에 AUX Data Display를 위한 Data Structure 추가
    public struct Display_AUX_Data
    {
        public string AUX_Name;
        public string AUX_Value1;
        public string AUX_Value2;
        public string AUX_Value3;
        public string AUX_Value4;

        public bool isBalancing;

        public void Reset()
        {
            AUX_Name = "";
            AUX_Value1 = "";
            AUX_Value2 = "";
            AUX_Value3 = "";
            AUX_Value4 = "";

            isBalancing = false;
        }
    }
    //-

    //+ Add by LBG - 230110 : 메인화면에 CAN Data Display를 위한 Data Structure 추가
    public struct Display_CAN_Data
    {
        public string CAN_Name;
        public string CAN_Value;

        public void Reset()
        {
            CAN_Name = "";
            CAN_Value = "";
        }
    }
    //-

    // -----------------------------------
    //+ Add by BGH - 230601 
    // DAU & BMS
    // -----------------------------------
    public struct Display_DAU_Data
    {
        public string DAU_Name;
        public List<string> DAU_Value;
        public void Reset()
        {
            DAU_Name = "";
            DAU_Value = null;
            DAU_Value = new List<string>();
        }
    }
    //-

    // -----------------------------------
    //+ Add by BGH - 230601 
    // -----------------------------------
    public struct Display_Chamber_Data
    {
        public string Cham_Name;
        public string Cham_Value;

        public void Reset()
        {
            Cham_Name = "";
            Cham_Value = "";
        }
    }

    //+ Revision by YMJ - 250902 : 부산TP 챔버 및 칠러 기능 이전
    public struct Display_Chiller_Data
    {
        public string Chiller_Name;
        public string Chiller_Value;

        public void Reset()
        {
            Chiller_Name = "";
            Chiller_Value = "";
        }
    }

    public struct Display_MTVD_Data
    {
        public string MTVD_Name;
        public string MTVD_Value;

        public void Reset()
        {
            MTVD_Name = "";
            MTVD_Value = "";
        }
    }
    //-

    //+ Add by YMJ - 241114 : CREVIS 데이터 디스플레이 구조체 추가
    public struct Display_CREVIS_Data
    {
        public string CREVIS_Name;
        public List<string> CREVIS_Value;
        public void Reset()
        {
            CREVIS_Name = "";
            CREVIS_Value = new List<string>();
        }
    }
    //-

    public enum STEP_TYPE
    {
        //+ Revision by LBG - 23.03.20 : 온도 Step 추가
        //PAUSE           = -3,
        //READY           = -2,
        //NONE            = -1,
        //CYCLE_START     = 0,
        //CHARGE          = 1,
        //DISCHARGE       = 2,
        //PATTERN         = 3,
        //OCV             = 4,
        //REST            = 5,
        //ACIA            = 6,
        //INSULATE        = 7,
        //CYCLE_END       = 8,
        //WORK_END        = 9
        WAIT            = -5,  // ghccc 20230412
        STOP            = -4,  // ghccc 20230412
        PAUSE           = -3,
        READY           = -2,
        NONE            = -1,
        CYCLE_START     = 0,
        CHARGE          = 1,
        DISCHARGE       = 2,
        PATTERN         = 3,
        OCV             = 4,
        REST            = 5,
        ACIA            = 6,
        INSULATE        = 7,
        TEMPERATURE     = 8,
        CYCLE_END       = 8,
        WORK_END        = 9
        //-
    }

    public enum STEP_MODE
    {
        NONE        = -1,
        CCCV        = 0,
        CC          = 1,
        CP          = 2,
        CR          = 3,
        CV          = 4,
        CPCV                 // ghbaik
    }

    public enum CAPACITY_TYPE
    {
        CAPACITY    = 0,
        ENERGY      = 1
    }

    // ghbaik 230427
    public enum BALANCING_RESISTER_TYPE
    {
        R20 = 0,
        R40 = 1,
        R60 = 2
    }

    public struct Step_Info
    {
        //+ 기본 Step 정보
        public int          Step_No;
        //public int          Step_Type;
        public STEP_TYPE    Step_Type;

        public int          Step_Mode;
        //-

        //+ 충/방전 작업 설정 값
        public double Work_CV;
        public double Work_CC;
        public double Work_CP;
        public double Work_CR;
        //public double Work_CCCV;  // ghbaik
        public string       Work_Pattern;
        //-

        //+ Add by LBG - 230425 : CAN 기준값 설정 사용 정의
        public bool BMS_V_Use;
        public BMS_Work_Info BMS_Work_Info_V;
        public bool BMS_P_Use;
        public BMS_Work_Info BMS_Work_Info_P;
        public bool BMS_C_Use;
        public BMS_Work_Info BMS_Work_Info_C;

        public bool AUX_V_Use;
        public AUX_Work_Info AUX_Work_Info_V;
        //-

        //+ ACIA 작업 설정 값
        public bool         ACIA_Use;
        public int          ACIA_Type;   // 0: MBT, 1: MxZ
        //+ MBT
        public double       MBT_Start_Signal;
        public double       MBT_End_Signal;
        public double       MBT_I_Range;
        public int          MBT_Density;
        public int          MBT_Delay;
        public int          MBT_Skip_Cycle;
        //-

        //+ MxZ
        public double MxZ_Start_Signal;
        public double MxZ_End_Signal;
        public double MxZ_I_Range;
        public int          MxZ_Density;
        public int          MxZ_Delay;
        public int MxZ_Skip_Cycle;
        //-
        //-

        //+ Insulate 작업 설정 값

        //-

        //+ 작업 종료 조건 설정 값
        public bool         Cut_Volt_Use;
        //+ Revision by LBG - 23.02.16 : Pattern 모드로 인한 전압 Cut-Off 상하한 설정 추가
        //public double        Cut_Volt;
        public double       Cut_Volt_Upper;
        public double       Cut_Volt_Lower;
        //-
        public bool         Cut_Curr_Use;
        public double       Cut_Curr;
        public bool         Cut_Time_Use;
        public int          Cut_Time;
        public int          Cut_Time_Hour;
        public int          Cut_Time_Min;
        public int          Cut_Time_Sec;
        public bool         Cut_Capacity_Use;
        //public int          Cut_Capacity_Type;
        public double       Cut_Capacity;
        public int          Cut_SOC_DOD_Type;
        public bool         Cut_SOC_DOD_Use;
        public int          Cut_No_SOC_DOD;
        public int          Cut_Per_SOC_DOD;

        //+ Revision by LBG - 23.02.16 : Cut-Off 조건 추가
        public bool         Cut_Watt_Use;
        public double       Cut_Watt;
        public bool         Cut_Power_Use;
        public double       Cut_Power;
        public bool         Cut_CV_Time_Use;
        public int          Cut_CV_Time;
        public int          Cut_CV_Time_Hour;
        public int          Cut_CV_Time_Min;
        public int          Cut_CV_Time_Sec;
        //-
        //-

        //+ Revision by LBG - 230410 : Chamber 조건 설정 추가
        public bool         Chamber_Use;
        public double       Chamber_Target_Temp;
        //public int          Chamber_Target_Time;
        //+ Revision by YMJ - 250902 : 부산TP 챔버 및 칠러 기능 이전
        public int          Chamber_Target_Humidity;
        //-
        public bool         Chamber_Maintain_Use;
        //-

        //+ Revision by LBG - 230410 : Chiller 조건 설정 추가
        public bool         Chiller_Use;
		public double       Chiller_Target_Temp;
        public double       Chiller_Target_LPM;
		//public int          Chiller_Target_Time;
		public bool         Chiller_Maintain_Use;
        //public double        Chiller_Maintain_Temp;
        //-

        //+ Cycle 횟수 설정 값
        public int          Cycle_Cnt;
        //-

        //+ 기록 조건 설정 값
        public bool         Logging_Time_Use;
        public int          Logging_Time;           // ms

        public bool         Logging_Time_Volt_Use;  // 전압의 큰 변위가 발생 했을 시 Logging Time 사용 유무
        public double       Logging_Time_Volt;      // 전압의 큰 변위가 발생 했을 시 mv; //Logging Time (ms)
        public bool         Logging_Time_Curr_Use;  // 전류의 큰 변위가 발생 했을 시 Logging Time 사용 유무
        public double       Logging_Time_Curr;      // 전류의 큰 변위가 발생 했을 시 mA; // Logging Time (ms)
        public bool         Logging_Time_Temp_Use;  // 온도의 큰 변위가 발생 했을 시 Logging Time 사용 유무
        public double       Logging_Time_Temp;      // 온도의 큰 변위가 발생 했을 시 degree; //Logging Time (ms)
        //-

        //+ Revision by LBG - 230410 : Step 안전 조건 추가
        public bool         Step_Safety_Volt_Use;
        public double       Step_Safety_Volt_Lower;
        public double       Step_Safety_Volt_Upper;
        public bool         Step_Safety_Curr_Use;
        public double       Step_Safety_Curr_Lower;
        public double       Step_Safety_Curr_Upper;

        public bool         Step_Safety_Capa_Use;
        public double       Step_Safety_Capa_Lower;
        public double       Step_Safety_Capa_Upper;
        public bool         Step_Safety_Temp_Use;
        public double       Step_Safety_Temp_Lower;
        public double       Step_Safety_Temp_Upper;
        //-

        //+ Cut-Off 조건으로 이동 Step 설정
        public int          Move_Step_Volt;
        public int          Move_Step_Curr;
        public int          Move_Step_Time;
        public int          Move_Step_Capa;
        public int          Move_Step_SOC_DOD;

        //+ Add by LBG - 23.02.16 : 종료 조건 추가로 인한 Next Step 정보 추가
        public int          Move_Step_Watt;
        public int          Move_Step_Power;
        public int          Move_Step_CV_Time;
        //-
        //-

        //+ CAN 조건 사용 여부 설정
        public bool         CAN_USE;
        public string       CAN_File;
        public List<int>    CAN_INDEX;
        public List<CAN_CUT_OFF_INFO>   CAN_CUT_OFF_INFO;
        //-

        //+ Add by YMJ - 250716 : CAN TX 관련 값 추가
        public List<CAN_TX_Info> CAN_TX_INFO;
        //-

        //+ CAN Power Supply 사용 설정
        public bool         CAN_POWER_USE;
        public double       CAN_POWER_VOLT;
        //-

        //+ AUX 조건 사용 여부 설정
        public bool         AUX_USE;
        public string       AUX_File;
        public List<int>    AUX_V_INDEX;
        public List<int>    AUX_T_INDEX;
        //-

        //+ AUX Cell Balancing 관련 설정 정보
        public BALANCING_RESISTER_TYPE AUX_Balancing_Volt_R_Type;       // 단위 : Ω
        public double AUX_Balancing_Volt_Lower;          // 단위 : V
        public double AUX_Balancing_Volt_Upper;          // 단위 : V
        public double AUX_Balancing_Volt_Dev_Start;      // 단위 : mV
        public double AUX_Balancing_Volt_Dev_End;        // 단위 : mV
        //-

        public void Reset()
        {
            //+ 기본 Step 정보
            Step_No                 = 0;
            Step_Type               = STEP_TYPE.NONE; //= -1;
            Step_Mode               = -1;
            //-

            //+ 충/방전 작업 설정 값
            Work_CV                 = 0;
            Work_CC                 = 0;
            Work_CP                 = 0;
            Work_CR                 = 0;
            Work_Pattern            = "";
            //-

            //+ Add by LBG - 230425 : BMS & AUX 기준값 설정 사용 정의
            BMS_V_Use = false;
            BMS_Work_Info_V = new BMS_Work_Info();
            BMS_P_Use = false;
            BMS_Work_Info_P = new BMS_Work_Info();
            BMS_C_Use = false;
            BMS_Work_Info_C = new BMS_Work_Info();
            AUX_V_Use = false;
            AUX_Work_Info_V = new AUX_Work_Info();
            //-

            //+ ACIA 작업 설정 값
            //+ [Revision By LBG] 22.05.02 : ACIA의 MBT와 MxZ로 분리
            //+ MBT
            MBT_Start_Signal        = 0;
            MBT_End_Signal          = 0;
            MBT_I_Range             = 0;
            MBT_Density             = 0;
            MBT_Delay               = 0;
            MBT_Skip_Cycle          = 0;
            //-

            //+ Revisioin by YMJ - 250213 : MxZ 설정값 변경
            //+ MxZ
            MxZ_Start_Signal = 0;
            MxZ_End_Signal = 0;
            MxZ_I_Range = 0;
            MxZ_Density             = 0;
            MxZ_Delay               = 0;
            MxZ_Skip_Cycle = 0;
            //-
            //-

            //+ Insulate 작업 설정 값

            //-

            //+ 작업 종료 조건 설정 값
            Cut_Volt_Use            = false;
            //+ Revision by LBG - 23.02.16 : Pattern 모드로 인한 전압 Cut-Off 상하한 설정 추가
            //Cut_Volt                  = 0;
            Cut_Volt_Upper          = 0;
            Cut_Volt_Lower          = 0;
            //-
            Cut_Curr_Use            = false;
            Cut_Curr                = 0;
            Cut_Time_Use            = false;
            Cut_Time                = 0;
            Cut_Time_Hour           = 0;
            Cut_Time_Min            = 0;
            Cut_Time_Sec            = 0;
            Cut_Capacity_Use        = false;
            //Cut_Capacity_Type       = 0;
            Cut_Capacity            = 0;
            Cut_SOC_DOD_Type        = 0;
            Cut_SOC_DOD_Use         = false;
            Cut_No_SOC_DOD          = 0;
            Cut_Per_SOC_DOD         = 0;
            //+ Revision by LBG - 23.02.16 : Cut-Off 조건 추가
            Cut_Watt_Use            = false;
            Cut_Watt                = 0;
            Cut_Power_Use           = false;
            Cut_Power               = 0;
            Cut_CV_Time_Use         = false;
            Cut_CV_Time             = 0;
            Cut_CV_Time_Hour        = 0;
            Cut_CV_Time_Min         = 0;
            Cut_CV_Time_Sec         = 0;
            //-
            //-

            //+ Revision by LBG - 230410 : Chamber 조건 설정 추가
            Chamber_Use             = false;
            Chamber_Target_Temp     = 0;
            //+ Revision by YMJ - 250902 : 부산TP 챔버 및 칠러 기능 이전
            //+ Add by LBG - 241031 : SIMPAC 챔버의 경우 습도 조절 기능이 있음, 해당 설정값 추가
            Chamber_Target_Humidity = 0;
            //-
            //-
            //Chamber_Target_Time     = 0;
            Chamber_Maintain_Use    = false;
            //-

            //+ Revision by LBG - 230410 : Chiller 조건 설정 추가
            Chiller_Use             = false;
		    Chiller_Target_Temp     = 0;
            Chiller_Target_LPM      = 0;
		    //Chiller_Target_Time     = 0;
		    Chiller_Maintain_Use    = false;
            //Chiller_Maintain_Temp   = 0;
            //-

            //+ Cycle 횟수 설정 값
            Cycle_Cnt = -1;
            //-

            //+ 기록 조건 설정 값
            Logging_Time_Use        = false;
            Logging_Time            = 0;        // ms

            Logging_Time_Volt_Use   = false;    // 전압의 큰 변위가 발생 했을 시 Logging Time 사용 유무
            Logging_Time_Volt       = 0;        // 전압의 큰 변위가 발생 했을 시 Logging Time (ms)
            Logging_Time_Curr_Use   = false;    // 전류의 큰 변위가 발생 했을 시 Logging Time 사용 유무
            Logging_Time_Curr       = 0;        // 전류의 큰 변위가 발생 했을 시 Logging Time (ms)
            Logging_Time_Temp_Use   = false;    // 온도의 큰 변위가 발생 했을 시 Logging Time 사용 유무
            Logging_Time_Temp       = 0;        // 온도의 큰 변위가 발생 했을 시 Logging Time (ms)
            //-

            //+ Revision by LBG - 230410 : Step 안전 조건 추가
            Step_Safety_Volt_Use    = false;
            Step_Safety_Volt_Lower  = 0;
            Step_Safety_Volt_Upper  = 0;
            Step_Safety_Curr_Use    = false;
            Step_Safety_Curr_Lower  = 0;
            Step_Safety_Curr_Upper  = 0;

            Step_Safety_Capa_Use    = false;
            Step_Safety_Capa_Lower  = 0;
            Step_Safety_Capa_Upper  = 0;
            Step_Safety_Temp_Use    = false;
            Step_Safety_Temp_Lower  = 0;
            Step_Safety_Temp_Upper  = 0;
            //-

            //+ Cut-Off 조건 별로 Step 이동 정의
            Move_Step_Volt          = -1;      // -99일 경우 Next Step, -9일 경우 End
            Move_Step_Curr          = -1;
            Move_Step_Time          = -1;
            Move_Step_Capa          = -1;
            Move_Step_SOC_DOD       = -1;
            //+ Add by LBg - 231012 : 초기화 과정 누락되어 추가
            Move_Step_Watt          = -1;
            Move_Step_Power         = -1;
            Move_Step_CV_Time       = -1;
            //-
            //-

            //+ CAN 조건 사용 여부 설정
            CAN_USE                 = false;
            CAN_INDEX               = new List<int>();
            CAN_CUT_OFF_INFO        = new List<CAN_CUT_OFF_INFO>();
            //-

            //+ Add by YMJ - 250716 : CAN TX 관련 값 추가
            CAN_TX_INFO = new List<CAN_TX_Info>();
            //-

            //+ CAN Power Supply 사용 설정
            CAN_POWER_USE           = false;
            CAN_POWER_VOLT          = 0;
            //-

            //+ AUX 조건 사용 여부 설정
            AUX_USE                 = false;
            AUX_V_INDEX             = new List<int>();
            AUX_T_INDEX             = new List<int>();
            //-

            //+ AUX Cell Balancing 관련 설정 정보
            AUX_Balancing_Volt_R_Type       = BALANCING_RESISTER_TYPE.R20; 
            AUX_Balancing_Volt_Lower        = 0;   // 단위 : V
            AUX_Balancing_Volt_Upper        = 0;   // 단위 : V
            AUX_Balancing_Volt_Dev_Start    = 0;   // 단위 : mV
            AUX_Balancing_Volt_Dev_End      = 0;   // 단위 : mV
            //-
        }
    }

    public struct CAN_Safety_Info
    {
        public bool     CAN_Use;
        public string   File_Name;
        public string   Signal_Name;                // Signal Name
        public int      Signal_Addr;                // Signal Address
        public int      Signal_SBit;                // Signal Start Bit
        public int      Signal_Size;                // Signal Size
        public string   Signal_ByteOrder;           // Signal Byte Order (0 : Little [L], 1 : Big [B])
        public string   Signal_Type;                // Signal Value Type ( Unsigned : U, Signed : S)
        public double   Signal_Factor;              // Signal Factor
        public int      Signal_Offset;              // Signal Offset
        public int      Signal_Min;                 // Signal Min
        public int      Signal_Max;                 // Signal Max
        public string   Signal_Unit;                // Signal Unit
        public string   Signal_Comment;             // Signal Description
        public string   Signal_Value;               // Signal Value

        public double   CAN_Safty_Upper;
        public double   CAN_Safty_Lower;
        public double   CAN_Cut_Upper;
        public double   CAN_Cut_Lower;

        public int      CODE_NO;
        public string   CODE_Name;
        public string   CODE_Desc;

        public void Reset()
        {
            File_Name           = string.Empty;
            Signal_Name         = string.Empty;
            Signal_Addr         = -1;
            Signal_SBit         = -1;
            Signal_Size         = -1;
            Signal_ByteOrder    = string.Empty;
            Signal_Type         = string.Empty;
            Signal_Factor       = -1;
            Signal_Offset       = -1;
            Signal_Min          = -1;
            Signal_Max          = -1;
            Signal_Unit         = string.Empty;
            Signal_Comment      = string.Empty;
            Signal_Value        = string.Empty;

            CAN_Safty_Upper     = -1;
            CAN_Safty_Lower     = -1;
            CAN_Cut_Upper       = -1;
            CAN_Cut_Lower       = -1;

            CODE_NO             = -1;
            CODE_Name           = string.Empty;
            CODE_Desc           = string.Empty;
        }
    }

    //+ Add by YMJ - 250716 : CAN TX 관련 값 추가
    public struct CAN_TX_Info
    {
        public int STEP_NO;
        public int Addr;
        public int Rate;
        public string Name;
        public int SBit;
        public int Size;
        public int DLC;
        public string ByteOrder;
        public string DataType;
        public double Factor;
        public int Offset;
        public int Value;

        public void Reset()
        {
            STEP_NO = -1;
            Addr = -1;
            Rate = -1;
            Name = string.Empty;
            SBit = -1;
            Size = -1;
            DLC = -1;
            ByteOrder = string.Empty;
            DataType = string.Empty;
            Factor = -1;
            Offset = -1;
            Value = -1;
        }
    }
    //-

    public struct BMS_Work_Info
    {
        public string   BMS_Name;                // Signal Name
        public int      BMS_Addr;                // Signal Address
        public int      BMS_SBit;                // Signal Start Bit
        public int      BMS_Size;                // Signal Size
        public string   BMS_ByteOrder;           // Signal Byte Order (0 : Little [L], 1 : Big [B])
        public string   BMS_Type;                // Signal Value Type ( Unsigned : U, Signed : S)
        public double BMS_Factor;              // Signal Factor
        public int      BMS_Offset;              // Signal Offset
        public int      BMS_Min;                 // Signal Min
        public int      BMS_Max;                 // Signal Max
        public string   BMS_Unit;                // Signal Unit
        public string   BMS_Comment;             // Signal Description

        public double BMS_Safty_Upper;
        public double BMS_Safty_Lower;
        public double BMS_Cut_Upper;
        public double BMS_Cut_Lower;

        public int      BMS_CODE_NO;
        public string   BMS_CODE_Name;
        public string   BMS_CODE_Desc;

        public void Reset()
        {
            BMS_Name            = string.Empty;
            BMS_Addr            = -1;
            BMS_SBit            = -1;
            BMS_Size            = -1;
            BMS_ByteOrder       = string.Empty;
            BMS_Type            = string.Empty;
            BMS_Factor          = -1;
            BMS_Offset          = -1;
            BMS_Min             = -1;
            BMS_Max             = -1;
            BMS_Unit            = string.Empty;
            BMS_Comment         = string.Empty;

            BMS_Safty_Upper     = -1;
            BMS_Safty_Lower     = -1;
            BMS_Cut_Upper       = -1;
            BMS_Cut_Lower       = -1;

            BMS_CODE_NO         = -1;
            BMS_CODE_Name       = string.Empty;
            BMS_CODE_Desc       = string.Empty;
        }
    }

    //+Add by LBG - 230725 : Step별 BMS 정보 Cut-Off 정보
    public struct CAN_CUT_OFF_INFO
    {
        public int      STEP_NO;
        public int      CAN_INDEX;
        public int      CAN_Move_Step;
        public double CAN_Cut_Upper;
        public double CAN_Cut_Lower;

        public void Reset()
        {
            STEP_NO             = -1;

            CAN_INDEX           = -1;
            CAN_Move_Step       = -1;
            CAN_Cut_Upper       = -1;
            CAN_Cut_Lower       = -1;
        }
    }
    //-

    public struct AUX_Work_Info
    {
        public int      Step_Idx;
        public int      AUX_Idx;
        public string   AUX_Name;
        public double   AUX_Upper;
        public double   AUX_Lower;
        public double   AUX_CV;

        public void Reset()
        {
            Step_Idx       = -1;
            AUX_Idx        = -1;
            AUX_Name       = string.Empty;
            AUX_Upper      = -1;
            AUX_Lower      = -1;
            AUX_CV         = -1;
        }
    }

    public struct MDBC_Info
    {
        public bool     CAN_Use;
        public string   File_Name;
        public int      Step_Index;
        public string   Signal_Name;                // Signal Name
        public int      Signal_Addr;                // Signal Address
        public int      Signal_SBit;                // Signal Start Bit
        public int      Signal_Size;                // Signal Size
        public string   Signal_ByteOrder;           // Signal Byte Order (0 : Little [L], 1 : Big [B])
        public string   Signal_Type;                // Signal Value Type ( Unsigned : U, Signed : S)
        public double Signal_Factor;              // Signal Factor
        public int      Signal_Offset;              // Signal Offset
        public int      Signal_Min;                 // Signal Min
        public int      Signal_Max;                 // Signal Max
        public string   Signal_Unit;                // Signal Unit
        public string   Signal_Comment;             // Signal Description
        public string   Signal_Value;               // Signal Value

        public double CAN_Safty_Upper;
        public double CAN_Safty_Lower;
        public double CAN_Cut_Upper;
        public double CAN_Cut_Lower;

        public int      CODE_NO;
        public string   CODE_Name;
        public string   CODE_Desc;

        public void Reset()
        {
            File_Name           = string.Empty;
            Step_Index          = -1;
            Signal_Name         = string.Empty;
            Signal_Addr         = -1;
            Signal_SBit         = -1;
            Signal_Size         = -1;
            Signal_ByteOrder    = string.Empty;
            Signal_Type         = string.Empty;
            Signal_Factor       = -1;
            Signal_Offset       = -1;
            Signal_Min          = -1;
            Signal_Max          = -1;
            Signal_Unit         = string.Empty;
            Signal_Comment      = string.Empty;
            Signal_Value        = string.Empty;

            CAN_Safty_Upper     = -1;
            CAN_Safty_Lower     = -1;
            CAN_Cut_Upper       = -1;
            CAN_Cut_Lower       = -1;

            CODE_NO             = -1;
            CODE_Name           = string.Empty;
            CODE_Desc           = string.Empty;
        }
    }

    public struct CAN_CODE_INFO
    {
        public string Code_No;
        public string Code_Name;
        public string Code_Desc;

        public void Reset()
        {
            Code_No     = string.Empty;
            Code_Name   = string.Empty;
            Code_Desc   = string.Empty;
        }
    }

    public static class AUX_Set_Table
    {
        public const string m_tb_Name_Volt = "AUX_Info_Volt";
        public const string m_tb_Name_Temp = "AUX_Info_Temp";
    }

    public struct AUX_Info_Volt
    {
        public int      AUX_V_Idx;
        public string   AUX_V_Name;
        public double   AUX_V_Upper;
        public double   AUX_V_Lower;

        public void Reset()
        {
            AUX_V_Idx   = -1;
            AUX_V_Name  = string.Empty;
            AUX_V_Upper = -1;
            AUX_V_Lower = -1;
        }
    }

    public struct AUX_Info_Temp
    {
        public int      AUX_T_Idx;
        public string   AUX_T_Name;
        public double AUX_T_Upper;
        public double AUX_T_Lower;

        public void Reset()
        {
            AUX_T_Idx   = -1;
            AUX_T_Name  = string.Empty;
            AUX_T_Upper = -1;
            AUX_T_Lower = -1;
        }
    }

    public struct AUX_Volt_Recipe
    {
        public bool     AUX_V_Recipe_Use;
        public string   AUX_V_Recipe_File;
        public int      AUX_V_Recipe_Step_Index;

        public int      AUX_V_Recipe_Idx;
        public string   AUX_V_Recipe_Name;
        public double   AUX_V_Recipe_Upper;
        public double   AUX_V_Recipe_Lower;
        public int      AUX_V_Recipe_Step;
        public bool     AUX_V_Recipe_Balance;

        public bool     AUX_V_CV_Use;
        public double   AUX_V_CV_Val;

        public void Reset()
        {
            AUX_V_Recipe_Use        = false;
            AUX_V_Recipe_File       = string.Empty;
            AUX_V_Recipe_Step_Index = -1;
            AUX_V_Recipe_Idx        = -1;
            AUX_V_Recipe_Name       = string.Empty;
            AUX_V_Recipe_Upper      = -1;
            AUX_V_Recipe_Lower      = -1;
            AUX_V_Recipe_Step       = -1;
            AUX_V_Recipe_Balance    = false;

            AUX_V_CV_Use            = false;
            AUX_V_CV_Val            = -1;
        }
    }

    public struct AUX_Temp_Recipe
    {
        public bool     AUX_T_Recipe_Use;
        public string   AUX_T_Recipe_File;
        public int      AUX_T_Recipe_Step_Index;
        public int      AUX_T_Recipe_Idx;
        public string   AUX_T_Recipe_Name;
        public double AUX_T_Recipe_Upper;
        public double AUX_T_Recipe_Lower;
        public int      AUX_T_Recipe_Step;

        public void Reset()
        {
            AUX_T_Recipe_Use    = false;
            AUX_T_Recipe_File   = string.Empty;
            AUX_T_Recipe_Step_Index = -1;
            AUX_T_Recipe_Idx    = -1;
            AUX_T_Recipe_Name   = string.Empty;
            AUX_T_Recipe_Upper  = -1;
            AUX_T_Recipe_Lower  = -1;
            AUX_T_Recipe_Step   = -1;
        }
    }

    public enum Alarm_Treat_Type
    {
        Work_None  = -1,
        Work_Resume = 0,
        Work_Stop   = 1,
        Work_NextStep = 2  // BGH
    }

    public struct Alarm_Info
    {
        public Alarm_Treat_Type         Alarm_Treat;        // 사용자 알람 처리 플래그
        public int                      CH_NO;
        public DateTime                 Alarm_Time;
        public TypeOfSafetyViolation    Alarm_Code;
        public string                   Alarm_Desc;
        public bool Alarm_NewInserted;  // 최초 발생시에 true, Display 후에 false

        public void Reset()
        {
            Alarm_Treat         = Alarm_Treat_Type.Work_None;
            CH_NO               = -1;
            Alarm_Time          = DateTime.Now;
            Alarm_Code          = TypeOfSafetyViolation.NONE;
            Alarm_Desc          = string.Empty;
            Alarm_NewInserted   = true;
        }
    }

    public struct Channel_Info
    {
        public int CH_NO;               // 호출된 CH 번호

        public bool CH_isChamberWarming;
        public bool CH_isPaused;
        // public bool SafetyWarmingUp;    // Safety 설정하는 시간동안 UI 표시 위해

        // -------------------------
        // 대기표시
        // -------------------------
        public bool CH_isWarmup;

        public STEP_TYPE CH_StepType;   // 작업 Type
        public STEP_MODE CH_STepMode;   // SStep Mode , ghbaik

        public string CH_Code;          // 동작 Code
        //public DateTime CH_Total_Time;      // 공정 진행 시간
        //public DateTime CH_Step_Time;       // Step 진행 시간
        public TimeSpan CH_TotSpent_Time;       // Step 진행 시간
        public TimeSpan CH_Spent_Time;

        public short CH_Total_Cycle;
        public short CH_Cur_Cycle;
        public int CH_Total_Step;
        public int CH_Cur_Step;

        // ghbaik
        public double CH_Volt;
        public double CH_Curr;
        public double CH_Capa;
        public double CH_Char_Capa;
        public double CH_Disc_Capa;
        public double CH_Energy;        // LBG
        public double CH_Charge_wh;     // LBG
        public double CH_Discharge_wh;  // LBG
        public double CH_Power;

        public double CH_Temperature;
        public double CH_Frequency;
        public double CH_IM;
        public double CH_RE;
        public double CH_RS;            // LBG
        public double CH_RCT;           // LBG

        public string CH_INSUL_1;  // BGH, 231029
        public string CH_INSUL_2;  // BGH, 231029

        public string Schedule_Name;
        //public string Schedule_Path;

        public string Work_Name;        //LBG

        //+ Add by LBG - 230308 : 작업 로그명 설정 기능 추가
        public bool Log_Opt_Chk_Work;
        public bool Log_Opt_Chk_File;
        public bool Log_Opt_Chk_Time;

        public string Log_Opt_File;
        //-


        public void Reset()
        {
            CH_NO = 0;               // 호출된 CH 번호
            CH_isChamberWarming = false;
            CH_isPaused = false;

            CH_isWarmup = false;

            CH_StepType = STEP_TYPE.NONE;   // 작업 Type

            CH_STepMode = STEP_MODE.NONE;

            CH_Code = "0";          // 동작 Code
            //CH_Total_Time = DateTime.Now;      // 공정 진행 시간
            //CH_Step_Time = DateTime.Now;       // Step 진행 시간 - Resume            
            CH_TotSpent_Time = new TimeSpan(0, 0, 0, 0, 0); 
            CH_Spent_Time = new TimeSpan(0, 0, 0, 0, 0); 

            CH_Total_Cycle = 0;
            CH_Cur_Cycle = 0;
            CH_Total_Step = 0;
            CH_Cur_Step = 0;

            // ghbaik
            CH_Volt = 0;
            CH_Curr = 0;
            CH_Capa = 0;
            CH_Char_Capa = 0;
            CH_Disc_Capa = 0;
            CH_Energy = 0;
            CH_Charge_wh = 0;
            CH_Discharge_wh = 0;
            CH_Power = 0;

            CH_Temperature = 0;
            CH_Frequency = -1; //  0;
            CH_IM = 0;
            CH_RE = 0;
            CH_RS = 0;
            CH_RCT = 0;

            CH_INSUL_1 = "";
            CH_INSUL_2 = "";

            Schedule_Name = "";
            //Schedule_Path = "";

            Work_Name = "";

            //+ Add by LBG - 230308 : 작업 로그명 설정 기능 추가
            Log_Opt_Chk_Work = false;
            Log_Opt_Chk_File = false;
            Log_Opt_Chk_Time = false;

            Log_Opt_File = "";
            //-
        }
    }

    public struct Pattern_Info
    {
        public int      Step_No;    // Pattern Step Index
        public string   Type;       // Pattern Type :Charge, Discharge
        public string   Mode;       // Pattern Work Mode : CC, CP, CR
        public string   Ref;        // Pattern Work Reference : A, W, Ω
        public double  Time;       // Pattern Work Time : Sec

        public void Reset()
        {
            Step_No = -1;
            Type    = string.Empty;
            Mode    = string.Empty;
            Ref     = string.Empty;
            Time = 0; // 0f;
        }
    }


    //+ Add By LBG - 22.11.18 : 채널 Status에서 Popup Menu 선택 시 반환 값
    public enum CH_Status_Popup_Ret
    {
        //+ Revision By LBG - 23.01.30 : 백이사님 요청으로 메뉴 변경 (Form -> Menustrip)
        //NONE            = -1,
        //Alarm_Clear     = 0,
        //Work_Continuous = 1,  // from Current Step, Added by ghbaik
        //Work_NextStep   = 2,  // from Next Step        
        //Work_Stop       = 3
        None            = -1,
        WorkStart       = 0,
        WorkStop        = 1,
        WorkStop_Now    = 2,
        WorkStop_Step   = 3,
        WorkStop_Cycle  = 4,
        WorkReserve     = 5,
        WorkPause       = 6,
        ContinueStart   = 7,
        NextStep        = 8,
        Initialize      = 9,
        AlarmClear      = 10,
        ScheduleView    = 11,
        ResultView      = 12,
        //+ Add By YMJ - 240416 : 챔버 정지 팝업 메뉴 추가
        GraphView       = 13,
        ChamberStop     = 14,
        //-
        //+ Add by YMJ - 250218 : 병렬 출력 모드 관련 이관 (TP코드 -> Main코드)
        SetParallelMode = 15,   // 231211 BGH
        ClearParallelMode = 16  // 231211 BGH
        //-
        //-
    }
    //-

    //+ Add By LBG - 230921 : 작업 Step 설정 기능 추가
    public struct Work_Step_Set
    {
        public bool     Start_Step_Use;
        public int      Start_Step_Idx;
        public bool     End_Step_Use;
        public int      End_Step_Idx;
        //+ Revision By YMJ - 250911 : End Step 설정 기능 수정 적용
        public int      End_Step_Cycle;
        //-

        public bool     Work_Reserve_Use;
        public DateTime Work_Reserve_Time;

        public void Reset()
        {
            Start_Step_Use      = false;
            Start_Step_Idx      = -1;
            End_Step_Use        = false;
            End_Step_Idx        = -1;

            Work_Reserve_Use    = false;
            Work_Reserve_Time   = DateTime.MinValue;
        }
    }
    //-

    //+ Add by LBG - 231012 : Cycle Start와 Cycle End의 짝이 맞는지 검사 기능 추가
    public enum Ret_Cycle_Match
    {
        Match_Ok    = 0,
        Match_Start = 1,
        Match_End   = 2,
        Match_None  = 3
    }
    //-
}