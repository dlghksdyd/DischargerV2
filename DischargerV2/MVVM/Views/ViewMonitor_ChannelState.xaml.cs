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
using MExpress.Mex;
using DischargerV2.MVVM.ViewModels;
using Ethernet.Client.Discharger;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewMonitor_ChannelState.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewMonitor_ChannelState : UserControl
    {
        public ViewMonitor_ChannelState()
        {
            InitializeComponent();
        }

        private void xSoundImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;

            if (EthernetClientDischarger.IsLampBuzzerUsed)
            {
                image.Source = (BitmapImage)new ImageColorConverter().Convert(ResImage.volume_off, null, ResColor.icon_primary, null);
                EthernetClientDischarger.IsLampBuzzerUsed = false;
            }
            else
            {
                image.Source = (BitmapImage)new ImageColorConverter().Convert(ResImage.volume_up, null, ResColor.icon_primary, null);
                EthernetClientDischarger.IsLampBuzzerUsed = true;
            }
        }
    }
}
