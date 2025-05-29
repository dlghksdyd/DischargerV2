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
                        if (ViewModelLogin.Instance.IsAdmin())
                        {
                            ViewModelMain viewModelMain = ViewModelMain.Instance;
                            viewModelMain.OpenPopup(ModelMain.EPopup.UserSetting);
                        }
                        else
                        {
                            MessageBox.Show("사용자 등록 정보를 볼 권한이 없습니다.");
                        }
                    }
                    else if (name == "xDeviceRegisterLabel")
                    {
                        if (ViewModelLogin.Instance.IsAdmin())
                        {
                            ViewModelMain viewModelMain = ViewModelMain.Instance;
                            viewModelMain.OpenPopup(ModelMain.EPopup.DeviceRegister);
                        }
                        else
                        {
                            MessageBox.Show("방전기 등록 정보를 볼 권한이 없습니다.");
                        }
                    }
                    else if (name == "xModelRegisterLabel")
                    {
                        if (ViewModelLogin.Instance.IsMintech())
                        {
                            ViewModelMain viewModelMain = ViewModelMain.Instance;
                            viewModelMain.OpenPopup(ModelMain.EPopup.ModelRegiseter);
                        }
                        else
                        {
                            MessageBox.Show("모델 등록 정보를 볼 권한이 없습니다.");
                        }
                    }
                    else if (name == "xLogoutLabel")
                    {
                        ViewModelPopup_Info viewModelPopup_Info = new ViewModelPopup_Info()
                        {
                            Title = "Confirm Log-out",
                            Comment = "Do you want to Log-out?",
                            CallBackDelegate = Logout,
                            ConfirmText = "Logout"
                        };

                        ViewModelMain viewModelMain = ViewModelMain.Instance;
                        viewModelMain.SetViewModelPopup_Info(viewModelPopup_Info);
                        viewModelMain.OpenPopup(ModelMain.EPopup.Info);
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
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffPopup();

            ViewModelLogin viewModelLogin = ViewModelLogin.Instance;
            viewModelLogin.Model.UserId = string.Empty;
            viewModelLogin.Model.Password = string.Empty;
            viewModelLogin.Model.Visibility = Visibility.Visible;
        }
    }
}
