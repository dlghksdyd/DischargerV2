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

            TableUserInfo user = userInfos.Find(x => x.UserId == Model.UserId && x.Password == Model.Password);

            if (user != null)
            {
                Model.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("아이디 또는 비밀번호가 잘못 되었습니다.\n" +
                    "아이디와 비밀번호를 정확히 입력해 주세요.");
            }
        }
    }
}
