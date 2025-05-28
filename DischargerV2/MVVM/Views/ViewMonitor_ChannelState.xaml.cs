using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MExpress.Mex;
using Ethernet.Client.Discharger;
using static DischargerV2.Ini.IniDischarge;

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
            InitializeIni();
        }

        private void InitializeIni()
        {
            // Ini
            var eSound = GetIniData<ESound>(EIniData.Sound);

            if (eSound == ESound.On)
            {
                xSoundImage.Source = (BitmapImage)new ImageColorConverter().Convert(ResImage.volume_up, null, ResColor.icon_primary, null);
                EthernetClientDischarger.IsLampBuzzerUsed = true;
            }
            else
            {
                xSoundImage.Source = (BitmapImage)new ImageColorConverter().Convert(ResImage.volume_off, null, ResColor.icon_primary, null);
                EthernetClientDischarger.IsLampBuzzerUsed = false;
            }
        }

        private void xSoundImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;

            if (EthernetClientDischarger.IsLampBuzzerUsed)
            {
                image.Source = (BitmapImage)new ImageColorConverter().Convert(ResImage.volume_off, null, ResColor.icon_primary, null);
                EthernetClientDischarger.IsLampBuzzerUsed = false;

                SetIniData(EIniData.Sound, ESound.Off);
            }
            else
            {
                image.Source = (BitmapImage)new ImageColorConverter().Convert(ResImage.volume_up, null, ResColor.icon_primary, null);
                EthernetClientDischarger.IsLampBuzzerUsed = true;

                SetIniData(EIniData.Sound, ESound.On);
            }
        }
    }
}
