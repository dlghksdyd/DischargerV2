using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Example;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_DeviceRegister : BindableBase
    {
        #region Command
        public DelegateCommand OpenNewDataCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        #endregion

        #region Model
        public ModelPopup_DeviceRegister Model { get; set; } = new ModelPopup_DeviceRegister();

        public string SelectedItem
        {
            get => Model.SelectedItem;
            set => Model.SelectedItem = value; 
        }
        #endregion

        public ViewModelPopup_DeviceRegister()
        {
            OpenNewDataCommand = new DelegateCommand(OpenNewData);
            CloseCommand = new DelegateCommand(Close);

            LoadDischargerInfoList();
        }

        private void OpenNewData()
        {
            Model.NewDeviceVisibility = Visibility.Visible;
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffPopup();
        }

        private void LoadDischargerInfoList()
        {
            List<TableDischargerInfo> tableDischargerInfo = SqliteDischargerInfo.GetData();
            ObservableCollection<TableDischargerInfo> content = new ObservableCollection<TableDischargerInfo>();

            for (int index = 0; index < tableDischargerInfo.Count; index++)
            {
                content.Add(tableDischargerInfo[index]);
            }

            Model.Content = content;
        }
    }
}
