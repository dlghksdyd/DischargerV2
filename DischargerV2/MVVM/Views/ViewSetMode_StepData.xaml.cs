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
        private ViewModelSetMode_StepData ViewModel = new ViewModelSetMode_StepData();

        public ViewSetMode_StepData()
        {
            InitializeComponent();

            this.DataContext = ViewModel;
        }
    }
}
