using DischargerV2.LOG;
using DischargerV2.MVVM.ViewModels;
using MExpress.Example;
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

        private ViewModelMain _viewModel = new ViewModelMain();

        public ViewMain()
        {
            try
            {
                InitializeComponent();

                SqliteUtility.InitializeDatabases();

                Instance = this;

                this.DataContext = _viewModel;

                _viewModel.UpdateDischargerInfoTableEvent += _viewModel_UpdateDischargerInfoTableEvent;

                new LogTrace(ELogTrace.TRACE_PROGRAM_START);
            }
            catch
            {
                new LogTrace(ELogTrace.ERROR_PROGRAM_START);
            }
        }

        private void _viewModel_UpdateDischargerInfoTableEvent(object sender, EventArgs e)
        {
            xViewDischargerInfoTable.UpdateUi();
        }
    }
}
