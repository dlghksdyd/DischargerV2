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

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewPopup_DeviceRegister.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewPopup_DeviceRegister : UserControl
    {
        private ViewModelPopup_DeviceRegister _viewModel = null;

        public ViewPopup_DeviceRegister()
        {
            InitializeComponent();

            _viewModel = new ViewModelPopup_DeviceRegister();

            this.DataContext = _viewModel;

            this.DataContextChanged += ViewPopup_DeviceRegister_DataContextChanged;
        }

        private void ViewPopup_DeviceRegister_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as ViewModelPopup_DeviceRegister;

            InitializeUI();
        }

        private void InitializeUI()
        {
            if (_viewModel.Model.Content.Count > 0)
            {
                xNoDataBorder.Visibility = Visibility.Collapsed;
                xContentPanel.Visibility = Visibility.Visible;

                xContentPanel.Children.Clear();

                // New Device
                Binding newDeviceVisibilityBinding = new Binding();
                newDeviceVisibilityBinding.Path = new PropertyPath("NewDeviceVisibility");
                newDeviceVisibilityBinding.Source = _viewModel.Model;
                newDeviceVisibilityBinding.Mode = BindingMode.TwoWay;

                ViewDeviceRegister_Add viewDeviceRegister_Add = new ViewDeviceRegister_Add();
                viewDeviceRegister_Add.SetBinding(VisibilityProperty, newDeviceVisibilityBinding);
                xContentPanel.Children.Add(viewDeviceRegister_Add);

                Grid spaceGrid = new Grid() { Height = 16 };
                spaceGrid.SetBinding(VisibilityProperty, newDeviceVisibilityBinding);

                xContentPanel.Children.Add(spaceGrid);

                // Device Info
                for (int index = 0; index < _viewModel.Model.Content.Count; index++)
                {
                    TableDischargerInfo tableDischargerInfo = _viewModel.Model.Content[index];

                    xContentPanel.Children.Add(new ViewDeviceRegister_Info()
                    {
                        DataContext = new ViewModelDeviceRegister_Info()
                        {
                            Name = tableDischargerInfo.DischargerName,
                            Ip = tableDischargerInfo.IpAddress,
                            EModel = tableDischargerInfo.Model,
                            EType = tableDischargerInfo.Type,
                            Channel = tableDischargerInfo.DischargerChannel,
                            VoltSpec = tableDischargerInfo.SpecVoltage,
                            CurrSpec = tableDischargerInfo.SpecCurrent,
                            Comport = tableDischargerInfo.TempModuleComPort,
                            ModuleChannel = tableDischargerInfo.TempModuleChannel,
                            TempChannel = tableDischargerInfo.TempChannel,
                        }
                    });

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
    }
}
