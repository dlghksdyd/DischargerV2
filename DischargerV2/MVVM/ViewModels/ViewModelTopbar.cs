using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static DischargerV2.LOG.LogTrace;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelTopbar : BindableBase
    {
        #region Command
        public ICommand xImage_MouseLeftButtonUpCommand { get; set; }
        public ICommand xImage_MouseEnterCommand { get; set; }
        public ICommand xImage_MouseLeaveCommand { get; set; }
        #endregion

        #region Model
        public ModelTopbar Model { get; set; } = new ModelTopbar();
        #endregion

        public ViewModelTopbar()
        {
            Model = new ModelTopbar();

            Model.Title = $"Discharger v{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";

            xImage_MouseLeftButtonUpCommand = new RelayCommand<object>(xImage_MouseLeftButtonUp);
            xImage_MouseEnterCommand = new RelayCommand<object>(xImage_MouseEnter);
            xImage_MouseLeaveCommand = new RelayCommand<object>(xImage_MouseLeave);
        }

        private void xImage_MouseLeftButtonUp(object obj)
        {
            if (obj is MouseEventArgs e)
            {
                if (e.Source is Image xImage)
                {
                    var name = xImage.Name;

                    if (name == "xMinimizeImage")
                    {
                        ViewModelMain viewModelMain = ViewModelMain.Instance;
                        viewModelMain.Model.WindowState = WindowState.Minimized;
                    }
                    else if (name == "xCloseImage")
                    {
                        ViewModelMain viewModelMain = ViewModelMain.Instance;

                        var viewModelDischarger = ViewModelDischarger.Instance;
                        if (viewModelDischarger.IsDischarging())
                        {
                            ViewModelPopup_Warning viewModelPopup_Warning = new ViewModelPopup_Warning()
                            {
                                Title = "Warning",
                                Comment = "Please stop discharging before opening this popup.",
                                CancelButtonVisibility = Visibility.Hidden,
                            };
                            viewModelMain.SetViewModelPopup_Warning(viewModelPopup_Warning);
                            viewModelMain.OpenNestedPopup(ModelMain.ENestedPopup.Warning);
                            return;
                        }

                        ViewModelPopup_Info viewModelPopup_Info = new ViewModelPopup_Info()
                        {
                            Title = "Confirm Exit",
                            Comment = "Do you want to exit?",
                            CallBackDelegate = Close,
                            ConfirmText = "Exit"
                        };
                        viewModelMain.SetViewModelPopup_Info(viewModelPopup_Info);
                        viewModelMain.OpenPopup(ModelMain.EPopup.Info);
                    }
                }
            }
        }

        private void xImage_MouseEnter(object obj)
        {
            if (obj is MouseEventArgs e)
            {
                if (e.Source is Image xImage)
                {
                    var name = xImage.Name;

                    if (name == "xMinimizeImage")
                    {
                        Model.Background_Minimize = ResColor.surface_secondary_hover;
                    }
                    else if (name == "xCloseImage")
                    {
                        Model.Background_Close = ResColor.surface_error2;
                    }
                }
            }
        }

        private void xImage_MouseLeave(object obj)
        {
            if (obj is MouseEventArgs e)
            {
                if (e.Source is Image xImage)
                {
                    var name = xImage.Name;

                    if (name == "xMinimizeImage")
                    {
                        Model.Background_Minimize = ResColor.surface_page;
                    }
                    else if (name == "xCloseImage")
                    {
                        Model.Background_Close = ResColor.surface_page;
                    }
                }
            }
        }

        public static void Close()
        {
            try
            {
                new LogTrace(ELogTrace.TRACE_PROGRAM_END);

                ViewModelDischarger.Instance.FinalizeDischarger();

                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_PROGRAM_END, ex);
            }
        }
    }
}
