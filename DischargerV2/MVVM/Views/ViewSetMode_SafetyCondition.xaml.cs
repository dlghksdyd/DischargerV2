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
    /// ViewSetMode_SafetyCondition.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewSetMode_SafetyCondition : UserControl
    {
        private ViewModelSetMode_SafetyCondition _viewModel = ViewModelSetMode_SafetyCondition.Instance;

        public ViewSetMode_SafetyCondition()
        {
            InitializeComponent();

            this.DataContext = _viewModel;
        }
    }
}
