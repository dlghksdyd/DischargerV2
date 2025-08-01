using DischargerV2.Languages.Strings;
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
        public DelegateCommand OpenPopupCommand { get; set; }
        public DelegateCommand ClosePopupCommand { get; set; }
        #endregion

        #region Model
        public ModelTopmenu Model { get; set; } = new ModelTopmenu();
        #endregion

        public ViewModelTopmenu()
        {
            Model = new ModelTopmenu();

            OpenPopupCommand = new DelegateCommand(OpenPopup);
            ClosePopupCommand = new DelegateCommand(ClosePopup);

            DispatcherTimer DateTimeTimer = new DispatcherTimer();
            DateTimeTimer.Interval = TimeSpan.FromMilliseconds(100);
            DateTimeTimer.Tick += new EventHandler(DateTimeTimer_Tick);
            DateTimeTimer.Start();
        }

        private void OpenPopup()
        {
            var viewModelDischarger = ViewModelDischarger.Instance;
            if (viewModelDischarger.IsDischarging())
            {
                var title = new DynamicString().GetDynamicString("PopupWarning_Title");
                var comment = new DynamicString().GetDynamicString("PopupWarning_Comment_OpenPopup");

                ViewModelPopup_Warning viewModelPopup_Warning = new ViewModelPopup_Warning()
                {
                    Title = title,
                    Comment = comment,
                    CancelButtonVisibility = Visibility.Hidden
                };

                ViewModelMain viewModelMain = ViewModelMain.Instance;
                viewModelMain.SetViewModelPopup_Warning(viewModelPopup_Warning);
                viewModelMain.OpenNestedPopup(ModelMain.ENestedPopup.Warning);
                return;
            }

            Model.IsPopupOpen = true;
        }

        private void ClosePopup()
        {
            Model.IsPopupOpen = false;
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Model.DateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            }
            catch { }
        }
    }
}
