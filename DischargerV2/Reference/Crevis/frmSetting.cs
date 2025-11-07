using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;


namespace ABT
{
    public partial class frmSystem_Setting : Form
    {
        // ------------------------------------------------------
        // Information Update Event
        // ------------------------------------------------------
        //public delegate void RaiseBoardInfoUpdatedEventHandler();
        //public RaiseBoardInfoUpdatedEventHandler OnBoardInfoUpdated = null;


        clsSystem_Setting m_System_Set = null;

        ListViewItem.ListViewSubItem m_CurSubItem_BD;
        ListViewItem.ListViewSubItem m_CurSubItem_DAQ;
        ListViewItem.ListViewSubItem m_CurSubItem_DMM;
        ListViewItem.ListViewSubItem m_CurSubItem_Ins;
        ListViewItem.ListViewSubItem m_CurSubItem_DAU;
        ListViewItem.ListViewSubItem m_CurSubItem_BMS;
        ListViewItem.ListViewSubItem m_CurSubItem_MCZ;
        ListViewItem.ListViewSubItem m_CurSubItem_MBT;
        ListViewItem.ListViewSubItem m_CurSubItem_MUX;

        //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
        ListViewItem.ListViewSubItem m_CurSubItem_Chamber;
        ListViewItem.ListViewSubItem m_CurSubItem_Chiller;
        //-
        //+ Add by KGY - 20240820 : SYS Setting 항목 추가
        ListViewItem.ListViewSubItem m_CurSubItem_AUX;
        ListViewItem.ListViewSubItem m_CurSubItem_CAN;
        ListViewItem.ListViewSubItem m_CurSubItem_MTVD;
        ListViewItem.ListViewSubItem m_CurSubItem_DIMS;
        //+ Add by KGY - 250428 : 항목 추가
        ListViewItem.ListViewSubItem m_CurSubItem_UDP;
        ListViewItem.ListViewSubItem m_CurSubItem_CREVIS;
        //-
        //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
        ListViewItem.ListViewSubItem m_CurSubItem_Parallel;
        //-

        public List<Sys_Set_DAU> m_Sys_DAU_Detail;
        public List<Sys_Set_Chamber> m_Sys_ChamberDIO;
        public List<Sys_Set_AUX_Detail> m_Sys_AUX_Detail;
        public List<Sys_Set_CAN> m_Sys_CAN_Detail;
        public List<Sys_Set_CREVIS> m_Sys_CREVIS_Detail;
        //-

        //+ Revision by KGY -20240924 : m_bIsEdit전역 변수로 변경(Detail페이지의 수정여부를 위해)
        public bool m_bIsEdit;
        string m_sNTCFilePath;
        string m_sMDBCFilePath;
        //-
        Mainframe m_MainFrame;

        public frmSystem_Setting(clsSystem_Setting n_System_Set, Mainframe n_MainFrame)
        {
            InitializeComponent();

            m_System_Set = n_System_Set;
            m_MainFrame = n_MainFrame;
            m_sNTCFilePath = m_MainFrame.Get_StartPath + "\\NTC";
            m_sMDBCFilePath = m_MainFrame.Get_StartPath + "\\mdbc";

            m_Sys_DAU_Detail = new List<Sys_Set_DAU>();
            m_Sys_ChamberDIO = new List<Sys_Set_Chamber>();
            m_Sys_AUX_Detail = new List<Sys_Set_AUX_Detail>();
            m_Sys_CAN_Detail = new List<Sys_Set_CAN>();
            m_Sys_CREVIS_Detail = new List<Sys_Set_CREVIS>();

            foreach (Sys_Set_DAU n_sys_set_DAU_Detail in m_System_Set.m_Sys_DAU)
            {
                m_Sys_DAU_Detail.Add(n_sys_set_DAU_Detail);
            }
            foreach (Sys_Set_Chamber n_sys_set_ChamberDIO in m_System_Set.m_Sys_Chamber)
            {
                m_Sys_ChamberDIO.Add(n_sys_set_ChamberDIO);
            }
            foreach (Sys_Set_AUX_Detail n_sys_set_AUX_Detail in m_System_Set.m_Sys_AUX_Detail)
            {
                m_Sys_AUX_Detail.Add(n_sys_set_AUX_Detail);
            }

            foreach (Sys_Set_CAN n_sys_set_CAN_Detail in m_System_Set.m_Sys_CAN)
            {
                m_Sys_CAN_Detail.Add(n_sys_set_CAN_Detail);
            }

            foreach (Sys_Set_CREVIS n_sys_set_CREVIS_Detail in m_System_Set.m_Sys_CREVIS)
            {
                m_Sys_CREVIS_Detail.Add(n_sys_set_CREVIS_Detail);
            }
            //+ Add by LBG - 230403 : 언어 변경 적용
            clsLang_Conv n_clsLang_Conv = new clsLang_Conv(m_MainFrame, this, m_MainFrame.Get_StartPath + "\\Language\\Define_Language.xml");
            //-
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            Init_Ctrl();
            //+ Add by KGY -20241209 : MDBC파일명 읽어오기
            Get_FileName();
            //-
            m_bIsEdit = false;
        }

        private void Init_Ctrl()
        {
            if (m_System_Set != null)
            {
                chb_BD.Checked = m_System_Set.m_bUse_BD;
                chb_DAQ.Checked = m_System_Set.m_bUse_DAQ;
                chb_DMM.Checked = m_System_Set.m_bUse_DMM;
                chb_Insulate.Checked = m_System_Set.m_bUse_Ins;
                chb_DAU.Checked = m_System_Set.m_bUse_DAU;
                chb_BMS.Checked = m_System_Set.m_bUse_BMS;
                chb_MCZ.Checked = m_System_Set.m_bUse_MCZ;
                chb_MBT.Checked = m_System_Set.m_bUse_MBT;
                chb_MUX.Checked = m_System_Set.m_bUse_MUX;

                //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                chb_Chamber.Checked = m_System_Set.m_bUse_Chamber;
                chb_Chiller.Checked = m_System_Set.m_bUse_Chiller;
                //-
                //+ Add by KGY - 20240820 : SYS Setting 항목 추가
                chb_AUX.Checked = m_System_Set.m_bUse_AUX;
                chb_CAN.Checked = m_System_Set.m_bUse_CAN;
                chb_MTVD.Checked = m_System_Set.m_bUse_MTVD;
                chb_DIMS.Checked = m_System_Set.m_bUse_DIMS;
                //-
                //+ Add by KGY - 250428 : 항목 추가
                chb_UDP.Checked = m_System_Set.m_bUse_UDP;
                chb_CREVIS.Checked = m_System_Set.m_bUse_CREVIS;
                //-
                //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
                chb_Parallel.Checked = m_System_Set.m_bUse_Parallel;
                //-

                if (m_System_Set.m_bUse_BD) { Enabled_Ctrl(lv_BD.Tag, true); Display_System_Setting(lv_BD.Tag); } else { Enabled_Ctrl(lv_BD.Tag); }
                if (m_System_Set.m_bUse_DAQ) { Enabled_Ctrl(lv_DAQ.Tag, true); Display_System_Setting(lv_DAQ.Tag); } else { Enabled_Ctrl(lv_DAQ.Tag); }
                if (m_System_Set.m_bUse_DMM) { Enabled_Ctrl(lv_DMM.Tag, true); Display_System_Setting(lv_DMM.Tag); } else { Enabled_Ctrl(lv_DMM.Tag); }
                if (m_System_Set.m_bUse_Ins) { Enabled_Ctrl(lv_Ins.Tag, true); Display_System_Setting(lv_Ins.Tag); } else { Enabled_Ctrl(lv_Ins.Tag); }
                if (m_System_Set.m_bUse_DAU) { Enabled_Ctrl(lv_DAU.Tag, true); Display_System_Setting(lv_DAU.Tag); } else { Enabled_Ctrl(lv_DAU.Tag); }
                if (m_System_Set.m_bUse_BMS) { Enabled_Ctrl(lv_BMS.Tag, true); Display_System_Setting(lv_BMS.Tag); } else { Enabled_Ctrl(lv_BMS.Tag); }
                if (m_System_Set.m_bUse_MCZ) { Enabled_Ctrl(lv_MCZ.Tag, true); Display_System_Setting(lv_MCZ.Tag); } else { Enabled_Ctrl(lv_MCZ.Tag); }
                if (m_System_Set.m_bUse_MBT) { Enabled_Ctrl(lv_MBT.Tag, true); Display_System_Setting(lv_MBT.Tag); } else { Enabled_Ctrl(lv_MBT.Tag); }
                if (m_System_Set.m_bUse_MUX) { Enabled_Ctrl(lv_MUX.Tag, true); Display_System_Setting(lv_MUX.Tag); } else { Enabled_Ctrl(lv_MUX.Tag); }

                //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                if (m_System_Set.m_bUse_Chamber) { Enabled_Ctrl(lv_Chamber.Tag, true); Display_System_Setting(lv_Chamber.Tag); } else { Enabled_Ctrl(lv_Chamber.Tag); }
                if (m_System_Set.m_bUse_Chiller) { Enabled_Ctrl(lv_Chiller.Tag, true); Display_System_Setting(lv_Chiller.Tag); } else { Enabled_Ctrl(lv_Chiller.Tag); }
                //-
                //+ Add by KGY - 20240820 : SYS Setting AUX 추가
                if (m_System_Set.m_bUse_AUX) { Enabled_Ctrl(lv_AUX.Tag, true); Display_System_Setting(lv_AUX.Tag); } else { Enabled_Ctrl(lv_AUX.Tag); }
                if (m_System_Set.m_bUse_CAN) { Enabled_Ctrl(lv_CAN.Tag, true); Display_System_Setting(lv_CAN.Tag); } else { Enabled_Ctrl(lv_CAN.Tag); }
                if (m_System_Set.m_bUse_MTVD) { Enabled_Ctrl(lv_MTVD.Tag, true); Display_System_Setting(lv_MTVD.Tag); } else { Enabled_Ctrl(lv_MTVD.Tag); }
                if (m_System_Set.m_bUse_DIMS) { Enabled_Ctrl(lv_DIMS.Tag, true); Display_System_Setting(lv_DIMS.Tag); } else { Enabled_Ctrl(lv_DIMS.Tag); }
                //-
                //+ Add by KGY - 250428 : 항목 추가
                if (m_System_Set.m_bUse_UDP) { Enabled_Ctrl(lv_UDP.Tag, true); Display_System_Setting(lv_UDP.Tag); } else { Enabled_Ctrl(lv_UDP.Tag); }
                if (m_System_Set.m_bUse_CREVIS) { Enabled_Ctrl(lv_CREVIS.Tag, true); Display_System_Setting(lv_CREVIS.Tag); } else { Enabled_Ctrl(lv_CREVIS.Tag); }
                //-
                //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
                if (m_System_Set.m_bUse_Parallel) { Enabled_Ctrl(lv_Parallel.Tag, true); Display_System_Setting(lv_Parallel.Tag); } else { Enabled_Ctrl(lv_Parallel.Tag); }
                //-
            }
            else
            {
                Enabled_Ctrl(-1);
            }
        }

        private void Enabled_Ctrl(object n_Tag, bool n_bEnabled = false)
        {
            switch (n_Tag)
            {
                case "BD":
                    //+ BD
                    lv_BD.Enabled = n_bEnabled;
                    bt_BD_New.Enabled = n_bEnabled;
                    bt_BD_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "DAQ":
                    //+ DAQ
                    lv_DAQ.Enabled = n_bEnabled;
                    bt_DAQ_New.Enabled = n_bEnabled;
                    bt_DAQ_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "DMM":
                    //+ DMM
                    lv_DMM.Enabled = n_bEnabled;
                    bt_DMM_New.Enabled = n_bEnabled;
                    bt_DMM_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "Ins":
                    //+ Insulate
                    lv_Ins.Enabled = n_bEnabled;
                    bt_Insulate_New.Enabled = n_bEnabled;
                    bt_Insulate_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "DAU":
                    //+ DAU
                    lv_DAU.Enabled = n_bEnabled;
                    bt_DAU_New.Enabled = n_bEnabled;
                    bt_DAU_Del.Enabled = n_bEnabled;
                    bt_DAU_Detail.Enabled = n_bEnabled;
                    //-
                    break;
                case "BMS":
                    //+ BMS
                    lv_BMS.Enabled = n_bEnabled;
                    bt_BMS_New.Enabled = n_bEnabled;
                    bt_BMS_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "MCZ":
                    //+ MCZ
                    lv_MCZ.Enabled = n_bEnabled;
                    bt_MxZ_New.Enabled = n_bEnabled;
                    bt_MxZ_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "MBT":
                    //+ MBT
                    lv_MBT.Enabled = n_bEnabled;
                    bt_MBT_New.Enabled = n_bEnabled;
                    bt_MBT_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "MUX":
                    //+ MUX
                    lv_MUX.Enabled = n_bEnabled;
                    bt_MUX_New.Enabled = n_bEnabled;
                    bt_MUX_Del.Enabled = n_bEnabled;
                    //-
                    break;
                //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                case "Chamber":
                    //+ Chamber
                    lv_Chamber.Enabled = n_bEnabled;
                    bt_Chamber_New.Enabled = n_bEnabled;
                    bt_Chamber_Del.Enabled = n_bEnabled;
                    bt_Chamber_Detail.Enabled = n_bEnabled;
                    //-
                    break;
                case "Chiller":
                    //+ Chiller
                    lv_Chiller.Enabled = n_bEnabled;
                    bt_Chiller_New.Enabled = n_bEnabled;
                    bt_Chiller_Del.Enabled = n_bEnabled;
                    //-
                    break;
                //-
                //+ Add by KGY - 20240820 : Sys Setting 항목 추가
                case "AUX":
                    //+ AUX
                    lv_AUX.Enabled = n_bEnabled;
                    bt_AUX_New.Enabled = n_bEnabled;
                    bt_AUX_Del.Enabled = n_bEnabled;
                    bt_AUX_Detail.Enabled = n_bEnabled;
                    //-
                    break;

                case "CAN":
                    //+ CAN
                    lv_CAN.Enabled = n_bEnabled;
                    bt_CAN_New.Enabled = n_bEnabled;
                    bt_CAN_Del.Enabled = n_bEnabled;
                    bt_CAN_Detail.Enabled = n_bEnabled;
                    //-
                    break;
                case "MTVD":
                    //+ MTVD
                    lv_MTVD.Enabled = n_bEnabled;
                    bt_MTVD_New.Enabled = n_bEnabled;
                    bt_MTVD_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "DIMS":
                    //+ DIMS
                    lv_DIMS.Enabled = n_bEnabled;
                    bt_DIMS_New.Enabled = n_bEnabled;
                    bt_DIMS_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "UDP":
                    //+ UDP
                    lv_UDP.Enabled = n_bEnabled;
                    bt_UDP_New.Enabled = n_bEnabled;
                    bt_UDP_Del.Enabled = n_bEnabled;
                    //-
                    break;
                case "CREVIS":
                    //+ CREVIS
                    lv_CREVIS.Enabled = n_bEnabled;
                    bt_CREVIS_New.Enabled = n_bEnabled;
                    bt_CREVIS_Del.Enabled = n_bEnabled;
                    //-
                    break;
                //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
                case "Parallel":
                    lv_Parallel.Enabled = n_bEnabled;
                    bt_Parallel_New.Enabled = n_bEnabled;
                    bt_Parallel_Del.Enabled = n_bEnabled;
                    break;
                //-
                //-
                default:
                    //+ BD
                    lv_BD.Enabled = n_bEnabled;
                    bt_BD_New.Enabled = n_bEnabled;
                    bt_BD_Del.Enabled = n_bEnabled;
                    //-
                    //+ DAQ
                    lv_DAQ.Enabled = n_bEnabled;
                    bt_DAQ_New.Enabled = n_bEnabled;
                    bt_DAQ_Del.Enabled = n_bEnabled;
                    //-
                    //+ DMM
                    lv_DMM.Enabled = n_bEnabled;
                    bt_DMM_New.Enabled = n_bEnabled;
                    bt_DMM_Del.Enabled = n_bEnabled;
                    //-
                    //+ Insulate
                    lv_Ins.Enabled = n_bEnabled;
                    bt_Insulate_New.Enabled = n_bEnabled;
                    bt_Insulate_Del.Enabled = n_bEnabled;
                    //-
                    //+ DAU
                    lv_DAU.Enabled = n_bEnabled;
                    bt_DAU_New.Enabled = n_bEnabled;
                    bt_DAU_Del.Enabled = n_bEnabled;
                    bt_DAU_Detail.Enabled = n_bEnabled;
                    //-
                    //+ BMS
                    lv_BMS.Enabled = n_bEnabled;
                    bt_BMS_New.Enabled = n_bEnabled;
                    bt_BMS_Del.Enabled = n_bEnabled;
                    //-
                    //+ MCZ
                    lv_MCZ.Enabled = n_bEnabled;
                    bt_MxZ_New.Enabled = n_bEnabled;
                    bt_MxZ_Del.Enabled = n_bEnabled;
                    //-
                    //+ MBT
                    lv_MBT.Enabled = n_bEnabled;
                    bt_MBT_New.Enabled = n_bEnabled;
                    bt_MBT_Del.Enabled = n_bEnabled;
                    //-
                    //+ MUX
                    lv_MUX.Enabled = n_bEnabled;
                    bt_MUX_New.Enabled = n_bEnabled;
                    bt_MUX_Del.Enabled = n_bEnabled;
                    //-
                    //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                    //+ Chamber
                    lv_Chamber.Enabled = n_bEnabled;
                    bt_Chamber_New.Enabled = n_bEnabled;
                    bt_Chamber_Del.Enabled = n_bEnabled;
                    bt_Chamber_Detail.Enabled = n_bEnabled;
                    //-
                    //+ Chiller
                    lv_Chiller.Enabled = n_bEnabled;
                    bt_Chiller_New.Enabled = n_bEnabled;
                    bt_Chiller_Del.Enabled = n_bEnabled;
                    //-
                    //-
                    //+ Add by KGY - 20240820 : Sys Setting 항목 추가
                    //+ AUX
                    lv_AUX.Enabled = n_bEnabled;
                    bt_AUX_New.Enabled = n_bEnabled;
                    bt_AUX_Del.Enabled = n_bEnabled;
                    bt_AUX_Detail.Enabled = n_bEnabled;
                    //-
                    //+ CAN
                    lv_CAN.Enabled = n_bEnabled;
                    bt_CAN_New.Enabled = n_bEnabled;
                    bt_CAN_Del.Enabled = n_bEnabled;
                    //-
                    //+ MTVD
                    lv_MTVD.Enabled = n_bEnabled;
                    bt_MTVD_New.Enabled = n_bEnabled;
                    bt_MTVD_Del.Enabled = n_bEnabled;
                    //-
                    //+ DIMS
                    lv_DIMS.Enabled = n_bEnabled;
                    bt_DIMS_New.Enabled = n_bEnabled;
                    bt_DIMS_Del.Enabled = n_bEnabled;
                    //-
                    //-
                    //+ UDP
                    lv_UDP.Enabled = n_bEnabled;
                    bt_UDP_New.Enabled = n_bEnabled;
                    bt_UDP_Del.Enabled = n_bEnabled;
                    //-
                    //+ CREVIS
                    lv_CREVIS.Enabled = n_bEnabled;
                    bt_CREVIS_New.Enabled = n_bEnabled;
                    bt_CREVIS_Del.Enabled = n_bEnabled;
                    //-
                    //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
                    lv_Parallel.Enabled = n_bEnabled;
                    bt_Parallel_New.Enabled = n_bEnabled;
                    bt_Parallel_Del.Enabled = n_bEnabled;
                    //-
                    break;
            }
        }

