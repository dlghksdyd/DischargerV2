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
using Sqlite.Common;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewModelRegister_Add.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewModelRegister_Add : UserControl
    {
        public ViewModelRegister_Add()
        {
            InitializeComponent();

            this.DataContext = new ViewModelModelRegister_Add();
        }
    }
}
