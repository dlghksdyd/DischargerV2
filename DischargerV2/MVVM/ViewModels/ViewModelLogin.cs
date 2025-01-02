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
        public DelegateCommand LoginCommand { get; set; }

        private string _userId;
        public string UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                SetProperty(ref _userId, value);
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        public ViewModelLogin()
        {
            LoginCommand = new DelegateCommand(Login);
        }

        private void Login()
        {
            List<TableUserInfo> userInfos = SqliteUserInfo.GetData();

            TableUserInfo user = userInfos.Find(x => x.UserId == _userId && x.Password == _password);

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
