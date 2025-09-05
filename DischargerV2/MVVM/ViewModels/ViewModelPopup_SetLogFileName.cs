using DischargerV2.Languages.Strings;
using DischargerV2.LOG;
using DischargerV2.MVVM.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Diagnostics;

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
            Description = new DynamicString().GetDynamicString("EnterLogFileName");

            ExitCommand = new DelegateCommand(Exit);
            CloseCommand = new DelegateCommand(Close);
        }

        public void CheckLogFileName()
        {
            if (Comment == null || Comment.Trim() == string.Empty)
            {
                Description = new DynamicString().GetDynamicString("EnterLogFileName");
                IsEnabledConfirm = false;
            }
            else
            {
                bool isExists = LogDischarge.CheckExists(Comment);

                if (isExists)
                {
                    var description = new DynamicString().GetDynamicString("LogFileIsExists");

                    Description = $"\"{Comment}.csv\" {description}";
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
