using DischargerV2.MVVM.ViewModels;
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

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewLogin.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewLogin : UserControl
    {
        ViewModelLogin _viewModel = ViewModelLogin.Instance;

        public ViewLogin()
        {
            InitializeComponent();

            this.DataContext = _viewModel;

            this.Loaded += ViewLogin_Loaded;
        }

        private void ViewLogin_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Initialize();
        }

        private void xPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                _viewModel.Login();
            }
        }

        private void xLoginButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Login();
        }

        private void xEnglishStackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ChangeLanguage(Enums.ELanguage.English);
        }

        private void xKoreanStackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ChangeLanguage(Enums.ELanguage.Korean);
        }
    }
}
