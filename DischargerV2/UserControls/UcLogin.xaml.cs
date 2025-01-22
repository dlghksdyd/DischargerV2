using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DischargerV2.UserControls
{
    /// <summary>
    /// UcLogin.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcLogin : UserControl
    {
        public UcLogin()
        {
            InitializeComponent();
        }

        private void xLoginButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            List<TableUserInfo> tableUserInfoList = SqliteUserInfo.GetData();

            TableUserInfo user = tableUserInfoList.Find(x => x.UserId == xIdTextBox.Text && x.Password == xPasswordBox.Text);
            
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
