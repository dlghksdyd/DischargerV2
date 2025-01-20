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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static DischargerV2.MVVM.Models.ModelMain;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelMain : BindableBase
    {
        #region Command
        #endregion

        #region Model
        public ModelMain Model { get; set; } = new ModelMain();
        #endregion

        private static ViewModelMain _instance = null;

        public static ViewModelMain Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelMain();
                }
                return _instance;
            }
        }

        public ViewModelMain()
        {
            _instance = this;

            InitializeModel();
        }

        private void InitializeModel()
        {
            OffPopup();
            OffPopup2();
        }

        public void OpenPopup(EPopup setPopup)
        {
            Visibility[] popupVisibility = new Visibility[Enum.GetValues(typeof(EPopup)).Length];

            foreach (EPopup popup in Enum.GetValues(typeof(EPopup)))
            {
                popupVisibility[(int)popup] = (setPopup.Equals(popup)) ? Visibility.Visible : Visibility.Collapsed;
            }

            if (setPopup.Equals(EPopup.UserSetting))
            {
                Model.ViewModelPopup_UserSetting = new ViewModelPopup_UserSetting();
            }

            Model.PopupVisibility = popupVisibility;
            Model.IsPopupOpen = true;
        }

        public void OpenPopup2(EPopup2 setPopup)
        {
            Visibility[] popupVisibility = new Visibility[Enum.GetValues(typeof(EPopup2)).Length];

            foreach (EPopup2 popup in Enum.GetValues(typeof(EPopup2)))
            {
                popupVisibility[(int)popup] = (setPopup.Equals(popup)) ? Visibility.Visible : Visibility.Collapsed;
            }

            Model.PopupVisibility2 = popupVisibility;
            Model.IsPopupOpen2 = true;
        }

        public void OffPopup()
        {
            Visibility[] popupVisibility = new Visibility[Enum.GetValues(typeof(EPopup)).Length];

            foreach (EPopup popup in Enum.GetValues(typeof(EPopup)))
            {
                popupVisibility[(int)popup] = Visibility.Collapsed;
            }

            Model.PopupVisibility = popupVisibility;
            Model.IsPopupOpen = false;
        }

        public void OffPopup2()
        {
            Visibility[] popupVisibility = new Visibility[Enum.GetValues(typeof(EPopup2)).Length];

            foreach (EPopup popup in Enum.GetValues(typeof(EPopup2)))
            {
                popupVisibility[(int)popup] = Visibility.Collapsed;
            }

            Model.ViewModelPopup_CreateNewUser = new ViewModelPopup_CreateNewUser();
            Model.ViewModelPopup_EditUser = new ViewModelPopup_EditUser();
            Model.ViewModelPopup_Warning = new ViewModelPopup_Warning();
            
            Model.PopupVisibility2 = popupVisibility;
            Model.IsPopupOpen2 = false;
        }

        public void SetViewModelPopup_EditUser(ViewModelPopup_EditUser viewModelPopup_EditUser)
        {
            Model.ViewModelPopup_EditUser = viewModelPopup_EditUser;
        }

        public void SetViewModelPopup_Warning(ViewModelPopup_Warning viewModelPopup_Warning)
        {
            Model.ViewModelPopup_Warning = viewModelPopup_Warning;
        }
    }
}
