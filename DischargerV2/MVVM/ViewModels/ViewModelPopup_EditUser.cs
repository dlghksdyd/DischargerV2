using DischargerV2.Database;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_EditUser : BindableBase
    {
        #region Command
        public DelegateCommand xCloseImage_MouseLeftButtonUpCommand { get; set; }
        public DelegateCommand xCancelButton_ClickCommand { get; set; }
        public DelegateCommand xEditButton_ClickCommand { get; set; }
        #endregion

        #region Model
        public ModelPopup_EditUser Model { get; set; } = new ModelPopup_EditUser();

        public bool IsAdmin
        {
            get
            {
                return Model.IsAdmin;
            }
            set
            {
                Model.IsAdmin = value;
            }
        }

        public string Id
        {
            get
            {
                return Model.Id;
            }
            set
            {
                Model.Id = value;
            }
        }

        public string ConfirmCurrentPassword
        {
            get
            {
                return Model.ConfirmCurrentPassword;
            }
            set
            {
                Model.ConfirmCurrentPassword = value;
            }
        }

        public string Name
        {
            get
            {
                return Model.Name;
            }
            set
            {
                Model.Name = value;
            }
        }
        #endregion

        public ViewModelPopup_EditUser()
        {
            xCloseImage_MouseLeftButtonUpCommand = new DelegateCommand(xCloseImage_MouseLeftButtonUp);
            xCancelButton_ClickCommand = new DelegateCommand(xCancelButton_Click);
            xEditButton_ClickCommand = new DelegateCommand(xEditButton_Click);
        }

        private void xCloseImage_MouseLeftButtonUp()
        {
            Close();
        }

        private void xCancelButton_Click()
        {
            Close();
        }

        private void xEditButton_Click()
        {
            if (Model.CurrentPassword == null || Model.CurrentPassword == "")
            {
                MessageBox.Show("Current Password: 필수 정보입니다.");
            }
            else if (Model.NewPassword == null || Model.NewPassword == "")
            {
                MessageBox.Show("New Password: 필수 정보입니다.");
            }
            else if (Model.ConfirmNewPassword == null || Model.ConfirmNewPassword == "")
            {
                MessageBox.Show("Confirm New Password: 필수 정보입니다.");
            }
            else if (Model.CurrentPassword != Model.ConfirmCurrentPassword)
            {
                MessageBox.Show("Current Password가 일치하지 않습니다.");
            }
            else if (Model.NewPassword != Model.ConfirmNewPassword)
            {
                MessageBox.Show("New Password가 일치하지 않습니다.");
            }
            else
            {
                TblUserInfo userInfo = new TblUserInfo(Model.Id, Model.NewPassword, Model.Name, Model.IsAdmin);
                DatabaseContext.UpdateUserInfo(userInfo);
                Return();
            }
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.Model.IsPopupOpen2 = false;
            viewModelMain.Model.PopupContent2 = null;
        }

        private void Return()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.Model.IsPopupOpen2 = false;
            viewModelMain.Model.PopupContent2 = null;
            viewModelMain.Model.IsPopupOpen = true;
            viewModelMain.Model.PopupContent = new ViewPopup_UserSetting();
        }
    }
}
