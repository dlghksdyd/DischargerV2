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

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_CreateNewUser : BindableBase
    {
        #region Command
        public DelegateCommand xCloseImage_MouseLeftButtonUpCommand { get; set; }
        public DelegateCommand xCancelButton_ClickCommand { get; set; }
        public DelegateCommand xCreateButton_ClickCommand { get; set; }
        
        #endregion

        #region Model
        public ModelPopup_CreateNewUser Model { get; set; } = new ModelPopup_CreateNewUser();
        #endregion
        
        public ViewModelPopup_CreateNewUser()
        {
            xCloseImage_MouseLeftButtonUpCommand = new DelegateCommand(xCloseImage_MouseLeftButtonUp);
            xCancelButton_ClickCommand = new DelegateCommand(xCancelButton_Click);
            xCreateButton_ClickCommand = new DelegateCommand(xCreateButton_Click);
        }

        private void xCloseImage_MouseLeftButtonUp()
        {
            Close();
        }

        private void xCancelButton_Click()
        {
            Close();
        }

        private void xCreateButton_Click()
        {
            List<TableUserInfo> tableUserInfoList = SqliteUserInfo.GetData();

            if (Model.Id == null || Model.Id == "")
            {
                MessageBox.Show("ID: 필수 정보입니다.");
            }
            else if (tableUserInfoList.Find(x => x.UserId == Model.Id) != null)
            {
                MessageBox.Show("ID: 이미 등록되어있는 정보입니다.");
            }
            else if (Model.Password == null || Model.Password == "")
            {
                MessageBox.Show("Password: 필수 정보입니다.");
            }
            else if (Model.ConfirmPassword == null || Model.ConfirmPassword == "")
            {
                MessageBox.Show("Confirm Password: 필수 정보입니다.");
            }
            else if (Model.Name == null || Model.Name == "")
            {
                MessageBox.Show("User Name: 필수 정보입니다.");
            }
            else if (Model.Password != Model.ConfirmPassword)
            {
                MessageBox.Show("Password가 일치하지 않습니다.");
            }
            else
            {
                TableUserInfo tableUserInfo = new TableUserInfo();
                tableUserInfo.UserId = Model.Id;
                tableUserInfo.Password = Model.Password;
                tableUserInfo.UserName = Model.Name;
                tableUserInfo.IsAdmin = Model.IsAdmin;

                SqliteUserInfo.InsertData(tableUserInfo);
                Return();
            }
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffPopup2();
        }

        private void Return()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffPopup2();
            viewModelMain.OpenPopup(ModelMain.EPopup.UserSetting);
        }
    }
}
