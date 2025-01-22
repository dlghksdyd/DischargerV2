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

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelDeviceRegister_Edit : BindableBase
    {
        #region Command
        public DelegateCommand xComboBox_MouseLeaveCommand { get; set; }
        public DelegateCommand xEditButton_ClickCommand { get; set; }
        public DelegateCommand xCancelButton_ClickCommand { get; set; }
        #endregion

        #region Model
        public ModelDeviceRegister_Edit Model { get; set; } = new ModelDeviceRegister_Edit();

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }

        public string Ip
        {
            get => Model.Ip;
            set => Model.Ip = value;
        }

        public string DischargerModel
        {
            get => Model.DischargerModel;
            set => Model.DischargerModel = value;
        }

        public string Type
        {
            get => Model.Type;
            set => Model.Type = value;
        }

        public string Channel
        {
            get => Model.Channel;
            set => Model.Channel = value;
        }

        public string VoltSpec
        {
            get => Model.VoltSpec;
            set => Model.VoltSpec = value;
        }

        public string CurrSpec
        {
            get => Model.CurrSpec;
            set => Model.CurrSpec = value;
        }

        public string Comport
        {
            get => Model.Comport;
            set => Model.Comport = value;
        }

        public string ModuleChannel
        {
            get => Model.ModuleChannel;
            set => Model.ModuleChannel = value;
        }

        public string TempChannel
        {
            get => Model.TempChannel;
            set => Model.TempChannel = value;
        }
        #endregion

        private static ViewModelDeviceRegister_Edit _instance = null;

        public static ViewModelDeviceRegister_Edit Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelDeviceRegister_Edit();
                }
                return _instance;
            }
        }

        public ViewModelDeviceRegister_Edit()
        {
            xComboBox_MouseLeaveCommand = new DelegateCommand(xComboBox_MouseLeave);
            xEditButton_ClickCommand = new DelegateCommand(xEditButton_Click);
            xCancelButton_ClickCommand = new DelegateCommand(xCancelButton_Click);

            LoadModelInfoList();
        }

        private void xComboBox_MouseLeave()
        {
            LoadModelInfoList();
        }

        private void xEditButton_Click()
        {
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
            tableDischargerInfo.TempModuleComPort = Model.Comport;
            tableDischargerInfo.TempModuleChannel = Convert.ToInt32(Model.ModuleChannel);
            tableDischargerInfo.TempChannel = Convert.ToInt32(Model.TempChannel);

            SqliteDischargerInfo.UpdateData(tableDischargerInfo);

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OpenPopup(ModelMain.EPopup.DeviceRegiseter);
        }

        private void xCancelButton_Click()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OpenPopup(ModelMain.EPopup.DeviceRegiseter);
        }

        private void LoadModelInfoList()
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
    }
}
