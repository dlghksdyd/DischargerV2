using DischargerV2.Languages.Strings;
using DischargerV2.LOG;
using DischargerV2.MVVM.ViewModels;
using Sqlite.Common;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static DischargerV2.LOG.LogTrace;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewMain : Window
    {
        public static ViewMain Instance;

        private LogTrace LogTraceProgramStart = new LogTrace(ELogTrace.SYSTEM_OK_PROGRAM_START);
        private ViewModelMain _viewModel = new ViewModelMain();

        public ViewMain()
        {
            InitializeComponent();
            this.DataContext = _viewModel;

            try
            {
                SqliteUtility.InitializeDatabases();

                Instance = this;

                _viewModel.UpdateDischargerInfoTableEvent += _viewModel_UpdateDischargerInfoTableEvent;
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.SYSTEM_ERROR_PROGRAM_START, ex);
            }
        }

        private void _viewModel_UpdateDischargerInfoTableEvent(object sender, EventArgs e)
        {
            xViewDischargerInfoTable.UpdateUi();
        }
    }
}
