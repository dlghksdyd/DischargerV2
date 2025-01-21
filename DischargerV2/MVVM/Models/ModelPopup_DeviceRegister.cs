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
    public class ModelPopup_DeviceRegister : BindableBase
    {
        private ObservableCollection<TableDischargerInfo> _content = new ObservableCollection<TableDischargerInfo>();
        public ObservableCollection<TableDischargerInfo> Content
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

        private Visibility _newDeviceVisibility = Visibility.Collapsed;
        public Visibility NewDeviceVisibility
        {
            get 
            { 
                return _newDeviceVisibility; 
            }
            set
            {
                SetProperty(ref _newDeviceVisibility, value);
            }
        }

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                SetProperty(ref _selectedIndex, value);
            }
        }
    }
}