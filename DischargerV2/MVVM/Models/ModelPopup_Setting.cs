using MExpress.Mex;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DischargerV2.MVVM.Models
{
    public class ModelPopup_Setting : BindableBase
    {
        // User Setting
        private SolidColorBrush _background_UserSetting = ResColor.surface_primary;
        public SolidColorBrush Background_UserSetting
        {
            get 
            { 
                return _background_UserSetting; 
            } 
            set 
            { 
                SetProperty(ref _background_UserSetting, value); 
            }
        }

        // Device Register
        private SolidColorBrush _background_DeviceRegister = ResColor.surface_primary;
        public SolidColorBrush Background_DeviceRegister
        {
            get
            {
                return _background_DeviceRegister;
            }
            set
            {
                SetProperty(ref _background_DeviceRegister, value);
            }
        }

        // DB Configuration
        private SolidColorBrush _background_DbConfiguration = ResColor.surface_primary;
        public SolidColorBrush Background_DbConfiguration
        {
            get
            {
                return _background_DbConfiguration;
            }
            set
            {
                SetProperty(ref _background_DbConfiguration, value);
            }
        }

        // Log out
        private SolidColorBrush _background_Logout = ResColor.surface_primary;
        public SolidColorBrush Background_Logout
        {
            get
            {
                return _background_Logout;
            }
            set
            {
                SetProperty(ref _background_Logout, value);
            }
        }
    }
}