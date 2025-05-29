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
using DischargerV2.MVVM.Models;
using System.Configuration;
using System.Diagnostics;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewMonitor_Step.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewMonitor_Step : UserControl
    {
        private ViewModelMonitor_Step _viewModel = ViewModelMonitor_Step.Instance;

        public ViewMonitor_Step()
        {
            InitializeComponent();

            this.DataContext = _viewModel;

            this.Loaded += ViewMonitor_Step_Loaded; ;
        }

        private void ViewMonitor_Step_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetScrollViewer(xScrollViewer);
        }
    }
}
