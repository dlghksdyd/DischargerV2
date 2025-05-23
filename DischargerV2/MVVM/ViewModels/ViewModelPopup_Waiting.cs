using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_Waiting : BindableBase
    {
        #region Properties
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private string _comment = string.Empty;
        public string Comment
        {
            get => _comment;
            set
            {
                SetProperty(ref _comment, value);
            }
        }

        #endregion

        public ViewModelPopup_Waiting()
        {

        }
    }
}
