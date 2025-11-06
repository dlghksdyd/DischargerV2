using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace DischargerV2.Modal
{
    public class ModalBaseViewModel : BindableBase
    {
        public ModalBaseViewModel(Brush background)
        {
            _background = (SolidColorBrush)background;
        }

        private ObservableCollection<ModalInfo> _popupList = new();
        public ObservableCollection<ModalInfo> PopupList
        {
            get => _popupList;
            set => SetProperty(ref _popupList, value);
        }

        public void OpenPopup(object popupViewModel)
        {
            var info = new ModalInfo()
            {
                Content = popupViewModel,
                Background = _background
            };
            PopupList.Add(info);
        }

        public void ClosePopup(object popupViewModel)
        {
            var info = PopupList.ToList().Find(p => ReferenceEquals(p.Content, popupViewModel));
            if (info != null) PopupList.Remove(info);
        }

        private readonly SolidColorBrush _background;
    }

    public class ModalInfo : BindableBase
    {
        private object _content;
        public object Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        private Brush _background = new SolidColorBrush(Color.FromArgb(0x4C, 0xD1, 0xE8, 0xEF));
        public Brush Background
        {
            get => _background;
            set => SetProperty(ref _background, value);
        }
    }
}
