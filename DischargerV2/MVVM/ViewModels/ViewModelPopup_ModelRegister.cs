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
    public class ViewModelPopup_ModelRegister : BindableBase
    {
        #region Command
        public DelegateCommand xCloseImage_MouseLeftButtonUpCommand { get; set; }
        public DelegateCommand xAddButton_ClickCommand { get; set; }
        #endregion

        #region Model
        public ModelPopup_ModelRegister Model { get; set; } = new ModelPopup_ModelRegister();

        public int SelectedId
        {
            get => Model.SelectedId;
            set => Model.SelectedId = value; 
        }
        #endregion

        public ViewModelPopup_ModelRegister()
        {
            xCloseImage_MouseLeftButtonUpCommand = new DelegateCommand(xCloseImage_MouseLeftButtonUp);
            xAddButton_ClickCommand = new DelegateCommand(xAddButton_Click);

            LoadDeviceModelList();
        }

        private void xCloseImage_MouseLeftButtonUp()
        {
            Close();
        }

        private void xAddButton_Click()
        {
            Model.NewDataVisibility = Visibility.Visible;
        }

        private void LoadDeviceModelList()
        {
            List<TableDischargerModel> tableDischargerModel = SqliteDischargerModel.GetData();

            ObservableCollection<TableDischargerModel> content = new ObservableCollection<TableDischargerModel>();

            for (int index = 0; index < tableDischargerModel.Count; index++)
            {
                content.Add(tableDischargerModel[index]);
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
