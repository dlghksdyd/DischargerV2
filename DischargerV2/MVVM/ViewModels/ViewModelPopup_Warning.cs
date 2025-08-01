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
using System.Xml.Linq;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelPopup_Warning : BindableBase
    {
        #region Command
        public DelegateCommand ConfirmCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }

        public delegate void callBackDelegate();
        public callBackDelegate CallBackDelegate { get; set; }
        #endregion

        #region Model
        public ModelPopup_Warning Model { get; set; } = new ModelPopup_Warning();

        public string Title
        {
            get
            {
                return Model.Title;
            }
            set
            {
                Model.Title = value;
            }
        }

        public string Comment
        {
            get
            {
                return Model.Comment;
            }
            set
            {
                value = value.Replace("?", "?\r\n").Replace(".", ".\r\n");
                Model.Comment = value;
            }
        }

        public Visibility CancelButtonVisibility
        {
            get
            {
                return Model.CancelButtonVisibility;
            }
            set
            {
                Model.CancelButtonVisibility = value;
            }
        }
        #endregion

        public ViewModelPopup_Warning()
        {
            ConfirmCommand = new DelegateCommand(Confirm);
            CloseCommand = new DelegateCommand(Close);
        }

        private void Confirm()
        {
            CallBackDelegate?.Invoke();
            Close();
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffNestedPopup();
        }
    }
}
