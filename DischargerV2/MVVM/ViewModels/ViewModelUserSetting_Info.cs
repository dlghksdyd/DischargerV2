using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using static DischargerV2.LOG.LogTrace;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelUserSetting_Info : BindableBase
    {
        #region Model
        public ModelUserSetting_Info Model { get; set; } = new ModelUserSetting_Info();

        public bool IsAdmin
        {
            get => Model.IsAdmin;
            set => Model.IsAdmin = value;
        }

        public string Initial
        {
            get => Model.Initial;
            set => Model.Initial = value;
        }

        public string Id
        {
            get => Model.Id;
            set => Model.Id = value;
        }

        public string Password
        {
            get => Model.Password;
            set => Model.Password = value;
        }

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }
        #endregion

        public void OpenPopupEditUser()
        {
            ViewModelPopup_EditUser viewModelPopup_EditUser = new ViewModelPopup_EditUser()
            {
                IsAdmin = Model.IsAdmin,
                Id = Model.Id,
                ConfirmCurrentPassword = Model.Password,
                Name = Model.Name,
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_EditUser(viewModelPopup_EditUser);
            viewModelMain.OpenNestedPopup(ModelMain.ENestedPopup.EditUser);
        }

        public void OpenPopupDelete()
        {
            ViewModelPopup_Warning viewModelPopup_Warning = new ViewModelPopup_Warning()
            {
                Title = string.Format("Delete User '{0}'?", Model.Name),
                Comment = "Are you sure you want to delete this data?\r\n" +
                          "Once you confirm, this data will be permanetly deleted.",
                CallBackDelegate = DeleteUserInfo,
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_Warning(viewModelPopup_Warning);
            viewModelMain.OpenNestedPopup(ModelMain.ENestedPopup.Warning);
        }

        public void DeleteUserInfo()
        {
            try
            {
                // 사용자 정보 제거
                bool isOk = SqliteUserInfo.DeleteData(Id);

                // 사용자 정보 제거 Trace Log 저장
                UserData userData = new UserData()
                {
                    UserId = Model.Id,
                    Password = Model.Password,
                    UserName = Model.Name,
                    IsAdmin = Model.IsAdmin,
                };

                if (isOk)
                {
                    new LogTrace(ELogTrace.SYSTEM_OK_DELETE_USER, userData);
                }
                else
                {
                    new LogTrace(ELogTrace.SYSTEM_ERROR_DELETE_USER, userData);
                }

                // 사용자 정보 등록 화면 표시
                ViewModelMain viewModelMain = ViewModelMain.Instance;
                viewModelMain.OpenPopup(ModelMain.EPopup.UserSetting);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                   $"Error 발생\n\n" +
                   $"ClassName: {this.GetType().Name}\n" +
                   $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                   $"Exception: {ex.Message}");

                new LogTrace(ELogTrace.SYSTEM_ERROR_DELETE_USER, ex);
            }
        }
    }
}
