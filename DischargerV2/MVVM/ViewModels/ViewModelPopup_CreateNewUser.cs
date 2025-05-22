using DischargerV2.LOG;
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
        public DelegateCommand InsertNewDataCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        #endregion

        #region Model
        public ModelPopup_CreateNewUser Model { get; set; } = new ModelPopup_CreateNewUser();
        #endregion

        public ViewModelPopup_CreateNewUser()
        {
            InsertNewDataCommand = new DelegateCommand(InsertNewData);
            CloseCommand = new DelegateCommand(Close);
        }

        private void InsertNewData()
        {
            if (!(CheckData() < 0))
            {
                InsertUserInfo();
                Close();
            }
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffNestedPopup();
            viewModelMain.OpenPopup(ModelMain.EPopup.UserSetting);
        }

        private int CheckData()
        {
            List<TableUserInfo> tableUserInfoList = SqliteUserInfo.GetData();

            if (Model.Id == null || Model.Id == "")
            {
                MessageBox.Show("ID: 필수 정보입니다.");
                return -1;
            }
            if (tableUserInfoList.Find(x => x.UserId == Model.Id) != null)
            {
                MessageBox.Show("ID: 이미 등록되어있는 정보입니다.");
                return -1;
            }
            if (Model.Password == null || Model.Password == "")
            {
                MessageBox.Show("Password: 필수 정보입니다.");
                return -1;
            }
            if (Model.ConfirmPassword == null || Model.ConfirmPassword == "")
            {
                MessageBox.Show("Confirm Password: 필수 정보입니다.");
                return -1;
            }
            if (Model.Name == null || Model.Name == "")
            {
                MessageBox.Show("User Name: 필수 정보입니다.");
                return -1;
            }
            if (Model.Password != Model.ConfirmPassword)
            {
                MessageBox.Show("Password가 일치하지 않습니다.");
                return -1;
            }
            return 0;
        }

        private void InsertUserInfo()
        {
            try
            {
                // 사용자 정보 추가
                TableUserInfo tableUserInfo = new TableUserInfo();
                tableUserInfo.UserId = Model.Id;
                tableUserInfo.Password = Model.Password;
                tableUserInfo.UserName = Model.Name;
                tableUserInfo.IsAdmin = Model.IsAdmin;

                bool isOk = SqliteUserInfo.InsertData(tableUserInfo);

                // 사용자 정보 추가 Trace Log 저장
                UserData userData = new UserData()
                {
                    UserId = Model.Id,
                    Password = Model.Password,
                    UserName = Model.Name,
                    IsAdmin = Model.IsAdmin,
                };

                if (isOk)
                {
                    new LogTrace(ELogTrace.TRACE_ADD_USER, userData);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_ADD_USER, userData);
                }
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_ADD_USER, ex);
            }
            
        }
    }
}