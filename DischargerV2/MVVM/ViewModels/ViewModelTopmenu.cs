using DischargerV2.MVVM.Models;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelTopmenu : BindableBase
    {
        #region Command
        public DelegateCommand xSettingButton_ClickCommand { get; set; }
        public DelegateCommand xSettingPopup_MouseLeftButtonUpCommand { get; set; }
        #endregion

        #region Model
        public ModelTopmenu Model { get; set; } = new ModelTopmenu();
        #endregion

        public ViewModelTopmenu()
        {
            Model = new ModelTopmenu();

            xSettingButton_ClickCommand = new DelegateCommand(xSettingButton_Click);
            xSettingPopup_MouseLeftButtonUpCommand = new DelegateCommand(xSettingPopup_MouseLeftButtonUp);

            DispatcherTimer DateTimeTimer = new DispatcherTimer();
            DateTimeTimer.Interval = TimeSpan.FromMilliseconds(100);
            DateTimeTimer.Tick += new EventHandler(DateTimeTimer_Tick);
            DateTimeTimer.Start();
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Model.DateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            }
            catch { }
        }

        private void xSettingButton_Click()
        {
            Model.IsPopupOpen = true;
        }

        private void xSettingPopup_MouseLeftButtonUp()
        {
            Model.IsPopupOpen = false;
        }
    }
}
