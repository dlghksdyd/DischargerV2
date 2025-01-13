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
        public DelegateCommand ClickSettingButtonCommand { get; set; }
        public DelegateCommand SelectSettingPopupCommand { get; set; }
        #endregion

        #region Model
        public ModelTopmenu Model { get; set; } = new ModelTopmenu();
        #endregion

        public ViewModelTopmenu()
        {
            Model = new ModelTopmenu();

            ClickSettingButtonCommand = new DelegateCommand(ClickSettingButton);
            SelectSettingPopupCommand = new DelegateCommand(SelectSettingPopup);

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

        private void ClickSettingButton()
        {
            Model.IsPopupOpen = true;
        }

        private void SelectSettingPopup()
        {
            Model.IsPopupOpen = false;
        }
    }
}
