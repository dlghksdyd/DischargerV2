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

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewMonitor_Graph.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewMonitor_Graph : UserControl
    {
        private ViewModelMonitor_Graph _viewModel = ViewModelMonitor_Graph.Instance;

        public ViewMonitor_Graph()
        {
            InitializeComponent();

            this.DataContext = _viewModel;
        }
    }
}
