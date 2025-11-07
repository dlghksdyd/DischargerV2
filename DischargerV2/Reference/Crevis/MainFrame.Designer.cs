namespace ABT
{
    partial class Mainframe
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            try
            {
                base.Dispose(disposing);
            }
            catch { }
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainframe));
            this.mn_Main = new System.Windows.Forms.MenuStrip();
            this.mn_File = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_File_Screen_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mn_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Start = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Reserve = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Pause = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Pause_Now = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Pause_Step = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Pause_Cycle = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Pause_Cycle_Loop = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Pause_ETC = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Pause_Cancel = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Resume = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Next_Step = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Move_Step = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mn_Control_Schedule = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mn_Control_Graph = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Control_Nyquist = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Setting = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Setting_System = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Setting_Mapping = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Setting_User = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Setting_Code = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Setting_Parallel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mn_Setting_AUX = new System.Windows.Forms.ToolStripMenuItem();
            this.DAUSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Schedule = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Schedule_Manage = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Schedule_Data_Manage = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Schedule_Data_Recover = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_View = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_View_Result = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_View_Log_Work = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_View_Log_System = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_View_Log_Device = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Language = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Language_Kor = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Language_Eng = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Help_Folder = new System.Windows.Forms.ToolStripMenuItem();
            this.mn_Help_About = new System.Windows.Forms.ToolStripMenuItem();
            this.tlp_MainBase = new System.Windows.Forms.TableLayoutPanel();
            this.pn_ChannelDetail = new System.Windows.Forms.Panel();
            this.grp_Ch_Info = new System.Windows.Forms.GroupBox();
            this.tlp_CH_Info = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_Work_Info_Data = new System.Windows.Forms.TableLayoutPanel();
            this.lb_V_CH_Schedule = new System.Windows.Forms.Label();
            this.lb_V_CH_WorkName = new System.Windows.Forms.Label();
            this.spc_Info = new System.Windows.Forms.SplitContainer();
            this.grp_Ch_Info_Work = new System.Windows.Forms.GroupBox();
            this.tlp_CH_Info_Work = new System.Windows.Forms.TableLayoutPanel();
            this.lb_T_CH_Type = new System.Windows.Forms.Label();
            this.lb_V_CH_Type = new System.Windows.Forms.Label();
            this.lb_T_CH_Code = new System.Windows.Forms.Label();
            this.lb_V_CH_Code = new System.Windows.Forms.Label();
            this.lb_T_CH_T_Time = new System.Windows.Forms.Label();
            this.lb_V_CH_T_Time = new System.Windows.Forms.Label();
            this.lb_V_CH_T_Cycle = new System.Windows.Forms.Label();
            this.lb_V_CH_T_Step = new System.Windows.Forms.Label();
            this.lb_T_CH_S_Time = new System.Windows.Forms.Label();
            this.lb_V_CH_S_Time = new System.Windows.Forms.Label();
            this.lb_T_CH_C_Cycle = new System.Windows.Forms.Label();
            this.lb_V_CH_C_Cycle = new System.Windows.Forms.Label();
            this.lb_T_CH_T_Cycle = new System.Windows.Forms.Label();
            this.lb_T_CH_C_Step = new System.Windows.Forms.Label();
            this.lb_V_CH_C_Step = new System.Windows.Forms.Label();
            this.lb_T_CH_T_Step = new System.Windows.Forms.Label();
            this.grp_Ch_Info_Data = new System.Windows.Forms.GroupBox();
            this.tlp_CH_Info_Data = new System.Windows.Forms.TableLayoutPanel();
            this.lb_V_CH_INSUL = new System.Windows.Forms.Label();
            this.lb_V_CH_Temperature = new System.Windows.Forms.Label();
            this.lb_T_CH_Temperature = new System.Windows.Forms.Label();
            this.lb_V_CH_C_Capa = new System.Windows.Forms.Label();
            this.lb_V_CH_Capa = new System.Windows.Forms.Label();
            this.lb_V_CH_Power = new System.Windows.Forms.Label();
            this.lb_T_CH_Power = new System.Windows.Forms.Label();
            this.lb_V_CH_Curr = new System.Windows.Forms.Label();
            this.lb_V_CH_Volt = new System.Windows.Forms.Label();
            this.lb_V_CH_D_Energy = new System.Windows.Forms.Label();
            this.lb_T_CH_D_Energy = new System.Windows.Forms.Label();
            this.lb_T_CH_C_Capa = new System.Windows.Forms.Label();
            this.lb_T_CH_Capa = new System.Windows.Forms.Label();
            this.lb_V_CH_C_Energy = new System.Windows.Forms.Label();
            this.lb_T_CH_C_Energy = new System.Windows.Forms.Label();
            this.lb_T_CH_Curr = new System.Windows.Forms.Label();
            this.lb_T_CH_Volt = new System.Windows.Forms.Label();
            this.lb_T_CH_D_Capa = new System.Windows.Forms.Label();
            this.lb_V_CH_D_Capa = new System.Windows.Forms.Label();
            this.lb_T_CH_IM = new System.Windows.Forms.Label();
            this.lb_V_CH_IM = new System.Windows.Forms.Label();
            this.lb_V_CH_RE = new System.Windows.Forms.Label();
            this.lb_T_CH_RE = new System.Windows.Forms.Label();
            this.lb_T_CH_Freq = new System.Windows.Forms.Label();
            this.lb_V_CH_Freq = new System.Windows.Forms.Label();
            this.lb_V_CH_RCT = new System.Windows.Forms.Label();
            this.lb_T_CH_RS = new System.Windows.Forms.Label();
            this.lb_T_CH_RCT = new System.Windows.Forms.Label();
            this.lb_V_CH_RS = new System.Windows.Forms.Label();
            this.lb_T_CH_Energy = new System.Windows.Forms.Label();
            this.lb_V_CH_Energy = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grp_chcontrol = new System.Windows.Forms.GroupBox();
            this.tlp_CH_Control = new System.Windows.Forms.TableLayoutPanel();
            this.bt_CH_Graph_RT_ACIA = new System.Windows.Forms.Button();
            this.bt_CH_Graph_RT_Cycler = new System.Windows.Forms.Button();
            this.bt_CH_Next_Step = new System.Windows.Forms.Button();
            this.bt_CH_Change_Scehdule = new System.Windows.Forms.Button();
            this.bt_CH_Work_Schedule = new System.Windows.Forms.Button();
            this.bt_CH_Work_Reserve = new System.Windows.Forms.Button();
            this.bt_CH_Work_Pause = new System.Windows.Forms.Button();
            this.bt_CH_Work_End = new System.Windows.Forms.Button();
            this.bt_CH_Work_Start = new System.Windows.Forms.Button();
            this.tlp_ControlStatus = new System.Windows.Forms.TableLayoutPanel();
            this.bt_BuzzerStop = new System.Windows.Forms.Button();
            this.bt_EMG = new System.Windows.Forms.Button();
            this.pn_Status_Board = new System.Windows.Forms.Panel();
            this.pn_Status_Channel = new System.Windows.Forms.Panel();
            this.mn_Main.SuspendLayout();
            this.tlp_MainBase.SuspendLayout();
            this.pn_ChannelDetail.SuspendLayout();
            this.grp_Ch_Info.SuspendLayout();
            this.tlp_CH_Info.SuspendLayout();
            this.tlp_Work_Info_Data.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spc_Info)).BeginInit();
            this.spc_Info.Panel1.SuspendLayout();
            this.spc_Info.Panel2.SuspendLayout();
            this.spc_Info.SuspendLayout();
            this.grp_Ch_Info_Work.SuspendLayout();
            this.tlp_CH_Info_Work.SuspendLayout();
            this.grp_Ch_Info_Data.SuspendLayout();
            this.tlp_CH_Info_Data.SuspendLayout();
            this.grp_chcontrol.SuspendLayout();
            this.tlp_CH_Control.SuspendLayout();
            this.tlp_ControlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // mn_Main
            // 
            this.mn_Main.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mn_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mn_File,
            this.mn_Edit,
            this.mn_Control,
            this.mn_Setting,
            this.mn_Schedule,
            this.mn_View,
            this.mn_Language,
            this.mn_Help});
            this.mn_Main.Location = new System.Drawing.Point(0, 0);
            this.mn_Main.Name = "mn_Main";
            this.mn_Main.Size = new System.Drawing.Size(1384, 24);
            this.mn_Main.TabIndex = 0;
            this.mn_Main.Text = "menuStrip1";
            // 
            // mn_File
            // 
            this.mn_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mn_File_Screen_Save,
            this.toolStripSeparator1,
            this.mn_File_Exit});
            this.mn_File.Name = "mn_File";
            this.mn_File.Size = new System.Drawing.Size(55, 20);
            this.mn_File.Text = "File (&F)";
            // 
            // mn_File_Screen_Save
            // 
            this.mn_File_Screen_Save.Name = "mn_File_Screen_Save";
            this.mn_File_Screen_Save.Size = new System.Drawing.Size(126, 22);
            this.mn_File_Screen_Save.Tag = "0-0";
            this.mn_File_Screen_Save.Text = "화면 저장";
            this.mn_File_Screen_Save.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(123, 6);
            // 
            // mn_File_Exit
            // 
            this.mn_File_Exit.Name = "mn_File_Exit";
            this.mn_File_Exit.Size = new System.Drawing.Size(126, 22);
            this.mn_File_Exit.Tag = "0-1";
            this.mn_File_Exit.Text = "Quit (&Q)";
            this.mn_File_Exit.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Edit
            // 
            this.mn_Edit.Name = "mn_Edit";
            this.mn_Edit.Size = new System.Drawing.Size(39, 20);
            this.mn_Edit.Tag = "1";
            this.mn_Edit.Text = "Edit";
            // 
            // mn_Control
            // 
            this.mn_Control.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mn_Control_Start,
            this.mn_Control_Stop,
            this.mn_Control_Reserve,
            this.mn_Control_Pause,
            this.mn_Control_Resume,
            this.mn_Control_Next_Step,
            this.mn_Control_Move_Step,
            this.mn_Control_Reset,
            this.toolStripSeparator3,
            this.mn_Control_Schedule,
            this.toolStripSeparator4,
            this.mn_Control_Graph,
            this.mn_Control_Nyquist});
            this.mn_Control.Name = "mn_Control";
            this.mn_Control.Size = new System.Drawing.Size(64, 20);
            this.mn_Control.Tag = "2";
            this.mn_Control.Text = "Controls";
            // 
            // mn_Control_Start
            // 
            this.mn_Control_Start.Name = "mn_Control_Start";
            this.mn_Control_Start.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Start.Tag = "2-0";
            this.mn_Control_Start.Text = "작업 시작";
            this.mn_Control_Start.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Control_Stop
            // 
            this.mn_Control_Stop.Name = "mn_Control_Stop";
            this.mn_Control_Stop.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Stop.Tag = "2-1";
            this.mn_Control_Stop.Text = "작업 종료";
            this.mn_Control_Stop.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Control_Reserve
            // 
            this.mn_Control_Reserve.Enabled = false;
            this.mn_Control_Reserve.Name = "mn_Control_Reserve";
            this.mn_Control_Reserve.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Reserve.Tag = "2-2";
            this.mn_Control_Reserve.Text = "Reserved schedule";
            this.mn_Control_Reserve.Visible = false;
            // 
            // mn_Control_Pause
            // 
            this.mn_Control_Pause.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mn_Control_Pause_Now,
            this.mn_Control_Pause_Step,
            this.mn_Control_Pause_Cycle,
            this.mn_Control_Pause_Cycle_Loop,
            this.mn_Control_Pause_ETC,
            this.mn_Control_Pause_Cancel});
            this.mn_Control_Pause.Enabled = false;
            this.mn_Control_Pause.Name = "mn_Control_Pause";
            this.mn_Control_Pause.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Pause.Tag = "2-3";
            this.mn_Control_Pause.Text = "Pause schedule";
            this.mn_Control_Pause.Visible = false;
            // 
            // mn_Control_Pause_Now
            // 
            this.mn_Control_Pause_Now.Name = "mn_Control_Pause_Now";
            this.mn_Control_Pause_Now.Size = new System.Drawing.Size(237, 22);
            this.mn_Control_Pause_Now.Text = "Pause schedule";
            // 
            // mn_Control_Pause_Step
            // 
            this.mn_Control_Pause_Step.Name = "mn_Control_Pause_Step";
            this.mn_Control_Pause_Step.Size = new System.Drawing.Size(237, 22);
            this.mn_Control_Pause_Step.Text = "Pause after step is completed";
            // 
            // mn_Control_Pause_Cycle
            // 
            this.mn_Control_Pause_Cycle.Name = "mn_Control_Pause_Cycle";
            this.mn_Control_Pause_Cycle.Size = new System.Drawing.Size(237, 22);
            this.mn_Control_Pause_Cycle.Text = "Pause after cycle is completed";
            // 
            // mn_Control_Pause_Cycle_Loop
            // 
            this.mn_Control_Pause_Cycle_Loop.Name = "mn_Control_Pause_Cycle_Loop";
            this.mn_Control_Pause_Cycle_Loop.Size = new System.Drawing.Size(237, 22);
            this.mn_Control_Pause_Cycle_Loop.Text = "Pause cycle loop is completed";
            // 
            // mn_Control_Pause_ETC
            // 
            this.mn_Control_Pause_ETC.Name = "mn_Control_Pause_ETC";
            this.mn_Control_Pause_ETC.Size = new System.Drawing.Size(237, 22);
            this.mn_Control_Pause_ETC.Text = "기타";
            this.mn_Control_Pause_ETC.Visible = false;
            // 
            // mn_Control_Pause_Cancel
            // 
            this.mn_Control_Pause_Cancel.Name = "mn_Control_Pause_Cancel";
            this.mn_Control_Pause_Cancel.Size = new System.Drawing.Size(237, 22);
            this.mn_Control_Pause_Cancel.Text = "Cancel pause";
            this.mn_Control_Pause_Cancel.Visible = false;
            // 
            // mn_Control_Resume
            // 
            this.mn_Control_Resume.Enabled = false;
            this.mn_Control_Resume.Name = "mn_Control_Resume";
            this.mn_Control_Resume.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Resume.Tag = "2-4";
            this.mn_Control_Resume.Text = "Resume schedule";
            this.mn_Control_Resume.Visible = false;
            // 
            // mn_Control_Next_Step
            // 
            this.mn_Control_Next_Step.Name = "mn_Control_Next_Step";
            this.mn_Control_Next_Step.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Next_Step.Tag = "2-5";
            this.mn_Control_Next_Step.Text = "다음 Step";
            this.mn_Control_Next_Step.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Control_Move_Step
            // 
            this.mn_Control_Move_Step.Enabled = false;
            this.mn_Control_Move_Step.Name = "mn_Control_Move_Step";
            this.mn_Control_Move_Step.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Move_Step.Tag = "2-6";
            this.mn_Control_Move_Step.Text = "Move step";
            this.mn_Control_Move_Step.Visible = false;
            // 
            // mn_Control_Reset
            // 
            this.mn_Control_Reset.Enabled = false;
            this.mn_Control_Reset.Name = "mn_Control_Reset";
            this.mn_Control_Reset.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Reset.Tag = "2-7";
            this.mn_Control_Reset.Text = "Reset channel";
            this.mn_Control_Reset.Visible = false;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(189, 6);
            // 
            // mn_Control_Schedule
            // 
            this.mn_Control_Schedule.Name = "mn_Control_Schedule";
            this.mn_Control_Schedule.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Schedule.Tag = "2-8";
            this.mn_Control_Schedule.Text = "스케줄 보기";
            this.mn_Control_Schedule.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(189, 6);
            // 
            // mn_Control_Graph
            // 
            this.mn_Control_Graph.Name = "mn_Control_Graph";
            this.mn_Control_Graph.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Graph.Tag = "2-9";
            this.mn_Control_Graph.Text = "실시간 그래프 [V/I/Q]";
            this.mn_Control_Graph.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Control_Nyquist
            // 
            this.mn_Control_Nyquist.Name = "mn_Control_Nyquist";
            this.mn_Control_Nyquist.Size = new System.Drawing.Size(192, 22);
            this.mn_Control_Nyquist.Tag = "2-10";
            this.mn_Control_Nyquist.Text = "실시간 그래프 [Z]";
            this.mn_Control_Nyquist.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Setting
            // 
            this.mn_Setting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mn_Setting_System,
            this.mn_Setting_Mapping,
            this.mn_Setting_User,
            this.mn_Setting_Code,
            this.mn_Setting_Parallel,
            this.toolStripSeparator2,
            this.mn_Setting_AUX,
            this.DAUSettingToolStripMenuItem});
            this.mn_Setting.Name = "mn_Setting";
            this.mn_Setting.Size = new System.Drawing.Size(57, 20);
            this.mn_Setting.Tag = "3";
            this.mn_Setting.Text = "Setting";
            // 
            // mn_Setting_System
            // 
            this.mn_Setting_System.Name = "mn_Setting_System";
            this.mn_Setting_System.Size = new System.Drawing.Size(193, 22);
            this.mn_Setting_System.Tag = "3-0";
            this.mn_Setting_System.Text = "System";
            this.mn_Setting_System.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Setting_Mapping
            // 
            this.mn_Setting_Mapping.Name = "mn_Setting_Mapping";
            this.mn_Setting_Mapping.Size = new System.Drawing.Size(193, 22);
            this.mn_Setting_Mapping.Tag = "3-1";
            this.mn_Setting_Mapping.Text = "Device Mapping";
            this.mn_Setting_Mapping.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Setting_User
            // 
            this.mn_Setting_User.Enabled = false;
            this.mn_Setting_User.Name = "mn_Setting_User";
            this.mn_Setting_User.Size = new System.Drawing.Size(193, 22);
            this.mn_Setting_User.Tag = "3-2";
            this.mn_Setting_User.Text = "사용자 Option";
            this.mn_Setting_User.Visible = false;
            // 
            // mn_Setting_Code
            // 
            this.mn_Setting_Code.Enabled = false;
            this.mn_Setting_Code.Name = "mn_Setting_Code";
            this.mn_Setting_Code.Size = new System.Drawing.Size(193, 22);
            this.mn_Setting_Code.Tag = "3-3";
            this.mn_Setting_Code.Text = "Channel Code 설정";
            this.mn_Setting_Code.Visible = false;
            // 
            // mn_Setting_Parallel
            // 
            this.mn_Setting_Parallel.Enabled = false;
            this.mn_Setting_Parallel.Name = "mn_Setting_Parallel";
            this.mn_Setting_Parallel.Size = new System.Drawing.Size(193, 22);
            this.mn_Setting_Parallel.Tag = "3-4";
            this.mn_Setting_Parallel.Text = "병렬 모드 설정";
            this.mn_Setting_Parallel.Visible = false;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(190, 6);
            // 
            // mn_Setting_AUX
            // 
            this.mn_Setting_AUX.Name = "mn_Setting_AUX";
            this.mn_Setting_AUX.Size = new System.Drawing.Size(193, 22);
            this.mn_Setting_AUX.Tag = "3-5";
            this.mn_Setting_AUX.Text = "External Setting (AUX)";
            this.mn_Setting_AUX.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // DAUSettingToolStripMenuItem
            // 
            this.DAUSettingToolStripMenuItem.Name = "DAUSettingToolStripMenuItem";
            this.DAUSettingToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.DAUSettingToolStripMenuItem.Tag = "3-6";
            this.DAUSettingToolStripMenuItem.Text = "DAU Setting";
            this.DAUSettingToolStripMenuItem.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Schedule
            // 
            this.mn_Schedule.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mn_Schedule_Manage,
            this.mn_Schedule_Data_Manage,
            this.mn_Schedule_Data_Recover});
            this.mn_Schedule.Name = "mn_Schedule";
            this.mn_Schedule.Size = new System.Drawing.Size(68, 20);
            this.mn_Schedule.Tag = "4";
            this.mn_Schedule.Text = "Schedule";
            this.mn_Schedule.Click += new System.EventHandler(this.도구ToolStripMenuItem_Click);
            // 
            // mn_Schedule_Manage
            // 
            this.mn_Schedule_Manage.Name = "mn_Schedule_Manage";
            this.mn_Schedule_Manage.Size = new System.Drawing.Size(183, 22);
            this.mn_Schedule_Manage.Tag = "4-0";
            this.mn_Schedule_Manage.Text = "Manage schedule";
            this.mn_Schedule_Manage.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Schedule_Data_Manage
            // 
            this.mn_Schedule_Data_Manage.Enabled = false;
            this.mn_Schedule_Data_Manage.Name = "mn_Schedule_Data_Manage";
            this.mn_Schedule_Data_Manage.Size = new System.Drawing.Size(183, 22);
            this.mn_Schedule_Data_Manage.Tag = "4-1";
            this.mn_Schedule_Data_Manage.Text = "작업 완료 Data 관리";
            this.mn_Schedule_Data_Manage.Visible = false;
            // 
            // mn_Schedule_Data_Recover
            // 
            this.mn_Schedule_Data_Recover.Enabled = false;
            this.mn_Schedule_Data_Recover.Name = "mn_Schedule_Data_Recover";
            this.mn_Schedule_Data_Recover.Size = new System.Drawing.Size(183, 22);
            this.mn_Schedule_Data_Recover.Tag = "4-2";
            this.mn_Schedule_Data_Recover.Text = "손실 Data 복구";
            this.mn_Schedule_Data_Recover.Visible = false;
            // 
            // mn_View
            // 
            this.mn_View.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mn_View_Result,
            this.mn_View_Log_Work,
            this.mn_View_Log_System,
            this.mn_View_Log_Device});
            this.mn_View.Name = "mn_View";
            this.mn_View.Size = new System.Drawing.Size(45, 20);
            this.mn_View.Tag = "5";
            this.mn_View.Text = "View";
            // 
            // mn_View_Result
            // 
            this.mn_View_Result.Name = "mn_View_Result";
            this.mn_View_Result.Size = new System.Drawing.Size(140, 22);
            this.mn_View_Result.Tag = "5-0";
            this.mn_View_Result.Text = "Result data";
            this.mn_View_Result.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_View_Log_Work
            // 
            this.mn_View_Log_Work.Name = "mn_View_Log_Work";
            this.mn_View_Log_Work.Size = new System.Drawing.Size(140, 22);
            this.mn_View_Log_Work.Tag = "5-2";
            this.mn_View_Log_Work.Text = "Working log";
            this.mn_View_Log_Work.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_View_Log_System
            // 
            this.mn_View_Log_System.Name = "mn_View_Log_System";
            this.mn_View_Log_System.Size = new System.Drawing.Size(140, 22);
            this.mn_View_Log_System.Tag = "5-3";
            this.mn_View_Log_System.Text = "System log";
            this.mn_View_Log_System.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_View_Log_Device
            // 
            this.mn_View_Log_Device.Name = "mn_View_Log_Device";
            this.mn_View_Log_Device.Size = new System.Drawing.Size(140, 22);
            this.mn_View_Log_Device.Tag = "5-4";
            this.mn_View_Log_Device.Text = "Device log";
            this.mn_View_Log_Device.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Language
            // 
            this.mn_Language.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mn_Language_Kor,
            this.mn_Language_Eng});
            this.mn_Language.Name = "mn_Language";
            this.mn_Language.Size = new System.Drawing.Size(71, 20);
            this.mn_Language.Tag = "6";
            this.mn_Language.Text = "Language";
            // 
            // mn_Language_Kor
            // 
            this.mn_Language_Kor.Name = "mn_Language_Kor";
            this.mn_Language_Kor.Size = new System.Drawing.Size(112, 22);
            this.mn_Language_Kor.Tag = "6-0";
            this.mn_Language_Kor.Text = "한국어";
            this.mn_Language_Kor.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Language_Eng
            // 
            this.mn_Language_Eng.Name = "mn_Language_Eng";
            this.mn_Language_Eng.Size = new System.Drawing.Size(112, 22);
            this.mn_Language_Eng.Tag = "6-1";
            this.mn_Language_Eng.Text = "English";
            this.mn_Language_Eng.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Help
            // 
            this.mn_Help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mn_Help_Folder,
            this.mn_Help_About});
            this.mn_Help.Name = "mn_Help";
            this.mn_Help.Size = new System.Drawing.Size(44, 20);
            this.mn_Help.Tag = "7";
            this.mn_Help.Text = "Help";
            // 
            // mn_Help_Folder
            // 
            this.mn_Help_Folder.Name = "mn_Help_Folder";
            this.mn_Help_Folder.Size = new System.Drawing.Size(148, 22);
            this.mn_Help_Folder.Tag = "7-0";
            this.mn_Help_Folder.Text = "Setup folder";
            this.mn_Help_Folder.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // mn_Help_About
            // 
            this.mn_Help_About.Name = "mn_Help_About";
            this.mn_Help_About.Size = new System.Drawing.Size(148, 22);
            this.mn_Help_About.Tag = "7-1";
            this.mn_Help_About.Text = "Program info.";
            this.mn_Help_About.Click += new System.EventHandler(this.MainMenuItem_Click);
            // 
            // tlp_MainBase
            // 
            this.tlp_MainBase.BackColor = System.Drawing.Color.White;
            this.tlp_MainBase.ColumnCount = 1;
            this.tlp_MainBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_MainBase.Controls.Add(this.pn_ChannelDetail, 0, 2);
            this.tlp_MainBase.Controls.Add(this.tlp_ControlStatus, 0, 0);
            this.tlp_MainBase.Controls.Add(this.pn_Status_Channel, 0, 1);
            this.tlp_MainBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_MainBase.Location = new System.Drawing.Point(0, 24);
            this.tlp_MainBase.Name = "tlp_MainBase";
            this.tlp_MainBase.RowCount = 3;
            this.tlp_MainBase.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp_MainBase.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_MainBase.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 380F));
            this.tlp_MainBase.Size = new System.Drawing.Size(1384, 937);
            this.tlp_MainBase.TabIndex = 1;
            // 
            // pn_ChannelDetail
            // 
            this.pn_ChannelDetail.AutoScroll = true;
            this.pn_ChannelDetail.Controls.Add(this.grp_Ch_Info);
            this.pn_ChannelDetail.Controls.Add(this.grp_chcontrol);
            this.pn_ChannelDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pn_ChannelDetail.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.pn_ChannelDetail.Location = new System.Drawing.Point(10, 562);
            this.pn_ChannelDetail.Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.pn_ChannelDetail.Name = "pn_ChannelDetail";
            this.pn_ChannelDetail.Size = new System.Drawing.Size(1364, 370);
            this.pn_ChannelDetail.TabIndex = 0;
            // 
            // grp_Ch_Info
            // 
            this.grp_Ch_Info.BackColor = System.Drawing.Color.White;
            this.grp_Ch_Info.Controls.Add(this.tlp_CH_Info);
            this.grp_Ch_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grp_Ch_Info.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.grp_Ch_Info.ForeColor = System.Drawing.Color.OrangeRed;
            this.grp_Ch_Info.Location = new System.Drawing.Point(0, 0);
            this.grp_Ch_Info.Name = "grp_Ch_Info";
            this.grp_Ch_Info.Size = new System.Drawing.Size(1114, 370);
            this.grp_Ch_Info.TabIndex = 2;
            this.grp_Ch_Info.TabStop = false;
            this.grp_Ch_Info.Text = " 채널 상세 정보";
            // 
            // tlp_CH_Info
            // 
            this.tlp_CH_Info.ColumnCount = 1;
            this.tlp_CH_Info.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_CH_Info.Controls.Add(this.tlp_Work_Info_Data, 0, 1);
            this.tlp_CH_Info.Controls.Add(this.spc_Info, 0, 0);
            this.tlp_CH_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_CH_Info.Location = new System.Drawing.Point(3, 23);
            this.tlp_CH_Info.Margin = new System.Windows.Forms.Padding(0);
            this.tlp_CH_Info.Name = "tlp_CH_Info";
            this.tlp_CH_Info.RowCount = 2;
            this.tlp_CH_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87F));
            this.tlp_CH_Info.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13F));
            this.tlp_CH_Info.Size = new System.Drawing.Size(1108, 344);
            this.tlp_CH_Info.TabIndex = 19;
            // 
            // tlp_Work_Info_Data
            // 
            this.tlp_Work_Info_Data.ColumnCount = 2;
            this.tlp_Work_Info_Data.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Work_Info_Data.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Work_Info_Data.Controls.Add(this.lb_V_CH_Schedule, 0, 0);
            this.tlp_Work_Info_Data.Controls.Add(this.lb_V_CH_WorkName, 1, 0);
            this.tlp_Work_Info_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Work_Info_Data.Location = new System.Drawing.Point(0, 299);
            this.tlp_Work_Info_Data.Margin = new System.Windows.Forms.Padding(0);
            this.tlp_Work_Info_Data.Name = "tlp_Work_Info_Data";
            this.tlp_Work_Info_Data.RowCount = 1;
            this.tlp_Work_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Work_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tlp_Work_Info_Data.Size = new System.Drawing.Size(1108, 45);
            this.tlp_Work_Info_Data.TabIndex = 18;
            // 
            // lb_V_CH_Schedule
            // 
            this.lb_V_CH_Schedule.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Schedule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_V_CH_Schedule.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Schedule.ForeColor = System.Drawing.Color.Black;
            this.lb_V_CH_Schedule.Location = new System.Drawing.Point(8, 8);
            this.lb_V_CH_Schedule.Margin = new System.Windows.Forms.Padding(8);
            this.lb_V_CH_Schedule.Name = "lb_V_CH_Schedule";
            this.lb_V_CH_Schedule.Padding = new System.Windows.Forms.Padding(3);
            this.lb_V_CH_Schedule.Size = new System.Drawing.Size(538, 29);
            this.lb_V_CH_Schedule.TabIndex = 16;
            this.lb_V_CH_Schedule.Text = "   Schedule 이름 ";
            this.lb_V_CH_Schedule.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb_V_CH_WorkName
            // 
            this.lb_V_CH_WorkName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_WorkName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_V_CH_WorkName.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_WorkName.ForeColor = System.Drawing.Color.Black;
            this.lb_V_CH_WorkName.Location = new System.Drawing.Point(562, 8);
            this.lb_V_CH_WorkName.Margin = new System.Windows.Forms.Padding(8);
            this.lb_V_CH_WorkName.Name = "lb_V_CH_WorkName";
            this.lb_V_CH_WorkName.Padding = new System.Windows.Forms.Padding(3);
            this.lb_V_CH_WorkName.Size = new System.Drawing.Size(538, 29);
            this.lb_V_CH_WorkName.TabIndex = 17;
            this.lb_V_CH_WorkName.Text = "   Work 이름 ";
            this.lb_V_CH_WorkName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spc_Info
            // 
            this.spc_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spc_Info.Location = new System.Drawing.Point(0, 0);
            this.spc_Info.Margin = new System.Windows.Forms.Padding(0);
            this.spc_Info.Name = "spc_Info";
            // 
            // spc_Info.Panel1
            // 
            this.spc_Info.Panel1.Controls.Add(this.grp_Ch_Info_Work);
            this.spc_Info.Panel1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.spc_Info.Panel1MinSize = 30;
            // 
            // spc_Info.Panel2
            // 
            this.spc_Info.Panel2.Controls.Add(this.grp_Ch_Info_Data);
            this.spc_Info.Panel2.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.spc_Info.Panel2MinSize = 30;
            this.spc_Info.Size = new System.Drawing.Size(1108, 299);
            this.spc_Info.SplitterDistance = 340;
            this.spc_Info.SplitterWidth = 3;
            this.spc_Info.TabIndex = 17;
            // 
            // grp_Ch_Info_Work
            // 
            this.grp_Ch_Info_Work.Controls.Add(this.tlp_CH_Info_Work);
            this.grp_Ch_Info_Work.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grp_Ch_Info_Work.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.grp_Ch_Info_Work.ForeColor = System.Drawing.Color.Black;
            this.grp_Ch_Info_Work.Location = new System.Drawing.Point(0, 10);
            this.grp_Ch_Info_Work.Name = "grp_Ch_Info_Work";
            this.grp_Ch_Info_Work.Padding = new System.Windows.Forms.Padding(5);
            this.grp_Ch_Info_Work.Size = new System.Drawing.Size(340, 289);
            this.grp_Ch_Info_Work.TabIndex = 0;
            this.grp_Ch_Info_Work.TabStop = false;
            this.grp_Ch_Info_Work.Text = "Step 정보";
            // 
            // tlp_CH_Info_Work
            // 
            this.tlp_CH_Info_Work.ColumnCount = 2;
            this.tlp_CH_Info_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_CH_Info_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_CH_Info_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_CH_Info_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_CH_Info_Work.Controls.Add(this.lb_T_CH_Type, 0, 0);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_V_CH_Type, 1, 0);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_T_CH_Code, 0, 1);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_V_CH_Code, 1, 1);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_T_CH_T_Time, 0, 3);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_V_CH_T_Time, 1, 3);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_V_CH_T_Cycle, 1, 5);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_V_CH_T_Step, 1, 7);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_T_CH_S_Time, 0, 2);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_V_CH_S_Time, 1, 2);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_T_CH_C_Cycle, 0, 4);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_V_CH_C_Cycle, 1, 4);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_T_CH_T_Cycle, 0, 5);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_T_CH_C_Step, 0, 6);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_V_CH_C_Step, 1, 6);
            this.tlp_CH_Info_Work.Controls.Add(this.lb_T_CH_T_Step, 0, 7);
            this.tlp_CH_Info_Work.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_CH_Info_Work.Location = new System.Drawing.Point(5, 23);
            this.tlp_CH_Info_Work.Name = "tlp_CH_Info_Work";
            this.tlp_CH_Info_Work.Padding = new System.Windows.Forms.Padding(3);
            this.tlp_CH_Info_Work.RowCount = 8;
            this.tlp_CH_Info_Work.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlp_CH_Info_Work.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlp_CH_Info_Work.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlp_CH_Info_Work.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlp_CH_Info_Work.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlp_CH_Info_Work.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlp_CH_Info_Work.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlp_CH_Info_Work.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tlp_CH_Info_Work.Size = new System.Drawing.Size(330, 261);
            this.tlp_CH_Info_Work.TabIndex = 18;
            this.tlp_CH_Info_Work.Resize += new System.EventHandler(this.tlp_CH_Info_Work_Resize);
            // 
            // lb_T_CH_Type
            // 
            this.lb_T_CH_Type.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_Type.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_Type.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_Type.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_Type.Location = new System.Drawing.Point(4, 4);
            this.lb_T_CH_Type.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_Type.Name = "lb_T_CH_Type";
            this.lb_T_CH_Type.Size = new System.Drawing.Size(160, 29);
            this.lb_T_CH_Type.TabIndex = 20;
            this.lb_T_CH_Type.Text = "작업 Type";
            this.lb_T_CH_Type.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_Type
            // 
            this.lb_V_CH_Type.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_Type.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Type.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Type.Location = new System.Drawing.Point(166, 4);
            this.lb_V_CH_Type.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_Type.Name = "lb_V_CH_Type";
            this.lb_V_CH_Type.Size = new System.Drawing.Size(160, 28);
            this.lb_V_CH_Type.TabIndex = 21;
            this.lb_V_CH_Type.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_Code
            // 
            this.lb_T_CH_Code.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_Code.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_Code.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_Code.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_Code.Location = new System.Drawing.Point(4, 35);
            this.lb_T_CH_Code.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_Code.Name = "lb_T_CH_Code";
            this.lb_T_CH_Code.Size = new System.Drawing.Size(160, 29);
            this.lb_T_CH_Code.TabIndex = 13;
            this.lb_T_CH_Code.Text = "상태 Code";
            this.lb_T_CH_Code.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_Code
            // 
            this.lb_V_CH_Code.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_Code.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Code.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Code.Location = new System.Drawing.Point(166, 35);
            this.lb_V_CH_Code.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_Code.Name = "lb_V_CH_Code";
            this.lb_V_CH_Code.Size = new System.Drawing.Size(160, 28);
            this.lb_V_CH_Code.TabIndex = 20;
            this.lb_V_CH_Code.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_T_Time
            // 
            this.lb_T_CH_T_Time.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_T_Time.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_T_Time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_T_Time.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_T_Time.Location = new System.Drawing.Point(4, 97);
            this.lb_T_CH_T_Time.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_T_Time.Name = "lb_T_CH_T_Time";
            this.lb_T_CH_T_Time.Size = new System.Drawing.Size(160, 29);
            this.lb_T_CH_T_Time.TabIndex = 1;
            this.lb_T_CH_T_Time.Text = "전체 소요 시간";
            this.lb_T_CH_T_Time.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_T_Time
            // 
            this.lb_V_CH_T_Time.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_T_Time.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_T_Time.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_T_Time.Location = new System.Drawing.Point(166, 97);
            this.lb_V_CH_T_Time.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_T_Time.Name = "lb_V_CH_T_Time";
            this.lb_V_CH_T_Time.Size = new System.Drawing.Size(160, 28);
            this.lb_V_CH_T_Time.TabIndex = 20;
            this.lb_V_CH_T_Time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_V_CH_T_Cycle
            // 
            this.lb_V_CH_T_Cycle.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_T_Cycle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_T_Cycle.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_T_Cycle.Location = new System.Drawing.Point(166, 159);
            this.lb_V_CH_T_Cycle.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_T_Cycle.Name = "lb_V_CH_T_Cycle";
            this.lb_V_CH_T_Cycle.Size = new System.Drawing.Size(160, 28);
            this.lb_V_CH_T_Cycle.TabIndex = 20;
            this.lb_V_CH_T_Cycle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_V_CH_T_Step
            // 
            this.lb_V_CH_T_Step.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_T_Step.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_T_Step.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_T_Step.Location = new System.Drawing.Point(166, 221);
            this.lb_V_CH_T_Step.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_T_Step.Name = "lb_V_CH_T_Step";
            this.lb_V_CH_T_Step.Size = new System.Drawing.Size(160, 33);
            this.lb_V_CH_T_Step.TabIndex = 20;
            this.lb_V_CH_T_Step.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_S_Time
            // 
            this.lb_T_CH_S_Time.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_S_Time.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_S_Time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_S_Time.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_S_Time.Location = new System.Drawing.Point(4, 66);
            this.lb_T_CH_S_Time.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_S_Time.Name = "lb_T_CH_S_Time";
            this.lb_T_CH_S_Time.Size = new System.Drawing.Size(160, 29);
            this.lb_T_CH_S_Time.TabIndex = 2;
            this.lb_T_CH_S_Time.Text = "Step 소요 시간";
            this.lb_T_CH_S_Time.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_S_Time
            // 
            this.lb_V_CH_S_Time.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_S_Time.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_S_Time.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_S_Time.Location = new System.Drawing.Point(166, 66);
            this.lb_V_CH_S_Time.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_S_Time.Name = "lb_V_CH_S_Time";
            this.lb_V_CH_S_Time.Size = new System.Drawing.Size(160, 28);
            this.lb_V_CH_S_Time.TabIndex = 20;
            this.lb_V_CH_S_Time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_C_Cycle
            // 
            this.lb_T_CH_C_Cycle.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_C_Cycle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_C_Cycle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_C_Cycle.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_C_Cycle.Location = new System.Drawing.Point(4, 128);
            this.lb_T_CH_C_Cycle.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_C_Cycle.Name = "lb_T_CH_C_Cycle";
            this.lb_T_CH_C_Cycle.Size = new System.Drawing.Size(160, 29);
            this.lb_T_CH_C_Cycle.TabIndex = 10;
            this.lb_T_CH_C_Cycle.Text = "현재 Cycle";
            this.lb_T_CH_C_Cycle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_C_Cycle
            // 
            this.lb_V_CH_C_Cycle.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_C_Cycle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_C_Cycle.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_C_Cycle.Location = new System.Drawing.Point(166, 128);
            this.lb_V_CH_C_Cycle.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_C_Cycle.Name = "lb_V_CH_C_Cycle";
            this.lb_V_CH_C_Cycle.Size = new System.Drawing.Size(160, 28);
            this.lb_V_CH_C_Cycle.TabIndex = 20;
            this.lb_V_CH_C_Cycle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_T_Cycle
            // 
            this.lb_T_CH_T_Cycle.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_T_Cycle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_T_Cycle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_T_Cycle.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_T_Cycle.Location = new System.Drawing.Point(4, 159);
            this.lb_T_CH_T_Cycle.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_T_Cycle.Name = "lb_T_CH_T_Cycle";
            this.lb_T_CH_T_Cycle.Size = new System.Drawing.Size(160, 29);
            this.lb_T_CH_T_Cycle.TabIndex = 3;
            this.lb_T_CH_T_Cycle.Text = "전체 Cycle";
            this.lb_T_CH_T_Cycle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_T_CH_C_Step
            // 
            this.lb_T_CH_C_Step.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_C_Step.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_C_Step.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_C_Step.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_C_Step.Location = new System.Drawing.Point(4, 190);
            this.lb_T_CH_C_Step.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_C_Step.Name = "lb_T_CH_C_Step";
            this.lb_T_CH_C_Step.Size = new System.Drawing.Size(160, 29);
            this.lb_T_CH_C_Step.TabIndex = 1;
            this.lb_T_CH_C_Step.Text = "현재 Step";
            this.lb_T_CH_C_Step.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_C_Step
            // 
            this.lb_V_CH_C_Step.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_C_Step.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_C_Step.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_C_Step.Location = new System.Drawing.Point(166, 190);
            this.lb_V_CH_C_Step.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_C_Step.Name = "lb_V_CH_C_Step";
            this.lb_V_CH_C_Step.Size = new System.Drawing.Size(160, 28);
            this.lb_V_CH_C_Step.TabIndex = 20;
            this.lb_V_CH_C_Step.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_T_Step
            // 
            this.lb_T_CH_T_Step.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_T_Step.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_T_Step.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_T_Step.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_T_Step.Location = new System.Drawing.Point(4, 221);
            this.lb_T_CH_T_Step.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_T_Step.Name = "lb_T_CH_T_Step";
            this.lb_T_CH_T_Step.Size = new System.Drawing.Size(160, 36);
            this.lb_T_CH_T_Step.TabIndex = 1;
            this.lb_T_CH_T_Step.Text = "전체 Step";
            this.lb_T_CH_T_Step.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grp_Ch_Info_Data
            // 
            this.grp_Ch_Info_Data.Controls.Add(this.tlp_CH_Info_Data);
            this.grp_Ch_Info_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grp_Ch_Info_Data.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.grp_Ch_Info_Data.ForeColor = System.Drawing.Color.Black;
            this.grp_Ch_Info_Data.Location = new System.Drawing.Point(0, 10);
            this.grp_Ch_Info_Data.Name = "grp_Ch_Info_Data";
            this.grp_Ch_Info_Data.Padding = new System.Windows.Forms.Padding(5);
            this.grp_Ch_Info_Data.Size = new System.Drawing.Size(765, 289);
            this.grp_Ch_Info_Data.TabIndex = 1;
            this.grp_Ch_Info_Data.TabStop = false;
            this.grp_Ch_Info_Data.Text = "동작 정보";
            // 
            // tlp_CH_Info_Data
            // 
            this.tlp_CH_Info_Data.ColumnCount = 4;
            this.tlp_CH_Info_Data.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlp_CH_Info_Data.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlp_CH_Info_Data.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlp_CH_Info_Data.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_INSUL, 3, 6);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_Temperature, 3, 0);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_Temperature, 2, 0);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_C_Capa, 1, 3);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_Capa, 1, 2);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_Power, 1, 8);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_Power, 0, 8);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_Curr, 1, 1);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_Volt, 1, 0);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_D_Energy, 1, 7);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_D_Energy, 0, 7);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_C_Capa, 0, 3);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_Capa, 0, 2);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_C_Energy, 1, 6);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_C_Energy, 0, 6);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_Curr, 0, 1);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_Volt, 0, 0);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_D_Capa, 0, 4);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_D_Capa, 1, 4);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_IM, 2, 3);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_IM, 3, 3);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_RE, 3, 2);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_RE, 2, 2);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_Freq, 2, 1);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_Freq, 3, 1);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_RCT, 3, 5);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_RS, 2, 4);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_RCT, 2, 5);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_RS, 3, 4);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_T_CH_Energy, 0, 5);
            this.tlp_CH_Info_Data.Controls.Add(this.lb_V_CH_Energy, 1, 5);
            this.tlp_CH_Info_Data.Controls.Add(this.label1, 2, 6);
            this.tlp_CH_Info_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_CH_Info_Data.Location = new System.Drawing.Point(5, 23);
            this.tlp_CH_Info_Data.Margin = new System.Windows.Forms.Padding(0);
            this.tlp_CH_Info_Data.Name = "tlp_CH_Info_Data";
            this.tlp_CH_Info_Data.Padding = new System.Windows.Forms.Padding(3);
            this.tlp_CH_Info_Data.RowCount = 9;
            this.tlp_CH_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlp_CH_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlp_CH_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlp_CH_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlp_CH_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlp_CH_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlp_CH_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlp_CH_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlp_CH_Info_Data.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tlp_CH_Info_Data.Size = new System.Drawing.Size(755, 261);
            this.tlp_CH_Info_Data.TabIndex = 18;
            this.tlp_CH_Info_Data.Resize += new System.EventHandler(this.tlp_CH_Info_Data_Resize);
            // 
            // lb_V_CH_INSUL
            // 
            this.lb_V_CH_INSUL.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_INSUL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_INSUL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_V_CH_INSUL.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_INSUL.Location = new System.Drawing.Point(565, 172);
            this.lb_V_CH_INSUL.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_INSUL.Name = "lb_V_CH_INSUL";
            this.lb_V_CH_INSUL.Size = new System.Drawing.Size(186, 26);
            this.lb_V_CH_INSUL.TabIndex = 30;
            this.lb_V_CH_INSUL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_V_CH_Temperature
            // 
            this.lb_V_CH_Temperature.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_Temperature.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Temperature.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Temperature.Location = new System.Drawing.Point(565, 4);
            this.lb_V_CH_Temperature.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_Temperature.Name = "lb_V_CH_Temperature";
            this.lb_V_CH_Temperature.Size = new System.Drawing.Size(186, 25);
            this.lb_V_CH_Temperature.TabIndex = 28;
            this.lb_V_CH_Temperature.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_Temperature
            // 
            this.lb_T_CH_Temperature.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_Temperature.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_Temperature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_Temperature.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_Temperature.Location = new System.Drawing.Point(378, 4);
            this.lb_T_CH_Temperature.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_Temperature.Name = "lb_T_CH_Temperature";
            this.lb_T_CH_Temperature.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_Temperature.TabIndex = 27;
            this.lb_T_CH_Temperature.Text = "온도 (℃)";
            this.lb_T_CH_Temperature.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_C_Capa
            // 
            this.lb_V_CH_C_Capa.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_C_Capa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_C_Capa.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_C_Capa.Location = new System.Drawing.Point(191, 88);
            this.lb_V_CH_C_Capa.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_C_Capa.Name = "lb_V_CH_C_Capa";
            this.lb_V_CH_C_Capa.Size = new System.Drawing.Size(185, 25);
            this.lb_V_CH_C_Capa.TabIndex = 20;
            this.lb_V_CH_C_Capa.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_V_CH_Capa
            // 
            this.lb_V_CH_Capa.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_Capa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Capa.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Capa.Location = new System.Drawing.Point(191, 60);
            this.lb_V_CH_Capa.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_Capa.Name = "lb_V_CH_Capa";
            this.lb_V_CH_Capa.Size = new System.Drawing.Size(185, 25);
            this.lb_V_CH_Capa.TabIndex = 20;
            this.lb_V_CH_Capa.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_V_CH_Power
            // 
            this.lb_V_CH_Power.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_Power.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Power.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Power.Location = new System.Drawing.Point(191, 228);
            this.lb_V_CH_Power.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_Power.Name = "lb_V_CH_Power";
            this.lb_V_CH_Power.Size = new System.Drawing.Size(185, 27);
            this.lb_V_CH_Power.TabIndex = 26;
            this.lb_V_CH_Power.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_Power
            // 
            this.lb_T_CH_Power.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_Power.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_Power.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_Power.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_Power.Location = new System.Drawing.Point(4, 228);
            this.lb_T_CH_Power.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_Power.Name = "lb_T_CH_Power";
            this.lb_T_CH_Power.Size = new System.Drawing.Size(185, 29);
            this.lb_T_CH_Power.TabIndex = 25;
            this.lb_T_CH_Power.Text = "Power (W)";
            this.lb_T_CH_Power.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_Curr
            // 
            this.lb_V_CH_Curr.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_Curr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Curr.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Curr.Location = new System.Drawing.Point(191, 32);
            this.lb_V_CH_Curr.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_Curr.Name = "lb_V_CH_Curr";
            this.lb_V_CH_Curr.Size = new System.Drawing.Size(185, 25);
            this.lb_V_CH_Curr.TabIndex = 20;
            this.lb_V_CH_Curr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_V_CH_Volt
            // 
            this.lb_V_CH_Volt.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_Volt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Volt.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Volt.Location = new System.Drawing.Point(191, 4);
            this.lb_V_CH_Volt.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_Volt.Name = "lb_V_CH_Volt";
            this.lb_V_CH_Volt.Size = new System.Drawing.Size(185, 25);
            this.lb_V_CH_Volt.TabIndex = 20;
            this.lb_V_CH_Volt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_V_CH_D_Energy
            // 
            this.lb_V_CH_D_Energy.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_D_Energy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_D_Energy.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_D_Energy.Location = new System.Drawing.Point(191, 200);
            this.lb_V_CH_D_Energy.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_D_Energy.Name = "lb_V_CH_D_Energy";
            this.lb_V_CH_D_Energy.Size = new System.Drawing.Size(185, 25);
            this.lb_V_CH_D_Energy.TabIndex = 24;
            this.lb_V_CH_D_Energy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_D_Energy
            // 
            this.lb_T_CH_D_Energy.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_D_Energy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_D_Energy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_D_Energy.Font = new System.Drawing.Font("맑은 고딕", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_D_Energy.Location = new System.Drawing.Point(4, 200);
            this.lb_T_CH_D_Energy.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_D_Energy.Name = "lb_T_CH_D_Energy";
            this.lb_T_CH_D_Energy.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_D_Energy.TabIndex = 23;
            this.lb_T_CH_D_Energy.Text = "전체 방전 Energy (Wh)";
            this.lb_T_CH_D_Energy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_T_CH_C_Capa
            // 
            this.lb_T_CH_C_Capa.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_C_Capa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_C_Capa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_C_Capa.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_C_Capa.Location = new System.Drawing.Point(4, 88);
            this.lb_T_CH_C_Capa.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_C_Capa.Name = "lb_T_CH_C_Capa";
            this.lb_T_CH_C_Capa.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_C_Capa.TabIndex = 7;
            this.lb_T_CH_C_Capa.Text = "전체 충전 용량 (Ah)";
            this.lb_T_CH_C_Capa.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_T_CH_Capa
            // 
            this.lb_T_CH_Capa.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_Capa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_Capa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_Capa.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_Capa.Location = new System.Drawing.Point(4, 60);
            this.lb_T_CH_Capa.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_Capa.Name = "lb_T_CH_Capa";
            this.lb_T_CH_Capa.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_Capa.TabIndex = 6;
            this.lb_T_CH_Capa.Text = "용량 (Ah)";
            this.lb_T_CH_Capa.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_C_Energy
            // 
            this.lb_V_CH_C_Energy.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_C_Energy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_C_Energy.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_C_Energy.Location = new System.Drawing.Point(191, 172);
            this.lb_V_CH_C_Energy.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_C_Energy.Name = "lb_V_CH_C_Energy";
            this.lb_V_CH_C_Energy.Size = new System.Drawing.Size(185, 25);
            this.lb_V_CH_C_Energy.TabIndex = 22;
            this.lb_V_CH_C_Energy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_C_Energy
            // 
            this.lb_T_CH_C_Energy.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_C_Energy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_C_Energy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_C_Energy.Font = new System.Drawing.Font("맑은 고딕", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_C_Energy.Location = new System.Drawing.Point(4, 172);
            this.lb_T_CH_C_Energy.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_C_Energy.Name = "lb_T_CH_C_Energy";
            this.lb_T_CH_C_Energy.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_C_Energy.TabIndex = 21;
            this.lb_T_CH_C_Energy.Text = "전체 충전 Energy (Wh)";
            this.lb_T_CH_C_Energy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_T_CH_Curr
            // 
            this.lb_T_CH_Curr.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_Curr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_Curr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_Curr.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_Curr.Location = new System.Drawing.Point(4, 32);
            this.lb_T_CH_Curr.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_Curr.Name = "lb_T_CH_Curr";
            this.lb_T_CH_Curr.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_Curr.TabIndex = 5;
            this.lb_T_CH_Curr.Text = "전류 (A)";
            this.lb_T_CH_Curr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_T_CH_Volt
            // 
            this.lb_T_CH_Volt.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_Volt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_Volt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_Volt.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_Volt.Location = new System.Drawing.Point(4, 4);
            this.lb_T_CH_Volt.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_Volt.Name = "lb_T_CH_Volt";
            this.lb_T_CH_Volt.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_Volt.TabIndex = 4;
            this.lb_T_CH_Volt.Text = "전압 (V)";
            this.lb_T_CH_Volt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_T_CH_D_Capa
            // 
            this.lb_T_CH_D_Capa.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_D_Capa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_D_Capa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_D_Capa.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_D_Capa.Location = new System.Drawing.Point(4, 116);
            this.lb_T_CH_D_Capa.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_D_Capa.Name = "lb_T_CH_D_Capa";
            this.lb_T_CH_D_Capa.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_D_Capa.TabIndex = 8;
            this.lb_T_CH_D_Capa.Text = "전체 방전 용량 (Ah)";
            this.lb_T_CH_D_Capa.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_D_Capa
            // 
            this.lb_V_CH_D_Capa.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_D_Capa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_D_Capa.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_D_Capa.Location = new System.Drawing.Point(191, 116);
            this.lb_V_CH_D_Capa.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_D_Capa.Name = "lb_V_CH_D_Capa";
            this.lb_V_CH_D_Capa.Size = new System.Drawing.Size(185, 25);
            this.lb_V_CH_D_Capa.TabIndex = 20;
            this.lb_V_CH_D_Capa.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_IM
            // 
            this.lb_T_CH_IM.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_IM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_IM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_IM.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_IM.Location = new System.Drawing.Point(378, 88);
            this.lb_T_CH_IM.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_IM.Name = "lb_T_CH_IM";
            this.lb_T_CH_IM.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_IM.TabIndex = 21;
            this.lb_T_CH_IM.Text = "IM (mΩ)";
            this.lb_T_CH_IM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_IM
            // 
            this.lb_V_CH_IM.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_IM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_IM.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_IM.Location = new System.Drawing.Point(565, 88);
            this.lb_V_CH_IM.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_IM.Name = "lb_V_CH_IM";
            this.lb_V_CH_IM.Size = new System.Drawing.Size(186, 25);
            this.lb_V_CH_IM.TabIndex = 21;
            this.lb_V_CH_IM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_V_CH_RE
            // 
            this.lb_V_CH_RE.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_RE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_RE.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_RE.Location = new System.Drawing.Point(565, 60);
            this.lb_V_CH_RE.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_RE.Name = "lb_V_CH_RE";
            this.lb_V_CH_RE.Size = new System.Drawing.Size(186, 25);
            this.lb_V_CH_RE.TabIndex = 21;
            this.lb_V_CH_RE.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_RE
            // 
            this.lb_T_CH_RE.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_RE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_RE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_RE.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_RE.Location = new System.Drawing.Point(378, 60);
            this.lb_T_CH_RE.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_RE.Name = "lb_T_CH_RE";
            this.lb_T_CH_RE.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_RE.TabIndex = 21;
            this.lb_T_CH_RE.Text = "RE (mΩ)";
            this.lb_T_CH_RE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_T_CH_Freq
            // 
            this.lb_T_CH_Freq.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_Freq.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_Freq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_Freq.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_Freq.Location = new System.Drawing.Point(378, 32);
            this.lb_T_CH_Freq.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_Freq.Name = "lb_T_CH_Freq";
            this.lb_T_CH_Freq.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_Freq.TabIndex = 21;
            this.lb_T_CH_Freq.Text = "주파수 (Hz)";
            this.lb_T_CH_Freq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_Freq
            // 
            this.lb_V_CH_Freq.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_Freq.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Freq.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Freq.Location = new System.Drawing.Point(565, 32);
            this.lb_V_CH_Freq.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_Freq.Name = "lb_V_CH_Freq";
            this.lb_V_CH_Freq.Size = new System.Drawing.Size(186, 25);
            this.lb_V_CH_Freq.TabIndex = 21;
            this.lb_V_CH_Freq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_V_CH_RCT
            // 
            this.lb_V_CH_RCT.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_RCT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_RCT.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_RCT.Location = new System.Drawing.Point(565, 144);
            this.lb_V_CH_RCT.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_RCT.Name = "lb_V_CH_RCT";
            this.lb_V_CH_RCT.Size = new System.Drawing.Size(186, 25);
            this.lb_V_CH_RCT.TabIndex = 20;
            this.lb_V_CH_RCT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_RS
            // 
            this.lb_T_CH_RS.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_RS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_RS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_RS.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_RS.Location = new System.Drawing.Point(378, 116);
            this.lb_T_CH_RS.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_RS.Name = "lb_T_CH_RS";
            this.lb_T_CH_RS.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_RS.TabIndex = 11;
            this.lb_T_CH_RS.Text = "Rs (mΩ)";
            this.lb_T_CH_RS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_T_CH_RCT
            // 
            this.lb_T_CH_RCT.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_RCT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_RCT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_RCT.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_RCT.Location = new System.Drawing.Point(378, 144);
            this.lb_T_CH_RCT.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_RCT.Name = "lb_T_CH_RCT";
            this.lb_T_CH_RCT.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_RCT.TabIndex = 9;
            this.lb_T_CH_RCT.Text = "Rct (mΩ)";
            this.lb_T_CH_RCT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_RS
            // 
            this.lb_V_CH_RS.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_RS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_RS.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_RS.Location = new System.Drawing.Point(565, 116);
            this.lb_V_CH_RS.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_RS.Name = "lb_V_CH_RS";
            this.lb_V_CH_RS.Size = new System.Drawing.Size(186, 25);
            this.lb_V_CH_RS.TabIndex = 20;
            this.lb_V_CH_RS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_T_CH_Energy
            // 
            this.lb_T_CH_Energy.BackColor = System.Drawing.Color.Beige;
            this.lb_T_CH_Energy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_T_CH_Energy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_T_CH_Energy.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_T_CH_Energy.Location = new System.Drawing.Point(4, 144);
            this.lb_T_CH_Energy.Margin = new System.Windows.Forms.Padding(1);
            this.lb_T_CH_Energy.Name = "lb_T_CH_Energy";
            this.lb_T_CH_Energy.Size = new System.Drawing.Size(185, 26);
            this.lb_T_CH_Energy.TabIndex = 12;
            this.lb_T_CH_Energy.Text = "Energy (Wh)";
            this.lb_T_CH_Energy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_V_CH_Energy
            // 
            this.lb_V_CH_Energy.BackColor = System.Drawing.Color.White;
            this.lb_V_CH_Energy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_V_CH_Energy.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lb_V_CH_Energy.Location = new System.Drawing.Point(191, 144);
            this.lb_V_CH_Energy.Margin = new System.Windows.Forms.Padding(1);
            this.lb_V_CH_Energy.Name = "lb_V_CH_Energy";
            this.lb_V_CH_Energy.Size = new System.Drawing.Size(185, 25);
            this.lb_V_CH_Energy.TabIndex = 20;
            this.lb_V_CH_Energy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Beige;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(378, 172);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 26);
            this.label1.TabIndex = 29;
            this.label1.Text = "Insulation R(MΩ)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grp_chcontrol
            // 
            this.grp_chcontrol.BackColor = System.Drawing.Color.White;
            this.grp_chcontrol.Controls.Add(this.tlp_CH_Control);
            this.grp_chcontrol.Dock = System.Windows.Forms.DockStyle.Right;
            this.grp_chcontrol.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.grp_chcontrol.ForeColor = System.Drawing.Color.OrangeRed;
            this.grp_chcontrol.Location = new System.Drawing.Point(1114, 0);
            this.grp_chcontrol.Name = "grp_chcontrol";
            this.grp_chcontrol.Padding = new System.Windows.Forms.Padding(5);
            this.grp_chcontrol.Size = new System.Drawing.Size(250, 370);
            this.grp_chcontrol.TabIndex = 0;
            this.grp_chcontrol.TabStop = false;
            this.grp_chcontrol.Text = "채널 제어";
            // 
            // tlp_CH_Control
            // 
            this.tlp_CH_Control.ColumnCount = 1;
            this.tlp_CH_Control.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_CH_Control.Controls.Add(this.bt_CH_Graph_RT_ACIA, 0, 8);
            this.tlp_CH_Control.Controls.Add(this.bt_CH_Graph_RT_Cycler, 0, 7);
            this.tlp_CH_Control.Controls.Add(this.bt_CH_Next_Step, 0, 6);
            this.tlp_CH_Control.Controls.Add(this.bt_CH_Change_Scehdule, 0, 5);
            this.tlp_CH_Control.Controls.Add(this.bt_CH_Work_Schedule, 0, 4);
            this.tlp_CH_Control.Controls.Add(this.bt_CH_Work_Reserve, 0, 3);
            this.tlp_CH_Control.Controls.Add(this.bt_CH_Work_Pause, 0, 2);
            this.tlp_CH_Control.Controls.Add(this.bt_CH_Work_End, 0, 1);
            this.tlp_CH_Control.Controls.Add(this.bt_CH_Work_Start, 0, 0);
            this.tlp_CH_Control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_CH_Control.Location = new System.Drawing.Point(5, 25);
            this.tlp_CH_Control.Name = "tlp_CH_Control";
            this.tlp_CH_Control.RowCount = 9;
            this.tlp_CH_Control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlp_CH_Control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlp_CH_Control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tlp_CH_Control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tlp_CH_Control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlp_CH_Control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tlp_CH_Control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlp_CH_Control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlp_CH_Control.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlp_CH_Control.Size = new System.Drawing.Size(240, 340);
            this.tlp_CH_Control.TabIndex = 1;
            // 
            // bt_CH_Graph_RT_ACIA
            // 
            this.bt_CH_Graph_RT_ACIA.BackColor = System.Drawing.Color.Lavender;
            this.bt_CH_Graph_RT_ACIA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_CH_Graph_RT_ACIA.Enabled = false;
            this.bt_CH_Graph_RT_ACIA.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.bt_CH_Graph_RT_ACIA.ForeColor = System.Drawing.Color.Black;
            this.bt_CH_Graph_RT_ACIA.Location = new System.Drawing.Point(3, 282);
            this.bt_CH_Graph_RT_ACIA.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bt_CH_Graph_RT_ACIA.Name = "bt_CH_Graph_RT_ACIA";
            this.bt_CH_Graph_RT_ACIA.Size = new System.Drawing.Size(234, 56);
            this.bt_CH_Graph_RT_ACIA.TabIndex = 3;
            this.bt_CH_Graph_RT_ACIA.Text = "실시간 그래프 [Z]";
            this.bt_CH_Graph_RT_ACIA.UseVisualStyleBackColor = false;
            this.bt_CH_Graph_RT_ACIA.Click += new System.EventHandler(this.bt_CH_Graph_RT_ACIA_Click);
            // 
            // bt_CH_Graph_RT_Cycler
            // 
            this.bt_CH_Graph_RT_Cycler.BackColor = System.Drawing.Color.Lavender;
            this.bt_CH_Graph_RT_Cycler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_CH_Graph_RT_Cycler.Enabled = false;
            this.bt_CH_Graph_RT_Cycler.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.bt_CH_Graph_RT_Cycler.ForeColor = System.Drawing.Color.Black;
            this.bt_CH_Graph_RT_Cycler.Location = new System.Drawing.Point(3, 226);
            this.bt_CH_Graph_RT_Cycler.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bt_CH_Graph_RT_Cycler.Name = "bt_CH_Graph_RT_Cycler";
            this.bt_CH_Graph_RT_Cycler.Size = new System.Drawing.Size(234, 52);
            this.bt_CH_Graph_RT_Cycler.TabIndex = 2;
            this.bt_CH_Graph_RT_Cycler.Text = "실시간 그래프 [V/I/Q]";
            this.bt_CH_Graph_RT_Cycler.UseVisualStyleBackColor = false;
            this.bt_CH_Graph_RT_Cycler.Click += new System.EventHandler(this.bt_CH_Graph_RT_Cycler_Click);
            // 
            // bt_CH_Next_Step
            // 
            this.bt_CH_Next_Step.BackColor = System.Drawing.Color.Lavender;
            this.bt_CH_Next_Step.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_CH_Next_Step.Enabled = false;
            this.bt_CH_Next_Step.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.bt_CH_Next_Step.ForeColor = System.Drawing.Color.Black;
            this.bt_CH_Next_Step.Location = new System.Drawing.Point(3, 170);
            this.bt_CH_Next_Step.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bt_CH_Next_Step.Name = "bt_CH_Next_Step";
            this.bt_CH_Next_Step.Size = new System.Drawing.Size(234, 52);
            this.bt_CH_Next_Step.TabIndex = 2;
            this.bt_CH_Next_Step.Text = "다음 Step";
            this.bt_CH_Next_Step.UseVisualStyleBackColor = false;
            this.bt_CH_Next_Step.Click += new System.EventHandler(this.bt_CH_Next_Step_Click);
            // 
            // bt_CH_Change_Scehdule
            // 
            this.bt_CH_Change_Scehdule.BackColor = System.Drawing.Color.Lavender;
            this.bt_CH_Change_Scehdule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_CH_Change_Scehdule.Enabled = false;
            this.bt_CH_Change_Scehdule.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_CH_Change_Scehdule.ForeColor = System.Drawing.Color.Black;
            this.bt_CH_Change_Scehdule.Location = new System.Drawing.Point(3, 171);
            this.bt_CH_Change_Scehdule.Name = "bt_CH_Change_Scehdule";
            this.bt_CH_Change_Scehdule.Size = new System.Drawing.Size(234, 1);
            this.bt_CH_Change_Scehdule.TabIndex = 2;
            this.bt_CH_Change_Scehdule.Text = "Change schedule";
            this.bt_CH_Change_Scehdule.UseVisualStyleBackColor = false;
            this.bt_CH_Change_Scehdule.Visible = false;
            this.bt_CH_Change_Scehdule.Click += new System.EventHandler(this.bt_CH_Change_Scehdule_Click);
            // 
            // bt_CH_Work_Schedule
            // 
            this.bt_CH_Work_Schedule.BackColor = System.Drawing.Color.Lavender;
            this.bt_CH_Work_Schedule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_CH_Work_Schedule.Enabled = false;
            this.bt_CH_Work_Schedule.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.bt_CH_Work_Schedule.ForeColor = System.Drawing.Color.Black;
            this.bt_CH_Work_Schedule.Location = new System.Drawing.Point(3, 114);
            this.bt_CH_Work_Schedule.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bt_CH_Work_Schedule.Name = "bt_CH_Work_Schedule";
            this.bt_CH_Work_Schedule.Size = new System.Drawing.Size(234, 52);
            this.bt_CH_Work_Schedule.TabIndex = 2;
            this.bt_CH_Work_Schedule.Text = "스케쥴 보기";
            this.bt_CH_Work_Schedule.UseVisualStyleBackColor = false;
            this.bt_CH_Work_Schedule.Click += new System.EventHandler(this.bt_CH_Work_Schedule_Click);
            // 
            // bt_CH_Work_Reserve
            // 
            this.bt_CH_Work_Reserve.BackColor = System.Drawing.Color.Lavender;
            this.bt_CH_Work_Reserve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_CH_Work_Reserve.Enabled = false;
            this.bt_CH_Work_Reserve.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_CH_Work_Reserve.ForeColor = System.Drawing.Color.Black;
            this.bt_CH_Work_Reserve.Location = new System.Drawing.Point(3, 115);
            this.bt_CH_Work_Reserve.Name = "bt_CH_Work_Reserve";
            this.bt_CH_Work_Reserve.Size = new System.Drawing.Size(234, 1);
            this.bt_CH_Work_Reserve.TabIndex = 2;
            this.bt_CH_Work_Reserve.Text = "Reserved schedule";
            this.bt_CH_Work_Reserve.UseVisualStyleBackColor = false;
            this.bt_CH_Work_Reserve.Visible = false;
            this.bt_CH_Work_Reserve.Click += new System.EventHandler(this.bt_CH_Work_Reserve_Click);
            // 
            // bt_CH_Work_Pause
            // 
            this.bt_CH_Work_Pause.BackColor = System.Drawing.Color.Lavender;
            this.bt_CH_Work_Pause.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_CH_Work_Pause.Enabled = false;
            this.bt_CH_Work_Pause.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_CH_Work_Pause.ForeColor = System.Drawing.Color.Black;
            this.bt_CH_Work_Pause.Location = new System.Drawing.Point(3, 115);
            this.bt_CH_Work_Pause.Name = "bt_CH_Work_Pause";
            this.bt_CH_Work_Pause.Size = new System.Drawing.Size(234, 1);
            this.bt_CH_Work_Pause.TabIndex = 2;
            this.bt_CH_Work_Pause.Text = "Pause schedule";
            this.bt_CH_Work_Pause.UseVisualStyleBackColor = false;
            this.bt_CH_Work_Pause.Visible = false;
            this.bt_CH_Work_Pause.Click += new System.EventHandler(this.bt_CH_Work_Pause_Click);
            // 
            // bt_CH_Work_End
            // 
            this.bt_CH_Work_End.BackColor = System.Drawing.Color.Lavender;
            this.bt_CH_Work_End.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_CH_Work_End.Enabled = false;
            this.bt_CH_Work_End.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.bt_CH_Work_End.ForeColor = System.Drawing.Color.Black;
            this.bt_CH_Work_End.Location = new System.Drawing.Point(3, 58);
            this.bt_CH_Work_End.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bt_CH_Work_End.Name = "bt_CH_Work_End";
            this.bt_CH_Work_End.Size = new System.Drawing.Size(234, 52);
            this.bt_CH_Work_End.TabIndex = 2;
            this.bt_CH_Work_End.Text = "작업 종료";
            this.bt_CH_Work_End.UseVisualStyleBackColor = false;
            this.bt_CH_Work_End.Click += new System.EventHandler(this.bt_CH_Work_End_Click);
            // 
            // bt_CH_Work_Start
            // 
            this.bt_CH_Work_Start.BackColor = System.Drawing.Color.Lavender;
            this.bt_CH_Work_Start.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_CH_Work_Start.Enabled = false;
            this.bt_CH_Work_Start.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.bt_CH_Work_Start.ForeColor = System.Drawing.Color.Black;
            this.bt_CH_Work_Start.Location = new System.Drawing.Point(3, 2);
            this.bt_CH_Work_Start.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bt_CH_Work_Start.Name = "bt_CH_Work_Start";
            this.bt_CH_Work_Start.Size = new System.Drawing.Size(234, 52);
            this.bt_CH_Work_Start.TabIndex = 0;
            this.bt_CH_Work_Start.Text = "작업 시작";
            this.bt_CH_Work_Start.UseVisualStyleBackColor = false;
            this.bt_CH_Work_Start.Click += new System.EventHandler(this.bt_CH_Work_Start_Click);
            // 
            // tlp_ControlStatus
            // 
            this.tlp_ControlStatus.BackColor = System.Drawing.Color.White;
            this.tlp_ControlStatus.ColumnCount = 3;
            this.tlp_ControlStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_ControlStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp_ControlStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp_ControlStatus.Controls.Add(this.bt_BuzzerStop, 1, 0);
            this.tlp_ControlStatus.Controls.Add(this.bt_EMG, 2, 0);
            this.tlp_ControlStatus.Controls.Add(this.pn_Status_Board, 0, 0);
            this.tlp_ControlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_ControlStatus.Location = new System.Drawing.Point(0, 0);
            this.tlp_ControlStatus.Margin = new System.Windows.Forms.Padding(0);
            this.tlp_ControlStatus.Name = "tlp_ControlStatus";
            this.tlp_ControlStatus.RowCount = 1;
            this.tlp_ControlStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_ControlStatus.Size = new System.Drawing.Size(1384, 150);
            this.tlp_ControlStatus.TabIndex = 1;
            // 
            // bt_BuzzerStop
            // 
            this.bt_BuzzerStop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bt_BuzzerStop.BackColor = System.Drawing.Color.Gainsboro;
            this.bt_BuzzerStop.BackgroundImage = global::ABT.Properties.Resources.Vent_Close;
            this.bt_BuzzerStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_BuzzerStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_BuzzerStop.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.bt_BuzzerStop.Location = new System.Drawing.Point(1094, 10);
            this.bt_BuzzerStop.Margin = new System.Windows.Forms.Padding(10, 10, 5, 10);
            this.bt_BuzzerStop.Name = "bt_BuzzerStop";
            this.bt_BuzzerStop.Size = new System.Drawing.Size(135, 130);
            this.bt_BuzzerStop.TabIndex = 0;
            this.bt_BuzzerStop.UseVisualStyleBackColor = false;
            this.bt_BuzzerStop.Click += new System.EventHandler(this.bt_Vent_Click);
            // 
            // bt_EMG
            // 
            this.bt_EMG.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bt_EMG.BackgroundImage = global::ABT.Properties.Resources.Emergency;
            this.bt_EMG.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_EMG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bt_EMG.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.bt_EMG.Location = new System.Drawing.Point(1239, 10);
            this.bt_EMG.Margin = new System.Windows.Forms.Padding(5, 10, 10, 10);
            this.bt_EMG.Name = "bt_EMG";
            this.bt_EMG.Size = new System.Drawing.Size(135, 130);
            this.bt_EMG.TabIndex = 1;
            this.bt_EMG.UseVisualStyleBackColor = true;
            this.bt_EMG.Click += new System.EventHandler(this.bt_EMG_Click);
            // 
            // pn_Status_Board
            // 
            this.pn_Status_Board.BackColor = System.Drawing.Color.White;
            this.pn_Status_Board.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pn_Status_Board.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.pn_Status_Board.Location = new System.Drawing.Point(10, 5);
            this.pn_Status_Board.Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.pn_Status_Board.Name = "pn_Status_Board";
            this.pn_Status_Board.Size = new System.Drawing.Size(1064, 140);
            this.pn_Status_Board.TabIndex = 2;
            // 
            // pn_Status_Channel
            // 
            this.pn_Status_Channel.BackColor = System.Drawing.Color.White;
            this.pn_Status_Channel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pn_Status_Channel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.pn_Status_Channel.Location = new System.Drawing.Point(10, 155);
            this.pn_Status_Channel.Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.pn_Status_Channel.Name = "pn_Status_Channel";
            this.pn_Status_Channel.Size = new System.Drawing.Size(1364, 397);
            this.pn_Status_Channel.TabIndex = 2;
            // 
            // Mainframe
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1384, 961);
            this.Controls.Add(this.tlp_MainBase);
            this.Controls.Add(this.mn_Main);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mn_Main;
            this.Name = "Mainframe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ABTProV2 [v1.1.0.0] - MinTech";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mainframe_FormClosing);
            this.Load += new System.EventHandler(this.Mainframe_Load);
            this.Shown += new System.EventHandler(this.Mainframe_Shown);
            this.mn_Main.ResumeLayout(false);
            this.mn_Main.PerformLayout();
            this.tlp_MainBase.ResumeLayout(false);
            this.pn_ChannelDetail.ResumeLayout(false);
            this.grp_Ch_Info.ResumeLayout(false);
            this.tlp_CH_Info.ResumeLayout(false);
            this.tlp_Work_Info_Data.ResumeLayout(false);
            this.spc_Info.Panel1.ResumeLayout(false);
            this.spc_Info.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spc_Info)).EndInit();
            this.spc_Info.ResumeLayout(false);
            this.grp_Ch_Info_Work.ResumeLayout(false);
            this.tlp_CH_Info_Work.ResumeLayout(false);
            this.grp_Ch_Info_Data.ResumeLayout(false);
            this.tlp_CH_Info_Data.ResumeLayout(false);
            this.tlp_CH_Info_Data.PerformLayout();
            this.grp_chcontrol.ResumeLayout(false);
            this.tlp_CH_Control.ResumeLayout(false);
            this.tlp_ControlStatus.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mn_Main;
        private System.Windows.Forms.ToolStripMenuItem mn_File;
        private System.Windows.Forms.TableLayoutPanel tlp_MainBase;
        private System.Windows.Forms.TableLayoutPanel tlp_ControlStatus;
        private System.Windows.Forms.Button bt_BuzzerStop;
        private System.Windows.Forms.Button bt_EMG;
        private System.Windows.Forms.ToolStripMenuItem mn_File_Screen_Save;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mn_File_Exit;
        private System.Windows.Forms.ToolStripMenuItem mn_Edit;
        private System.Windows.Forms.ToolStripMenuItem mn_Control;
        private System.Windows.Forms.ToolStripMenuItem mn_Setting;
        private System.Windows.Forms.ToolStripMenuItem mn_Schedule;
        private System.Windows.Forms.ToolStripMenuItem mn_View;
        private System.Windows.Forms.ToolStripMenuItem mn_Language;
        private System.Windows.Forms.ToolStripMenuItem mn_Help;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Start;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Reserve;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Pause;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Pause_Now;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Pause_Step;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Pause_Cycle;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Pause_Cycle_Loop;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Pause_ETC;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Pause_Cancel;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Resume;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Next_Step;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Move_Step;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Stop;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Reset;
        private System.Windows.Forms.ToolStripMenuItem mn_Setting_System;
        private System.Windows.Forms.ToolStripMenuItem mn_Setting_User;
        private System.Windows.Forms.ToolStripMenuItem mn_Setting_Code;
        private System.Windows.Forms.ToolStripMenuItem mn_Setting_Parallel;
        private System.Windows.Forms.ToolStripMenuItem mn_Schedule_Manage;
        private System.Windows.Forms.ToolStripMenuItem mn_Schedule_Data_Manage;
        private System.Windows.Forms.ToolStripMenuItem mn_Schedule_Data_Recover;
        private System.Windows.Forms.ToolStripMenuItem mn_View_Result;
        private System.Windows.Forms.ToolStripMenuItem mn_View_Log_Work;
        private System.Windows.Forms.ToolStripMenuItem mn_View_Log_System;
        private System.Windows.Forms.ToolStripMenuItem mn_View_Log_Device;
        private System.Windows.Forms.ToolStripMenuItem mn_Language_Kor;
        private System.Windows.Forms.ToolStripMenuItem mn_Language_Eng;
        private System.Windows.Forms.ToolStripMenuItem mn_Help_Folder;
        private System.Windows.Forms.ToolStripMenuItem mn_Help_About;
        private System.Windows.Forms.Panel pn_Status_Board;
        private System.Windows.Forms.Panel pn_Status_Channel;
        private System.Windows.Forms.ToolStripMenuItem mn_Setting_Mapping;
        private System.Windows.Forms.Panel pn_ChannelDetail;
        private System.Windows.Forms.GroupBox grp_chcontrol;
        private System.Windows.Forms.TableLayoutPanel tlp_CH_Control;
        private System.Windows.Forms.Button bt_CH_Graph_RT_Cycler;
        private System.Windows.Forms.Button bt_CH_Next_Step;
        private System.Windows.Forms.Button bt_CH_Change_Scehdule;
        private System.Windows.Forms.Button bt_CH_Work_Schedule;
        private System.Windows.Forms.Button bt_CH_Work_Reserve;
        private System.Windows.Forms.Button bt_CH_Work_Pause;
        private System.Windows.Forms.Button bt_CH_Work_End;
        private System.Windows.Forms.Button bt_CH_Work_Start;
        private System.Windows.Forms.GroupBox grp_Ch_Info;
        private System.Windows.Forms.TableLayoutPanel tlp_CH_Info;
        private System.Windows.Forms.TableLayoutPanel tlp_CH_Info_Work;
        private System.Windows.Forms.Label lb_V_CH_C_Cycle;
        private System.Windows.Forms.Label lb_V_CH_T_Cycle;
        private System.Windows.Forms.Label lb_V_CH_S_Time;
        private System.Windows.Forms.Label lb_V_CH_T_Time;
        private System.Windows.Forms.Label lb_V_CH_Code;
        private System.Windows.Forms.Label lb_V_CH_C_Step;
        private System.Windows.Forms.Label lb_V_CH_T_Step;
        private System.Windows.Forms.Label lb_T_CH_Code;
        private System.Windows.Forms.Label lb_T_CH_C_Step;
        private System.Windows.Forms.Label lb_T_CH_T_Step;
        private System.Windows.Forms.Label lb_T_CH_C_Cycle;
        private System.Windows.Forms.Label lb_T_CH_T_Cycle;
        private System.Windows.Forms.Label lb_T_CH_S_Time;
        private System.Windows.Forms.Label lb_T_CH_T_Time;
        private System.Windows.Forms.Label lb_V_CH_Type;
        private System.Windows.Forms.Label lb_T_CH_Type;
        private System.Windows.Forms.SplitContainer spc_Info;
        private System.Windows.Forms.GroupBox grp_Ch_Info_Work;
        private System.Windows.Forms.GroupBox grp_Ch_Info_Data;
        private System.Windows.Forms.TableLayoutPanel tlp_CH_Info_Data;
        private System.Windows.Forms.Label lb_V_CH_C_Capa;
        private System.Windows.Forms.Label lb_V_CH_Capa;
        private System.Windows.Forms.Label lb_V_CH_Curr;
        private System.Windows.Forms.Label lb_V_CH_Volt;
        private System.Windows.Forms.Label lb_T_CH_C_Capa;
        private System.Windows.Forms.Label lb_T_CH_Capa;
        private System.Windows.Forms.Label lb_T_CH_Curr;
        private System.Windows.Forms.Label lb_T_CH_Volt;
        private System.Windows.Forms.Label lb_T_CH_D_Capa;
        private System.Windows.Forms.Label lb_T_CH_RCT;
        private System.Windows.Forms.Label lb_T_CH_RS;
        private System.Windows.Forms.Label lb_T_CH_Energy;
        private System.Windows.Forms.Label lb_V_CH_D_Capa;
        private System.Windows.Forms.Label lb_V_CH_RCT;
        private System.Windows.Forms.Label lb_V_CH_RS;
        private System.Windows.Forms.Label lb_V_CH_Energy;
        private System.Windows.Forms.Label lb_T_CH_Freq;
        private System.Windows.Forms.Label lb_T_CH_IM;
        private System.Windows.Forms.Label lb_T_CH_RE;
        private System.Windows.Forms.Label lb_V_CH_Freq;
        private System.Windows.Forms.Label lb_V_CH_IM;
        private System.Windows.Forms.Label lb_V_CH_RE;
        private System.Windows.Forms.Label lb_V_CH_Temperature;
        private System.Windows.Forms.Label lb_T_CH_Temperature;
        private System.Windows.Forms.Label lb_V_CH_Power;
        private System.Windows.Forms.Label lb_T_CH_Power;
        private System.Windows.Forms.Label lb_V_CH_D_Energy;
        private System.Windows.Forms.Label lb_T_CH_D_Energy;
        private System.Windows.Forms.Label lb_V_CH_C_Energy;
        private System.Windows.Forms.Label lb_T_CH_C_Energy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mn_Setting_AUX;
        private System.Windows.Forms.Button bt_CH_Graph_RT_ACIA;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Schedule;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Graph;
        private System.Windows.Forms.ToolStripMenuItem mn_Control_Nyquist;
        private System.Windows.Forms.TableLayoutPanel tlp_Work_Info_Data;
        private System.Windows.Forms.Label lb_V_CH_Schedule;
        private System.Windows.Forms.Label lb_V_CH_WorkName;
        private System.Windows.Forms.Label lb_V_CH_INSUL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem DAUSettingToolStripMenuItem;
    }
}

