using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelTopbar : BindableBase
    {
        #region Command
        public DelegateCommand xTopbar_MouseLeftButtonDownCommand { get; set; }
        public DelegateCommand xMinimizeImage_MouseLeftButtonUpCommand { get; set; }
        public DelegateCommand xMinimizeImage_MouseEnterCommand { get; set; }
        public DelegateCommand xMinimizeImage_MouseLeaveCommand { get; set; }
        public DelegateCommand xCloseImage_MouseLeftButtonUpCommand { get; set; }
        public DelegateCommand xCloseImage_MouseEnterCommand { get; set; }
        public DelegateCommand xCloseImage_MouseLeaveCommand { get; set; }
        #endregion

        #region Model
        public ModelTopbar Model { get; set; } = new ModelTopbar();
        #endregion

        public ViewModelTopbar()
        {
            Model = new ModelTopbar();

            xTopbar_MouseLeftButtonDownCommand = new DelegateCommand(xTopbar_MouseLeftButtonDown);

            xMinimizeImage_MouseLeftButtonUpCommand = new DelegateCommand(xMinimizeImage_MouseLeftButtonUp);
            xMinimizeImage_MouseEnterCommand = new DelegateCommand(xMinimizeImage_MouseEnter);
            xMinimizeImage_MouseLeaveCommand = new DelegateCommand(xMinimizeImage_MouseLeave);

            xCloseImage_MouseLeftButtonUpCommand = new DelegateCommand(xCloseImage_MouseLeftButtonUp);
            xCloseImage_MouseEnterCommand = new DelegateCommand(xCloseImage_MouseEnter);
            xCloseImage_MouseLeaveCommand = new DelegateCommand(xCloseImage_MouseLeave);
        }

        private void xTopbar_MouseLeftButtonDown()
        {
            ViewMain viewMain = ViewMain.Instance;
            viewMain.DragMove();
        }

        private void xMinimizeImage_MouseLeftButtonUp()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.Model.WindowState = WindowState.Minimized;
        }

        private void xMinimizeImage_MouseEnter()
        {
            Model.Background_Minimize = ResColor.surface_secondary_hover;
        }

        private void xMinimizeImage_MouseLeave()
        {
            Model.Background_Minimize = ResColor.surface_page;
        }

        private void xCloseImage_MouseLeftButtonUp()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void xCloseImage_MouseEnter()
        {
            Model.Background_Close = ResColor.surface_error2;
        }

        private void xCloseImage_MouseLeave()
        {
            Model.Background_Close = ResColor.surface_page;
        }
    }
}
