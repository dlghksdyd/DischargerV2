using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public ViewModelLogin()
        {
            _instance = this;

            LoginCommand = new DelegateCommand(Login);
        }

        public void Login()
        {
            try
            {
                List<TableUserInfo> tableUserInfoList = SqliteUserInfo.GetData();

                TableUserInfo user = tableUserInfoList.Find(x =>
                    x.UserId == Model.UserId && x.Password == Model.Password);

                if (user != null)
                {
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
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_LOGIN, ex);
            }
        }
    }
}
