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
    public class ModelPopup_ModelRegister : BindableBase
    {
        private ObservableCollection<TableDischargerModel> _content = new ObservableCollection<TableDischargerModel>();
        public ObservableCollection<TableDischargerModel> Content
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

        private Visibility _newDataVisibility = Visibility.Collapsed;
        public Visibility NewDataVisibility
        {
            get 
            { 
                return _newDataVisibility; 
            }
            set
            {
                SetProperty(ref _newDataVisibility, value);
            }
        }

        private int _selectedId;
        public int SelectedId
        {
            get
            {
                return _selectedId;
            }
            set
            {
                SetProperty(ref _selectedId, value);
            }
        }
    }
}