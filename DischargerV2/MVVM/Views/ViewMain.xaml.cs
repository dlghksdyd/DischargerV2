using DischargerV2.MVVM.ViewModels;
using Sqlite.Common;
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
    /// ViewMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewMain : Window
    {
        public static ViewMain Instance;

        private ViewModelMain ViewModel = new ViewModelMain();

        public ViewMain()
        {
            InitializeComponent();

            SqliteUtility.InitializeDatabases();

            Instance = this;

            InitializeUI();

            this.DataContext = ViewModel;
        }

        private void InitializeUI()
        {

        }
    }
}
