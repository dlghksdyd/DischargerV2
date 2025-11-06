using DischargerV2.Languages.Strings;
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
using DischargerV2.Modal;

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
                        var viewModelDischarger = ViewModelDischarger.Instance;

                        if (viewModelDischarger.IsDischarging())
                        {
                            var title_Warning = new DynamicString().GetDynamicString("PopupWarning_Title");
                            var comment_Warning = new DynamicString().GetDynamicString("PopupWarning_Comment_CloseProgram");

                            var viewModelPopup_Warning = new ViewModelPopup_Warning()
                            {
                                Title = title_Warning,
                                Comment = comment_Warning,
                                CancelButtonVisibility = Visibility.Hidden
                            };

                            ModalManager.Open(viewModelPopup_Warning);
                            return;
                        }

                        var title_Info = new DynamicString().GetDynamicString("PopupInfo_Title_CloseProgram");
                        var comment_Info = new DynamicString().GetDynamicString("PopupInfo_Comment_CloseProgram");
                        var confirmText_Info = new DynamicString().GetDynamicString("Exit");

                        ViewModelPopup_Info viewModelPopup_Info = new ViewModelPopup_Info()
                        {
                            Title = title_Info,
                            Comment = comment_Info,
                            Parameter = string.Empty,
                            CallBackDelegate = Close,
                            ConfirmText = confirmText_Info,
                            CancelVisibility = Visibility.Visible
                        };
                        ModalManager.Open(viewModelPopup_Info);
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
                new LogTrace(ELogTrace.SYSTEM_OK_PROGRAM_END);

                ViewModelDischarger.Instance.FinalizeDischarger();

                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: ViewModelTopbar\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");

                new LogTrace(ELogTrace.SYSTEM_ERROR_PROGRAM_END, ex);
            }
        }
    }
}
