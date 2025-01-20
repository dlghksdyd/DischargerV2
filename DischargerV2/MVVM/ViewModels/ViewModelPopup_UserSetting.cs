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
using System.Reflection;
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
            
            LoadUserInfoList();
        }

        private void xCloseImage_MouseLeftButtonUp()
        {
            Close();
        }

        private void LoadUserInfoList()
        {
            List<TableUserInfo> tableUserInfoList = SqliteUserInfo.GetData();

            ObservableCollection<TableUserInfo> content = new ObservableCollection<TableUserInfo>();

            for (int index = 0; index < tableUserInfoList.Count; index++)
            {
                content.Add(tableUserInfoList[index]);
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
