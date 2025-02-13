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
using MExpress.Example;
using System.Runtime.Remoting.Channels;
using DischargerV2.MVVM.Models;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewPopup_ModelRegister.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewPopup_ModelRegister : UserControl
    {
        private ViewModelPopup_ModelRegister _viewModel = new ViewModelPopup_ModelRegister();

        public ViewPopup_ModelRegister()
        {
            InitializeComponent();

            this.DataContext = _viewModel;

            this.DataContextChanged += ViewPopup_ModelRegister_DataContextChanged;
        }

        private void ViewPopup_ModelRegister_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as ViewModelPopup_ModelRegister;

            UpdateUI(_viewModel.Model.SelectedId);
        }

        private void UpdateUI(int selectedId = -1)
        {
            if(_viewModel.Model.Content.Count > 0)
            {
                xNoDataBorder.Visibility = Visibility.Collapsed;
                xContentPanel.Visibility = Visibility.Visible;

                xContentPanel.Children.Clear();

                // New 
                Binding newDataVisibilityBinding = new Binding();
                newDataVisibilityBinding.Path = new PropertyPath("NewDataVisibility");
                newDataVisibilityBinding.Source = _viewModel.Model;
                newDataVisibilityBinding.Mode = BindingMode.TwoWay;

                ViewModelRegister_Add view_Add = new ViewModelRegister_Add();
                view_Add.SetBinding(VisibilityProperty, newDataVisibilityBinding);
                view_Add.IsVisibleChanged += ViewModelRegister_Add_IsVisibleChanged;
                xContentPanel.Children.Add(view_Add);

                Grid spaceGrid = new Grid() { Height = 16 };
                spaceGrid.SetBinding(VisibilityProperty, newDataVisibilityBinding);
                xContentPanel.Children.Add(spaceGrid);

                // TableDischargerModel
                for (int index = 0; index < _viewModel.Model.Content.Count; index++)
                {
                    TableDischargerModel tableDischargerModel = _viewModel.Model.Content[index];

                    // Edit
                    if (tableDischargerModel.Id == selectedId)
                    {
                        ModelModelRegister modelModelRegister = new ModelModelRegister()
                        {
                            Id = tableDischargerModel.Id,
                            DischargerModel = tableDischargerModel.Model.ToString(),
                            Type = tableDischargerModel.Type.ToString(),
                            Channel = tableDischargerModel.Channel.ToString(),
                            VoltSpec = tableDischargerModel.SpecVoltage.ToString(),
                            CurrSpec = tableDischargerModel.SpecCurrent.ToString(),
                            VoltMax = tableDischargerModel.SafetyVoltMax.ToString("F1"),
                            VoltMin = tableDischargerModel.SafetyVoltMin.ToString("F1"),
                            CurrMax = tableDischargerModel.SafetyCurrentMax.ToString("F1"),
                            CurrMin = tableDischargerModel.SafetyCurrentMin.ToString("F1"),
                            TempMax = tableDischargerModel.SafetyTempMax.ToString("F1"),
                            TempMin = tableDischargerModel.SafetyTempMin.ToString("F1")
                        };
                        ViewModelModelRegister_Edit.Instance.SetModelData(modelModelRegister);

                        xContentPanel.Children.Add(new ViewModelRegister_Edit());
                    }
                    // Info
                    else
                    {
                        xContentPanel.Children.Add(new ViewModelRegister_Info()
                        {
                            DataContext = new ViewModelModelRegister_Info()
                            {
                                Id = tableDischargerModel.Id,
                                DischargerModel = tableDischargerModel.Model.ToString(),
                                Type = tableDischargerModel.Type.ToString(),
                                Channel = tableDischargerModel.Channel.ToString(),
                                VoltSpec = tableDischargerModel.SpecVoltage.ToString(),
                                CurrSpec = tableDischargerModel.SpecCurrent.ToString(),
                                VoltMax = tableDischargerModel.SafetyVoltMax.ToString("F1"),
                                VoltMin = tableDischargerModel.SafetyVoltMin.ToString("F1"),
                                CurrMax = tableDischargerModel.SafetyCurrentMax.ToString("F1"),
                                CurrMin = tableDischargerModel.SafetyCurrentMin.ToString("F1"),
                                TempMax = tableDischargerModel.SafetyTempMax.ToString("F1"),
                                TempMin = tableDischargerModel.SafetyTempMin.ToString("F1"),
                            }
                        });
                    }

                    if (index < _viewModel.Model.Content.Count - 1)
                    {
                        xContentPanel.Children.Add(new Grid() { Height = 16 });
                    }
                }
            }
            else
            {
                xNoDataBorder.Visibility = Visibility.Visible;
                xContentPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ViewModelRegister_Add_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                xScrollViewer.ScrollToVerticalOffset(0);
            }
        }
    }
}
