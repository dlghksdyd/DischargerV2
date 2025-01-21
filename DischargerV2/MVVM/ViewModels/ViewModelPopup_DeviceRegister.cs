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
        public DelegateCommand xCloseImage_MouseLeftButtonUpCommand { get; set; }
        public DelegateCommand xAddButton_ClickCommand { get; set; }
        #endregion

        #region Model
        public ModelPopup_DeviceRegister Model { get; set; } = new ModelPopup_DeviceRegister();
        #endregion

        public ViewModelPopup_DeviceRegister()
        {
            xCloseImage_MouseLeftButtonUpCommand = new DelegateCommand(xCloseImage_MouseLeftButtonUp);
            xAddButton_ClickCommand = new DelegateCommand(xAddButton_Click);

            LoadDeviceInfoList();
        }

        private void xCloseImage_MouseLeftButtonUp()
        {
            Close();
        }

        private void xAddButton_Click()
        {
            Model.NewDeviceVisibility = Visibility.Visible;
        }

        private void LoadDeviceInfoList()
        {
            List<TableDischargerInfo> tblDeviceInfo = SqliteDischargerInfo.GetData();

            ObservableCollection<TableDischargerInfo> content = new ObservableCollection<TableDischargerInfo>();

            for (int index = 0; index < tblDeviceInfo.Count; index++)
            {
                content.Add(tblDeviceInfo[index]);
            }

            Model.Content = content;
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffPopup();
        }
    }
}
