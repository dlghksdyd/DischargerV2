using DischargerV2.Languages.Strings;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using DischargerV2.Modal;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_Setting : BindableBase
    {
        #region Command
        public ICommand xLabel_MouseLeftButtonUpCommand { get; set; }
        public ICommand xLabel_MouseEnterCommand { get; set; }
        public ICommand xLabel_MouseLeaveCommand { get; set; }
        #endregion

        #region Model
        public ModelPopup_Setting Model { get; set; } = new ModelPopup_Setting();
        #endregion
        
        public ViewModelPopup_Setting()
        {
            xLabel_MouseLeftButtonUpCommand = new RelayCommand<object>(xLabel_MouseLeftButtonUp);
            xLabel_MouseEnterCommand = new RelayCommand<object>(xLabel_MouseEnter);
            xLabel_MouseLeaveCommand = new RelayCommand<object>(xLabel_MouseLeave);
        }

        private void xLabel_MouseLeftButtonUp(object obj)
        {
            if (obj is MouseEventArgs e)
            {
                if (e.Source is MexLabel mexLabel)
                {
                    var name = mexLabel.Name;

                    if (name == "xUserSettingLabel")
                    {
                        if (ViewModelLogin.Instance.IsLocalDb() && ViewModelLogin.Instance.IsAdmin())
                        {
                            var vm = new ViewModelPopup_UserSetting();
                            ModalManager.Open(vm);
                        }
                        else
                        {
                            var title = new DynamicString().GetDynamicString("PopupWarning_Title_NoPermission");
                            var comment = new DynamicString().GetDynamicString("PopupWarning_Comment_NoPermissionUser");

                            var vm = new ViewModelPopup_Warning()
                            {
                                Title = title,
                                Comment = comment,
                                CancelButtonVisibility = Visibility.Hidden
                            };

                            ModalManager.Open(vm);
                        }
                    }
                    else if (name == "xDeviceRegisterLabel")
                    {
                        if (ViewModelLogin.Instance.IsAdmin())
                        {
                            var vm = new ViewModelPopup_DeviceRegister();
                            ModalManager.Open(vm);
                        }
                        else
                        {
                            var title = new DynamicString().GetDynamicString("PopupWarning_Title_NoPermission");
                            var comment = new DynamicString().GetDynamicString("PopupWarning_Comment_NoPermissionDevice");

                            var vm = new ViewModelPopup_Warning()
                            {
                                Title = title,
                                Comment = comment,
                                CancelButtonVisibility = Visibility.Hidden
                            };

                            ModalManager.Open(vm);
                        }
                    }
                    else if (name == "xModelRegisterLabel")
                    {
                        if (ViewModelLogin.Instance.IsMintech())
                        {
                            var vm = new ViewModelPopup_ModelRegister();
                            ModalManager.Open(vm);
                        }
                        else
                        {
                            var title = new DynamicString().GetDynamicString("PopupWarning_Title_NoPermission");
                            var comment = new DynamicString().GetDynamicString("PopupWarning_Comment_NoPermissionModel");

                            var vm = new ViewModelPopup_Warning()
                            {
                                Title = title,
                                Comment = comment,
                                CancelButtonVisibility = Visibility.Hidden
                            };

                            ModalManager.Open(vm);
                        }
                    }
                    else if (name == "xLogoutLabel")
                    {
                        string title = new DynamicString().GetDynamicString("PopupInfo_Title_Logout");
                        string comment = new DynamicString().GetDynamicString("PopupInfo_Comment_Logout");
                        string confirmText = new DynamicString().GetDynamicString("Logout");

                        ViewModelPopup_Info vm = new ViewModelPopup_Info()
                        {
                            Title = title,
                            Comment = comment,
                            Parameter = string.Empty,
                            CallBackDelegate = Logout,
                            ConfirmText = confirmText,
                            CancelVisibility = Visibility.Visible
                        };

                        ModalManager.Open(vm);
                    }
                }
            }
        }

        private void xLabel_MouseEnter(object obj)
        {
            if (obj is MouseEventArgs e)
            {
                if (e.Source is MexLabel mexLabel)
                {
                    SolidColorBrush setColor = ResColor.surface_action_hover2;
                    var name = mexLabel.Name;

                    if (name == "xUserSettingLabel")
                    {
                        Model.Background_UserSetting = setColor;
                    }
                    else if (name == "xDeviceRegisterLabel")
                    {
                        Model.Background_DeviceRegister = setColor;
                    }
                    else if (name == "xModelRegisterLabel")
                    {
                        Model.Background_DbConfiguration = setColor;
                    }
                    else if (name == "xLogoutLabel")
                    {
                        Model.Background_Logout = setColor;
                    }
                }
            }
        }

        private void xLabel_MouseLeave(object obj)
        {
            if (obj is MouseEventArgs e)
            {
                if (e.Source is MexLabel mexLabel)
                {
                    SolidColorBrush setColor = ResColor.surface_primary;
                    var name = mexLabel.Name;

                    if (name == "xUserSettingLabel")
                    {
                        Model.Background_UserSetting = setColor;
                    }
                    else if (name == "xDeviceRegisterLabel")
                    {
                        Model.Background_DeviceRegister = setColor;
                    }
                    else if (name == "xModelRegisterLabel")
                    {
                        Model.Background_DbConfiguration = setColor;
                    }
                    else if (name == "xLogoutLabel")
                    {
                        Model.Background_Logout = setColor;
                    }
                }
            }
        }

        private void Logout()
        {
            // 기존 OffPopup 제거. Logout 로직만 수행.
            ViewModelDischarger viewModelDischarger = ViewModelDischarger.Instance;
            viewModelDischarger.FinalizeDischarger();

            ViewModelLogin viewModelLogin = ViewModelLogin.Instance;
            viewModelLogin.Model.UserName = string.Empty;
            viewModelLogin.Model.UserId = string.Empty;
            viewModelLogin.Model.Password = string.Empty;
            viewModelLogin.Model.Visibility = Visibility.Visible;
        }
    }
}
