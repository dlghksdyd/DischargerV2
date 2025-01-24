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
    public class ViewModelPopup_Info : BindableBase
    {
        #region Command
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }

        public delegate void callBackDelegate();
        public callBackDelegate CallBackDelegate { get; set; }
        #endregion

        #region Model
        public ModelPopup_Info Model { get; set; } = new ModelPopup_Info();

        public string Title
        {
            get => Model.Title;
            set => Model.Title = value;
        }

        public string Comment
        {
            get => Model.Comment;
            set => Model.Comment = value;
        }

        public string Parameter
        {
            get => Model.Parameter;
            set => Model.Parameter = value;
        }
        #endregion

        public ViewModelPopup_Info()
        {
            ExitCommand = new DelegateCommand(Exit);
            CloseCommand = new DelegateCommand(Close);
        }


        private void Exit()
        {
            CallBackDelegate();
            Close();
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffPopup();
        }
    }
}
