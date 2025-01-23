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
    public class ViewModelUserSetting_Add : BindableBase
    {
        #region Command
        public DelegateCommand xAddButton_ClickCommand { get; set; }
        #endregion

        #region Model
        public ModelUserSetting_Add Model { get; set; } = new ModelUserSetting_Add();
        #endregion
        
        public ViewModelUserSetting_Add()
        {
            xAddButton_ClickCommand = new DelegateCommand(xAddButton_Click);
        }

        private void xAddButton_Click()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OpenNestedPopup(ModelMain.ENestedPopup.CreateNewUser);
        }
    }
}
