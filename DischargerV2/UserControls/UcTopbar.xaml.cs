using MExpress.Mex;
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
    /// UserControlTopbar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControlTopbar : UserControl
    {
        public UserControlTopbar()
        {
            InitializeComponent();
        }

        private void xMinusImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.WindowState = WindowState.Minimized;
        }

        private void xCloseImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void xCloseImageGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;

            grid.Background = ResColor.surface_error2;
        }

        private void xCloseImageGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;

            grid.Background = ResColor.transparent;
        }

        private void xMinusImageGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;

            grid.Background = ResColor.surface_secondary_hover;
        }

        private void xMinusImageGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;

            grid.Background = ResColor.transparent;
        }
    }
}
