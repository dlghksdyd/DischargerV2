using DischargerV2.Database;
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
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.Model.IsPopupOpen = false;
            viewModelMain.Model.PopupContent = null;
        }

        private void LoadUserInfoList()
        {
            List<TblUserInfo> tblUserInfo = DatabaseContext.SelectAllUserinfo();

            ObservableCollection<object> content = new ObservableCollection<object>();

            content.Add(new ViewUserSetting_Add() { Margin = new Thickness(20, 20, 0, 0) });

            for (int index = 0; index < tblUserInfo.Count; index++)
            {
                content.Add(new ViewUserSetting_Info()
                {
                    DataContext = new ViewModelUserSetting_Info()
                    {
                        IsAdmin = (tblUserInfo[index].UserId.ToUpper() == "ADMIN" && tblUserInfo[index].UserName.ToUpper() == "ADMIN") ? true : false ,
                        Id = tblUserInfo[index].UserId,
                        Name = tblUserInfo[index].UserName
                    },
                    Margin = new Thickness(20, 20, 0, 0)
                });
            }

            ViewUserSetting_Info viewUserSetting_Info = content[content.Count - 1] as ViewUserSetting_Info;
            viewUserSetting_Info.Margin = new Thickness(20, 20, 0, 20);

            if (content.Count % 2 == 0)
            {
                viewUserSetting_Info = content[content.Count - 2] as ViewUserSetting_Info;
                viewUserSetting_Info.Margin = new Thickness(20, 20, 0, 20);
            }

            Model.Content = content;
        }
    }
}
