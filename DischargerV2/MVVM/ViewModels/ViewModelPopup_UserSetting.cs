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
        public DelegateCommand CloseCommand { get; set; }
        #endregion

        #region Model
        public ModelPopup_UserSetting Model { get; set; } = new ModelPopup_UserSetting();
        #endregion
        
        public ViewModelPopup_UserSetting()
        {
            CloseCommand = new DelegateCommand(Close);
            
            LoadUserInfoList();
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffPopup();
        }

        private void LoadUserInfoList()
        {
            try
            {
                List<TableUserInfo> tableUserInfoList = SqliteUserInfo.GetData();

                ObservableCollection<TableUserInfo> content = new ObservableCollection<TableUserInfo>();

                for (int index = 0; index < tableUserInfoList.Count; index++)
                {
                    content.Add(tableUserInfoList[index]);
                }

                Model.Content = content;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       $"Error 발생\n\n" +
                       $"ClassName: {this.GetType().Name}\n" +
                       $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                       $"Exception: {ex.Message}");
            }
        }
    }
}
