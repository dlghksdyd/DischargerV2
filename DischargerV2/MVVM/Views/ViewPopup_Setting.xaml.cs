using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MExpress.Mex;
using DischargerV2.MVVM.ViewModels;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewPopup_Setting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewPopup_Setting : UserControl
    {
        private ViewModelPopup_Setting ViewModel = new ViewModelPopup_Setting();

        public ViewPopup_Setting()
        {
            InitializeComponent();

            this.DataContext = ViewModel;
        }

        private void xItemLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MexLabel mexLabel = sender as MexLabel;

            if (mexLabel == xUserSettingLabel)
            {

            }
            else if (mexLabel == xDeviceRegisterLabel)
            {

            }
            else if (mexLabel == xDbConfigurationLabel)
            {

            }
            else if (mexLabel == xLogoutLabel)
            {

            }
            else
                Debug.WriteLine(string.Format("{0} MouseDown", mexLabel.Name));
        }

        private void xItemLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            MexLabel mexLabel = sender as MexLabel;

            mexLabel.Background = ResColor.surface_action_hover2;
        }

        private void xItemLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            MexLabel mexLabel = sender as MexLabel;

            mexLabel.Background = ResColor.surface_primary;
        }
    }
}
