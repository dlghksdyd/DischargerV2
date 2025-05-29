using DischargerV2.MVVM.Models;
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
using Utility.Common;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewSetMode_Preset.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewSetMode_Preset : UserControl
    {
        private ViewModelSetMode_Preset _viewModel = ViewModelSetMode_Preset.Instance;

        public ViewSetMode_Preset()
        {
            InitializeComponent();

            this.DataContext = _viewModel;
        }

        private void xBatteryTypeItemBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.GetCurrentSoc(_viewModel.Model, -1);
        }
    }
}
