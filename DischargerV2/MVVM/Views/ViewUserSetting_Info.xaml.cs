using System.Windows;
using System.Windows.Controls;
using DischargerV2.MVVM.ViewModels;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewUserSetting_Info.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewUserSetting_Info : UserControl
    {
        public ViewUserSetting_Info()
        {
            InitializeComponent();

            this.DataContext = new ViewModelUserSetting_Info();

            this.DataContextChanged += ViewUserSetting_Info_DataContextChanged;
        }

        private void ViewUserSetting_Info_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is ViewModelUserSetting_Info viewModelUserSetting)
            {
                if (viewModelUserSetting.Id.ToLower() == "admin" ||
                    viewModelUserSetting.Id.ToLower() == "mintech")
                {
                    xDeleteImage.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void xEditImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.DataContext is ViewModelUserSetting_Info viewModelUserSetting)
            {
                viewModelUserSetting.OpenPopupEditUser();
            }
        }

        private void xDeleteImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.DataContext is ViewModelUserSetting_Info viewModelUserSetting)
            {
                viewModelUserSetting.OpenPopupDelete();
            }
        }
    }
}
