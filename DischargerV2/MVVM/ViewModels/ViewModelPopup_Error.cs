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
using DischargerV2.Modal;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_Error : BindableBase
    {
        #region Command
        public DelegateCommand ResetCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }

        public delegate void callBackDelegate(string parameter);
        public callBackDelegate CallBackDelegate { get; set; }
        #endregion

        #region Model
        public ModelPopup_Error Model { get; set; } = new ModelPopup_Error();

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

        public ViewModelPopup_Error()
        {
            ResetCommand = new DelegateCommand(Reset);
            CloseCommand = new DelegateCommand(Close);
        }


        private void Reset()
        {
            CallBackDelegate(Model.Parameter);
            Close();
        }

        private void Close()
        {
            ModalManager.Close(this, ModalResult.Ok);
        }
    }
}
