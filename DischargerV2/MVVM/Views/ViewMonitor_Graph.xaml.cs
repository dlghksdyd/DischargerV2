using System.Windows;
using System.Windows.Controls;
using MExpress.Mex;
using DischargerV2.MVVM.ViewModels;
using ScottPlot.Plottables;
using ScottPlot.AxisPanels;
using ScottPlot;
using System.Threading;
using System.Windows.Threading;
using System;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewMonitor_Graph.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewMonitor_Graph : UserControl
    {
        private ViewModelMonitor_Graph _viewModel = ViewModelMonitor_Graph.Instance;

        public ViewMonitor_Graph()
        {
            InitializeComponent();
            InitializeUI();

            this.DataContext = _viewModel;
            
            _viewModel.DischargerChanged += _viewModel_DischargerChanged;
            _viewModel.DischargeModeChanged += _viewModel_DischargeModeChanged;
            _viewModel.GetDataChanged += _viewModel_GetDataChanged;
        }

        private void InitializeUI()
        {
            // 그래프 색상 설정
            xDataGraph.Plot.FigureBackground.Color = ScottPlot.Color.FromColor(System.Drawing.Color.Transparent);
            xDataGraph.Plot.Grid.MajorLineColor = _viewModel.LineColor;
            xDataGraph.Plot.Axes.Color(_viewModel.TextColor);

            // 데이터 초기화
            xDataGraph.Plot.PlottableList.Clear();

            // X/Y축 디자인 변경
            xDataGraph.Plot.Axes.Bottom.TickLabelStyle.IsVisible = false;
            xDataGraph.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            xDataGraph.Plot.Axes.Bottom.MinorTickStyle.Length = 0;
            xDataGraph.Plot.Axes.Left.TickLabelStyle.IsVisible = false;
            xDataGraph.Plot.Axes.Left.TickLabelStyle.FontSize = 0;
            xDataGraph.Plot.Axes.Left.MajorTickStyle.Length = 0;
            xDataGraph.Plot.Axes.Left.MinorTickStyle.Length = 0;

            // Y축 생성 및 디자인 변경 - SoC
            _viewModel.SocAxis = xDataGraph.Plot.Axes.AddLeftAxis();
            _viewModel.SocAxis.TickLabelStyle.ForeColor = _viewModel.TextColor;
            _viewModel.SocAxis.MajorTickStyle.Length = 0;
            _viewModel.SocAxis.MinorTickStyle.Length = 0;
            _viewModel.SocAxis.FrameLineStyle.Color = _viewModel.SocColor;
            _viewModel.SocAxis.FrameLineStyle.Width = 2;
            _viewModel.SocAxis.LabelFontColor = _viewModel.TextColor;
            _viewModel.SocAxis.LabelText = "SoC (%)";
            _viewModel.SocAxis.LabelFontName = "맑은 고딕";
            _viewModel.SocAxis.LabelFontSize = (float)ResFontSize.heading_6;

            // Y축 생성 및 디자인 변경 - Temp
            _viewModel.TempAxis = xDataGraph.Plot.Axes.AddLeftAxis();
            _viewModel.TempAxis.TickLabelStyle.ForeColor = _viewModel.TextColor;
            _viewModel.TempAxis.MajorTickStyle.Length = 0;
            _viewModel.TempAxis.MinorTickStyle.Length = 0;
            _viewModel.TempAxis.FrameLineStyle.Color = _viewModel.TempColor;
            _viewModel.TempAxis.FrameLineStyle.Width = 2;
            _viewModel.TempAxis.LabelFontColor = _viewModel.TextColor;
            _viewModel.TempAxis.LabelText = "Temp (℃)";
            _viewModel.TempAxis.LabelFontName = "맑은 고딕";
            _viewModel.TempAxis.LabelFontSize = (float)ResFontSize.heading_6;

            // Y축 생성 및 디자인 변경 - Current
            _viewModel.CurrentAxis = xDataGraph.Plot.Axes.AddLeftAxis();
            _viewModel.CurrentAxis.TickLabelStyle.ForeColor = _viewModel.TextColor;
            _viewModel.CurrentAxis.MajorTickStyle.Length = 0;
            _viewModel.CurrentAxis.MinorTickStyle.Length = 0;
            _viewModel.CurrentAxis.FrameLineStyle.Color = _viewModel.CurrentColor;
            _viewModel.CurrentAxis.FrameLineStyle.Width = 2;
            _viewModel.CurrentAxis.LabelFontColor = _viewModel.TextColor;
            _viewModel.CurrentAxis.LabelText = "Current (A)";
            _viewModel.CurrentAxis.LabelFontName = "맑은 고딕";
            _viewModel.CurrentAxis.LabelFontSize = (float)ResFontSize.heading_6;

            //  Y축 생성 및 디자인 변경 - Voltage
            _viewModel.VoltageAxis = xDataGraph.Plot.Axes.AddLeftAxis();
            _viewModel.VoltageAxis.TickLabelStyle.ForeColor = _viewModel.TextColor;
            _viewModel.VoltageAxis.MajorTickStyle.Length = 0;
            _viewModel.VoltageAxis.MinorTickStyle.Length = 0;
            _viewModel.VoltageAxis.FrameLineStyle.Color = _viewModel.VoltageColor;
            _viewModel.VoltageAxis.FrameLineStyle.Width = 2;
            _viewModel.VoltageAxis.LabelFontColor = _viewModel.TextColor;
            _viewModel.VoltageAxis.LabelText = "Voltage (V)";
            _viewModel.VoltageAxis.LabelFontName = "맑은 고딕";
            _viewModel.VoltageAxis.LabelFontSize = (float)ResFontSize.heading_6;

            DrawGraph();
        }

        private void _viewModel_DischargerChanged(object sender, System.EventArgs e)
        {
            UpdateUI();
            DrawGraph();
        }

        private void _viewModel_DischargeModeChanged(object sender, System.EventArgs e)
        {
            UpdateUI();
        }

        private void _viewModel_GetDataChanged(object sender, System.EventArgs e)
        {
            DrawGraph();
        }

        private void xCheckBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DrawGraph();
        }

        private void UpdateUI()
        {
            xVoltCheckBox.IsChecked = _viewModel.IsCheckedVoltage;
            xCurrent.IsChecked = _viewModel.IsCheckedCurrent;
            xTempCheckBox.IsChecked = _viewModel.IsCheckedTemp;
            xSocCheckBox.IsChecked = _viewModel.IsCheckedSoc;
            xSocCheckBox.Visibility = _viewModel.VisibilitySoc;
        }

        private void DrawGraph()
        {
            // 데이터 초기화
            xDataGraph.Plot.PlottableList.Clear();

            // Voltage 
            if (_viewModel.IsCheckedVoltage)
            {
                // Y축 설정
                _viewModel.VoltageAxis.IsVisible = true;

                xDataGraph.Plot.Axes.Left.Min = _viewModel.VoltageAxis.Min;
                xDataGraph.Plot.Axes.Left.Max = _viewModel.VoltageAxis.Max;

                // Data 생성
                Dispatcher.Invoke(() =>
                {
                    _viewModel.VoltageScatter = xDataGraph.Plot.Add.Scatter(_viewModel.Model.DataNoList.ToArray(), _viewModel.Model.DataVoltageList.ToArray(), _viewModel.Model.GreenScottPlotColor);
                    _viewModel.VoltageScatter.LineWidth = (float)1.5;
                    _viewModel.VoltageScatter.MarkerSize = 5;
                    _viewModel.VoltageScatter.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
                    _viewModel.VoltageScatter.Axes.YAxis = _viewModel.VoltageAxis;
                });
            }
            else
            {
                // Y축 설정
                _viewModel.VoltageAxis.IsVisible = false;
            }

            // Current 
            if (_viewModel.IsCheckedCurrent)
            {
                // Y축 설정
                _viewModel.CurrentAxis.IsVisible = true;

                xDataGraph.Plot.Axes.Left.Min = _viewModel.CurrentAxis.Min;
                xDataGraph.Plot.Axes.Left.Max = _viewModel.CurrentAxis.Max;

                // Data 생성
                Dispatcher.Invoke(() =>
                {
                    _viewModel.CurrentScatter = xDataGraph.Plot.Add.Scatter(_viewModel.DataNoArray, _viewModel.DataCurrentArray, _viewModel.CurrentColor);
                    _viewModel.CurrentScatter.LineWidth = (float)1.5;
                    _viewModel.CurrentScatter.MarkerSize = 5;
                    _viewModel.CurrentScatter.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
                    _viewModel.CurrentScatter.Axes.YAxis = _viewModel.CurrentAxis;
                });
            }
            else
            {
                // Y축 설정
                _viewModel.CurrentAxis.IsVisible = false;
            }

            // Temp 
            if (_viewModel.IsCheckedTemp)
            {
                // Y축 설정
                _viewModel.TempAxis.IsVisible = true;

                xDataGraph.Plot.Axes.Left.Min = _viewModel.TempAxis.Min;
                xDataGraph.Plot.Axes.Left.Max = _viewModel.TempAxis.Max;

                // Data 생성
                Dispatcher.Invoke(() =>
                {
                    _viewModel.TempScatter = xDataGraph.Plot.Add.Scatter(_viewModel.DataNoArray, _viewModel.DataTempArray, _viewModel.TempColor);
                    _viewModel.TempScatter.LineWidth = (float)1.5;
                    _viewModel.TempScatter.MarkerSize = 5;
                    _viewModel.TempScatter.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
                    _viewModel.TempScatter.Axes.YAxis = _viewModel.TempAxis;
                });
            }
            else
            {
                // Y축 설정
                _viewModel.TempAxis.IsVisible = false;
            }

            // SoC 
            if (_viewModel.IsCheckedSoc)
            {
                // Y축 설정
                _viewModel.SocAxis.IsVisible = true;

                xDataGraph.Plot.Axes.Left.Min = _viewModel.SocAxis.Min;
                xDataGraph.Plot.Axes.Left.Max = _viewModel.SocAxis.Max;

                // Data 생성
                Dispatcher.Invoke(() =>
                {
                    _viewModel.SocScatter = xDataGraph.Plot.Add.Scatter(_viewModel.DataNoArray, _viewModel.DataSocArray, _viewModel.SocColor);
                    _viewModel.SocScatter.LineWidth = (float)1.5;
                    _viewModel.SocScatter.MarkerSize = 5;
                    _viewModel.SocScatter.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
                    _viewModel.SocScatter.Axes.YAxis = _viewModel.SocAxis;
                });
            }
            else
            {
                // Y축 설정
                _viewModel.SocAxis.IsVisible = false;
            }

            xDataGraph.Refresh();
            xDataGraph.Plot.Axes.AutoScale();
        }
    }
}