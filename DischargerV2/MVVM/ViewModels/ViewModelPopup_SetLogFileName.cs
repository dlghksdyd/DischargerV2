using DischargerV2.LOG;
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
    public class ViewModelPopup_SetLogFileName : BindableBase
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

        public string ConfirmText
        {
            get => Model.ConfirmText;
            set => Model.ConfirmText = value;
        }

        private string _description = "Please enter LogFileName";
        public string Description 
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private bool _isEnabledConfirm = false;
        public bool IsEnabledConfirm 
        {
            get => _isEnabledConfirm;
            set => SetProperty(ref _isEnabledConfirm, value);
        } 
        #endregion

        public ViewModelPopup_SetLogFileName()
        {
            ExitCommand = new DelegateCommand(Exit);
            CloseCommand = new DelegateCommand(Close);
        }

        public void CheckLogFileName()
        {
            if (Comment == null || Comment == string.Empty)
            {
                Description = "Please enter LogFileName";
                IsEnabledConfirm = false;
            }
            else
            {
                bool isExit = LogDischarge.CheckExit(Comment);

                if (isExit)
                {
                    Description = string.Format(
                        "대상 폴더에 이름이 \"{0}.csv\"인 파일이 이미 있습니다.", Comment);
                    IsEnabledConfirm = false;
                }
                else
                {
                    Description = string.Empty;
                    IsEnabledConfirm = true;
                }
            }
        }

        private void Exit()
        {
            CallBackDelegate();
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OffPopup();
        }
    }
}
