using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using static DischargerV2.LOG.LogTrace;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelDeviceRegister_Add : BindableBase
    {
        #region Command
        public DelegateCommand LoadModelInfoListCommand { get; set; }
        public DelegateCommand InsertNewDataCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        #endregion

        #region Model
        public ModelDeviceRegister Model { get; set; } = new ModelDeviceRegister();
        #endregion

        private static ViewModelDeviceRegister_Add _instance = null;

        public static ViewModelDeviceRegister_Add Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelDeviceRegister_Add();
                }
                return _instance;
            }
        }

        public ViewModelDeviceRegister_Add()
        {
            LoadModelInfoListCommand = new DelegateCommand(LoadModelInfoList);
            InsertNewDataCommand = new DelegateCommand(InsertNewData);
            CloseCommand = new DelegateCommand(Close);
        }

        private void InsertNewData()
        {
            if (CheckData())
            {
                bool isOk = InsertDischargerInfo();

                if (isOk)
                {
                    Close();
                }
                else
                {
                    MessageBox.Show("장비 정보 추가 실패");
                }
            }
        }

        private void Close()
        {
            // 기입한 값 초기화
            Model.Name = "";
            Model.Ip = "";
            Model.DischargerModel = "";
            Model.Type = "";
            Model.Channel = "";
            Model.VoltSpec = "";
            Model.CurrSpec = "";
            Model.IsTempModule = false;
            Model.Comport = "";
            Model.ModuleChannel = "";
            Model.TempChannel = "";

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OpenPopup(ModelMain.EPopup.DeviceRegister);
        }

        private void LoadModelInfoList()
        {
            try
            {
                List<TableDischargerModel> tableDischargerModelList = SqliteDischargerModel.GetData();

                // DischargerModelList
                string model = Model.DischargerModel;

                List<string> modelList = tableDischargerModelList.Select(x => x.Model.ToString()).ToList();
                modelList = modelList.Distinct().ToList();

                Model.DischargerModelList = modelList;
                Model.DischargerModel = modelList.Contains(model) ? model : "";

                // TypeList
                string type = Model.Type;

                tableDischargerModelList = tableDischargerModelList.FindAll(x => x.Model.ToString() == Model.DischargerModel);

                List<string> typeList = tableDischargerModelList.Select(x => x.Type.ToString()).ToList();
                typeList = typeList.Distinct().ToList();

                Model.TypeList = typeList;
                Model.Type = typeList.Contains(type) ? type : "";

                // ChannelList
                string channel = Model.Channel;

                tableDischargerModelList = tableDischargerModelList.FindAll(x => x.Type.ToString() == Model.Type);

                List<string> channelList = tableDischargerModelList.Select(x => x.Channel.ToString()).ToList();
                channelList = channelList.Distinct().ToList();

                Model.ChannelList = channelList;
                Model.Channel = channelList.Contains(channel) ? channel : "";

                // VoltSpecList
                string voltSpec = Model.VoltSpec;

                tableDischargerModelList = tableDischargerModelList.FindAll(x => x.Channel.ToString() == Model.Channel);

                List<string> voltSpecList = tableDischargerModelList.Select(x => x.SpecVoltage.ToString()).ToList();
                voltSpecList = voltSpecList.Distinct().ToList();

                Model.VoltSpecList = voltSpecList;
                Model.VoltSpec = voltSpecList.Contains(voltSpec) ? voltSpec : "";

                // CurrSpecList
                string currSpec = Model.CurrSpec;

                tableDischargerModelList = tableDischargerModelList.FindAll(x => x.SpecVoltage.ToString() == Model.VoltSpec);

                List<string> currSpecList = tableDischargerModelList.Select(x => x.SpecCurrent.ToString()).ToList();
                currSpecList = currSpecList.Distinct().ToList();

                Model.CurrSpecList = currSpecList;
                Model.CurrSpec = currSpecList.Contains(currSpec) ? currSpec : "";
            }
            catch (Exception ex) 
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");
            }
        }

        private bool CheckData()
        {
            try
            {
                List<TableDischargerInfo> dischargerInfos = SqliteDischargerInfo.GetData();

                if (dischargerInfos == null)
                {
                    MessageBox.Show("방전기 DB 정보를 불러오는데 실패하였습니다.");
                    return false;
                }

                int dischargerNameIndex = dischargerInfos.FindIndex(x => x.DischargerName.ToUpper() == Model.Name.ToUpper());

                if (dischargerNameIndex >= 0)
                {
                    MessageBox.Show("Name: 이미 존재하는 이름입니다.");
                    return false;
                }
                if (Model.Name == null || Model.Name == "")
                {
                    MessageBox.Show("Name: 필수 정보입니다.");
                    return false;
                }
                if (Model.Ip == null || Model.Ip == "")
                {
                    MessageBox.Show("Ip: 필수 정보입니다.");
                    return false;
                }
                if (Model.DischargerModel == null || Model.DischargerModel == "")
                {
                    MessageBox.Show("Model: 필수 정보입니다.");
                    return false;
                }
                if (Model.Type == null || Model.Type == "")
                {
                    MessageBox.Show("Type: 필수 정보입니다.");
                    return false;
                }
                if (Model.Channel == null || Model.Channel == "")
                {
                    MessageBox.Show("Channel: 필수 정보입니다.");
                    return false;
                }
                if (!Int16.TryParse(Model.Channel, out Int16 channel))
                {
                    MessageBox.Show("Channel: 데이터 형식이 잘못되었습니다.");
                    return false;
                }
                if (Model.VoltSpec == null || Model.VoltSpec == "")
                {
                    MessageBox.Show("VoltSpec: 필수 정보입니다.");
                    return false;
                }
                if (!double.TryParse(Model.VoltSpec, out double voltSpec))
                {
                    MessageBox.Show("VoltSpec: 데이터 형식이 잘못되었습니다.");
                    return false;
                }
                if (Model.CurrSpec == null || Model.CurrSpec == "")
                {
                    MessageBox.Show("CurrSpec: 필수 정보입니다.");
                    return false;
                }
                if (!double.TryParse(Model.CurrSpec, out double surrSpec))
                {
                    MessageBox.Show("CurrSpec: 데이터 형식이 잘못되었습니다.");
                    return false;
                }
                if (Model.ModuleChannel != null && Model.ModuleChannel != ""
                    && !Int32.TryParse(Model.ModuleChannel, out Int32 moduleChannel))
                {
                    MessageBox.Show("ModuleChannel: 데이터 형식이 잘못되었습니다.");
                    return false;
                }
                if (Model.TempChannel != null && Model.TempChannel != ""
                    && !Int32.TryParse(Model.TempChannel, out Int32 tempChannel))
                {
                    MessageBox.Show("TempChannel: 데이터 형식이 잘못되었습니다.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");
                return false;
            }
        }

        private bool InsertDischargerInfo()
        {
            try
            {
                // 방전기 정보 추가
                TableDischargerInfo tableDischargerInfo = new TableDischargerInfo();

                tableDischargerInfo.DischargerName = Model.Name;
                tableDischargerInfo.IpAddress = Model.Ip;

                foreach (EDischargerModel eDischargerModel in Enum.GetValues(typeof(EDischargerModel)))
                {
                    if (Model.DischargerModel == eDischargerModel.ToString())
                    {
                        tableDischargerInfo.Model = eDischargerModel;
                    }
                }

                foreach (EDischargeType eDischargerType in Enum.GetValues(typeof(EDischargeType)))
                {
                    if (Model.Type == eDischargerType.ToString())
                    {
                        tableDischargerInfo.Type = eDischargerType;
                    }
                }

                tableDischargerInfo.DischargerChannel = Convert.ToInt16(Model.Channel);
                tableDischargerInfo.SpecVoltage = Convert.ToDouble(Model.VoltSpec);
                tableDischargerInfo.SpecCurrent = Convert.ToDouble(Model.CurrSpec);
                tableDischargerInfo.IsTempModule = Convert.ToBoolean(Model.IsTempModule);
                tableDischargerInfo.TempModuleComPort = Model.Comport;
                tableDischargerInfo.TempModuleChannel = Model.ModuleChannel;
                tableDischargerInfo.TempChannel = Model.TempChannel;

                bool isOk = SqliteDischargerInfo.InsertData(tableDischargerInfo);

                // 방전기 정보 추가 Trace Log 저장
                DeviceData deviceData = new DeviceData()
                {
                    Name = Model.Name,
                    Model = Model.DischargerModel,
                    Type = Model.Type,
                    Channel = Model.Channel,
                    SpecVoltage = Model.VoltSpec,
                    SpecCurrent = Model.CurrSpec,
                    IpAddress = Model.Ip,
                    IsTempModule = Model.IsTempModule,
                    TempModuleComPort = Model.Comport,
                    TempModuleChannel = Model.ModuleChannel,
                    TempChannel = Model.TempChannel,
                };

                if (isOk)
                {
                    new LogTrace(ELogTrace.TRACE_ADD_DISCHARGER, deviceData);
                }
                else
                {
                    MessageBox.Show("장비 정보 추가 실패");

                    new LogTrace(ELogTrace.ERROR_ADD_DISCHARGER, deviceData);
                }

                return isOk;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");

                new LogTrace(ELogTrace.ERROR_ADD_DISCHARGER, ex);
                return false;
            }
        }
    }
}