        private void Display_System_Setting(object n_Tag)
        {
            switch (n_Tag)
            {
                case "BD":
                    //+ BD
                    lv_BD.BeginUpdate();
                    if (lv_BD.Items.Count > 0)
                    {
                        lv_BD.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_BD.Count; i++)
                    {
                        if (m_System_Set.m_Sys_BD[i].BD_No > 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_BD[i].BD_Use;
                            n_Item.SubItems.Add(m_System_Set.m_Sys_BD[i].BD_No.ToString());
                            if (m_System_Set.m_Sys_BD[i].BD_Type > -1)
                            {
                                n_Item.SubItems.Add(cb_BD_Type.Items[m_System_Set.m_Sys_BD[i].BD_Type].ToString());
                            }
                            else
                            {
                                n_Item.SubItems.Add("");
                            }
                            //+ Revision by KGY -20240924 : 값이 -1일경우 화면에 출력 안되도록 변경
                            if (m_System_Set.m_Sys_BD[i].BD_CH_Cnt != -1) { n_Item.SubItems.Add(m_System_Set.m_Sys_BD[i].BD_CH_Cnt.ToString()); }
                            else { n_Item.SubItems.Add(""); }

                            n_Item.SubItems.Add(m_System_Set.m_Sys_BD[i].BD_IP);

                            if (m_System_Set.m_Sys_BD[i].BD_TCP_Port != -1) { n_Item.SubItems.Add(m_System_Set.m_Sys_BD[i].BD_TCP_Port.ToString()); }
                            else { n_Item.SubItems.Add(""); }
                            //-
                            n_Item.SubItems.Add(m_System_Set.m_Sys_BD[i].BD_COM_Port.ToString());

                            lv_BD.Items.Add(n_Item);
                        }
                    }
                    lv_BD.EndUpdate();
                    //-
                    break;
                case "DAQ":
                    //+ DAQ
                    lv_DAQ.BeginUpdate();
                    if (lv_DAQ.Items.Count > 0)
                    {
                        lv_DAQ.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_DAQ.Count; i++)
                    {
                        if (m_System_Set.m_Sys_DAQ[i].DAQ_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_DAQ[i].DAQ_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_DAQ[i].DAQ_No+1).ToString());
                            if (m_System_Set.m_Sys_DAQ[i].DAQ_ComType > -1)
                            {
                                n_Item.SubItems.Add(cb_DAQ_ComType.Items[m_System_Set.m_Sys_DAQ[i].DAQ_ComType].ToString());
                            }
                            else
                            {
                                n_Item.SubItems.Add("");
                            }
                            n_Item.SubItems.Add(m_System_Set.m_Sys_DAQ[i].DAQ_IP);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_DAQ[i].DAQ_TCP_Port.ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_DAQ[i].DAQ_COM_Port.ToString());

                            lv_DAQ.Items.Add(n_Item);
                        }
                    }
                    lv_DAQ.EndUpdate();
                    //-
                    break;
                case "DMM":
                    //+ DMM
                    lv_DMM.BeginUpdate();
                    if (lv_DMM.Items.Count > 0)
                    {
                        lv_DMM.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_DMM.Count; i++)
                    {
                        if (m_System_Set.m_Sys_DMM[i].DMM_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_DMM[i].DMM_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_DMM[i].DMM_No+1).ToString());
                            if (m_System_Set.m_Sys_DMM[i].DMM_ComType > -1)
                            {
                                n_Item.SubItems.Add(cb_DMM_ComType.Items[m_System_Set.m_Sys_DMM[i].DMM_ComType].ToString());
                            }
                            else
                            {
                                n_Item.SubItems.Add("");
                            }
                            n_Item.SubItems.Add(m_System_Set.m_Sys_DMM[i].DMM_IP);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_DMM[i].DMM_TCP_Port.ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_DMM[i].DMM_COM_Port.ToString());

                            lv_DMM.Items.Add(n_Item);
                        }
                    }
                    lv_DMM.EndUpdate();
                    //-
                    break;
                case "Ins":
                    //+ Insulate
                    lv_Ins.BeginUpdate();
                    if (lv_Ins.Items.Count > 0)
                    {
                        lv_Ins.Items.Clear();
                    }
                    //+ Revision by KGY -20240820 : 절연저항기 Sys Setting 항목 변경
                    for (int i = 0; i < m_System_Set.m_Sys_Ins.Count; i++)
                    {
                        if (m_System_Set.m_Sys_Ins[i].Ins_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_Ins[i].Ins_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_Ins[i].Ins_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Ins[i].Ins_COM_Port);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Ins[i].Ins_Mode);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Ins[i].Ins_Rly_COM_Port);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Ins[i].Ins_Voltage.ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Ins[i].Ins_Time.ToString());

                            lv_Ins.Items.Add(n_Item);
                        }
                    }
                    //-
                    lv_Ins.EndUpdate();
                    //-
                    break;
                case "DAU":
                    //+ DAU
                    lv_DAU.BeginUpdate();
                    if (lv_DAU.Items.Count > 0)
                    {
                        lv_DAU.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_DAU.Count; i++)
                    {
                        if (m_System_Set.m_Sys_DAU[i].DAU_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_DAU[i].DAU_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_DAU[i].DAU_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_DAU[i].DAU_IP);
                            //+add by KGY -20240820 : 세부설정 여부 판단 후 Display를 위해
                            bool allItemsNotEmpty = false;
                            
                            if (m_System_Set.m_Sys_DAU[i].DAU_VModule_Count >= 0 && m_System_Set.m_Sys_DAU[i].DAU_Temp_Count>= 0 && m_System_Set.m_Sys_DAU[i].DAU_Temp_Type>=0 )
                            {
                                allItemsNotEmpty = true;
                            }
                            else
                            {
                                allItemsNotEmpty = false;
                            }
                            
                            if (allItemsNotEmpty)
                            {
                                n_Item.SubItems.Add("OK");

                            }
                            else
                            {
                                n_Item.SubItems.Add("Empty");
                            }
                            //-
                            lv_DAU.Items.Add(n_Item);
                        }
                    }
                    lv_DAU.EndUpdate();
                    //-
                    break;
                case "BMS":
                    //+ BMS
                    lv_BMS.BeginUpdate();
                    if (lv_BMS.Items.Count > 0)
                    {
                        lv_BMS.Items.Clear();
                    }
                    //+ Revision by KGY -20240820 : BMS Sys Setting 항목 변경
                    for (int i = 0; i < m_System_Set.m_Sys_BMS.Count; i++)
                    {
                        if (m_System_Set.m_Sys_BMS[i].BMS_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_BMS[i].BMS_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_BMS[i].BMS_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_BMS[i].BMS_CAN_Id);
                            //+ Revision by KGY -20240924 : 값이 -1일경우 화면에 출력 안되도록 변경
                            if (m_System_Set.m_Sys_BMS[i].BMS_Voltage_Count != -1) { n_Item.SubItems.Add(m_System_Set.m_Sys_BMS[i].BMS_Voltage_Count.ToString()); }
                            else { n_Item.SubItems.Add(""); }
                            if (m_System_Set.m_Sys_BMS[i].BMS_Temp_Count != -1) { n_Item.SubItems.Add(m_System_Set.m_Sys_BMS[i].BMS_Temp_Count.ToString()); }
                            else { n_Item.SubItems.Add(""); }
                            //-
                            lv_BMS.Items.Add(n_Item);
                        }
                    }
                    //-
                    lv_BMS.EndUpdate();
                    //-
                    break;
                case "MCZ":
                    //+ MCZ
                    lv_MCZ.BeginUpdate();
                    if (lv_MCZ.Items.Count > 0)
                    {
                        lv_MCZ.Items.Clear();
                    }
                    //+ Revision by KGY -20240820 : MCZ Sys Setting 항목 변경
                    for (int i = 0; i < m_System_Set.m_Sys_MCZ.Count; i++)
                    {
                        if (m_System_Set.m_Sys_MCZ[i].MCZ_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_MCZ[i].MCZ_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_MCZ[i].MCZ_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MCZ[i].MCZ_IP);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MCZ[i].MCZ_Port.ToString());

                            lv_MCZ.Items.Add(n_Item);
                        }
                    }
                    //-
                    lv_MCZ.EndUpdate();
                    //-
                    break;
                case "MBT":
                    //+ MBT
                    lv_MBT.BeginUpdate();
                    if (lv_MBT.Items.Count > 0)
                    {
                        lv_MBT.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_MBT.Count; i++)
                    {
                        if (m_System_Set.m_Sys_MBT[i].MBT_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_MBT[i].MBT_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_MBT[i].MBT_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MBT[i].MBT_IP);
                            //+ Revision by KGY -20240924 : 값이 -1일경우 화면에 출력 안되도록 변경
                            if (m_System_Set.m_Sys_MBT[i].MBT_TCP_Port != -1) { n_Item.SubItems.Add(m_System_Set.m_Sys_MBT[i].MBT_TCP_Port.ToString()); }
                            else { n_Item.SubItems.Add(""); }
                            //-
                            lv_MBT.Items.Add(n_Item);
                        }
                    }
                    lv_MBT.EndUpdate();
                    //-
                    break;
                case "MUX":
                    //+ MUX
                    lv_MUX.BeginUpdate();
                    if (lv_MUX.Items.Count > 0)
                    {
                        lv_MUX.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_MUX.Count; i++)
                    {
                        if (m_System_Set.m_Sys_MUX[i].MUX_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_MUX[i].MUX_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_MUX[i].MUX_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MUX[i].MUX_IP);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MUX[i].MUX_TCP_Port.ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MUX[i].MUX_CH_Cnt.ToString());

                            lv_MUX.Items.Add(n_Item);
                        }
                    }
                    lv_MUX.EndUpdate();
                    //-
                    break;
                //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                case "Chamber":
                    //+ Chamber
                    lv_Chamber.BeginUpdate();
                    if (lv_Chamber.Items.Count > 0)
                    {
                        lv_Chamber.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_Chamber.Count; i++)
                    {
                        if (m_System_Set.m_Sys_Chamber[i].Chamber_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_Chamber[i].Chamber_Use;

                            n_Item.SubItems.Add((m_System_Set.m_Sys_Chamber[i].Chamber_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Chamber[i].Chamber_Model);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Chamber[i].Chamber_COM_Port);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Chamber[i].Chamber_Baudrate);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Chamber[i].Chamber_Checksum.ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Chamber[i].Chamber_Alarm_Auto_Stop.ToString());
                            //+add by KGY -20240820 : 세부설정 여부 판단 후 Display를 위해
                            bool allItemsNotEmpty = false;
                            
                            if (m_System_Set.m_Sys_Chamber[i].ChamberDIO_Module_No >= 0 && !string.IsNullOrEmpty(m_System_Set.m_Sys_Chamber[i].ChamberDIO_COM_Port)
                                && !string.IsNullOrEmpty(m_System_Set.m_Sys_Chamber[i].ChamberDIO_Baudrate))
                            {
                                allItemsNotEmpty = true;
                            }
                            else
                            {
                                allItemsNotEmpty = false;
                            }
                              
                            if (allItemsNotEmpty)
                            {
                                n_Item.SubItems.Add("OK");

                            }
                            else
                            {
                                n_Item.SubItems.Add("Empty");
                            }
                            //-
                            lv_Chamber.Items.Add(n_Item);
                        }
                    }
                    lv_Chamber.EndUpdate();
                    //-
                    break;
                case "Chiller":
                    //+ Chiller
                    lv_Chiller.BeginUpdate();
                    if (lv_Chiller.Items.Count >= 0)
                    {
                        lv_Chiller.Items.Clear();
                    }
                    //+ Revision by KGY -20240820 : Chiller Sys Setting 항목 변경
                    for (int i = 0; i < m_System_Set.m_Sys_Chiller.Count; i++)
                    {
                        if (m_System_Set.m_Sys_Chiller[i].Chiller_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_Chiller[i].Chiller_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_Chiller[i].Chiller_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Chiller[i].Chiller_Model);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Chiller[i].Chiller_COM_Port);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Chiller[i].Chiller_Baudrate);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Chiller[i].Chiller_Checksum.ToString());

                            lv_Chiller.Items.Add(n_Item);
                        }
                    }
                    //-
                    lv_Chiller.EndUpdate();
                    //-
                    break;
                //-
                //+ Add by KGY - 20240820 : Sys Setting 항목 추가
                case "AUX":
                    //+ AUX
                    lv_AUX.BeginUpdate();
                    if (lv_AUX.Items.Count > 0)
                    {
                        lv_AUX.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_AUX.Count; i++)
                    {
                        if (m_System_Set.m_Sys_AUX[i].AUX_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_AUX[i].AUX_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_AUX[i].AUX_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_AUX[i].AUX_COM_Port);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_AUX[i].AUX_Baudrate);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_AUX[i].AUX_VModule_Count.ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_AUX[i].AUX_TModule_Count.ToString());
                            //+ Revision by KGY -20240924 : 값이 -1일경우 화면에 출력 안되도록 변경
                            if (m_System_Set.m_Sys_AUX[i].AUX_Voltage_Count != -1) { n_Item.SubItems.Add(m_System_Set.m_Sys_AUX[i].AUX_Voltage_Count.ToString()); }
                            else { n_Item.SubItems.Add(""); }
                            if (m_System_Set.m_Sys_AUX[i].AUX_Voltage_Count != -1) { n_Item.SubItems.Add(m_System_Set.m_Sys_AUX[i].AUX_Temp_Count.ToString()); }
                            else { n_Item.SubItems.Add(""); }
                            //-
                            n_Item.SubItems.Add(m_System_Set.m_Sys_AUX[i].AUX_Applied_ModuleNo.ToString());
                            lv_AUX.Items.Add(n_Item);
                        }
                    }
                    lv_AUX.EndUpdate();
                    //-
                    break;
                case "CAN":
                    //+ CAN
                    lv_CAN.BeginUpdate();
                    if (lv_CAN.Items.Count > 0)
                    {
                        lv_CAN.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_CAN.Count; i++)
                    {
                        if (m_System_Set.m_Sys_CAN[i].CAN_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_CAN[i].CAN_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_CAN[i].CAN_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_CAN[i].CAN_Type);
                            //+add by KGY -20240820 : 세부설정 여부 판단 후 Display를 위해
                            bool allItemsNotEmpty = false;
                            
                            if(m_System_Set.m_Sys_CAN[i].CAN_Type == "HS")
                            {
                                if (!string.IsNullOrEmpty(m_System_Set.m_Sys_CAN[i].CAN_ID)
                                //+ Add by YMJ - 250725 : CAN 채널 추가
                                && !string.IsNullOrEmpty(m_System_Set.m_Sys_CAN[i].CAN_CH)
                                //-
                                && !string.IsNullOrEmpty(m_System_Set.m_Sys_CAN[i].CAN_HS_Bitrate))
                                {
                                    allItemsNotEmpty = true;
                                    
                                }
                                else
                                {
                                    allItemsNotEmpty = false;
                                    
                                }
                                if (allItemsNotEmpty)
                                {
                                    n_Item.SubItems.Add("OK");

                                }
                                else
                                {
                                    n_Item.SubItems.Add("Empty");
                                }
                            }
                            else if(m_System_Set.m_Sys_CAN[i].CAN_Type == "FD")
                            {
                                if (!string.IsNullOrEmpty(m_System_Set.m_Sys_CAN[i].CAN_FD_ClockFrequency)
                                    && !string.IsNullOrEmpty(m_System_Set.m_Sys_CAN[i].CAN_FD_BitRate)
                                    && !string.IsNullOrEmpty(m_System_Set.m_Sys_CAN[i].CAN_FD_DataBitRate)
                                    && !string.IsNullOrEmpty(m_System_Set.m_Sys_CAN[i].CAN_ID)
                                    //+ Add by YMJ - 250725 : CAN 채널 추가
                                    && !string.IsNullOrEmpty(m_System_Set.m_Sys_CAN[i].CAN_CH)
                                    //-
                                    && m_System_Set.m_Sys_CAN[i].CAN_No != -1)
                                {
                                    allItemsNotEmpty = true;
                                    
                                }
                                else
                                {
                                    allItemsNotEmpty = false;
                                   
                                }

                                if (allItemsNotEmpty)
                                {
                                    n_Item.SubItems.Add("OK");

                                }
                                else
                                {
                                    n_Item.SubItems.Add("Empty");
                                }
                            }
                            n_Item.SubItems.Add(m_System_Set.m_Sys_CAN[i].CAN_MDBC);
                            lv_CAN.Items.Add(n_Item);
                        }
                    }

                    lv_CAN.EndUpdate();
                    //-
                    break;
                case "MTVD":
                    //+ MTVD
                    lv_MTVD.BeginUpdate();
                    if (lv_MTVD.Items.Count > 0)
                    {
                        lv_MTVD.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_MTVD.Count; i++)
                    {
                        if (m_System_Set.m_Sys_MTVD[i].MTVD_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_MTVD[i].MTVD_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_MTVD[i].MTVD_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MTVD[i].MTVD_Model);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MTVD[i].MTVD_VOC);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MTVD[i].MTVD_IP);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_MTVD[i].MTVD_Port);

                            lv_MTVD.Items.Add(n_Item);
                        }
                    }
                    lv_MTVD.EndUpdate();
                    //-
                    break;
                case "DIMS":
                    //+ DIMS
                    lv_DIMS.BeginUpdate();
                    if (lv_DIMS.Items.Count > 0)
                    {
                        lv_DIMS.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_DIMS.Count; i++)
                    {
                        if (m_System_Set.m_Sys_DIMS[i].DIMS_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_DIMS[i].DIMS_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_DIMS[i].DIMS_No+1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_DIMS[i].DIMS_IP);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_DIMS[i].DIMS_Port);
                            if (m_System_Set.m_Sys_DIMS[i].DIMS_Send_Interval != -1) { n_Item.SubItems.Add(m_System_Set.m_Sys_DIMS[i].DIMS_Send_Interval.ToString()); }
                            else { n_Item.SubItems.Add(""); }

                            lv_DIMS.Items.Add(n_Item);
                        }
                    }
                    lv_DIMS.EndUpdate();
                    //-
                    break;
                //-
                case "UDP":
                    //+ UDP
                    lv_UDP.BeginUpdate();
                    if (lv_UDP.Items.Count > 0)
                    {
                        lv_UDP.Items.Clear();
                    }
                    if (m_System_Set.m_Sys_UDP.Count > 0)
                    {
                        ListViewItem n_Item = new ListViewItem("");
                        n_Item.Checked = m_System_Set.m_Sys_UDP[0].UDP_Enable;
                        n_Item.SubItems.Add(m_System_Set.m_Sys_UDP[0].UDP_Server_IP);
                        n_Item.SubItems.Add(m_System_Set.m_Sys_UDP[0].UDP_Server_Port);
                        if (m_System_Set.m_Sys_UDP[0].UDP_State_Send_Interval != -1) { n_Item.SubItems.Add(m_System_Set.m_Sys_UDP[0].UDP_State_Send_Interval.ToString()); }
                        else { n_Item.SubItems.Add(""); }

                        lv_UDP.Items.Add(n_Item);
                    }
                    lv_UDP.EndUpdate();
                    break;
                //-
                case "CREVIS":
                    //+ CREVIS
                    lv_CREVIS.BeginUpdate();
                    if (lv_CREVIS.Items.Count > 0)
                    {
                        lv_CREVIS.Items.Clear();
                    }

                    for (int i = 0; i < m_System_Set.m_Sys_CREVIS.Count; i++)
                    {
                        if (m_System_Set.m_Sys_CREVIS[i].CREVIS_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_CREVIS[i].CREVIS_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_CREVIS[i].CREVIS_No + 1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_CREVIS[i].CREVIS_IP);
                            n_Item.SubItems.Add(m_System_Set.m_Sys_CREVIS[i].CREVIS_VOLT.ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_CREVIS[i].CREVIS_TEMP.ToString());
                            
                            lv_CREVIS.Items.Add(n_Item);
                        }
                    }
                    lv_CREVIS.EndUpdate();
                    break;
                //-
                //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
                case "Parallel":
                    lv_Parallel.BeginUpdate();
                    if(lv_Parallel.Items.Count > 0)
                    {
                        lv_Parallel.Items.Clear();
                    }

                    for(int i = 0; i < m_System_Set.m_Sys_Parallel.Count; i++)
                    {
                        if(m_System_Set.m_Sys_Parallel[i].Parallel_No >= 0)
                        {
                            ListViewItem n_Item = new ListViewItem("");
                            n_Item.Checked = m_System_Set.m_Sys_Parallel[i].Parallel_Use;
                            n_Item.SubItems.Add((m_System_Set.m_Sys_Parallel[i].Parallel_No + 1).ToString());
                            n_Item.SubItems.Add(m_System_Set.m_Sys_Parallel[i].Parallel_IP);

                            lv_Parallel.Items.Add(n_Item);
                        }
                    }
                    lv_Parallel.EndUpdate();
                    break;
                //-
            }
        }

        private void ckb_Use_MouseClick(object sender, MouseEventArgs e)
        {
            CheckBox n_chb_Sender = (CheckBox)sender;
            Enabled_Ctrl(n_chb_Sender.Tag, n_chb_Sender.Checked);

            if (n_chb_Sender.Checked)
            {
                Display_System_Setting(n_chb_Sender.Tag);
            }
            else
            {
                //+ Add by KGY - 20241014 : 체크 해제시 List제거
                string listViewName = "lv_" + n_chb_Sender.Tag;
                ListView n_lv_name = this.Controls.Find(listViewName, true).FirstOrDefault() as ListView;
                if (n_lv_name != null)
                {
                    n_lv_name.Items.Clear();
                }
                //-
            }
        }

        private void bt_New_Click(object sender, EventArgs e)
        {
            Button n_Btn = (Button)sender;
            ListView_Add(n_Btn.Tag);
        }

        private void ListView_Add(object n_tag)
        {
            ListViewItem n_Item = new ListViewItem("");
            n_Item.Checked = false;

            switch (n_tag)
            {
                case "BD":
                    if (lv_BD.Items.Count > 0) { n_Item.SubItems.Add((lv_BD.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_BD.Items.Add(n_Item);
                    break;
                case "DAQ":
                    if (lv_DAQ.Items.Count > 0) { n_Item.SubItems.Add((lv_DAQ.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_DAQ.Items.Add(n_Item);
                    break;
                case "DMM":
                    if (lv_DMM.Items.Count > 0) { n_Item.SubItems.Add((lv_DMM.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_DMM.Items.Add(n_Item);
                    break;
                //+ Revision by KGY -20240820 : 절연저항기 Sys Setting 항목 변경
                case "Ins":
                    if (lv_Ins.Items.Count > 0) { n_Item.SubItems.Add((lv_Ins.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_Ins.Items.Add(n_Item);
                    break;
                //-
                //+ Revision by KGY -20240820 : DAU Sys Setting 항목 변경
                case "DAU":
                    if (lv_DAU.Items.Count > 0) { n_Item.SubItems.Add((lv_DAU.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("Empty");

                    lv_DAU.Items.Add(n_Item);
                    break;
                //-
                //+ Revision by KGY -20240820 : BMS Sys Setting 항목 변경
                case "BMS":
                    if (lv_BMS.Items.Count > 0) { n_Item.SubItems.Add((lv_BMS.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_BMS.Items.Add(n_Item);
                    break;
                case "MCZ":
                    if (lv_MCZ.Items.Count > 0) { n_Item.SubItems.Add((lv_MCZ.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_MCZ.Items.Add(n_Item);
                    break;
                case "MBT":
                    if (lv_MBT.Items.Count > 0) { n_Item.SubItems.Add((lv_MBT.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_MBT.Items.Add(n_Item);
                    break;
                case "MUX":
                    if (lv_MUX.Items.Count > 0) { n_Item.SubItems.Add((lv_MUX.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_MUX.Items.Add(n_Item);
                    break;
                //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                //+ Revision by KGY -20240820 : Chiller Sys Setting 항목 변경
                case "Chamber":
                    if (lv_Chamber.Items.Count > 0) { n_Item.SubItems.Add((lv_Chamber.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("Empty");

                    lv_Chamber.Items.Add(n_Item);
                    break;
                //-
                case "Chiller":
                    if (lv_Chiller.Items.Count > 0) { n_Item.SubItems.Add((lv_Chiller.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");


                    lv_Chiller.Items.Add(n_Item);
                    break;
                //-
                //+ Add by KGY - 20240820 : Sys Setting 항목 추가
                case "AUX":
                    if (lv_AUX.Items.Count > 0) { n_Item.SubItems.Add((lv_AUX.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_AUX.Items.Add(n_Item);
                    break;
                case "CAN":
                    if (lv_CAN.Items.Count > 0) { n_Item.SubItems.Add((lv_CAN.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("Empty");
                    n_Item.SubItems.Add("");

                    lv_CAN.Items.Add(n_Item);
                    break;
                case "MTVD":
                    if (lv_MTVD.Items.Count > 0) { n_Item.SubItems.Add((lv_MTVD.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_MTVD.Items.Add(n_Item);
                    break;
                case "DIMS":
                    if (lv_DIMS.Items.Count > 0) { n_Item.SubItems.Add((lv_DIMS.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_DIMS.Items.Add(n_Item);
                    break;
                //-
                case "UDP":
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_UDP.Items.Add(n_Item);
                    break;
                case "CREVIS":
                    if (lv_CREVIS.Items.Count > 0) { n_Item.SubItems.Add((lv_CREVIS.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");
                    n_Item.SubItems.Add("");

                    lv_CREVIS.Items.Add(n_Item);
                    break;
                //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
                case "Parallel":
                    if(lv_Parallel.Items.Count > 0) { n_Item.SubItems.Add((lv_Parallel.Items.Count + 1).ToString()); } else { n_Item.SubItems.Add("1"); }
                    n_Item.SubItems.Add("");

                    lv_Parallel.Items.Add(n_Item);
                    break;
                //-
            }
        }

        private void bt_Del_Click(object sender, EventArgs e)
        {
            Button n_Btn = (Button)sender;
            ListView_Del(n_Btn.Tag);
            m_bIsEdit = true;
        }

        private void ListView_Del(object n_tag)
        {
            switch (n_tag)
            {
                case "BD":
                    if (lv_BD.Items.Count > 1) { lv_BD.Items.RemoveAt(lv_BD.Items.Count - 1); }
                    break;
                case "DAQ":
                    if (lv_DAQ.Items.Count > 0) { lv_DAQ.Items.RemoveAt(lv_DAQ.Items.Count - 1); }
                    break;
                case "DMM":
                    if (lv_DMM.Items.Count > 0) { lv_DMM.Items.RemoveAt(lv_DMM.Items.Count - 1); }
                    break;
                case "Ins":
                    if (lv_Ins.Items.Count > 0) { lv_Ins.Items.RemoveAt(lv_Ins.Items.Count - 1); }
                    break;
                case "DAU":
                    if (lv_DAU.Items.Count > 0) { lv_DAU.Items.RemoveAt(lv_DAU.Items.Count - 1); }
                    break;
                case "BMS":
                    if (lv_BMS.Items.Count > 0) { lv_BMS.Items.RemoveAt(lv_BMS.Items.Count - 1); }
                    break;
                case "MCZ":
                    if (lv_MCZ.Items.Count > 0) { lv_MCZ.Items.RemoveAt(lv_MCZ.Items.Count - 1); }
                    break;
                case "MBT":
                    if (lv_MBT.Items.Count > 0) { lv_MBT.Items.RemoveAt(lv_MBT.Items.Count - 1); }
                    break;
                case "MUX":
                    if (lv_MUX.Items.Count > 0) { lv_MUX.Items.RemoveAt(lv_MUX.Items.Count - 1); }
                    break;
                //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                case "Chamber":
                    if (lv_Chamber.Items.Count > 0) { lv_Chamber.Items.RemoveAt(lv_Chamber.Items.Count - 1); }
                    break;
                case "Chiller":
                    if (lv_Chiller.Items.Count > 0) { lv_Chiller.Items.RemoveAt(lv_Chiller.Items.Count - 1); }
                    break;
                //-
                //+ Add by KGY - 20240820 : Sys Setting 항목 추가
                case "AUX":
                    if (lv_AUX.Items.Count > 0) { lv_AUX.Items.RemoveAt(lv_AUX.Items.Count - 1); }
                    break;
                case "CAN":
                    if (lv_CAN.Items.Count > 0) { lv_CAN.Items.RemoveAt(lv_CAN.Items.Count - 1); }
                    break;
                case "MTVD":
                    if (lv_MTVD.Items.Count > 0) { lv_MTVD.Items.RemoveAt(lv_MTVD.Items.Count - 1); }
                    break;
                case "DIMS":
                    if (lv_DIMS.Items.Count > 0) { lv_DIMS.Items.RemoveAt(lv_DIMS.Items.Count - 1); }
                    break;
                //-
                case "UDP":
                    if (lv_UDP.Items.Count > 0) { lv_UDP.Items.RemoveAt(lv_UDP.Items.Count - 1); }
                    break;
                case "CREVIS":
                    if (lv_CREVIS.Items.Count > 0) { lv_CREVIS.Items.RemoveAt(lv_CREVIS.Items.Count - 1); }
                    break;
                //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
                case "Parallel":
                    if (lv_Parallel.Items.Count > 0) { lv_Parallel.Items.RemoveAt(lv_Parallel.Items.Count - 1); }
                    break;
                //-
            }
        }

        private void ListView_MouseClick(object sender, MouseEventArgs e)
        {
            ListView n_ListView = (ListView)sender;

            ListViewItem n_Item = n_ListView.GetItemAt(e.X, e.Y);
            ListViewItem.ListViewSubItem n_ItemSub = n_Item.GetSubItemAt(e.X, e.Y);

            if (n_Item != null && n_ItemSub != null)
            {
                ListView_Edit(n_ListView.Tag, n_Item, n_ItemSub);
            }
        }

        private void ListView_Edit(object n_tag, ListViewItem n_Item, ListViewItem.ListViewSubItem n_ItemSub)
        {
            int n_idxSub = n_Item.SubItems.IndexOf(n_ItemSub);
            int n_idxItem = n_Item.Index;
            int n_Posleft = n_ItemSub.Bounds.Left + 2;
            int n_PosWidth = n_ItemSub.Bounds.Width;

            if (n_idxSub == 0 && n_idxSub == 1)
            {
                return;
            }

            switch (n_tag)
            {
                case "BD":
                    ListView_Edit_BD(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "DAQ":
                    ListView_Edit_DAQ(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "DMM":
                    ListView_Edit_DMM(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "Ins":
                    ListView_Edit_Ins(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "DAU":
                    ListView_Edit_DAU(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "BMS":
                    ListView_Edit_BMS(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "MCZ":
                    ListView_Edit_MCZ(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "MBT":
                    ListView_Edit_MBT(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "MUX":
                    ListView_Edit_MUX(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                case "Chamber":
                    ListView_Edit_Chamber(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "Chiller":
                    ListView_Edit_Chiller(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                //-
                //+ Add by KGY - 20240820 : Sys Setting 항목 추가
                case "AUX":
                    ListView_Edit_AUX(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "CAN":
                    ListView_Edit_CAN(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth, n_idxItem);
                    break;
                case "MTVD":
                    ListView_Edit_MTVD(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "DIMS":
                    ListView_Edit_DIMS(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                //-
                case "UDP":
                    ListView_Edit_UDP(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                case "CREVIS":
                    ListView_Edit_CREVIS(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
                case "Parallel":
                    ListView_Edit_Parallel(n_ItemSub, n_idxSub, n_Posleft, n_PosWidth);
                    break;
                //-
            }
        }

        private void ListView_Edit_BD(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                cb_BD_Type.SetBounds(n_Posleft + lv_BD.Left, n_ItemSub.Bounds.Top + lv_BD.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_BD_Type.Items.Count; i++)
                    {
                        if (cb_BD_Type.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_BD_Type.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_BD_Type.Show();
                cb_BD_Type.Focus();
            }
            else if (n_idxSub == 4)
            {
                txt_BD_IP_Input.SetBounds(n_Posleft + lv_BD.Left, n_ItemSub.Bounds.Top + lv_BD.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_BD_IP_Input.Text = "";
                }
                else
                {
                    txt_BD_IP_Input.Text = n_ItemSub.Text;
                }

                txt_BD_IP_Input.Show();
                txt_BD_IP_Input.Focus();
                txt_BD_IP_Input.SelectAll();
            }
            else if (n_idxSub == 3 || n_idxSub == 5)
            {
                txt_BD_Num_Input.SetBounds(n_Posleft + lv_BD.Left, n_ItemSub.Bounds.Top + lv_BD.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_BD_Num_Input.Text = "";
                }
                else
                {
                    txt_BD_Num_Input.Text = n_ItemSub.Text;
                }

                txt_BD_Num_Input.Show();
                txt_BD_Num_Input.Focus();   
                txt_BD_Num_Input.SelectAll();
            }
            else if (n_idxSub == 6)
            {
                cb_BD_ComPort.SetBounds(n_Posleft + lv_BD.Left, n_ItemSub.Bounds.Top + lv_BD.Top, n_PosWidth, n_ItemSub.Bounds.Height);
                
                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_BD_ComPort.Items.Count; i++)
                    {
                        if (cb_BD_ComPort.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_BD_ComPort.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_BD_ComPort.Show();
                cb_BD_ComPort.Focus();
            }
            m_CurSubItem_BD = n_ItemSub;
        }

        private void ListView_Edit_DAQ(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                cb_DAQ_ComType.SetBounds(n_Posleft + lv_DAQ.Left, n_ItemSub.Bounds.Top + lv_DAQ.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_DAQ_ComType.Items.Count; i++)
                    {
                        if (cb_DAQ_ComType.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_DAQ_ComType.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_DAQ_ComType.Show();
                cb_DAQ_ComType.Focus();
            }
            else if (n_idxSub == 3 || n_idxSub == 4)
            {
                txt_DAQ_Input.SetBounds(n_Posleft + lv_DAQ.Left, n_ItemSub.Bounds.Top + lv_DAQ.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_DAQ_Input.Text = "";
                }
                else
                {
                    txt_DAQ_Input.Text = n_ItemSub.Text;
                }

                txt_DAQ_Input.Show();
                txt_DAQ_Input.Focus();
                txt_DAQ_Input.SelectAll();
            }
            else if (n_idxSub == 5)
            {
                cb_DAQ_ComPort.SetBounds(n_Posleft + lv_DAQ.Left, n_ItemSub.Bounds.Top + lv_DAQ.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                cb_DAQ_ComPort.Items.Clear();
                string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string port in availablePorts)
                {
                    cb_DAQ_ComPort.Items.Add(port);
                }

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_DAQ_ComPort.Items.Count; i++)
                    {
                        if (cb_DAQ_ComPort.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_DAQ_ComPort.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_DAQ_ComPort.Show();
                cb_DAQ_ComPort.Focus();
            }

            m_CurSubItem_DAQ = n_ItemSub;
        }

        private void ListView_Edit_DMM(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {

            if (n_idxSub == 2)
            {
                cb_DMM_ComType.SetBounds(n_Posleft + lv_DMM.Left, n_ItemSub.Bounds.Top + lv_DMM.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_DMM_ComType.Items.Count; i++)
                    {
                        if (cb_DMM_ComType.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_DMM_ComType.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_DMM_ComType.Show();
                cb_DMM_ComType.Focus();
            }
            else if (n_idxSub == 3 || n_idxSub == 4)
            {
                txt_DMM_Input.SetBounds(n_Posleft + lv_DMM.Left, n_ItemSub.Bounds.Top + lv_DMM.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_DMM_Input.Text = "";
                }
                else
                {
                    txt_DMM_Input.Text = n_ItemSub.Text;
                }

                txt_DMM_Input.Show();
                txt_DMM_Input.Focus();
                txt_DMM_Input.SelectAll();
            }
            else if (n_idxSub == 5)
            {
                cb_DMM_ComPort.SetBounds(n_Posleft + lv_DMM.Left, n_ItemSub.Bounds.Top + lv_DMM.Top, n_PosWidth, n_ItemSub.Bounds.Height);
                cb_DMM_ComPort.Items.Clear();
                string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string port in availablePorts)
                {
                    cb_DMM_ComPort.Items.Add(port);
                }
                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_DMM_ComPort.Items.Count; i++)
                    {
                        if (cb_DMM_ComPort.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_DMM_ComPort.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_DMM_ComPort.Show();
                cb_DMM_ComPort.Focus();
            }

            m_CurSubItem_DMM = n_ItemSub;
        }

        private void ListView_Edit_Ins(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {

            if (n_idxSub == 2)
            {
                cb_Ins_ComPort.SetBounds(n_Posleft + lv_Ins.Left, n_ItemSub.Bounds.Top + lv_Ins.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                cb_Ins_ComPort.Items.Clear();
                string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string port in availablePorts)
                {
                    cb_Ins_ComPort.Items.Add(port);
                }

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Ins_ComPort.Items.Count; i++)
                    {
                        if (cb_Ins_ComPort.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Ins_ComPort.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Ins_ComPort.Show();
                cb_Ins_ComPort.Focus();
            }
            else if (n_idxSub == 3)
            {
                cb_Ins_Mode.SetBounds(n_Posleft + lv_Ins.Left, n_ItemSub.Bounds.Top + lv_Ins.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Ins_Mode.Items.Count; i++)
                    {
                        if (cb_Ins_Mode.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Ins_Mode.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Ins_Mode.Show();
                cb_Ins_Mode.Focus();
            }
            else if (n_idxSub == 4)
            {
                cb_Ins_Rly_ComPort.SetBounds(n_Posleft + lv_Ins.Left, n_ItemSub.Bounds.Top + lv_Ins.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                cb_Ins_Rly_ComPort.Items.Clear();
                string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string port in availablePorts)
                {
                    cb_Ins_Rly_ComPort.Items.Add(port);
                }

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Ins_Rly_ComPort.Items.Count; i++)
                    {
                        if (cb_Ins_Rly_ComPort.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Ins_Rly_ComPort.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Ins_Rly_ComPort.Show();
                cb_Ins_Rly_ComPort.Focus();
            }
            else if (n_idxSub == 5 || n_idxSub == 6)
            {
                txt_insulate_Num_Input.SetBounds(n_Posleft + lv_Ins.Left, n_ItemSub.Bounds.Top + lv_Ins.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_insulate_Num_Input.Text = "";
                }
                else
                {
                    txt_insulate_Num_Input.Text = n_ItemSub.Text;
                }

                txt_insulate_Num_Input.Show();
                txt_insulate_Num_Input.Focus();
                txt_insulate_Num_Input.SelectAll();
            }
            m_CurSubItem_Ins = n_ItemSub;
        }

        private void ListView_Edit_DAU(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                txt_DAU_IP_Input.SetBounds(n_Posleft + lv_DAU.Left, n_ItemSub.Bounds.Top + lv_DAU.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_DAU_IP_Input.Text = "";
                }
                else
                {
                    txt_DAU_IP_Input.Text = n_ItemSub.Text;
                }

                txt_DAU_IP_Input.Show();
                txt_DAU_IP_Input.Focus();
                txt_DAU_IP_Input.SelectAll();
            }
            else if (n_idxSub == 3)
            {
                FrmDetail_Setting_View("DAU");
            }

            m_CurSubItem_DAU = n_ItemSub;
        }

        private void ListView_Edit_BMS(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {

            if (n_idxSub == 2)
            {
                txt_BMS_Input.SetBounds(n_Posleft + lv_BMS.Left, n_ItemSub.Bounds.Top + lv_BMS.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_BMS_Input.Text = "";
                }
                else
                {
                    txt_BMS_Input.Text = n_ItemSub.Text;
                }

                txt_BMS_Input.Show();
                txt_BMS_Input.Focus();

            }
            else if (n_idxSub == 3 || n_idxSub == 4)
            {
                txt_BMS_Num_Input.SetBounds(n_Posleft + lv_BMS.Left, n_ItemSub.Bounds.Top + lv_BMS.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_BMS_Num_Input.Text = "";
                }
                else
                {
                    txt_BMS_Num_Input.Text = n_ItemSub.Text;
                }

                txt_BMS_Num_Input.Show();
                txt_BMS_Num_Input.Focus();
            }
            m_CurSubItem_BMS = n_ItemSub;
        }

        private void ListView_Edit_MCZ(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            //if (n_idxSub == 2)
            //{
            //    cb_MxZ_Type.SetBounds(n_Posleft + lv_MCZ.Left, n_ItemSub.Bounds.Top + lv_MCZ.Top, n_PosWidth, n_ItemSub.Bounds.Height);

            //    if (!string.IsNullOrEmpty(n_ItemSub.Text))
            //    {
            //        for (int i = 0; i < cb_MxZ_Type.Items.Count; i++)
            //        {
            //            if (cb_MxZ_Type.Items[i].Equals(n_ItemSub.Text))
            //            {
            //                cb_MxZ_Type.SelectedIndex = i;
            //                break;
            //            }
            //        }
            //    }

            //    cb_MxZ_Type.Show();
            //    cb_MxZ_Type.Focus();
            //}
            //else if (n_idxSub == 3)
            //{
            //    cb_MCZ_ComPort.SetBounds(n_Posleft + lv_MCZ.Left, n_ItemSub.Bounds.Top + lv_MCZ.Top, n_PosWidth, n_ItemSub.Bounds.Height);

            //    cb_MCZ_ComPort.Items.Clear();
            //    string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
            //    foreach (string port in availablePorts)
            //    {
            //        cb_MCZ_ComPort.Items.Add(port);
            //    }

            //    if (!string.IsNullOrEmpty(n_ItemSub.Text))
            //    {
            //        for (int i = 0; i < cb_MCZ_ComPort.Items.Count; i++)
            //        {
            //            if (cb_MCZ_ComPort.Items[i].Equals(n_ItemSub.Text))
            //            {
            //                cb_MCZ_ComPort.SelectedIndex = i;
            //                break;
            //            }
            //        }
            //    }

            //    cb_MCZ_ComPort.Show();
            //    cb_MCZ_ComPort.Focus();
            //}
            if (n_idxSub == 2)
            {
                txt_MCZ_IP_Input.SetBounds(n_Posleft + lv_BMS.Left, n_ItemSub.Bounds.Top + lv_BMS.Top, n_PosWidth, n_ItemSub.Bounds.Height);
                
                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_MCZ_IP_Input.Text = "";
                }
                else
                {
                    txt_MCZ_IP_Input.Text = n_ItemSub.Text;
                }

                txt_MCZ_IP_Input.Show();
                txt_MCZ_IP_Input.Focus();
            }
            else if (n_idxSub == 3)
            {
                txt_MCZ_Num_Input.SetBounds(n_Posleft + lv_BMS.Left, n_ItemSub.Bounds.Top + lv_BMS.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_MCZ_Num_Input.Text = "";
                }
                else
                {
                    txt_MCZ_Num_Input.Text = n_ItemSub.Text;
                }

                txt_MCZ_Num_Input.Show();
                txt_MCZ_Num_Input.Focus();
            }

            m_CurSubItem_MCZ = n_ItemSub;
        }

        private void ListView_Edit_MBT(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2 )
            {
                txt_MBT_IP_Input.SetBounds(n_Posleft + lv_MBT.Left, n_ItemSub.Bounds.Top + lv_MBT.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_MBT_IP_Input.Text = "";
                }
                else
                {
                    txt_MBT_IP_Input.Text = n_ItemSub.Text;
                }

                txt_MBT_IP_Input.Show();
                txt_MBT_IP_Input.Focus();
                txt_MBT_IP_Input.SelectAll();
            }else if (n_idxSub == 3)
            {
                txt_MBT_Num_Input.SetBounds(n_Posleft + lv_MBT.Left, n_ItemSub.Bounds.Top + lv_MBT.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_MBT_Num_Input.Text = "";
                }
                else
                {
                    txt_MBT_Num_Input.Text = n_ItemSub.Text;
                }

                txt_MBT_Num_Input.Show();
                txt_MBT_Num_Input.Focus();
                txt_MBT_Num_Input.SelectAll();
            }
            m_CurSubItem_MBT = n_ItemSub;
        }

        private void ListView_Edit_MUX(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (2 <= n_idxSub && n_idxSub <= 4)
            {
                txt_MUX_Input.SetBounds(n_Posleft + lv_MUX.Left, n_ItemSub.Bounds.Top + lv_MUX.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_MUX_Input.Text = "";
                }
                else
                {
                    txt_MUX_Input.Text = n_ItemSub.Text;
                }

                txt_MUX_Input.Show();
                txt_MUX_Input.Focus();
                txt_MUX_Input.SelectAll();
            }
            m_CurSubItem_MUX = n_ItemSub;
        }

        //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
        private void ListView_Edit_Chamber(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                cb_Chamber_Model.SetBounds(n_Posleft + lv_Chamber.Left, n_ItemSub.Bounds.Top + lv_Chamber.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Chamber_Model.Items.Count; i++)
                    {
                        if (cb_Chamber_Model.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Chamber_Model.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Chamber_Model.Show();
                cb_Chamber_Model.Focus();
            }
            else if (n_idxSub == 3)
            {
                cb_Chamber_ComPort.SetBounds(n_Posleft + lv_Chamber.Left, n_ItemSub.Bounds.Top + lv_Chamber.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                cb_Chamber_ComPort.Items.Clear();
                string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string port in availablePorts)
                {
                    cb_Chamber_ComPort.Items.Add(port);
                }

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Chamber_ComPort.Items.Count; i++)
                    {
                        if (cb_Chamber_ComPort.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Chamber_ComPort.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Chamber_ComPort.Show();
                cb_Chamber_ComPort.Focus();
            }
            else if (n_idxSub == 4)
            {
                cb_Chamber_Baudrate.SetBounds(n_Posleft + lv_Chamber.Left, n_ItemSub.Bounds.Top + lv_Chamber.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Chamber_Baudrate.Items.Count; i++)
                    {
                        if (cb_Chamber_Baudrate.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Chamber_Baudrate.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Chamber_Baudrate.Show();
                cb_Chamber_Baudrate.Focus();
            }
            else if (n_idxSub == 5)
            {
                cb_Chamber_Checksum.SetBounds(n_Posleft + lv_Chamber.Left, n_ItemSub.Bounds.Top + lv_Chamber.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Chamber_Checksum.Items.Count; i++)
                    {
                        if (cb_Chamber_Checksum.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Chamber_Checksum.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Chamber_Checksum.Show();
                cb_Chamber_Checksum.Focus();
            }
            else if (n_idxSub == 6)
            {
                cb_Chamber_Alarm_Auto_Stop.SetBounds(n_Posleft + lv_Chamber.Left, n_ItemSub.Bounds.Top + lv_Chamber.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Chamber_Alarm_Auto_Stop.Items.Count; i++)
                    {
                        if (cb_Chamber_Alarm_Auto_Stop.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Chamber_Alarm_Auto_Stop.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Chamber_Alarm_Auto_Stop.Show();
                cb_Chamber_Alarm_Auto_Stop.Focus();
            }
            else if (n_idxSub == 7)
            {
                FrmDetail_Setting_View("Chamber");
            }

            m_CurSubItem_Chamber = n_ItemSub;
        }

        private void ListView_Edit_Chiller(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                cb_Chiller_Model.SetBounds(n_Posleft + lv_Chiller.Left, n_ItemSub.Bounds.Top + lv_Chiller.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Chiller_Model.Items.Count; i++)
                    {
                        if (cb_Chiller_Model.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Chiller_Model.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Chiller_Model.Show();
                cb_Chiller_Model.Focus();
            }
            else if (n_idxSub == 3)
            {
                cb_Chiller_ComPort.SetBounds(n_Posleft + lv_Chiller.Left, n_ItemSub.Bounds.Top + lv_Chiller.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                cb_Chiller_ComPort.Items.Clear();
                string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string port in availablePorts)
                {
                    cb_Chiller_ComPort.Items.Add(port);
                }

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Chiller_ComPort.Items.Count; i++)
                    {
                        if (cb_Chiller_ComPort.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Chiller_ComPort.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Chiller_ComPort.Show();
                cb_Chiller_ComPort.Focus();
            }
            else if (n_idxSub == 4)
            {
                cb_Chiller_Baudrate.SetBounds(n_Posleft + lv_Chiller.Left, n_ItemSub.Bounds.Top + lv_Chiller.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Chiller_Baudrate.Items.Count; i++)
                    {
                        if (cb_Chiller_Baudrate.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Chiller_Baudrate.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Chiller_Baudrate.Show();
                cb_Chiller_Baudrate.Focus();
            }
            else if (n_idxSub == 5)
            {
                cb_Chiller_Checksum.SetBounds(n_Posleft + lv_Chiller.Left, n_ItemSub.Bounds.Top + lv_Chiller.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_Chiller_Checksum.Items.Count; i++)
                    {
                        if (cb_Chiller_Checksum.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_Chiller_Checksum.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_Chiller_Checksum.Show();
                cb_Chiller_Checksum.Focus();
            }

            m_CurSubItem_Chiller = n_ItemSub;
        }
        //-
        private void ListView_Edit_AUX(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                cb_AUX_ComPort.SetBounds(n_Posleft + lv_AUX.Left, n_ItemSub.Bounds.Top + lv_AUX.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                cb_AUX_ComPort.Items.Clear();
                string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string port in availablePorts)
                {
                    cb_AUX_ComPort.Items.Add(port);
                }

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_AUX_ComPort.Items.Count; i++)
                    {
                        if (cb_AUX_ComPort.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_AUX_ComPort.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_AUX_ComPort.Show();
                cb_AUX_ComPort.Focus();
            }
            else if (n_idxSub == 3)
            {
                cb_AUX_Baudrate.SetBounds(n_Posleft + lv_AUX.Left, n_ItemSub.Bounds.Top + lv_AUX.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_AUX_Baudrate.Items.Count; i++)
                    {
                        if (cb_AUX_Baudrate.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_AUX_Baudrate.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_AUX_Baudrate.Show();
                cb_AUX_Baudrate.Focus();
            }
            else if (n_idxSub == 4)
            {
                txt_AUX_VModule_Input.SetBounds(n_Posleft + lv_AUX.Left, n_ItemSub.Bounds.Top + lv_AUX.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_AUX_VModule_Input.Text = "";
                }
                else
                {
                    txt_AUX_VModule_Input.Text = n_ItemSub.Text;
                }

                txt_AUX_VModule_Input.Show();
                txt_AUX_VModule_Input.Focus();
                txt_AUX_VModule_Input.SelectAll();
            }
            else if (n_idxSub == 5)
            {
                txt_AUX_TModule_Input.SetBounds(n_Posleft + lv_AUX.Left, n_ItemSub.Bounds.Top + lv_AUX.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_AUX_TModule_Input.Text = "";
                }
                else
                {
                    txt_AUX_TModule_Input.Text = n_ItemSub.Text;
                }

                txt_AUX_TModule_Input.Show();
                txt_AUX_TModule_Input.Focus();
                txt_AUX_TModule_Input.SelectAll();
            }
            else if (n_idxSub == 6 || n_idxSub == 7)
            {
                FrmDetail_Setting_View("AUX");
            }
            else if (n_idxSub == 8)
            {
                txt_AUX_Input.SetBounds(n_Posleft + lv_AUX.Left, n_ItemSub.Bounds.Top + lv_AUX.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_AUX_Input.Text = "";
                }
                else
                {
                    txt_AUX_Input.Text = n_ItemSub.Text;
                }

                txt_AUX_Input.Show();
                txt_AUX_Input.Focus();
                txt_AUX_Input.SelectAll();
            }

            m_CurSubItem_AUX = n_ItemSub;
        }
        private void ListView_Edit_CAN(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth,int n_idxItem)
        {
            if (n_idxSub == 2)
            {
                cb_CAN_Type.SetBounds(n_Posleft + lv_CAN.Left, n_ItemSub.Bounds.Top + lv_CAN.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_CAN_Type.Items.Count; i++)
                    {
                        if (cb_CAN_Type.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_CAN_Type.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_CAN_Type.Show();
                cb_CAN_Type.Focus();
            }
            else if (n_idxSub == 3)
            {
                
                FrmDetail_Setting_View("CAN");
               
            }
            else if (n_idxSub == 4)
            {
                cb_CAN_mdbc.SetBounds(n_Posleft + lv_CAN.Left, n_ItemSub.Bounds.Top + lv_CAN.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_CAN_mdbc.Items.Count; i++)
                    {
                        if (cb_CAN_mdbc.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_CAN_mdbc.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_CAN_mdbc.Show();
                cb_CAN_mdbc.Focus();
            }
            m_CurSubItem_CAN = n_ItemSub;
        }
        private void ListView_Edit_MTVD(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                cb_MTVD_Model.SetBounds(n_Posleft + lv_MTVD.Left, n_ItemSub.Bounds.Top + lv_MTVD.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (!string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    for (int i = 0; i < cb_MTVD_Model.Items.Count; i++)
                    {
                        if (cb_MTVD_Model.Items[i].Equals(n_ItemSub.Text))
                        {
                            cb_MTVD_Model.SelectedIndex = i;
                            break;
                        }
                    }
                }

                cb_MTVD_Model.Show();
                cb_MTVD_Model.Focus();
            }
            else if (n_idxSub == 3 || n_idxSub == 5)
            {
                txt_MTVD_Num_Input.SetBounds(n_Posleft + lv_MTVD.Left, n_ItemSub.Bounds.Top + lv_MTVD.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_MTVD_Num_Input.Text = "";
                }
                else
                {
                    txt_MTVD_Num_Input.Text = n_ItemSub.Text;
                }

                txt_MTVD_Num_Input.Show();
                txt_MTVD_Num_Input.Focus();
                txt_MTVD_Num_Input.SelectAll();

            }
            else if (n_idxSub == 4)
            {
                txt_MTVD_IP_Input.SetBounds(n_Posleft + lv_MTVD.Left, n_ItemSub.Bounds.Top + lv_MTVD.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_MTVD_IP_Input.Text = "";
                }
                else
                {
                    txt_MTVD_IP_Input.Text = n_ItemSub.Text;
                }

                txt_MTVD_IP_Input.Show();
                txt_MTVD_IP_Input.Focus();
                txt_MTVD_IP_Input.SelectAll();

            }
            m_CurSubItem_MTVD = n_ItemSub;
        }
        private void ListView_Edit_DIMS(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                txt_DIMS_IP_Input.SetBounds(n_Posleft + lv_DIMS.Left, n_ItemSub.Bounds.Top + lv_DIMS.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_DIMS_IP_Input.Text = "";
                }
                else
                {
                    txt_DIMS_IP_Input.Text = n_ItemSub.Text;
                }

                txt_DIMS_IP_Input.Show();
                txt_DIMS_IP_Input.Focus();
                txt_DIMS_IP_Input.SelectAll();
            }
            else if (n_idxSub == 3 || n_idxSub == 4)
            {
                txt_DIMS_Num_Input.SetBounds(n_Posleft + lv_DIMS.Left, n_ItemSub.Bounds.Top + lv_DIMS.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_DIMS_Num_Input.Text = "";
                }
                else
                {
                    txt_DIMS_Num_Input.Text = n_ItemSub.Text;
                }

                txt_DIMS_Num_Input.Show();
                txt_DIMS_Num_Input.Focus();
                txt_DIMS_Num_Input.SelectAll();
            }
            m_CurSubItem_DIMS = n_ItemSub;
        }

        private void ListView_Edit_UDP(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 1)
            {
                txt_UDP_IP_Input.SetBounds(n_Posleft + lv_UDP.Left, n_ItemSub.Bounds.Top + lv_UDP.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_UDP_IP_Input.Text = "";
                }
                else
                {
                    txt_UDP_IP_Input.Text = n_ItemSub.Text;
                }

                txt_UDP_IP_Input.Show();
                txt_UDP_IP_Input.Focus();
                txt_UDP_IP_Input.SelectAll();
            }
            else if (n_idxSub == 2 || n_idxSub == 3)
            {
                txt_UDP_Num_Input.SetBounds(n_Posleft + lv_UDP.Left, n_ItemSub.Bounds.Top + lv_UDP.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_UDP_Num_Input.Text = "";
                }
                else
                {
                    txt_UDP_Num_Input.Text = n_ItemSub.Text;
                }

                txt_UDP_Num_Input.Show();
                txt_UDP_Num_Input.Focus();
                txt_UDP_Num_Input.SelectAll();
            }
            m_CurSubItem_UDP = n_ItemSub;
        }

        private void ListView_Edit_CREVIS(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                txt_CREVIS_IP_Input.SetBounds(n_Posleft + lv_CREVIS.Left, n_ItemSub.Bounds.Top + lv_CREVIS.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_CREVIS_IP_Input.Text = "";
                }
                else
                {
                    txt_CREVIS_IP_Input.Text = n_ItemSub.Text;
                }

                txt_CREVIS_IP_Input.Show();
                txt_CREVIS_IP_Input.Focus();
                txt_CREVIS_IP_Input.SelectAll();
            }
            else if (n_idxSub == 3 || n_idxSub == 4)
            {
                txt_CREVIS_Num_Input.SetBounds(n_Posleft + lv_CREVIS.Left, n_ItemSub.Bounds.Top + lv_CREVIS.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_CREVIS_Num_Input.Text = "";
                }
                else
                {
                    txt_CREVIS_Num_Input.Text = n_ItemSub.Text;
                }

                txt_CREVIS_Num_Input.Show();
                txt_CREVIS_Num_Input.Focus();
                txt_CREVIS_Num_Input.SelectAll();
            }
            m_CurSubItem_CREVIS = n_ItemSub;
        }

        //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
        private void ListView_Edit_Parallel(ListViewItem.ListViewSubItem n_ItemSub, int n_idxSub, int n_Posleft, int n_PosWidth)
        {
            if (n_idxSub == 2)
            {
                txt_Parallel_IP_Input.SetBounds(n_Posleft + lv_Parallel.Left, n_ItemSub.Bounds.Top + lv_Parallel.Top, n_PosWidth, n_ItemSub.Bounds.Height);

                if (string.IsNullOrEmpty(n_ItemSub.Text))
                {
                    txt_Parallel_IP_Input.Text = "";
                }
                else
                {
                    txt_Parallel_IP_Input.Text = n_ItemSub.Text;
                }

                txt_Parallel_IP_Input.Show();
                txt_Parallel_IP_Input.Focus();
                txt_Parallel_IP_Input.SelectAll();
            }
            m_CurSubItem_Parallel = n_ItemSub;
        }
        //-

        private void ComboBox_Leave(object sender, EventArgs e)
        {
            ComboBox n_ComboBox = (ComboBox)sender;
            string n_Selected_Text = n_ComboBox.Text;
            n_ComboBox.Hide();

            m_bIsEdit = true;

            switch (n_ComboBox.Tag)
            {
                case "BD_Type":
                case "BD_ComPort":
                    m_CurSubItem_BD.Text = n_Selected_Text;
                    break;
                case "DAQ_ComType":
                case "DAQ_ComPort":
                    m_CurSubItem_DAQ.Text = n_Selected_Text;
                    break;
                case "DMM_ComType":
                case "DMM_ComPort":
                    m_CurSubItem_DMM.Text = n_Selected_Text;
                    break;
                case "MCZ_ComPort":
                    m_CurSubItem_MCZ.Text = n_Selected_Text;
                    break;
                case "MCZ_Type":
                    m_CurSubItem_MCZ.Text = n_Selected_Text;
                    break;
                //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                case "Chamber_Model":
                    m_CurSubItem_Chamber.Text = n_Selected_Text;
                    break;
                case "Chamber_ComPort":
                    m_CurSubItem_Chamber.Text = n_Selected_Text;
                    break;
                case "Chamber_Baudrate":
                    m_CurSubItem_Chamber.Text = n_Selected_Text;
                    break;
                case "Chamber_Checksum":
                    m_CurSubItem_Chamber.Text = n_Selected_Text;
                    break;
                case "Chamber_Alarm_Auto_Stop":
                    m_CurSubItem_Chamber.Text = n_Selected_Text;
                    break;

                case "Chiller_ComPort":
                    m_CurSubItem_Chiller.Text = n_Selected_Text;
                    break;

                //-
                //+ Add by KGY - 20240820 : SYS Setting 추가 ComboBox tag
                case "DAU_Auto_Logging":
                    m_CurSubItem_DAU.Text = n_Selected_Text;
                    break;
                case "DAU_Temp_Type":
                    m_CurSubItem_DAU.Text = n_Selected_Text;
                    break;
                case "Ins_ComPort":
                    m_CurSubItem_Ins.Text = n_Selected_Text;
                    break;
                case "Ins_Mode":
                    m_CurSubItem_Ins.Text = n_Selected_Text;
                    break;
                case "Ins_Rly_ComPort":
                    m_CurSubItem_Ins.Text = n_Selected_Text;
                    break;

                case "Chiller_Checksum":
                    m_CurSubItem_Chiller.Text = n_Selected_Text;
                    break;
                case "Chiller_Baudrate":
                    m_CurSubItem_Chiller.Text = n_Selected_Text;
                    break;
                case "Chiller_Model_Type":
                    m_CurSubItem_Chiller.Text = n_Selected_Text;
                    break;
                case "CAN_mdbc":
                    m_CurSubItem_CAN.Text = n_Selected_Text;
                    break;
                case "CAN_Type":
                    //+ Add by KGY - 20241015 : CAN 타입 변경 시 알림
                    string n_sMsgBox = string.Empty;
                    
                    if (m_CurSubItem_CAN.Text != "")
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            n_sMsgBox = "CAN 타입을 변경하시면 세부설정이 초기화 됩니다. \n\n변경하시겠습니까?";
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            n_sMsgBox = "If you change the CAN type, detailed settings will be initialized.\n\nDo you want to change it?";
                        }
                        if (MessageBox.Show(n_sMsgBox, "ABT System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            m_CurSubItem_CAN.Text = n_Selected_Text;
                            for (int j = 0; j < lv_CAN.Items.Count; j++)
                            {
                                for (int i = 0; i < m_Sys_CAN_Detail.Count; i++)
                                {
                                    if (lv_CAN.Items[j].SubItems[2].Text == "HS")
                                    {
                                        if ((m_Sys_CAN_Detail[i].CAN_No+1).ToString() == lv_CAN.Items[j].SubItems[1].Text)
                                        {
                                            if (!string.IsNullOrEmpty(m_Sys_CAN_Detail[i].CAN_HS_Bitrate)
                                                    && !string.IsNullOrEmpty(m_Sys_CAN_Detail[i].CAN_ID)
                                                    //+ Add by YMJ - 250725 : CAN 채널 추가
                                                    && !string.IsNullOrEmpty(m_Sys_CAN_Detail[i].CAN_CH)
                                                    //-
                                                    && m_Sys_CAN_Detail[i].CAN_No != -1)
                                            {
                                                lv_CAN.Items[j].SubItems[3].Text = "OK";
                                                break;
                                            }
                                            else
                                            {
                                                lv_CAN.Items[j].SubItems[3].Text = "Empty";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            lv_CAN.Items[j].SubItems[3].Text = "Empty";
                                        }
                                    }
                                    else if (lv_CAN.Items[j].SubItems[2].Text == "FD")
                                    {
                                        if ((m_Sys_CAN_Detail[i].CAN_No+1).ToString() == lv_CAN.Items[j].SubItems[1].Text)
                                        {
                                            if (!string.IsNullOrEmpty(m_Sys_CAN_Detail[i].CAN_FD_ClockFrequency)
                                                    && !string.IsNullOrEmpty(m_Sys_CAN_Detail[i].CAN_FD_BitRate)
                                                    && !string.IsNullOrEmpty(m_Sys_CAN_Detail[i].CAN_FD_DataBitRate)
                                                    && !string.IsNullOrEmpty(m_Sys_CAN_Detail[i].CAN_ID)
                                                    //+ Add by YMJ - 250725 : CAN 채널 추가
                                                    && !string.IsNullOrEmpty(m_Sys_CAN_Detail[i].CAN_CH)
                                                    //-
                                                    && m_Sys_CAN_Detail[i].CAN_No != -1)
                                            {
                                                lv_CAN.Items[j].SubItems[3].Text = "OK";
                                                break;
                                            }
                                            else
                                            {
                                                lv_CAN.Items[j].SubItems[3].Text = "Empty";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            lv_CAN.Items[j].SubItems[3].Text = "Empty";
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                            {
                                MessageBox.Show("CAN 타입을 변경하였습니다.\n\n세부 설정을 진행해주세요", "ABT System");
                            }
                            else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                            {
                                MessageBox.Show("\r\nThe CAN type has been changed.\n\nPlease proceed with detailed settings.", "ABT System");
                            }
                        }
                    }
                    else
                    {
                        m_CurSubItem_CAN.Text = n_Selected_Text;
                    }
                    //-
                    break;
                case "AUX_ComPort":
                    m_CurSubItem_AUX.Text = n_Selected_Text;
                    break;
                case "AUX_Baudrate":
                    m_CurSubItem_AUX.Text = n_Selected_Text;
                    break;
                case "MTVD_Model":
                    m_CurSubItem_MTVD.Text = n_Selected_Text;
                    break;


            }
        }

        private void txt_Input_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox n_TextBox = (TextBox)sender;
            m_bIsEdit = true;
            //+ Revision by KGY -20240820 : 입력 값 정규화를 위하여 키 입력 방지 추가 // 숫자, IP 형식, 기타(문자
            if (n_TextBox.Tag.ToString().Contains("Num"))
            {
                //+ Revision by KGY -250725 : 방향키 입력 추가
                if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) ||
                (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) ||
                 e.KeyCode == Keys.Back ||
                 e.KeyCode == Keys.Enter ||
                 e.KeyCode == Keys.Escape || 
                 e.KeyCode == Keys.Left || e.KeyCode == Keys.Right ||
                 e.KeyCode == Keys.Delete)
                 //-
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            e.Handled = true;

                            Set_Input_Text(n_TextBox.Tag, n_TextBox.Text);
                            n_TextBox.Hide();
                            break;
                        case Keys.Escape:
                            e.Handled = true;
                            n_TextBox.Hide();
                            break;
                        default:
                            return;
                    }
                }
                else
                {
                    e.SuppressKeyPress = true;
                }
            }
            else if(n_TextBox.Tag.ToString().Contains("IP"))
            {
                //3자리수 마다 . 넣기 위한 코드 but, 
                /*
                string[] n_LastSplitText = n_TextBox.Text.Split('.');
                if (
                    (n_LastSplitText.Length>=4 &&(e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal )) || 
                    (n_LastSplitText.Length>=4 && n_LastSplitText[n_LastSplitText.Length - 1].Length == 3 && ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)))&&
                    e.KeyCode != Keys.Back
                   )
                {
                    e.SuppressKeyPress = true;
                    return;
                }
                if (n_LastSplitText[n_LastSplitText.Length-1].Length==3 && e.KeyCode != Keys.OemPeriod && e.KeyCode != Keys.Decimal && e.KeyCode != Keys.Back && e.KeyCode != Keys.Enter && n_TextBox.Text.Length < 14)
                {
                    n_TextBox.Text += ".";
                    n_TextBox.SelectionStart = n_TextBox.Text.Length;
                }
                //-
                */
                //+ Revision by KGY -250725 : 방향키 입력 추가
                if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) ||
                (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) ||
                 e.KeyCode == Keys.Back ||
                 e.KeyCode == Keys.Enter ||
                 e.KeyCode == Keys.Escape || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal || 
                 e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || 
                 e.KeyCode == Keys.Delete)
                 //-
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            e.Handled = true;
                            Set_Input_Text(n_TextBox.Tag, n_TextBox.Text);
                            n_TextBox.Hide();
                            break;
                        case Keys.Escape:
                            e.Handled = true;
                            n_TextBox.Hide();
                            break;
                        default:
                            return;
                    }
                }
                else
                {
                    e.SuppressKeyPress = true;
                }
                
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        e.Handled = true;

                        Set_Input_Text(n_TextBox.Tag, n_TextBox.Text);
                        n_TextBox.Hide();
                        break;
                    case Keys.Escape:
                        e.Handled = true;
                        n_TextBox.Hide();
                        break;
                    default:
                        return;
                }
            }
            //-
        }
        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox n_ComboBox = (ComboBox)sender;
            n_ComboBox.Hide();

            m_bIsEdit = true;
           if(e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
           {
                if (n_ComboBox.Tag.ToString().Contains("BD"))
                {
                    m_CurSubItem_BD.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("DAQ"))
                {
                    m_CurSubItem_DAQ.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("DMM"))
                {
                    m_CurSubItem_DMM.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("MCZ"))
                {
                    m_CurSubItem_MCZ.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("Chamber"))
                {
                    m_CurSubItem_Chamber.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("Chiller"))
                {
                    m_CurSubItem_Chiller.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("DAU"))
                {
                    m_CurSubItem_DAU.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("Ins"))
                {
                    m_CurSubItem_Ins.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("CAN"))
                {
                    m_CurSubItem_CAN.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("Aux"))
                {
                    m_CurSubItem_AUX.Text = "";
                }
                else if (n_ComboBox.Tag.ToString().Contains("MTVD"))
                {
                    m_CurSubItem_MTVD.Text = "";
                }
           }
        }
        private void Set_Input_Text(object n_tag, string n_InputText)
        {
            //+Revision by KGY -20240820 : IP 입력 형식 및 입력값 판단을 위해 수정
            //Regex IP_Regex = new Regex(@"^([0-9]{1,3}\.){3}[0-9]{1,3}\z");
            Regex IP_Regex = new Regex(@"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b\z");
            string n_tempText;
            switch (n_tag)
            {
                case "BD_IP":
                    if (!(IP_Regex.IsMatch(n_InputText)))
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            MessageBox.Show("잘못 입력하셨습니다. \nIP 양식을 맞추시오.", "ABT System");
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            MessageBox.Show("You entered it incorrectly. \nPlease match the IP format.", "ABT System");
                        }
                        
                        return;
                    }
                    else
                    {
                        m_CurSubItem_BD.Text = n_InputText;
                        lv_BD.Focus();
                        break;
                    }
                case "BD_Num":
                    m_CurSubItem_BD.Text = n_InputText;
                    lv_BD.Focus();
                    break;
                case "DAQ":
                    m_CurSubItem_DAQ.Text = n_InputText;
                    lv_DAQ.Focus();
                    break;
                case "DMM":
                    m_CurSubItem_DMM.Text = n_InputText;
                    lv_DMM.Focus();
                    break;
                case "Ins_Num":
                    m_CurSubItem_Ins.Text = n_InputText;
                    lv_Ins.Focus();
                    break;
                case "DAU_IP":
                    if (!(IP_Regex.IsMatch(n_InputText)))
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            MessageBox.Show("잘못 입력하셨습니다. \nIP 양식을 맞추시오.", "ABT System");
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            MessageBox.Show("You entered it incorrectly. \nPlease match the IP format.", "ABT System");
                        }
                        return;
                    }
                    else
                    {
                        m_CurSubItem_DAU.Text = n_InputText;
                        lv_DAU.Focus();
                        break;
                    }
                case "DAU_Num":
                    m_CurSubItem_DAU.Text = n_InputText;
                    lv_DAU.Focus();
                    break;
                case "BMS":
                    m_CurSubItem_BMS.Text = n_InputText;
                    lv_BMS.Focus();
                    break;
                case "BMS_Num":
                    m_CurSubItem_BMS.Text = n_InputText;
                    lv_BMS.Focus();
                    break;
                case "MCZ_Num":
                    m_CurSubItem_MCZ.Text = n_InputText;
                    lv_MCZ.Focus();
                    break;
                case "MCZ_IP":
                    if (!(IP_Regex.IsMatch(n_InputText)))
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            MessageBox.Show("잘못 입력하셨습니다. \nIP 양식을 맞추시오.", "ABT System");
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            MessageBox.Show("You entered it incorrectly. \nPlease match the IP format.", "ABT System");
                        }
                        return;
                    }
                    else
                    {
                        m_CurSubItem_MCZ.Text = n_InputText;
                        lv_MCZ.Focus();
                        break;
                    }
                case "MBT_IP":
                    if (!(IP_Regex.IsMatch(n_InputText)))
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            MessageBox.Show("잘못 입력하셨습니다. \nIP 양식을 맞추시오.","ABT System");
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            MessageBox.Show("You entered it incorrectly. \nPlease match the IP format.", "ABT System");
                        }
                        return;
                    }
                    else
                    {
                        m_CurSubItem_MBT.Text = n_InputText;
                        lv_MBT.Focus();
                        break;
                    }
                case "MBT_Num":
                    m_CurSubItem_MBT.Text = n_InputText;
                    lv_MBT.Focus();
                    break;
                case "MUX":
                    m_CurSubItem_MUX.Text = n_InputText;
                    lv_MUX.Focus();
                    break;
                //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
                case "Chamber":
                    m_CurSubItem_Chamber.Text = n_InputText;
                    lv_Chamber.Focus();
                    break;
                case "Chiller":
                    m_CurSubItem_Chiller.Text = n_InputText;
                    lv_Chiller.Focus();
                    break;
                //-
                case "AUX_Num":
                    m_CurSubItem_AUX.Text = n_InputText;
                    lv_AUX.Focus();
                    break;
                case "AUX_VModule_Num":
                    n_tempText = m_CurSubItem_AUX.Text;
                    m_CurSubItem_AUX.Text = n_InputText;
                    lv_AUX.Focus();
                    //+ Add by KGY - 20241011 : AUX 모듈 수량 입력시 상세 설정 수량과 비교 후 알림
                    int n_tempInt;
                    if (!int.TryParse(n_tempText, out n_tempInt)) { n_tempInt = 0; }
                    
                    if (!CheckAUXDetail() && n_tempInt != 0)
                    {
                        string n_sMsgBox = string.Empty;
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            n_sMsgBox = "세부 설정의 모듈 수량과 다름니다.\n\n 변경하시겠습니까?";
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            n_sMsgBox = "Register the module in detailed settings.\n\nDo you want to change it?";
                        }
                        if (MessageBox.Show(n_sMsgBox, "ABT System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            m_CurSubItem_AUX.Text = n_tempText;
                        }
                        else
                        {
                            //FrmDetail_Setting_View("AUX");
                            if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                            {
                                MessageBox.Show("모듈 수량을 변경하였습니다. \n\n세부 설정을 진행해주세요", "ABT System");
                            }
                            else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                            {
                                MessageBox.Show("The module quantity has been changed.\n\nPlease proceed with detailed settings.", "ABT System");
                            }
                        }
                    }
                    //-
                    break;
                case "AUX_TModule_Num":
                    n_tempText = m_CurSubItem_AUX.Text;
                    m_CurSubItem_AUX.Text = n_InputText;
                    lv_AUX.Focus();
                    if (!CheckAUXDetail() && n_tempText != "")
                    {
                        string n_sMsgBox = string.Empty;
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            n_sMsgBox = "세부 설정의 모듈 수량과 다름니다. 변경하시겠습니까?";
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            n_sMsgBox = "Are you sure you want to reset the overall settings?\n(Your modifications will not be saved.)";
                        }
                        if (MessageBox.Show(n_sMsgBox, "ABT System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            m_CurSubItem_AUX.Text = n_tempText;
                        }
                        else
                        {
                            //FrmDetail_Setting_View("AUX");
                            if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                            {
                                MessageBox.Show("모듈 수량을 변경하였습니다. \n\n세부 설정을 진행해주세요", "ABT System");
                            }
                            else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                            {
                                MessageBox.Show("The module quantity has been changed.\n\nPlease proceed with detailed settings.", "ABT System");
                            }
                        }
                    }
                    break;
                case "CAN":
                    m_CurSubItem_CAN.Text = n_InputText;
                    lv_CAN.Focus();
                    break;
                case "MTVD_IP":
                    if (!(IP_Regex.IsMatch(n_InputText)))
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            MessageBox.Show("잘못 입력하셨습니다. \nIP 양식을 맞추시오.", "ABT System");
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            MessageBox.Show("You entered it incorrectly. \nPlease match the IP format.", "ABT System");
                        }
                        return;
                    }
                    else
                    {
                        m_CurSubItem_MTVD.Text = n_InputText;
                        lv_MTVD.Focus();
                        break;
                    }
                case "MTVD_Num":
                    m_CurSubItem_MTVD.Text = n_InputText;
                    lv_MTVD.Focus();
                    break;
                case "DIMS_IP":
                    if (!(IP_Regex.IsMatch(n_InputText)))
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            MessageBox.Show("잘못 입력하셨습니다. \nIP 양식을 맞추시오.", "ABT System");
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            MessageBox.Show("You entered it incorrectly. \nPlease match the IP format.", "ABT System");
                        }
                        return;
                    }
                    else
                    {
                        m_CurSubItem_DIMS.Text = n_InputText;
                        lv_DIMS.Focus();
                        break;
                    }
                case "DIMS_Num":
                    m_CurSubItem_DIMS.Text = n_InputText;
                    lv_DIMS.Focus();
                    break;
                case "UDP_IP":
                    if (!(IP_Regex.IsMatch(n_InputText)))
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            MessageBox.Show("잘못 입력하셨습니다. \nIP 양식을 맞추시오.", "ABT System");
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            MessageBox.Show("You entered it incorrectly. \nPlease match the IP format.", "ABT System");
                        }
                        return;
                    }
                    else
                    {
                        m_CurSubItem_UDP.Text = n_InputText;
                        lv_UDP.Focus();
                        break;
                    }
                case "UDP_Num":
                    m_CurSubItem_UDP.Text = n_InputText;
                    lv_UDP.Focus();
                    break;
                case "CREVIS_IP":
                    if (!(IP_Regex.IsMatch(n_InputText)))
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            MessageBox.Show("잘못 입력하셨습니다. \nIP 양식을 맞추시오.", "ABT System");
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            MessageBox.Show("You entered it incorrectly. \nPlease match the IP format.", "ABT System");
                        }
                        return;
                    }
                    else
                    {
                        m_CurSubItem_CREVIS.Text = n_InputText;
                        lv_CREVIS.Focus();
                        break;
                    }
                case "CREVIS_Num":
                    m_CurSubItem_CREVIS.Text = n_InputText;
                    lv_CREVIS.Focus();
                    break;
                //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
                case "Parallel_IP":
                    if (!(IP_Regex.IsMatch(n_InputText)))
                    {
                        if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                        {
                            MessageBox.Show("잘못 입력하셨습니다. \nIP 양식을 맞추시오.", "ABT System");
                        }
                        else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                        {
                            MessageBox.Show("You entered it incorrectly. \nPlease match the IP format.", "ABT System");
                        }
                        return;
                    }
                    else
                    {
                        m_CurSubItem_Parallel.Text = n_InputText;
                        lv_Parallel.Focus();
                        break;
                    }
                //-
            }
            //-
        }

        private void txt_Input_Leave(object sender, EventArgs e)
        {
            TextBox n_TextBox = (TextBox)sender;
            n_TextBox.Hide();
        }

        private void bt_Init_Click(object sender, EventArgs e)
        {
            //+Add by KGY -20240517 : 언어 변경을 위한 코드 추가
            string n_sMsgBox = string.Empty;
            if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
            {
                n_sMsgBox = "전체 설정 값을 초기화 하시겠습니까?\n(수정하시던 내용은 저장되지 않습니다.)";
            }
            else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
            {
                n_sMsgBox = "Are you sure you want to reset the overall settings?\n(Your modifications will not be saved.)";
            }

            if (MessageBox.Show(n_sMsgBox, "ABT System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Init_Ctrl();
            }
            //-
        }

        private void bt_Ok_Click(object sender, EventArgs e)
        {
            //+Add by KGY -20240517 : 언어 변경 적용
            string n_sMsgBox = string.Empty;
            if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
            {
                n_sMsgBox = "현재 수정한 내용을 저장 하시겠습니까?";
            }
            else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
            {
                n_sMsgBox = "Do you want to save the current modifications?";
            }
            if (MessageBox.Show(n_sMsgBox, "ABT System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (CheckDetail())
                {
                    Save_System_Setting();
                    m_System_Set.Save_System_Setting();

                    m_bIsEdit = false;

                    m_MainFrame.DAU_VT_Recount();

                    if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                    {
                        //var d = MessageBox.Show("저장을 완료하였습니다.", "ABT System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                         var d = MessageBox.Show("저장을 완료하였습니다.\n\n해당 내용을 적용하시려면 ABTProV2 프로그램을\n종료 후 재시작 해주세요.", "ABT System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                    {
                        MessageBox.Show("The save has been successfully saved.\n\nTo apply this, exit and restart the ABTProV2 program.", "ABT System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                
            }
            //-
        }

        private void Save_System_Setting()
        {
            m_System_Set.m_bUse_BD = chb_BD.Checked;
            m_System_Set.m_bUse_DAQ = chb_DAQ.Checked;
            m_System_Set.m_bUse_DMM = chb_DMM.Checked;
            m_System_Set.m_bUse_Ins = chb_Insulate.Checked;
            m_System_Set.m_bUse_DAU = chb_DAU.Checked;
            m_System_Set.m_bUse_BMS = chb_BMS.Checked;
            m_System_Set.m_bUse_MCZ = chb_MCZ.Checked;
            m_System_Set.m_bUse_MBT = chb_MBT.Checked;
            m_System_Set.m_bUse_MUX = chb_MUX.Checked;

            //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
            m_System_Set.m_bUse_Chamber = chb_Chamber.Checked;
            m_System_Set.m_bUse_Chiller = chb_Chiller.Checked;
            //-
            m_System_Set.m_bUse_AUX = chb_AUX.Checked;
            m_System_Set.m_bUse_CAN = chb_CAN.Checked;
            m_System_Set.m_bUse_MTVD = chb_MTVD.Checked;
            m_System_Set.m_bUse_DIMS = chb_DIMS.Checked;
            m_System_Set.m_bUse_UDP = chb_UDP.Checked;
            m_System_Set.m_bUse_CREVIS = chb_CREVIS.Checked;
            //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
            m_System_Set.m_bUse_Parallel = chb_Parallel.Checked;
            //-

            //+ BD
            if (lv_BD.Items.Count > 0)
            {
                if (m_System_Set.m_Sys_BD.Count > 0)
                {
                    m_System_Set.m_Sys_BD.Clear();
                }

                for (int i = 0; i < lv_BD.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_BD.Items[i];

                    Sys_Set_BD n_Data = new Sys_Set_BD();

                    n_Data.BD_Use = n_Item.Checked;
                    n_Data.BD_No = int.Parse(n_Item.SubItems[1].Text);

                    for (int k = 0; k < cb_BD_Type.Items.Count; k++)
                    {
                        if (cb_BD_Type.Items[k].Equals(n_Item.SubItems[2].Text))
                        {
                            n_Data.BD_Type = k;
                            break;
                        }
                    }
                    if (!int.TryParse(n_Item.SubItems[3].Text, out n_Data.BD_CH_Cnt))
                    {
                        n_Data.BD_CH_Cnt = -1;
                    }
                    n_Data.BD_IP = n_Item.SubItems[4].Text;
                    if (!int.TryParse(n_Item.SubItems[5].Text, out n_Data.BD_TCP_Port))
                    {
                        n_Data.BD_TCP_Port = -1;
                    }
                    n_Data.BD_COM_Port = n_Item.SubItems[6].Text;

                    m_System_Set.m_Sys_BD.Add(n_Data);
                }
            }
            //-
            //+ DAQ
            if (lv_DAQ.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_DAQ.Count >= 0)
                {
                    m_System_Set.m_Sys_DAQ.Clear();
                }

                for (int i = 0; i < lv_DAQ.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_DAQ.Items[i];

                    Sys_Set_DAQ n_Data = new Sys_Set_DAQ();

                    n_Data.DAQ_Use = n_Item.Checked;
                    n_Data.DAQ_No = int.Parse(n_Item.SubItems[1].Text)-1;

                    for (int k = 0; k < cb_DAQ_ComType.Items.Count; k++)
                    {
                        if (cb_DAQ_ComType.Items[k].Equals(n_Item.SubItems[2].Text))
                        {
                            n_Data.DAQ_ComType = k;
                            break;
                        }
                    }
                    n_Data.DAQ_IP = n_Item.SubItems[3].Text;
                    if (!int.TryParse(n_Item.SubItems[4].Text, out n_Data.DAQ_TCP_Port))
                    {
                        n_Data.DAQ_TCP_Port = -1;
                    }
                    n_Data.DAQ_COM_Port = n_Item.SubItems[5].Text;

                    m_System_Set.m_Sys_DAQ.Add(n_Data);
                }
            }
            //-
            //+ DMM
            if (lv_DMM.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_DMM.Count > 0)
                {
                    m_System_Set.m_Sys_DMM.Clear();
                }

                for (int i = 0; i < lv_DMM.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_DMM.Items[i];

                    Sys_Set_DMM n_Data = new Sys_Set_DMM();

                    n_Data.DMM_Use = n_Item.Checked;
                    n_Data.DMM_No = int.Parse(n_Item.SubItems[1].Text)-1;

                    for (int k = 0; k < cb_DMM_ComType.Items.Count; k++)
                    {
                        if (cb_DMM_ComType.Items[k].Equals(n_Item.SubItems[2].Text))
                        {
                            n_Data.DMM_ComType = k;
                            break;
                        }
                    }
                    n_Data.DMM_IP = n_Item.SubItems[3].Text;
                    if (!int.TryParse(n_Item.SubItems[4].Text, out n_Data.DMM_TCP_Port))
                    {
                        n_Data.DMM_TCP_Port = -1;
                    }
                    n_Data.DMM_COM_Port = n_Item.SubItems[5].Text;

                    m_System_Set.m_Sys_DMM.Add(n_Data);
                }
            }
            //-
            //+ Insulate
            if (lv_Ins.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_Ins.Count >= 0)
                {
                    m_System_Set.m_Sys_Ins.Clear();
                }

                for (int i = 0; i < lv_Ins.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_Ins.Items[i];

                    Sys_Set_Insulate n_Data = new Sys_Set_Insulate();

                    n_Data.Ins_Use = n_Item.Checked;
                    n_Data.Ins_No = int.Parse(n_Item.SubItems[1].Text)-1;
                    n_Data.Ins_COM_Port = n_Item.SubItems[2].Text;
                    n_Data.Ins_Mode = n_Item.SubItems[3].Text;
                    n_Data.Ins_Rly_COM_Port = n_Item.SubItems[4].Text;
                    if (!int.TryParse(n_Item.SubItems[5].Text, out n_Data.Ins_Voltage))
                    {
                        n_Data.Ins_Voltage = 500;
                    }
                    if (!int.TryParse(n_Item.SubItems[6].Text, out n_Data.Ins_Time))
                    {
                        n_Data.Ins_Time = 60;
                    }
                    m_System_Set.m_Sys_Ins.Add(n_Data);
                }
            }
            //-
            //+ DAU
            if (lv_DAU.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_DAU.Count >= 0)
                {
                    m_System_Set.m_Sys_DAU.Clear();
                }

                for (int i = 0; i < lv_DAU.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_DAU.Items[i];
                    Sys_Set_DAU n_Data = new Sys_Set_DAU();

                    n_Data.DAU_Use = n_Item.Checked;
                    n_Data.DAU_No = int.Parse(n_Item.SubItems[1].Text) - 1;
                    n_Data.DAU_IP = n_Item.SubItems[2].Text;
                    n_Data.DAU_VModule_Count = 0;
                    n_Data.DAU_TModule_Count = 0;
                    n_Data.DAU_Voltage_Count = 0;
                    n_Data.DAU_Temp_Count = 0;
                    n_Data.DAU_Temp_Type = 0;
                    n_Data.DAU_Auto_Logging = false;
                    n_Data.DAU_Multi_Use = false;
                    n_Data.DAU_NTC = "";
                    n_Data.DAU_Temp_MultiCnt = 1;

                    for (int j =0; j< m_Sys_DAU_Detail.Count; j++)
                    {
                        if (lv_DAU.Items[i].SubItems[1].Text == (m_Sys_DAU_Detail[j].DAU_No+1).ToString())
                        {
                            n_Data.DAU_VModule_Count = m_Sys_DAU_Detail[j].DAU_VModule_Count;
                            n_Data.DAU_TModule_Count = m_Sys_DAU_Detail[j].DAU_TModule_Count;
                            n_Data.DAU_Voltage_Count = m_Sys_DAU_Detail[j].DAU_Voltage_Count;
                            n_Data.DAU_Temp_Count    = m_Sys_DAU_Detail[j].DAU_Temp_Count;
                            n_Data.DAU_Temp_Type     = m_Sys_DAU_Detail[j].DAU_Temp_Type;
                            n_Data.DAU_Auto_Logging  = m_Sys_DAU_Detail[j].DAU_Auto_Logging;
                            n_Data.DAU_Multi_Use     = m_Sys_DAU_Detail[j].DAU_Multi_Use;
                            if(m_Sys_DAU_Detail[j].DAU_NTC != null) { n_Data.DAU_NTC = m_Sys_DAU_Detail[j].DAU_NTC; }
                            if (m_Sys_DAU_Detail[j].DAU_Temp_MultiCnt <= 0) { n_Data.DAU_Temp_MultiCnt = 1; } else { n_Data.DAU_Temp_MultiCnt = m_Sys_DAU_Detail[j].DAU_Temp_MultiCnt; }
                        }
                    }
                    m_System_Set.m_Sys_DAU.Add(n_Data);
                }
            }
            //-
            //+ BMS
            if (lv_BMS.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_BMS.Count >= 0)
                {
                    m_System_Set.m_Sys_BMS.Clear();
                }

                for (int i = 0; i < lv_BMS.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_BMS.Items[i];

                    Sys_Set_BMS n_Data = new Sys_Set_BMS();

                    n_Data.BMS_Use = n_Item.Checked;
                    n_Data.BMS_No = int.Parse(n_Item.SubItems[1].Text)-1;
                    n_Data.BMS_CAN_Id = n_Item.SubItems[2].Text;
                    if (!int.TryParse(n_Item.SubItems[3].Text, out n_Data.BMS_Voltage_Count))
                    {
                        n_Data.BMS_Voltage_Count = -1;
                    }
                    if (!int.TryParse(n_Item.SubItems[4].Text, out n_Data.BMS_Temp_Count))
                    {
                        n_Data.BMS_Temp_Count = -1;
                    }

                    m_System_Set.m_Sys_BMS.Add(n_Data);
                }
            }
            //-
            //+ MCZ
            if (lv_MCZ.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_MCZ.Count >= 0)
                {
                    m_System_Set.m_Sys_MCZ.Clear();
                }

                for (int i = 0; i < lv_MCZ.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_MCZ.Items[i];

                    Sys_Set_MCZ n_Data = new Sys_Set_MCZ();

                    n_Data.MCZ_Use      = n_Item.Checked;
                    n_Data.MCZ_No       = int.Parse(n_Item.SubItems[1].Text)-1;
                    //n_Data.MCZ_Type     = n_Item.SubItems[2].Text;
                    //n_Data.MCZ_COM_Port = n_Item.SubItems[3].Text;
                    n_Data.MCZ_IP       = n_Item.SubItems[2].Text;
                    n_Data.MCZ_Port     = int.Parse(n_Item.SubItems[3].Text);

                    m_System_Set.m_Sys_MCZ.Add(n_Data);
                }
            }
            //-
            //+ MBT
            if (lv_MBT.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_MBT.Count >= 0)
                {
                    m_System_Set.m_Sys_MBT.Clear();
                }

                for (int i = 0; i < lv_MBT.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_MBT.Items[i];

                    Sys_Set_MBT n_Data = new Sys_Set_MBT();

                    n_Data.MBT_Use = n_Item.Checked;
                    n_Data.MBT_No = int.Parse(n_Item.SubItems[1].Text)-1;
                    n_Data.MBT_IP = n_Item.SubItems[2].Text;
                    if (!int.TryParse(n_Item.SubItems[3].Text, out n_Data.MBT_TCP_Port))
                    {
                        n_Data.MBT_TCP_Port = -1;
                    }

                    m_System_Set.m_Sys_MBT.Add(n_Data);
                }
            }
            //-
            //+ MUX
            if (lv_MUX.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_MUX.Count >= 0)
                {
                    m_System_Set.m_Sys_MUX.Clear();
                }

                for (int i = 0; i < lv_MUX.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_MUX.Items[i];

                    Sys_Set_MUX n_Data = new Sys_Set_MUX();

                    n_Data.MUX_Use = n_Item.Checked;
                    n_Data.MUX_No = int.Parse(n_Item.SubItems[1].Text)-1;
                    n_Data.MUX_IP = n_Item.SubItems[2].Text;

                    if (!int.TryParse(n_Item.SubItems[3].Text, out n_Data.MUX_TCP_Port))
                    {
                        n_Data.MUX_TCP_Port = -1;
                    }
                    if (!int.TryParse(n_Item.SubItems[4].Text, out n_Data.MUX_CH_Cnt))
                    {
                        n_Data.MUX_CH_Cnt = -1;
                    }


                    m_System_Set.m_Sys_MUX.Add(n_Data);
                }
            }
            //-
            //+ Add by LBG - 23.03.20 : 온도 관련 Device 정보 추가
            //+ Chamber
            if (lv_Chamber.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_Chamber.Count >= 0)
                {
                    m_System_Set.m_Sys_Chamber.Clear();
                }

                for (int i = 0; i < lv_Chamber.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_Chamber.Items[i];
                    
                        
                    Sys_Set_Chamber n_Data = new Sys_Set_Chamber();

                    n_Data.Chamber_Use = n_Item.Checked;
                    n_Data.Chamber_No = int.Parse(n_Item.SubItems[1].Text) - 1;
                    n_Data.Chamber_Model = n_Item.SubItems[2].Text;
                    n_Data.Chamber_COM_Port = n_Item.SubItems[3].Text;
                    n_Data.Chamber_Baudrate = n_Item.SubItems[4].Text;
                    if (!bool.TryParse(n_Item.SubItems[5].Text, out n_Data.Chamber_Checksum))
                    {
                        n_Data.Chamber_Checksum = false;
                    }
                    if (!bool.TryParse(n_Item.SubItems[6].Text, out n_Data.Chamber_Alarm_Auto_Stop))
                    {
                        n_Data.Chamber_Alarm_Auto_Stop = false;

                    }
                    for (int j = 0; j <m_Sys_ChamberDIO.Count; j++) 
                    {
                        if (n_Data.Chamber_No == m_Sys_ChamberDIO[j].Chamber_No)
                        {
                            n_Data.ChamberDIO_Device = m_Sys_ChamberDIO[j].ChamberDIO_Device;
                            n_Data.ChamberDIO_Module_No = m_Sys_ChamberDIO[j].ChamberDIO_Module_No - 1;
                            n_Data.ChamberDIO_COM_Port = m_Sys_ChamberDIO[j].ChamberDIO_COM_Port;
                            n_Data.ChamberDIO_Baudrate = m_Sys_ChamberDIO[j].ChamberDIO_Baudrate;
                        }
                    }
                    m_System_Set.m_Sys_Chamber.Add(n_Data);
                }
            }
            //-
            //+ Chiller
            if (lv_Chiller.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_Chiller.Count >= 0)
                {
                    m_System_Set.m_Sys_Chiller.Clear();
                }

                for (int i = 0; i < lv_Chiller.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_Chiller.Items[i];

                    Sys_Set_Chiller n_Data = new Sys_Set_Chiller();

                    n_Data.Chiller_Use = n_Item.Checked;
                    n_Data.Chiller_No = int.Parse(n_Item.SubItems[1].Text)-1;
                    n_Data.Chiller_Model = n_Item.SubItems[2].Text;
                    n_Data.Chiller_COM_Port = n_Item.SubItems[3].Text;
                    n_Data.Chiller_Baudrate = n_Item.SubItems[4].Text;
                    if (!bool.TryParse(n_Item.SubItems[5].Text, out n_Data.Chiller_Checksum))
                    {
                        n_Data.Chiller_Checksum = false;
                    }

                    m_System_Set.m_Sys_Chiller.Add(n_Data);
                }
            }
            //-
            //-
            //+ Add by KGY - 20240820 : Sys Setting 항목 추가
            //+ AUX
            if (lv_AUX.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_AUX.Count >= 0)
                {
                    m_System_Set.m_Sys_AUX.Clear();
                }
                for (int i = 0; i < lv_AUX.Items.Count; i++)
                {

                    ListViewItem n_Item = lv_AUX.Items[i];

                    Sys_Set_AUX n_Data = new Sys_Set_AUX();
                    //n_Data.AUX_DetailList =new List<int>();
                    n_Data.AUX_Use = n_Item.Checked;
                    n_Data.AUX_No = int.Parse(n_Item.SubItems[1].Text)-1;
                    n_Data.AUX_COM_Port = n_Item.SubItems[2].Text;
                    n_Data.AUX_Baudrate = n_Item.SubItems[3].Text;
                    if (!int.TryParse(n_Item.SubItems[4].Text, out n_Data.AUX_VModule_Count))
                    {
                        n_Data.AUX_VModule_Count = 0;
                    }
                    if (!int.TryParse(n_Item.SubItems[5].Text, out n_Data.AUX_TModule_Count))
                    {
                        n_Data.AUX_TModule_Count = 0;
                    }
                    if (!int.TryParse(n_Item.SubItems[6].Text, out n_Data.AUX_Voltage_Count))
                    {
                        n_Data.AUX_Voltage_Count = -1;
                    }
                    if (!int.TryParse(n_Item.SubItems[7].Text, out n_Data.AUX_Temp_Count))
                    {
                        n_Data.AUX_Temp_Count = -1;
                    }
                    if (!int.TryParse(n_Item.SubItems[8].Text, out n_Data.AUX_Applied_ModuleNo))
                    {
                        n_Data.AUX_Applied_ModuleNo = 1;
                    }
                    m_System_Set.m_Sys_AUX.Add(n_Data);
                }
            }
            if (m_Sys_AUX_Detail.Count >= 0)
            {
                if(lv_AUX.Items.Count == 0)
                {
                    m_Sys_AUX_Detail.Clear();
                }
                if (m_System_Set.m_Sys_AUX_Detail.Count >= 0)
                {
                    m_System_Set.m_Sys_AUX_Detail.Clear();
                }
                foreach (Sys_Set_AUX_Detail Item in m_Sys_AUX_Detail)
                {
                    m_System_Set.m_Sys_AUX_Detail.Add(Item);
                }
            }
            //-
            //+ CAN
            if (lv_CAN.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_CAN.Count >= 0)
                {
                    m_System_Set.m_Sys_CAN.Clear();
                }

                for (int i = 0; i < lv_CAN.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_CAN.Items[i];
                    for (int j = 0; j < m_Sys_CAN_Detail.Count; j++)
                    {
                        if (n_Item.SubItems[1].Text == (m_Sys_CAN_Detail[j].CAN_No+1).ToString())
                        {
                            Sys_Set_CAN n_Data = new Sys_Set_CAN();

                            n_Data.CAN_Use = n_Item.Checked;
                            n_Data.CAN_No = int.Parse(n_Item.SubItems[1].Text)-1;
                            n_Data.CAN_Type = n_Item.SubItems[2].Text;
                            n_Data.CAN_ID = m_Sys_CAN_Detail[j].CAN_ID;
                            //+ Add by YMJ - 250725 : CAN 채널 추가
                            n_Data.CAN_CH = m_Sys_CAN_Detail[j].CAN_CH;
                            //-
                            n_Data.CAN_MDBC = n_Item.SubItems[4].Text;

                            n_Data.CAN_HS_Bitrate = m_Sys_CAN_Detail[j].CAN_HS_Bitrate;
                            n_Data.CAN_FD_ClockFrequency = m_Sys_CAN_Detail[j].CAN_FD_ClockFrequency;
                            n_Data.CAN_FD_BitRate = m_Sys_CAN_Detail[j].CAN_FD_BitRate;
                            n_Data.CAN_FD_DataBitRate = m_Sys_CAN_Detail[j].CAN_FD_DataBitRate;

                            m_System_Set.m_Sys_CAN.Add(n_Data);
                        } 
                    }
                }
            }
            //-
            //+ MTVD
            if (lv_MTVD.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_MTVD.Count >= 0)
                {
                    m_System_Set.m_Sys_MTVD.Clear();
                }

                for (int i = 0; i < lv_MTVD.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_MTVD.Items[i];

                    Sys_Set_MTVD n_Data = new Sys_Set_MTVD();

                    n_Data.MTVD_Use = n_Item.Checked;
                    n_Data.MTVD_No = int.Parse(n_Item.SubItems[1].Text)-1;
                    n_Data.MTVD_Model = n_Item.SubItems[2].Text;
                    n_Data.MTVD_VOC = n_Item.SubItems[3].Text;
                    n_Data.MTVD_IP = n_Item.SubItems[4].Text;
                    n_Data.MTVD_Port = n_Item.SubItems[5].Text;

                    m_System_Set.m_Sys_MTVD.Add(n_Data);
                }
            }
            //-
            //+ DIMS
            if (lv_DIMS.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_DIMS.Count >= 0)
                {
                    m_System_Set.m_Sys_DIMS.Clear();
                }

                for (int i = 0; i < lv_DIMS.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_DIMS.Items[i];

                    Sys_Set_DIMS n_Data = new Sys_Set_DIMS();

                    n_Data.DIMS_Use = n_Item.Checked;
                    n_Data.DIMS_No = int.Parse(n_Item.SubItems[1].Text)-1;
                    n_Data.DIMS_IP = n_Item.SubItems[2].Text;
                    n_Data.DIMS_Port = n_Item.SubItems[3].Text;
                    if (!int.TryParse(n_Item.SubItems[4].Text, out n_Data.DIMS_Send_Interval))
                    {
                        n_Data.DIMS_Send_Interval = -1;
                    }

                    m_System_Set.m_Sys_DIMS.Add(n_Data);
                }
               
            }
            //-
            //-
            //+ UDP
            if (lv_UDP.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_UDP.Count >= 0)
                {
                    m_System_Set.m_Sys_UDP.Clear();
                }

                for (int i = 0; i < lv_UDP.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_UDP.Items[i];

                    Sys_Set_UDP n_Data = new Sys_Set_UDP();

                    n_Data.UDP_Enable = n_Item.Checked;
                    n_Data.UDP_Server_IP = n_Item.SubItems[1].Text;
                    n_Data.UDP_Server_Port = n_Item.SubItems[2].Text;
                    if (!int.TryParse(n_Item.SubItems[3].Text, out n_Data.UDP_State_Send_Interval))
                    {
                        n_Data.UDP_State_Send_Interval = -1;
                    }

                    m_System_Set.m_Sys_UDP.Add(n_Data);
                }

            }
            //-
            //+ CREVIS
            if (lv_CREVIS.Items.Count >= 0)
            {
                if (m_System_Set.m_Sys_CREVIS.Count >= 0)
                {
                    m_System_Set.m_Sys_CREVIS.Clear();
                }

                for (int i = 0; i < lv_CREVIS.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_CREVIS.Items[i];

                    Sys_Set_CREVIS n_Data = new Sys_Set_CREVIS();

                    n_Data.CREVIS_Use = n_Item.Checked;
                    n_Data.CREVIS_No = int.Parse(n_Item.SubItems[1].Text) - 1;
                    n_Data.CREVIS_IP = n_Item.SubItems[2].Text;
                    if (!int.TryParse(n_Item.SubItems[3].Text, out n_Data.CREVIS_VOLT))
                    {
                        n_Data.CREVIS_VOLT = -1;
                    }
                    if (!int.TryParse(n_Item.SubItems[4].Text, out n_Data.CREVIS_TEMP))
                    {
                        n_Data.CREVIS_TEMP = -1;
                    }
                    //+ Revision by KGY -250724 : CREVIS 저장오류 수정
                    n_Data.CREVIS_CALIBRATION = new List<float>();
                    for (int j = 0; j < m_Sys_CREVIS_Detail.Count; j++)
                    {
                        if (m_Sys_CREVIS_Detail.Count == 1 && m_Sys_CREVIS_Detail[j].CREVIS_No == -1 )
                        {
                            for(int n =0; n< n_Data.CREVIS_TEMP; n++)
                            {
                                n_Data.CREVIS_CALIBRATION.Add(0);
                            }
                            break;
                        }
                        if (n_Item.SubItems[1].Text == (m_Sys_CREVIS_Detail[j].CREVIS_No + 1).ToString())
                        {
                            n_Data.CREVIS_CALIBRATION = m_Sys_CREVIS_Detail[j].CREVIS_CALIBRATION;
                            break;
                        }
                    }
                    //-
                    m_System_Set.m_Sys_CREVIS.Add(n_Data);
                }
            }
            //-
            //+ Add by YMJ - 250819 : 자동병렬 기능 관련 항목 추가
            if (lv_Parallel.Items.Count >= 0)
            {
                if(m_System_Set.m_Sys_Parallel.Count >= 0)
                {
                    m_System_Set.m_Sys_Parallel.Clear();
                }

                for(int i = 0; i < lv_Parallel.Items.Count; i++)
                {
                    ListViewItem n_Item = lv_Parallel.Items[i];

                    Sys_Set_Parallel n_Data = new Sys_Set_Parallel();

                    n_Data.Parallel_Use = n_Item.Checked;
                    n_Data.Parallel_No = int.Parse(n_Item.SubItems[1].Text) - 1;
                    n_Data.Parallel_IP = n_Item.SubItems[2].Text;

                    m_System_Set.m_Sys_Parallel.Add(n_Data);
                }
            }
            //-
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            if (m_bIsEdit)
            {
                //+ Add by KGY - 240517 : 언어 변경 적용
                string n_sMsgBox = string.Empty;
                if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                {
                    n_sMsgBox = "현재 수정 작업을 취소 하시겠습니까?\n(수정하시던 내용은 저장되지 않습니다.)";
                }
                else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                {
                    n_sMsgBox = "Are you sure you want to cancel the current modification?\n(Your modifications will not be saved.)";
                }
                //-
                if (MessageBox.Show(n_sMsgBox, "ABT System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }
        private void bt_Detail_Click(object sender, EventArgs e)
        {
            Button n_Btn = (Button)sender;
            FrmDetail_Setting_View(n_Btn.Tag);
        }
        public void FrmDetail_Setting_View(object n_tag)
        {
            Form n_Form = Application.OpenForms["frmSetting_Detail"];
            if(n_tag.ToString() == "AUX")
            {
                for (int i = 0; i < lv_AUX.Items.Count; i++)
                {
                    if (lv_AUX.Items[i].Checked)
                    {
                        if (lv_AUX.Items[i].SubItems[4].Text == "" || lv_AUX.Items[i].SubItems[5].Text == "")
                        {
                            if (m_MainFrame.SystemLanguage == LanguageType.KOREAN)
                            {
                                MessageBox.Show("모듈 수량을 입력하시오", "ABT System");
                            }
                            else if (m_MainFrame.SystemLanguage == LanguageType.ENGLISH)
                            {
                                MessageBox.Show("Enter module Count", "ABT System");
                            }
                            return;
                        }
                    }
                }
            }
            ListView n_ListView =new ListView();
            switch (n_tag)
            {
                case "AUX":
                    n_ListView = lv_AUX;
                    break;
                case "Chamber":
                    n_ListView = lv_Chamber;
                    break;
                case "DAU":
                    n_ListView = lv_DAU;
                    break;
                case "CAN":
                    n_ListView = lv_CAN;
                    break;
                case "CREVIS":
                    n_ListView = lv_CREVIS;
                    break;
            }
            if (n_Form == null)
            {
                frmDetail_Setting n_frmDetail_Set = new frmDetail_Setting(n_tag, m_System_Set, this, m_MainFrame, n_ListView);
                n_frmDetail_Set.WindowState = FormWindowState.Normal;
                n_frmDetail_Set.StartPosition = FormStartPosition.CenterParent;
                
                n_frmDetail_Set.ShowDialog();
            }
            else
            {
                if (n_Form.WindowState == FormWindowState.Minimized)
                {
                    n_Form.WindowState = FormWindowState.Maximized;
                }
                n_Form.Activate();
            }

        }
        private bool CheckAUXDetail()
        {
            bool n_ok = true;
            if (lv_AUX.Items.Count > 0)
            {
                foreach (ListViewItem n_Item in lv_AUX.Items)
                {
                    if (n_Item.Checked)
                    {
                        foreach (Sys_Set_AUX_Detail n_sys_Aux_Detail in m_Sys_AUX_Detail)
                        {
                            if (n_sys_Aux_Detail.AUX_No == (int.Parse(n_Item.SubItems[1].Text)-1))
                            {
                                int n_Vmodule_cnt;
                                int n_Tmodule_cnt;
                                if (!int.TryParse(n_Item.SubItems[4].Text, out n_Vmodule_cnt)) { n_Vmodule_cnt = 0; };
                                if (!int.TryParse(n_Item.SubItems[5].Text, out n_Tmodule_cnt)) { n_Tmodule_cnt = 0; };
                                if (n_Vmodule_cnt == n_sys_Aux_Detail.AUX_VList.Count && n_Tmodule_cnt == n_sys_Aux_Detail.AUX_TList.Count)
                                {
                                    n_ok = true;
                                    break;
                                }
                                else
                                {
                                    n_ok = false;
                                    break;
                                }
                            }
                            n_ok = false;
                        }
                        if(!n_ok)
                        {
                            break;
                        }
                    }
                }
            }
            return n_ok;
        }
        private bool CheckDetail()
        {
            if (!CheckAUXDetail())
            {
                var d = MessageBox.Show("AUX Module 갯수와 세부설정 항목의 갯수가 맞지 않습니다.\n\n세부 설정을 다시 완료한 후 진행해 주세요.", "ABT System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;

            }
            foreach (ListViewItem n_Item in lv_DAU.Items)
            {
                if (n_Item.Checked)
                {
                    if (n_Item.SubItems[3].Text.Equals("Empty"))
                    {
                        var d = MessageBox.Show("DAU의 세부 설정이 필요합니다.\n\n세부 설정을 완료한 후 진행해 주세요.", "ABT System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;

                    }
                }
            }
            foreach (ListViewItem n_Item in lv_CAN.Items)
            {
                if (n_Item.Checked)
                {
                    if (n_Item.SubItems[3].Text.Equals("Empty"))
                    {
                        var d = MessageBox.Show("CAN의 세부 설정이 필요합니다.\n\n세부 설정을 완료한 후 진행해 주세요.", "ABT System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;

                    }
                }
            }
            return true;
        }
        private void Get_FileName()
        {
            try
            {
                if (Directory.Exists(m_sMDBCFilePath))
                {
                    string[] csvFiles = Directory.GetFiles(m_sMDBCFilePath, "*.mdbc");

                    if (csvFiles.Length > 0)
                    {
                        foreach (string csvFile in csvFiles)
                        {
                            cb_CAN_mdbc.Items.Add(Path.GetFileName(csvFile));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_MainFrame.WriteSystemLog($"Cls_frmDAU_Set(Get_MDBCFile):Error loading csv data: {ex.Message}");
            }
        }

    }
}
