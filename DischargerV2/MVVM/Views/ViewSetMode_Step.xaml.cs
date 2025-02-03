using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.ViewModels;
using MExpress.Mex;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewSetMode_Step.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewSetMode_Step : UserControl
    {
        private ViewModelSetMode_Step _viewModel = new ViewModelSetMode_Step();

        public ViewSetMode_Step()
        {
            InitializeComponent();

            this.DataContext = _viewModel;

            _viewModel.Model.Content.CollectionChanged += Content_CollectionChanged;

            InitializeUI();
        }

        private void Content_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateUI();
        }

        private void InitializeUI()
        {
            _viewModel.Model.Content.Add(new ModelSetMode_StepData());
        }

        private void UpdateUI()
        {
            // StepData 입력한 값 백업
            List<StepInfo> stepInfoList = new List<StepInfo>();

            foreach (var viewSetMode_StepData in xContentPanel.Children.OfType<ViewSetMode_StepData>())
            {
                var viewModelSetMode_StepData = viewSetMode_StepData.DataContext as ViewModelSetMode_StepData;

                if (!double.TryParse(viewModelSetMode_StepData.Voltage, out double voltage))
                {
                    if (viewModelSetMode_StepData.Voltage == null || viewModelSetMode_StepData.Voltage == "")
                    {
                        voltage = double.MaxValue;
                    }
                }

                if (!double.TryParse(viewModelSetMode_StepData.Current, out double current))
                {
                    if (viewModelSetMode_StepData.Current == null || viewModelSetMode_StepData.Current == "")
                    {
                        current = double.MaxValue;
                    }
                }

                if (!double.TryParse(viewModelSetMode_StepData.CRate, out double cRate))
                {
                    if (viewModelSetMode_StepData.CRate == null || viewModelSetMode_StepData.CRate == "")
                    {
                        cRate = double.MaxValue;
                    }
                }

                stepInfoList.Add(new StepInfo()
                {
                    IsFixedCurrentUse = viewModelSetMode_StepData.IsFixedCurrent,
                    VoltPerModule = voltage,
                    FixedCurrent = current,
                    CratePerModule = cRate
                });
            }

            // xContentPanel에 ViewSetMode_StepData 추가
            xContentPanel.Children.Clear();

            for (int index = 0; index < _viewModel.Model.Content.Count; index++)
            {
                StepInfo content = ((index + 1) <= stepInfoList.Count) ?
                    stepInfoList[index] : new StepInfo() {
                        IsFixedCurrentUse = false,
                        VoltPerModule = double.MaxValue,
                        FixedCurrent = double.MaxValue,
                        CratePerModule = double.MaxValue
                    };

                xContentPanel.Children.Add(new ViewSetMode_StepData()
                {
                    DataContext = new ViewModelSetMode_StepData()
                    {
                        No = (index + 1).ToString(),
                        IsFixedCurrent = content.IsFixedCurrentUse,
                        Voltage = (content.VoltPerModule != double.MaxValue) ? content.VoltPerModule.ToString() : "",
                        Current = (content.FixedCurrent != double.MaxValue) ? content.FixedCurrent.ToString() : "",
                        CRate = (content.CratePerModule != double.MaxValue) ? content.CratePerModule.ToString() : "",
                    }
                });

                if (index < _viewModel.Model.Content.Count - 1)
                {
                    xContentPanel.Children.Add(new Grid() { Height = 12 });
                }
            }
        }
    }
}
