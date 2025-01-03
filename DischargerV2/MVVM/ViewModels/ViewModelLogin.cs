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
        public ModelLoginUserInfo LoginUserInfo { get; set; } = new ModelLoginUserInfo();
        #endregion

        private static ViewModelLogin _instance = null;

        public static ViewModelLogin Instance()
        {
            return _instance;
        }

        public ViewModelLogin()
        {
            _instance = this;

            LoginCommand = new DelegateCommand(Login);
        }

        private void Login()
        {
            List<TableUserInfo> userInfos = SqliteUserInfo.GetData();

            TableUserInfo user = userInfos.Find(x => x.UserId == LoginUserInfo.UserId && x.Password == LoginUserInfo.Password);

            if (user != null)
            {
                MessageBox.Show("로그인 성공");
            }
            else
            {
                MessageBox.Show("로그인 실패");
            }
        }
    }
}
