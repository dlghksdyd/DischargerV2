using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_UserSetting : BindableBase
    {
        #region Command
        public DelegateCommand xCloseImage_MouseLeftButtonUpCommand { get; set; }
        #endregion

        #region Model
        public ModelPopup_UserSetting Model { get; set; } = new ModelPopup_UserSetting();
        #endregion
        
        public ViewModelPopup_UserSetting()
        {
            xCloseImage_MouseLeftButtonUpCommand = new DelegateCommand(xCloseImage_MouseLeftButtonUp);
        }

        private void xCloseImage_MouseLeftButtonUp()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.Model.IsPopupOpen = false;
            viewModelMain.Model.PopupContent = null;
        }
    }
}
