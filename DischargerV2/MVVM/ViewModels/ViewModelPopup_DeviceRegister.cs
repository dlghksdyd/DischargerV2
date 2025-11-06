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
using DischargerV2.Modal;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_DeviceRegister : BindableBase
    {
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
            LoadDischargerInfoList();
        }

        public void OpenNewData()
        {
            Model.NewDeviceVisibility = Visibility.Visible;
        }

        public void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.UpdateDischarger();
            ModalManager.Close(this, ModalResult.Ok);
        }

        private void LoadDischargerInfoList()
        {
            try
            {
                ObservableCollection<TableDischargerInfo> content = new ObservableCollection<TableDischargerInfo>();

                List<TableDischargerInfo> tableDischargerInfoList = SqliteDischargerInfo.GetData();

                for (int index = 0; index < tableDischargerInfoList.Count; index++)
                {
                    content.Add(tableDischargerInfoList[index]);
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
