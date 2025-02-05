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
            UserSetting, DeviceRegister, ModelRegiseter, 
            Info, Error,
        }

        public enum ENestedPopup
        {
            CreateNewUser, EditUser, 
            Warning,
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

        private string _selectedDischargerName;
        public string SelectedDischargerName
        {
            get
            { 
                return _selectedDischargerName; 
            }
            set
            {
                SetProperty(ref _selectedDischargerName, value);
            }
        }

        private bool _isSetMode = true;
        public bool IsSetMode
        {
            get
            {
                return _isSetMode;
            }
            set
            {
                SetProperty(ref _isSetMode, value);
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

        private bool _isNestedPopupOpen = false;
        public bool IsNestedPopupOpen
        {
            get
            {
                return _isNestedPopupOpen;
            }
            set
            {
                SetProperty(ref _isNestedPopupOpen, value);
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

        private Visibility[] _nestedPopupVisibility = new Visibility[Enum.GetValues(typeof(ENestedPopup)).Length];
        public Visibility[] NestedPopupVisibility
        {
            get
            {
                return _nestedPopupVisibility;
            }
            set
            {
                SetProperty(ref _nestedPopupVisibility, value);
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

        private ViewModelPopup_DeviceRegister _viewModelPopup_DeviceRegister = new ViewModelPopup_DeviceRegister();
        public ViewModelPopup_DeviceRegister ViewModelPopup_DeviceRegister
        {
            get
            {
                return _viewModelPopup_DeviceRegister;
            }
            set
            {
                SetProperty(ref _viewModelPopup_DeviceRegister, value);
            }
        }

        private ViewModelPopup_ModelRegister _viewModelPopup_ModelRegister = new ViewModelPopup_ModelRegister();
        public ViewModelPopup_ModelRegister ViewModelPopup_ModelRegister
        {
            get
            {
                return _viewModelPopup_ModelRegister;
            }
            set
            {
                SetProperty(ref _viewModelPopup_ModelRegister, value);
            }
        }

        private ViewModelPopup_Info _viewModelPopup_Info = new ViewModelPopup_Info();
        public ViewModelPopup_Info ViewModelPopup_Info
        {
            get
            {
                return _viewModelPopup_Info;
            }
            set
            {
                SetProperty(ref _viewModelPopup_Info, value);
            }
        }

        private ViewModelPopup_Error _viewModelPopup_Error = new ViewModelPopup_Error();
        public ViewModelPopup_Error ViewModelPopup_Error
        {
            get
            {
                return _viewModelPopup_Error;
            }
            set
            {
                SetProperty(ref _viewModelPopup_Error, value);
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

        private ViewModelSetMode _viewModelSetMode = new ViewModelSetMode();
        public ViewModelSetMode ViewModelSetMode
        {
            get
            {
                return _viewModelSetMode;
            }
            set
            {
                SetProperty(ref _viewModelSetMode, value);
            }
        }

        private Dictionary<string, ModelSetMode> _modelSetModeDictionary = new Dictionary<string, ModelSetMode>();
        public Dictionary<string, ModelSetMode> ModelSetModeDictionary
        {
            get
            {
                return _modelSetModeDictionary;
            }
            set
            {
                SetProperty(ref _modelSetModeDictionary, value);
            }
        }
    }
}
