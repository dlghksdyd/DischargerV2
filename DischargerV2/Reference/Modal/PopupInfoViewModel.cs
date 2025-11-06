using System.Windows.Input;
using Prism.Mvvm;
using Prism.Commands;
using DischargerV2.Modal;

namespace Mindims.Mvvm.Popup
{
    public class PopupInfoViewModel : BindableBase
    {
        private double _width = 400;
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }
        
        private double _height = 300;
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }
        
        private string _title = "Title";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        private string _message = "Message";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private bool _isCancel = true;
        public bool IsCancel
        {
            get => _isCancel;
            set => SetProperty(ref _isCancel, value);
        }

        private string _okStr = "확인";
        public string OkStr
        {
            get => _okStr;
            set => SetProperty(ref _okStr, value);
        }

        private string _cancelStr = "취소";
        public string CancelStr
        {
            get => _cancelStr;
            set => SetProperty(ref _cancelStr, value);
        }
        
        public ICommand OkCommand => new DelegateCommand(() =>
        {
            ModalManager.Close(this, ModalResult.Ok);
        });
        
        public ICommand CancelCommand => new DelegateCommand(() =>
        {
            ModalManager.Close(this, ModalResult.Cancel);
        });
    }
}
