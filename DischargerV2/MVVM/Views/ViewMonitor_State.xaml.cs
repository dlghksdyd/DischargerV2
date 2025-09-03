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
using static System.Windows.Forms.AxHost;
using Ethernet.Client.Discharger;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewMonitor_State.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewMonitor_State : UserControl
    {
        private ViewModelMonitor_State _viewModel = ViewModelMonitor_State.Instance;

        public ViewMonitor_State()
        {
            InitializeComponent();

            this.DataContext = _viewModel;
        }

        private void xStateTextBlock_TextChanged(object sender, EventArgs e)
        {
            if (xPauseButton == null || xResumeButton == null ||
                xStopButton == null || xFinishButton == null)
            {
                return;
            }

            string state = xStateTextBlock.Text;

            if (state == EDischargerState.Discharging.ToString())
            {
                xPauseButton.IsEnabled = xResumeButton.IsEnabled = true;
                xStopButton.IsEnabled = true;
                xFinishButton.IsEnabled = false;
            }
            else if (state == EDischargerState.Ready.ToString())
            {
                xPauseButton.IsEnabled = xResumeButton.IsEnabled = false;
                xStopButton.IsEnabled = false;
                xFinishButton.IsEnabled = true;
            }
            else if (state == EDischargerState.Pause.ToString())
            {
                xPauseButton.IsEnabled = xResumeButton.IsEnabled = true;
                xStopButton.IsEnabled = true;
                xFinishButton.IsEnabled = false;
            }
            else if (state == EDischargerState.SafetyOutOfRange.ToString() ||
                     state == EDischargerState.ReturnCodeError.ToString() ||
                     state == EDischargerState.ChStatusError.ToString() ||
                     state == EDischargerState.DeviceError.ToString())
            {
                xPauseButton.IsEnabled = xResumeButton.IsEnabled = false;
                xStopButton.IsEnabled = false;
                xFinishButton.IsEnabled = true;
            }
        }

        private void xDetailButton_Click(object sender, RoutedEventArgs e)
        {
            string dischargerName = ViewModelSetMode.Instance.Model.DischargerName;

            ViewModelDischarger.Instance.OpenPopupError(dischargerName);
        }
    }
}
