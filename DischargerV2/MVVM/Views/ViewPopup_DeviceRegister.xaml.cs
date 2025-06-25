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
using static System.Net.Mime.MediaTypeNames;
using DischargerV2.MVVM.Models;
using System.Collections.ObjectModel;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewPopup_DeviceRegister.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewPopup_DeviceRegister : UserControl
    {
        private ViewModelPopup_DeviceRegister _viewModel = new ViewModelPopup_DeviceRegister();

        public ViewPopup_DeviceRegister()
        {
            InitializeComponent();

            this.DataContext = _viewModel;

            this.DataContextChanged += ViewPopup_DeviceRegister_DataContextChanged;
        }

        private void xAddButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.OpenNewData();
        }

        private void xCloseImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.Close();
        }

        private void ViewPopup_DeviceRegister_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as ViewModelPopup_DeviceRegister;

            UpdateUI(_viewModel.Model.SelectedItem);
        }

        private void UpdateUI(string selectedItem = "")
        {
            if (_viewModel.Model.Content.Count > 0)
            {
                var getType = _viewModel.Model.Content.GetType();

                xNoDataBorder.Visibility = Visibility.Collapsed;
                xContentPanel.Visibility = Visibility.Visible;

                xContentPanel.Children.Clear();

                // New 
                Binding newDeviceVisibilityBinding = new Binding();
                newDeviceVisibilityBinding.Path = new PropertyPath("NewDeviceVisibility");
                newDeviceVisibilityBinding.Source = _viewModel.Model;
                newDeviceVisibilityBinding.Mode = BindingMode.TwoWay;

                ViewDeviceRegister_Add view_Add = new ViewDeviceRegister_Add();
                view_Add.SetBinding(VisibilityProperty, newDeviceVisibilityBinding);
                view_Add.IsVisibleChanged += ViewDeviceRegister_Add_IsVisibleChanged;
                xContentPanel.Children.Add(view_Add);

                Grid spaceGrid = new Grid() { Height = 16 };
                spaceGrid.SetBinding(VisibilityProperty, newDeviceVisibilityBinding);
                xContentPanel.Children.Add(spaceGrid);

                // TableDischargerInfo
                for (int index = 0; index < _viewModel.Model.Content.Count; index++)
                {
                    TableDischargerInfo tableDischargerInfo = _viewModel.Model.Content[index];

                    // Edit
                    if (tableDischargerInfo.DischargerName == selectedItem)
                    {
                        ModelDeviceRegister modelDeviceRegister = new ModelDeviceRegister()
                        {
                            MachineCode = tableDischargerInfo.MC_CD,
                            Name = tableDischargerInfo.DischargerName,
                            Ip = tableDischargerInfo.IpAddress,
                            DischargerModel = tableDischargerInfo.Model.ToString(),
                            Type = tableDischargerInfo.Type.ToString(),
                            Channel = tableDischargerInfo.DischargerChannel.ToString(),
                            VoltSpec = tableDischargerInfo.SpecVoltage.ToString(),
                            CurrSpec = tableDischargerInfo.SpecCurrent.ToString(),
                            IsTempModule = tableDischargerInfo.IsTempModule,
                            Comport = tableDischargerInfo.TempModuleComPort,
                            ModuleChannel = tableDischargerInfo.TempModuleChannel.ToString(),
                            TempChannel = tableDischargerInfo.TempChannel.ToString()
                        };
                        ViewModelDeviceRegister_Edit.Instance.SetModelData(modelDeviceRegister);

                        xContentPanel.Children.Add(new ViewDeviceRegister_Edit());
                    }
                    // Info
                    else
                    {
                        xContentPanel.Children.Add(new ViewDeviceRegister_Info()
                        {
                            DataContext = new ViewModelDeviceRegister_Info()
                            {
                                MachineCode = tableDischargerInfo.MC_CD,
                                Name = tableDischargerInfo.DischargerName,
                                Ip = tableDischargerInfo.IpAddress,
                                DischargerModel = tableDischargerInfo.Model.ToString(),
                                Type = tableDischargerInfo.Type.ToString(),
                                Channel = tableDischargerInfo.DischargerChannel.ToString(),
                                VoltSpec = tableDischargerInfo.SpecVoltage.ToString(),
                                CurrSpec = tableDischargerInfo.SpecCurrent.ToString(),
                                IsTempModule = tableDischargerInfo.IsTempModule,
                                Comport = tableDischargerInfo.TempModuleComPort,
                                ModuleChannel = tableDischargerInfo.TempModuleChannel.ToString(),
                                TempChannel = tableDischargerInfo.TempChannel.ToString(),
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

        private void ViewDeviceRegister_Add_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                xScrollViewer.ScrollToVerticalOffset(0);
            }
        }
    }
}
