using DischargerV2.Languages.Strings;
using DischargerV2.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DischargerV2.Modal;
using DischargerV2.MVVM.Views;
using DischargerV2.MVVM.ViewModels;

namespace DischargerV2
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure modal infrastructure and register popup views
            ModalManager.Configure("#33FFFFFF");
            ModalManager.RegisterView<ViewPopup_Info, ViewModelPopup_Info>();
            ModalManager.RegisterView<ViewPopup_Error, ViewModelPopup_Error>();
            ModalManager.RegisterView<ViewPopup_UserSetting, ViewModelPopup_UserSetting>();
            ModalManager.RegisterView<ViewPopup_DeviceRegister, ViewModelPopup_DeviceRegister>();
            ModalManager.RegisterView<ViewPopup_ModelRegister, ViewModelPopup_ModelRegister>();
            ModalManager.RegisterView<ViewPopup_Warning, ViewModelPopup_Warning>();
            ModalManager.RegisterView<ViewPopup_Waiting, ViewModelPopup_Waiting>();
            ModalManager.RegisterView<ViewPopup_EditUser, ViewModelPopup_EditUser>();
            ModalManager.RegisterView<ViewPopup_CreateNewUser, ViewModelPopup_CreateNewUser>();
        }
    }
}
