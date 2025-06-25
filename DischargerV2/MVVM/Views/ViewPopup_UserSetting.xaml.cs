using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MExpress.Mex;
using DischargerV2.MVVM.ViewModels;
using Sqlite.Common;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewPopup_UserSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewPopup_UserSetting : UserControl
    {
        private ViewModelPopup_UserSetting _viewModel = null;

        public ViewPopup_UserSetting()
        {
            InitializeComponent();

            _viewModel = new ViewModelPopup_UserSetting();

            this.DataContext = _viewModel;

            this.DataContextChanged += ViewPopup_UserSetting_DataContextChanged;
        }

        private void ViewPopup_UserSetting_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as ViewModelPopup_UserSetting;

            UpdateUI();
        }

        private void UpdateUI()
        {
            xContentPanel.Children.Clear();

            xContentPanel.Children.Add(new ViewUserSetting_Add() 
            { 
                Margin = (_viewModel.Model.Content.Count > 1) ? new Thickness(20, 20, 0, 0) : new Thickness(20, 20, 0, 20)
            });

            int oddOrEven = ((_viewModel.Model.Content.Count + 1) % 2 == 0) ? 2 : 1;

            for (int index = 0; index < _viewModel.Model.Content.Count; index++)
            {
                TableUserInfo tableUserInfo = _viewModel.Model.Content[index] as TableUserInfo;

                xContentPanel.Children.Add(new ViewUserSetting_Info()
                {
                    DataContext = new ViewModelUserSetting_Info()
                    {
                        IsAdmin = tableUserInfo.IsAdmin,
                        Id = tableUserInfo.UserId,
                        Password = tableUserInfo.Password,
                        Name = tableUserInfo.UserName
                    },
                    Margin = (index < _viewModel.Model.Content.Count - oddOrEven) ? new Thickness(20, 20, 0, 0) : new Thickness(20, 20, 0, 20)
                });
            }
        }
    }
}
