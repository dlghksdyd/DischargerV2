using DischargerV2.MVVM.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DischargerV2.MVVM.Models
{
    public class ModelMain : BindableBase
    {
        public enum EPopup 
        { 
            UserSetting, DeviceRegiseter, ModelRegiseter, 
            Info
        }

        public enum EPopup2
        {
            CreateNewUser, EditUser, 
            Warning
        }

        private WindowState _windowState = WindowState.Maximized;
        public WindowState WindowState
        {
            get
            {
                return _windowState;
            }
            set
            {
                SetProperty(ref _windowState, value);
            }
        }

        private bool _isPopupOpen = false;
        public bool IsPopupOpen
        {
            get
            {
                return _isPopupOpen;
            }
            set
            {
                SetProperty(ref _isPopupOpen, value);
            }
        }

        private bool _isPopupOpen2 = false;
        public bool IsPopupOpen2
        {
            get
            {
                return _isPopupOpen2;
            }
            set
            {
                SetProperty(ref _isPopupOpen2, value);
            }
        }

        private Visibility[] _popupVisibility = new Visibility[Enum.GetValues(typeof(EPopup)).Length];
        public Visibility[] PopupVisibility
        {
            get
            {
                return _popupVisibility;
            }
            set
            {
                SetProperty(ref _popupVisibility, value);
            }
        }

        private Visibility[] _popupVisibility2 = new Visibility[Enum.GetValues(typeof(EPopup2)).Length];
        public Visibility[] PopupVisibility2
        {
            get
            {
                return _popupVisibility2;
            }
            set
            {
                SetProperty(ref _popupVisibility2, value);
            }
        }

        private ViewModelPopup_UserSetting _viewModelPopup_UserSetting = new ViewModelPopup_UserSetting();
        public ViewModelPopup_UserSetting ViewModelPopup_UserSetting
        {
            get
            {
                return _viewModelPopup_UserSetting;
            }
            set
            {
                SetProperty(ref _viewModelPopup_UserSetting, value);
            }
        }

        private ViewModelPopup_CreateNewUser _viewModelPopup_CreateNewUser = new ViewModelPopup_CreateNewUser();
        public ViewModelPopup_CreateNewUser ViewModelPopup_CreateNewUser
        {
            get
            {
                return _viewModelPopup_CreateNewUser;
            }
            set
            {
                SetProperty(ref _viewModelPopup_CreateNewUser, value);
            }
        }

        private ViewModelPopup_EditUser _viewModelPopup_EditUser = new ViewModelPopup_EditUser();
        public ViewModelPopup_EditUser ViewModelPopup_EditUser
        {
            get
            {
                return _viewModelPopup_EditUser;
            }
            set
            {
                SetProperty(ref _viewModelPopup_EditUser, value);
            }
        }

        private ViewModelPopup_Warning _viewModelPopup_Warning = new ViewModelPopup_Warning();
        public ViewModelPopup_Warning ViewModelPopup_Warning
        {
            get
            {
                return _viewModelPopup_Warning;
            }
            set
            {
                SetProperty(ref _viewModelPopup_Warning, value);
            }
        }
    }
}