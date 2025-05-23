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
    public class ViewModelDeviceRegister_Info : BindableBase
    {
        #region Command
        public DelegateCommand OpenEditDataCommand { get; set; }
        public DelegateCommand OpenPopupDeleteCommand { get; set; }
        #endregion

        #region Model
        public ModelDeviceRegister Model { get; set; } = new ModelDeviceRegister();

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

        public bool IsTempModule
        {
            get => Model.IsTempModule;
            set => Model.IsTempModule = value;
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

        public ViewModelDeviceRegister_Info()
        {
            OpenEditDataCommand = new DelegateCommand(OpenEditData);
            OpenPopupDeleteCommand = new DelegateCommand(OpenPopupDelete);
        }

        private void OpenEditData()
        {
            ViewModelPopup_DeviceRegister viewModelPopup_DeviceRegister = new ViewModelPopup_DeviceRegister()
            {
                SelectedItem = Model.Name
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_DeviceRegister(viewModelPopup_DeviceRegister);
        }

        private void OpenPopupDelete()
        {
            ViewModelPopup_Warning viewModelPopup_Warning = new ViewModelPopup_Warning()
            {
                Title = string.Format("Delete Device '{0}'?", Name),
                Comment = "Are you sure you want to delete this data?\r\n" +
                          "Once you confirm, this data will be permanetly deleted.",
                CallBackDelegate = DeleteDischargerInfo,
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_Warning(viewModelPopup_Warning);
            viewModelMain.OpenNestedPopup(ModelMain.ENestedPopup.Warning);
        }

        public void DeleteDischargerInfo()
        {
            try
            {
                // 방전기 정보 제거
                bool isOk = SqliteDischargerInfo.DeleteData(Name);

                // 방전기 정보 제거 Trace Log 저장
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
                    new LogTrace(ELogTrace.TRACE_DELETE_DISCHARGER, deviceData);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_DELETE_DISCHARGER, deviceData);
                }

                // 장비 정보 등록 화면 표시
                ViewModelMain viewModelMain = ViewModelMain.Instance;
                viewModelMain.OpenPopup(ModelMain.EPopup.DeviceRegister);
            }
            catch (Exception ex) 
            {
                new LogTrace(ELogTrace.ERROR_DELETE_DISCHARGER, ex);
            }
        }
    }
}
