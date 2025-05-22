using DischargerV2.MVVM.ViewModels;
using MExpress.Mex;
using Microsoft.Xaml.Behaviors;
using ScottPlot.Palettes;
using Sqlite.Common;
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
using Utility.Common;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewDischargerInfoTable.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewDischargerInfoTable : UserControl
    {
        private ViewModelDischarger _viewModelDischarger = null;
        private ViewModelTempModule _viewModelTempModule = null;

        public ViewDischargerInfoTable()
        {
            InitializeComponent();

            _viewModelDischarger = ViewModelDischarger.Instance;
            _viewModelTempModule = ViewModelTempModule.Instance;

            InitializeDischargerSelection();
        }

        public void UpdateUi()
        {
            _viewModelDischarger.InitializeDischarger();

            InitializeDischargerSelection();
        }

        private void xTempReconnectImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            StackPanel stackPanel = image.Parent as StackPanel;
            Grid grid = stackPanel.Parent as Grid;
            MexTextBlock textBlock = grid.Children[1] as MexTextBlock;

            string dischargerName = textBlock.Text;

            _viewModelTempModule.ReconnectTempModule(dischargerName);
        }

        private void xDischargerReconnectStackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;
            Grid grid = stackPanel.Parent as Grid;
            MexTextBlock textBlock = grid.Children[1] as MexTextBlock;

            string dischargerName = textBlock.Text;

            _viewModelDischarger.ReconnectDischarger(dischargerName);
        }

        private void xErrorStackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;
            Grid grid = stackPanel.Parent as Grid;
            MexTextBlock textBlock = grid.Children[1] as MexTextBlock;

            string dischargerName = textBlock.Text;

            _viewModelDischarger.OpenPopupError(dischargerName);
        }

        private void xOneDischargerBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            Grid grid = border.Child as Grid;
            MexTextBlock dischargerNoTextBlock = grid.Children[0] as MexTextBlock;
            MexTextBlock dischargerNameTextBlock = grid.Children[1] as MexTextBlock;

            string dischargerName = dischargerNameTextBlock.Text;

            _viewModelDischarger.SelectedDischargerName = dischargerName;
            ViewModelMain.Instance.Model.DischargerIndex = int.Parse(dischargerNoTextBlock.Text) - 1;
            ViewModelMain.Instance.Model.SelectedDischargerName = dischargerName;
            ViewModelSetMode.Instance.SetDischargerName(dischargerName);

            _viewModelDischarger.SelectDischarger(int.Parse(dischargerNoTextBlock.Text) - 1);

            // Table row 배경 설정
            foreach (var oneRow in _viewModelDischarger.Model)
            {
                oneRow.Background = ResColor.surface_primary;

                if (oneRow.DischargerName == dischargerName)
                {
                    oneRow.Background = ResColor.table_selected;
                }
            }
        }

        private void InitializeDischargerSelection()
        {
            if (_viewModelDischarger.Model.Count >= 1)
            {
                _viewModelDischarger.SelectedDischargerName = _viewModelDischarger.Model[0].DischargerName;
                ViewModelMain.Instance.Model.DischargerIndex = 0;
                ViewModelMain.Instance.Model.SelectedDischargerName = _viewModelDischarger.Model[0].DischargerName;
                ViewModelSetMode.Instance.SetDischargerName(_viewModelDischarger.Model[0].DischargerName);
            }
            else
            {
                _viewModelDischarger.SelectedDischargerName = string.Empty;
                ViewModelMain.Instance.Model.DischargerIndex = -1;
                ViewModelMain.Instance.Model.SelectedDischargerName = string.Empty;
                ViewModelSetMode.Instance.SetDischargerName(string.Empty);
            }

            // 첫번째 방전기 선택 UI
            foreach (var oneRow in _viewModelDischarger.Model)
            {
                oneRow.Background = ResColor.surface_primary;
            }
            _viewModelDischarger.Model[0].Background = ResColor.table_selected;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Grid grid = border.Child as Grid;
            MexTextBlock dischargerNoTextBlock = grid.Children[0] as MexTextBlock;
            MexTextBlock dischargerNameTextBlock = grid.Children[1] as MexTextBlock;

            string dischargerName = dischargerNameTextBlock.Text;

            int index = _viewModelDischarger.Model.ToList().FindIndex(x => x.DischargerName == dischargerName);

            if (_viewModelDischarger.Model[index].Background != ResColor.table_selected)
            {
                _viewModelDischarger.Model[index].Background = ResColor.table_action_hover;
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Grid grid = border.Child as Grid;
            MexTextBlock dischargerNoTextBlock = grid.Children[0] as MexTextBlock;
            MexTextBlock dischargerNameTextBlock = grid.Children[1] as MexTextBlock;

            string dischargerName = dischargerNameTextBlock.Text;

            int index = _viewModelDischarger.Model.ToList().FindIndex(x => x.DischargerName == dischargerName);

            if (_viewModelDischarger.Model[index].Background != ResColor.table_selected)
            {
                _viewModelDischarger.Model[index].Background = ResColor.surface_primary;
            }
        }
    }
}
