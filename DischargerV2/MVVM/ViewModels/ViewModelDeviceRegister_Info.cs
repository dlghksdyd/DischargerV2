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
    public class ViewModelDeviceRegister_Info : BindableBase
    {
        #region Command
        public DelegateCommand xEditButton_ClickCommand { get; set; }
        public DelegateCommand xDeleteButton_ClickCommand { get; set; }
        #endregion

        #region Model
        public ModelDeviceRegister_Info Model { get; set; } = new ModelDeviceRegister_Info();

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

        public EDischargerModel EModel
        {
            get => Model.EModel;
            set => Model.EModel = value;
        }

        public EDischargeType EType
        {
            get => Model.EType;
            set => Model.EType = value;
        }

        public short Channel
        {
            get => Model.Channel;
            set => Model.Channel = value;
        }

        public double VoltSpec
        {
            get => Model.VoltSpec;
            set => Model.VoltSpec = value;
        }

        public double CurrSpec
        {
            get => Model.CurrSpec;
            set => Model.CurrSpec = value;
        }

        public string Comport
        {
            get => Model.Comport;
            set => Model.Comport = value;
        }

        public int ModuleChannel
        {
            get => Model.ModuleChannel;
            set => Model.ModuleChannel = value;
        }

        public int TempChannel
        {
            get => Model.TempChannel;
            set => Model.TempChannel = value;
        }
        #endregion

        public ViewModelDeviceRegister_Info()
        {
            xEditButton_ClickCommand = new DelegateCommand(xEditButton_Click);
            xDeleteButton_ClickCommand = new DelegateCommand(xDeleteButton_Click);
        }

        private void xEditButton_Click()
        {
            ViewModelPopup_DeviceRegister viewModelPopup_DeviceRegister = new ViewModelPopup_DeviceRegister()
            {
                SelectedItem = Model.Name
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_DeviceRegister(viewModelPopup_DeviceRegister);
        }

        private void xDeleteButton_Click()
        {
            ViewModelPopup_Warning viewModelPopup_Warning = new ViewModelPopup_Warning()
            {
                Title = string.Format("Delete Device '{0}'?", Name),
                Comment = "Are you sure you want to delete this Device?\r\n" +
                          "Once you confirm, this device data will be permanetly deleted.",
                CallBackDeledate = DeleteDevice,
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_Warning(viewModelPopup_Warning);
            viewModelMain.OpenNestedPopup(ModelMain.ENestedPopup.Warning);
        }

        public void DeleteDevice()
        {
            SqliteDischargerInfo.DeleteData(Name);

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OpenPopup(ModelMain.EPopup.DeviceRegister);
        }
    }
}
