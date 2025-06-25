using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DischargerV2.MVVM.Models
{
    public class ModelPopup_UserSetting : BindableBase
    {
        private ObservableCollection<object> _content = new ObservableCollection<object>();
        public ObservableCollection<object> Content
        {
            get
            {
                return _content;
            }
            set
            {
                SetProperty(ref _content, value);
            }
        }
    }
}