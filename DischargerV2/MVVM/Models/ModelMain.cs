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

        private ContentControl _popupContent;
        public ContentControl PopupContent
        {
            get
            {
                return _popupContent;
            }
            set
            {
                SetProperty(ref _popupContent, value);
            }
        }

        private ContentControl _popupContent2;
        public ContentControl PopupContent2
        {
            get
            {
                return _popupContent2;
            }
            set
            {
                SetProperty(ref _popupContent2, value);
            }
        }
    }
}