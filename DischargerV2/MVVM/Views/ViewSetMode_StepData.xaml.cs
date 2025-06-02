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
    /// ViewSetMode_StepData.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewSetMode_StepData : UserControl
    {
        public ViewSetMode_StepData()
        {
            InitializeComponent();
        }

        private void xCurrTextBox_TextChanged(object sender, EventArgs e)
        {
            var viewModel = (DataContext as ViewModelSetMode_StepData);
            viewModel.CalculateCRate();
        }

        private void xCRateTextBox_TextChanged(object sender, EventArgs e)
        {
            var viewModel = (DataContext as ViewModelSetMode_StepData);
            viewModel.CalculateCurrent();
        }
    }
}
