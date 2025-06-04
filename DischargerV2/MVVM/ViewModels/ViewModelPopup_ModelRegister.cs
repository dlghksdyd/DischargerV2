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
        public DelegateCommand OpenNewDataCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
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
            OpenNewDataCommand = new DelegateCommand(OpenNewData);
            CloseCommand = new DelegateCommand(Close);

            LoadDischargerModelList();
        }

        private void OpenNewData()
        {
            Model.NewDataVisibility = Visibility.Visible;
        }
        
        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffPopup();
        }

        private void LoadDischargerModelList()
        {
            try
            {
                List<TableDischargerModel> tableDischargerModel = SqliteDischargerModel.GetData();

                ObservableCollection<TableDischargerModel> content = new ObservableCollection<TableDischargerModel>();

                for (int index = 0; index < tableDischargerModel.Count; index++)
                {
                    content.Add(tableDischargerModel[index]);
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
