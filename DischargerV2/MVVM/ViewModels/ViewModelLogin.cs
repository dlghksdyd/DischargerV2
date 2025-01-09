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
        private ModelLogin Model = null;

        public ViewModelLogin()
        {
            Model = new ModelLogin();
        }

        public Visibility Visibility
        {
            get
            {
                return Model.visibility;
            }
            set
            {
                SetProperty(ref Model.visibility, value);
            }
        }

        public string UserId
        {
            get
            {
                return Model.userId;
            }
            set
            {
                SetProperty(ref Model.userId, value);
            }
        }

        public string Password
        {
            get
            {
                return Model.password;
            }
            set
            {
                SetProperty(ref Model.password, value);
            }
        }

        public DelegateCommand LoginCommand => new DelegateCommand(Login);

        private void Login()
        {
            if (UserId == null || UserId.Length == 0)
            {
                MessageBox.Show("아이디를 입력해 주세요.");
            }
            else if (Password == null || Password.Length == 0)
            {
                MessageBox.Show("비밀번호를 입력해 주세요.");
            }
            else
            {
                List<TableUserInfo> userInfos = SqliteUserInfo.GetData();

                TableUserInfo user = userInfos.Find(x => x.UserId == UserId && x.Password == Password);

                if (user != null)
                {
                    Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("아이디 또는 비밀번호가 잘못 되었습니다.\n" +
                        "아이디와 비밀번호를정확히 입력해 주세요.");
                }
            }
        }
    }
}
