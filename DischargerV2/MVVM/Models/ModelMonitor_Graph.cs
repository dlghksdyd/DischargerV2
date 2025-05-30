using DischargerV2.MVVM.Enums;
using MExpress.Mex;
using Prism.Mvvm;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DischargerV2.MVVM.Models
{
    public class ModelMonitor_Graph : BindableBase
    {
        private SolidColorBrush _borderColor = ResColor.border_primary;
        public SolidColorBrush BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                SetProperty(ref _borderColor, value);
            }
        }

        public ScottPlot.Color BorderScottPlotColor
        {
            get
            {
                System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(_borderColor.Color.R, _borderColor.Color.G, _borderColor.Color.B);
                
                return ScottPlot.Color.FromColor(drawingColor);
            }
        }

        private SolidColorBrush _textColor = ResColor.text_body;
        public SolidColorBrush TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                SetProperty(ref _textColor, value);
            }
        }

        public ScottPlot.Color TextScottPlotColor
        {
            get
            {
                System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(_textColor.Color.R, _textColor.Color.G, _textColor.Color.B);

                return ScottPlot.Color.FromColor(drawingColor);
            }
        }


        private SolidColorBrush _greenColor = ResColor.border_success;
        public SolidColorBrush GreenColor
        {
            get
            {
                return _greenColor;
            }
            set
            {
                SetProperty(ref _greenColor, value);
            }
        }

        public ScottPlot.Color GreenScottPlotColor
        {
            get
            {
                System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(_greenColor.Color.R, _greenColor.Color.G, _greenColor.Color.B);

                return ScottPlot.Color.FromColor(drawingColor);
            }
        }

        private SolidColorBrush _yellowColor = ResColor.border_warning;
        public SolidColorBrush YellowColor
        {
            get
            {
                return _yellowColor;
            }
            set
            {
                SetProperty(ref _yellowColor, value);
            }
        }

        public ScottPlot.Color YellowScottPlotColor
        {
            get
            {
                System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(_yellowColor.Color.R, _yellowColor.Color.G, _yellowColor.Color.B);

                return ScottPlot.Color.FromColor(drawingColor);
            }
        }

        private SolidColorBrush _blueColor = ResColor.border_infomation;
        public SolidColorBrush BlueColor
        {
            get
            {
                return _blueColor;
            }
            set
            {
                SetProperty(ref _blueColor, value);
            }
        }

        public ScottPlot.Color BlueScottPlotColor
        {
            get
            {
                System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(_blueColor.Color.R, _blueColor.Color.G, _blueColor.Color.B);

                return ScottPlot.Color.FromColor(drawingColor);
            }
        }

        private SolidColorBrush _redColor = ResColor.border_error;
        public SolidColorBrush RedColor
        {
            get
            {
                return _redColor;
            }
            set
            {
                SetProperty(ref _redColor, value);
            }
        }

        public ScottPlot.Color RedScottPlotColor
        {
            get
            {
                System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(_redColor.Color.R, _redColor.Color.G, _redColor.Color.B);

                return ScottPlot.Color.FromColor(drawingColor);
            }
        }

        private Scatter _voltageScatter;
        public Scatter VoltageScatter
        {
            get
            {
                return _voltageScatter;
            }
            set
            {
                SetProperty(ref _voltageScatter, value);
            }
        }

        private Scatter _currentScatter;
        public Scatter CurrentScatter
        {
            get
            {
                return _currentScatter;
            }
            set
            {
                SetProperty(ref _currentScatter, value);
            }
        }

        private Scatter _tempScatter;
        public Scatter TempScatter
        {
            get
            {
                return _tempScatter;
            }
            set
            {
                SetProperty(ref _tempScatter, value);
            }
        }

        private Scatter _socScatter;
        public Scatter SocScatter
        {
            get
            {
                return _socScatter;
            }
            set
            {
                SetProperty(ref _socScatter, value);
            }
        }

        private EDischargeMode _eDischargeMode = EDischargeMode.Preset;
        public EDischargeMode EDischargeMode
        {
            get
            {
                return _eDischargeMode;
            }
            set
            {
                SetProperty(ref _eDischargeMode, value);
            }
        }

        private bool _isCheckedVoltage = true;
        public bool IsCheckedVoltage
        {
            get
            {
                return _isCheckedVoltage;
            }
            set
            {
                SetProperty(ref _isCheckedVoltage, value);
            }
        }

        private bool _isCheckedCurrent = true;
        public bool IsCheckedCurrent
        {
            get
            {
                return _isCheckedCurrent;
            }
            set
            {
                SetProperty(ref _isCheckedCurrent, value);
            }
        }

        private bool _isCheckedTemp = true;
        public bool IsCheckedTemp
        {
            get
            {
                return _isCheckedTemp;
            }
            set
            {
                SetProperty(ref _isCheckedTemp, value);
            }
        }

        private bool _isCheckedSoc = true;
        public bool IsCheckedSoc
        {
            get
            {
                return _isCheckedSoc;
            }
            set
            {
                SetProperty(ref _isCheckedSoc, value);
            }
        }

        private Visibility _visibilitySoc = Visibility.Visible;
        public Visibility VisibilitySoc
        {
            get
            {
                return _visibilitySoc;
            }
            set
            {
                SetProperty(ref _visibilitySoc, value);
            }
        }

        private List<double> _dataNoList = new List<double>();
        public List<double> DataNoList
        {
            get
            {
                return _dataNoList;
            }
            set
            {
                SetProperty(ref _dataNoList, value);
            }
        }

        private List<double> _dataVoltageList = new List<double>();
        public List<double> DataVoltageList
        {
            get
            {
                return _dataVoltageList;
            }
            set
            {
                SetProperty(ref _dataVoltageList, value);
            }
        }

        private List<double> _dataCurrentList = new List<double>();
        public List<double> DataCurrentList
        {
            get
            {
                return _dataCurrentList;
            }
            set
            {
                SetProperty(ref _dataCurrentList, value);
            }
        }

        private List<double> _dataTempList = new List<double>();
        public List<double> DataTempList
        {
            get
            {
                return _dataTempList;
            }
            set
            {
                SetProperty(ref _dataTempList, value);
            }
        }

        private List<double> _dataSocList = new List<double>();
        public List<double> DataSocList
        {
            get
            {
                return _dataSocList;
            }
            set
            {
                SetProperty(ref _dataSocList, value);
            }
        }

        private int _activeTimeCount = 0;
        public int ActiveCount
        {
            get
            {
                return _activeTimeCount;
            }
            set
            {
                SetProperty(ref _activeTimeCount, value);
            }
        }

        private int _receiveTimePeriod = 1;
        public int ReceiveCount
        {
            get
            {
                return _receiveTimePeriod;
            }
            set
            {
                SetProperty(ref _receiveTimePeriod, value);
            }
        }
    }
}