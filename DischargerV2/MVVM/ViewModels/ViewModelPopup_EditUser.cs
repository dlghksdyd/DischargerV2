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
using System.Xml.Linq;
using static DischargerV2.LOG.LogTrace;
using static System.Net.Mime.MediaTypeNames;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_EditUser : BindableBase
    {
        #region Command
        public DelegateCommand UpdateEditDataCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
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
            CloseCommand = new DelegateCommand(Close);
            UpdateEditDataCommand = new DelegateCommand(UpdateEditData);
        }


        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffNestedPopup();
            viewModelMain.OpenPopup(ModelMain.EPopup.UserSetting);
        }

        private void UpdateEditData()
        {
            if (!(CheckData() < 0))
            {
                bool isOk = UpdateUserInfo();

                if (isOk)
                {
                    Close();
                }
                else
                {
                    MessageBox.Show("사용자 정보 변경 실패");
                }
            }
        }

        private int CheckData()
        {
            try
            {
                if (Model.CurrentPassword == null || Model.CurrentPassword == "")
                {
                    MessageBox.Show("Current Password: 필수 정보입니다.");
                    return -1;
                }
                if (Model.NewPassword == null || Model.NewPassword == "")
                {
                    MessageBox.Show("New Password: 필수 정보입니다.");
                    return -1;
                }
                if (Model.ConfirmNewPassword == null || Model.ConfirmNewPassword == "")
                {
                    MessageBox.Show("Confirm New Password: 필수 정보입니다.");
                    return -1;
                }
                if (Model.CurrentPassword != Model.ConfirmCurrentPassword)
                {
                    MessageBox.Show("Current Password가 일치하지 않습니다.");
                    return -1;
                }
                if (Model.NewPassword != Model.ConfirmNewPassword)
                {
                    MessageBox.Show("New Password가 일치하지 않습니다.");
                    return -1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");

                return -1;
            }
        }

        private bool UpdateUserInfo()
        {
            try
            {
                // 사용자 정보 수정
                TableUserInfo tableUserInfo = new TableUserInfo();
                tableUserInfo.UserId = Model.Id;
                tableUserInfo.Password = Model.NewPassword;
                tableUserInfo.UserName = Model.Name;

                bool isOk = SqliteUserInfo.UpdateData(tableUserInfo);

                // 사용자 정보 수정 Trace Log 저장
                UserData userData = new UserData()
                {
                    UserId = Model.Id,
                    Password = Model.CurrentPassword,
                    PasswordNew = Model.NewPassword,
                    UserName = Model.Name,
                    IsAdmin = Model.IsAdmin,
                };

                if (isOk)
                {
                    new LogTrace(ELogTrace.TRACE_EDIT_USER, userData);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_EDIT_USER, userData);
                }

                return isOk;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");

                new LogTrace(ELogTrace.ERROR_EDIT_USER, ex);

                return false;
            }
        }
    }
}
