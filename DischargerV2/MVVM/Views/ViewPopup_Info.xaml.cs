using DischargerV2.MVVM.ViewModels;
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
    /// ViewPopup_Info.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewPopup_Info : UserControl
    {
        public ViewPopup_Info()
        {
            InitializeComponent();
        }

        private void xOkButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ViewModelPopup_Info viewModel)
            {
                viewModel.Ok();
            }
        }

        private void xCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ViewModelPopup_Info viewModel)
            {
                viewModel.Cancel();
            }
        }

        private void xCloseImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is ViewModelPopup_Info viewModel)
            {
                viewModel.Cancel();
            }
        }
    }
}
