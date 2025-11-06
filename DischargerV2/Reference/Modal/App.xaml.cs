using HYSoft.Presentation.Modal;
using System.Configuration;
using System.Data;
using System.Windows;
using HYSoft.Data.Mssql;
using Mindims.Communication.Mssql;
using Mindims.Mvvm.BatteryManagement;
using Mindims.Mvvm.LabelManagement;
using Mindims.Mvvm.Popup;
using Mindims.Communication.Configuration; // added
using Mindims.Global;
using Mindims.Mvvm.InspectionHistory.InspAbt.Result;
using Mindims.Mvvm.InspectionHistory.InspBds.Result; // EVendorKey
using Mindims.Mvvm.InspectionHistory.InspIis.Result; // register IIS result modal

namespace Mindims
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IniConfiguration.FtpConfig Ftp { get; private set; } = new IniConfiguration.FtpConfig();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ensure Configuration.ini exists next to the executable
            IniConfiguration.EnsureIniFileExists();

            // Load DIMS configuration from INI
            var dims = IniConfiguration.LoadDimsConfig();
            // Load FTP configuration
            Ftp = IniConfiguration.LoadFtpConfig();

            ModalManager.Configure("#33ffffff");
            ModalManager.RegisterView<PopupInfoView, PopupInfoViewModel>();
            ModalManager.RegisterView<BatteryInfoCreationView, BatteryInfoCreationViewModel>();
            ModalManager.RegisterView<LabelInfoCreationView, LabelInfoCreationViewModel>();
            ModalManager.RegisterView<SubLabelInfoCreationView, SubLabelInfoCreationViewModel>();
            ModalManager.RegisterView<AbtResultView, AbtResultViewModel>();
            ModalManager.RegisterView<BdsResultView, BdsResultViewModel>();
            ModalManager.RegisterView<IisResultView, IisResultViewModel>();
            ModalManager.RegisterView<CustomLabelCreationView, CustomLabelCreationViewModel>();
            ModalManager.RegisterView<CustomLabelModifyView, CustomLabelModifyViewModel>();
            
            DbContextBase.Initialize(dims.Ip, dims.Port, dims.Id, dims.Pass, dims.DbName);
            // dims.SiteKey is available if needed elsewhere
        }
    }
}
