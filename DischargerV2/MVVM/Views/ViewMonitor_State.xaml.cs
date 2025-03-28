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
            MexTextBlock mexTextBlock = sender as MexTextBlock;

            _viewModel.ChangedState(mexTextBlock.Text);
        }

        private void xDetailButton_Click(object sender, RoutedEventArgs e)
        {
            string dischargerName = ViewModelSetMode.Instance.Model.DischargerName;

            ViewModelDischarger.Instance.OpenPopupError(dischargerName);
        }
    }
}
