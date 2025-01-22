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
    public class ViewModelPopup_Warning : BindableBase
    {
        #region Command
        public DelegateCommand xCloseImage_MouseLeftButtonUpCommand { get; set; }
        public DelegateCommand xCancelButton_ClickCommand { get; set; }
        public DelegateCommand xConfirmButton_ClickCommand { get; set; }

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
                Model.Comment = value;
            }
        }
        #endregion

        public ViewModelPopup_Warning()
        {
            xCloseImage_MouseLeftButtonUpCommand = new DelegateCommand(xCloseImage_MouseLeftButtonUp);
            xCancelButton_ClickCommand = new DelegateCommand(xCancelButton_Click);
            xConfirmButton_ClickCommand = new DelegateCommand(xConfirmButton_Click);
        }

        private void xCloseImage_MouseLeftButtonUp()
        {
            Close();
        }

        private void xCancelButton_Click()
        {
            Close();
        }

        private void xConfirmButton_Click()
        {
            CallBackDelegate();
            Close();
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffNestedPopup();
        }
    }
}
