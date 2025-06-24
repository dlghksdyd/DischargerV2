using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static DischargerV2.Ini.IniDischarge;
using static DischargerV2.LOG.LogTrace;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelLogin : BindableBase
    {
        #region Command
        public DelegateCommand LoginCommand { get; set; }
        #endregion

        #region Model
        public ModelLoginUserInfo Model { get; set; } = new ModelLoginUserInfo();
        #endregion

        #region Property
        private static ViewModelLogin _instance = null;
        public static ViewModelLogin Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelLogin();
                }
                return _instance;
            }
        }
        #endregion

        public ViewModelLogin()
        {
            _instance = this;

            LoginCommand = new DelegateCommand(Login);
        }

        public void Initialize()
        {
            Model.IsLocalDb = GetIniData<bool>(EIniData.IsLocalDb);

            // Server DB 사용 시, 해당 내용 불러오기
            if (!Model.IsLocalDb)
            {
                Model.ServerIp = GetIniData<string>(EIniData.ServerIp);
                Model.ServerPort = GetIniData<string>(EIniData.ServerPort);
                Model.ServerName = GetIniData<string>(EIniData.DatabaseName);

                Sqlite.Server.SqliteUserInfo.UpdateConnectionString(Model.ServerIp, Model.ServerPort, Model.ServerName);
            }
        }

        public void Login()
        {
            try
            {
                if (Model.IsLocalDb)
                {
                    var user = Sqlite.Common.SqliteUserInfo.FindUserInfo(Model.UserId, Model.Password);

                    if (user != null)
                    {
                        Model.UserName = user.UserName;
                        Model.Initial = user.UserName.Substring(0, 1).ToUpper();
                        Model.IsAdmin = user.IsAdmin;
                        Model.Permission = user.IsAdmin ? "Admin" : "User";
                        Model.Visibility = Visibility.Collapsed;

                        new LogTrace(ELogTrace.TRACE_LOGIN, Model.UserId);
                    }
                    else
                    {
                        MessageBox.Show("아이디 또는 비밀번호가 잘못 되었습니다.\n" +
                            "아이디와 비밀번호를 정확히 입력해 주세요.");

                        new LogTrace(ELogTrace.ERROR_LOGIN, Model.UserId);
                    }
                }
                else
                {
                    var user = Sqlite.Server.SqliteUserInfo.FindUserInfo(Model.UserId, Model.Password);

                    if (user != null)
                    {
                        Model.UserName = user.USER_NM;
                        Model.Initial = user.USER_NM.Substring(0, 1).ToUpper();
                        Model.IsAdmin = (user.ADMIN_GROUP.ToUpper() == "ADMIN") ? true : false;
                        Model.Permission = user.ADMIN_GROUP;
                        Model.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MessageBox.Show("아이디 또는 비밀번호가 잘못 되었습니다.\n" +
                            "아이디와 비밀번호를 정확히 입력해 주세요.");

                        new LogTrace(ELogTrace.ERROR_LOGIN, Model.UserId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");

                new LogTrace(ELogTrace.ERROR_LOGIN, ex);
            }
        }

        public bool IsAdmin()
        {
            return Model.IsAdmin;
        }

        public bool IsMintech()
        {
            return (Model.UserId == "mintech") ? true : false;
        }
    }
}
