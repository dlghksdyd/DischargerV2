using System.Windows;
using System.Windows.Controls;
using MExpress.Mex;
using DischargerV2.MVVM.ViewModels;
using ScottPlot.Plottables;
using ScottPlot.AxisPanels;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewMonitor_Graph.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewMonitor_Graph : UserControl
    {
        private ViewModelMonitor_Graph _viewModel = ViewModelMonitor_Graph.Instance;

        private SignalXY _signalVoltage;
        private SignalXY _signalCurrent;
        private SignalXY _signalTemp;
        private SignalXY _signalSoc;

        private LeftAxis _yAxisVoltage;
        private LeftAxis _yAxisCurrent;
        private LeftAxis _yAxisTemp;
        private LeftAxis _yAxisSoc;

        public ViewMonitor_Graph()
        {
            InitializeComponent();
            InitializeUI();

            this.DataContext = _viewModel;

            this.IsVisibleChanged += ViewMonitor_Graph_IsVisibleChanged;
        }

        private void ViewMonitor_Graph_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible && isVisible)
            {
                //System.Drawing.Color _voltageColor = System.Drawing.Color.FromArgb(ResColor.border_success.Color.R, ResColor.border_success.Color.G, ResColor.border_success.Color.B);
                //ScottPlot.Color voltageColor = ScottPlot.Color.FromColor(_voltageColor);

                //double[] exNoArray1 = new double[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                //double[] exVoltageArray1 = new double[10] { 90, 85, 80, 75, 70, 65, 60, 55, 50, 45 };
                //double[] exNoArray2 = new double[15] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                //double[] exVoltageArray2 = new double[15] { 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 60, 65, 70, 75, 80 };

                //SignalXYSourceDoubleArray sourceVoltage;

                ////sourceVoltage = new SignalXYSourceDoubleArray(exNoArray2, exVoltageArray2);
                //sourceVoltage = new SignalXYSourceDoubleArray(exNoArray1, exVoltageArray1);

                //xDataGraph.Plot.Remove(_signalVoltage);

                //_signalVoltage = xDataGraph.Plot.Add.SignalXY(sourceVoltage, voltageColor);

                //_signalSoc.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
                //_signalSoc.Axes.YAxis = _yAxisSoc;

                //xDataGraph.Refresh();
            }
        }

        private void InitializeUI()
        {
            System.Drawing.Color borderDrawingColor = System.Drawing.Color.FromArgb(ResColor.border_primary.Color.R, ResColor.border_primary.Color.G, ResColor.border_primary.Color.B);
            ScottPlot.Color borderScottPlotColor = ScottPlot.Color.FromColor(borderDrawingColor);

            System.Drawing.Color textDrawingColor = System.Drawing.Color.FromArgb(ResColor.text_body.Color.R, ResColor.text_body.Color.G, ResColor.text_body.Color.B);
            ScottPlot.Color textScottPlotColor = ScottPlot.Color.FromColor(textDrawingColor);

            System.Drawing.Color voltageDrawingColor = System.Drawing.Color.FromArgb(ResColor.border_success.Color.R, ResColor.border_success.Color.G, ResColor.border_success.Color.B);
            ScottPlot.Color voltageScottPlotColor = ScottPlot.Color.FromColor(voltageDrawingColor);

            System.Drawing.Color currentDrawingColor = System.Drawing.Color.FromArgb(ResColor.border_warning.Color.R, ResColor.border_warning.Color.G, ResColor.border_warning.Color.B);
            ScottPlot.Color currentScottPlotColor = ScottPlot.Color.FromColor(currentDrawingColor);

            System.Drawing.Color tempDrawingColor = System.Drawing.Color.FromArgb(ResColor.border_infomation.Color.R, ResColor.border_infomation.Color.G, ResColor.border_infomation.Color.B);
            ScottPlot.Color tempScottPlotColor = ScottPlot.Color.FromColor(tempDrawingColor);

            System.Drawing.Color socDrawingColor = System.Drawing.Color.FromArgb(ResColor.border_error.Color.R, ResColor.border_error.Color.G, ResColor.border_error.Color.B);
            ScottPlot.Color socScottPlotColor = ScottPlot.Color.FromColor(socDrawingColor);

            double[] exNoArray = new double[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] exVoltageArray = new double[10] { 90, 85, 80, 75, 70, 65, 60, 55, 50, 45 };
            double[] exCurrentrray = new double[10] { 80, 80, 70, 70, 60, 60, 50, 50, 40, 40 };
            double[] exTempArray = new double[10] { 70, 75, 70, 65, 60, 65, 60, 55, 50, 45 };
            double[] exSocArray = new double[10] { 60, 50, 40, 30, 20, 10, 5, 4, 5, 2 };

            // 그래프 색상 설정
            xDataGraph.Plot.FigureBackground.Color = ScottPlot.Color.FromColor(System.Drawing.Color.Transparent);
            xDataGraph.Plot.Grid.MajorLineColor = borderScottPlotColor;
            xDataGraph.Plot.Axes.Color(textScottPlotColor);

            // 데이터 초기화
            xDataGraph.Plot.PlottableList.Clear();

            _signalVoltage = xDataGraph.Plot.Add.SignalXY(exNoArray, exVoltageArray, voltageScottPlotColor);
            _signalCurrent = xDataGraph.Plot.Add.SignalXY(exNoArray, exCurrentrray, currentScottPlotColor);
            _signalTemp = xDataGraph.Plot.Add.SignalXY(exNoArray, exTempArray, tempScottPlotColor);
            //_signalSoc = xDataGraph.Plot.Add.SignalXY(exNoArray, exSocArray, socScottPlotColor);

            // Soc - Y축 생성
            _yAxisSoc = (LeftAxis)xDataGraph.Plot.Axes.Left;
            _yAxisSoc.Color(textScottPlotColor);
            _yAxisSoc.LabelText = "SoC(%)";
            _yAxisSoc.LabelFontColor = socScottPlotColor;
            _yAxisSoc.LabelFontName = "맑은 고딕";
            _yAxisSoc.LabelFontSize = (float)ResFontSize.heading_6;

            //_signalSoc.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
            //_signalSoc.Axes.YAxis = _yAxisSoc;

            var scatterSoc = xDataGraph.Plot.Add.Scatter(exNoArray, exSocArray, socScottPlotColor);
            scatterSoc.LineWidth = (float)1.5;
            scatterSoc.MarkerSize = 5;
            scatterSoc.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
            scatterSoc.Axes.YAxis = _yAxisSoc;

            // Temp
            _yAxisTemp = xDataGraph.Plot.Axes.AddLeftAxis();
            _yAxisTemp.Color(textScottPlotColor);
            _yAxisTemp.LabelText = "Temp(℃)";
            _yAxisTemp.LabelFontColor = tempScottPlotColor;
            _yAxisTemp.LabelFontName = "맑은 고딕";
            _yAxisTemp.LabelFontSize = (float)ResFontSize.heading_6;

            _signalTemp.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
            _signalTemp.Axes.YAxis = _yAxisTemp;

            // Current
            _yAxisCurrent = xDataGraph.Plot.Axes.AddLeftAxis();
            _yAxisCurrent.Color(textScottPlotColor);
            _yAxisCurrent.LabelText = "Current(A)";
            _yAxisCurrent.LabelFontColor = currentScottPlotColor;
            _yAxisCurrent.LabelFontName = "맑은 고딕";
            _yAxisCurrent.LabelFontSize = (float)ResFontSize.heading_6;

            _signalCurrent.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
            _signalCurrent.Axes.YAxis = _yAxisCurrent;

            // Voltage
            _yAxisVoltage = xDataGraph.Plot.Axes.AddLeftAxis();
            _yAxisVoltage.Color(textScottPlotColor);
            _yAxisVoltage.LabelText = "Voltage(V)";
            _yAxisVoltage.LabelFontColor = voltageScottPlotColor;
            _yAxisVoltage.LabelFontName = "맑은 고딕";
            _yAxisVoltage.LabelFontSize = (float)ResFontSize.heading_6;

            _signalVoltage.Axes.XAxis = xDataGraph.Plot.Axes.Bottom;
            _signalVoltage.Axes.YAxis = _yAxisVoltage;

            xDataGraph.Refresh();
        }
    }
}