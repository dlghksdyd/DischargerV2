using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Common;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelMonitor_Graph : BindableBase
    {
        #region Event
        public event EventHandler GetDataChanged;
        #endregion

        #region Model
        public ModelMonitor_Graph Model { get; set; } = new ModelMonitor_Graph();
        #endregion

        #region Property
        private static ViewModelMonitor_Graph _instance = new ViewModelMonitor_Graph();
        public static ViewModelMonitor_Graph Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelMonitor_Graph();
                }
                return _instance;
            }
        }

        private Dictionary<string, ModelMonitor_Graph> _modelDictionary = new Dictionary<string, ModelMonitor_Graph>();
        public Dictionary<string, ModelMonitor_Graph> ModelDictionary
        {
            get => _modelDictionary;
            set => SetProperty(ref _modelDictionary, value);
        }

        public ScottPlot.Color LineColor
        {
            get => Model.BorderScottPlotColor;
        }

        public ScottPlot.Color TextColor
        {
            get => Model.TextScottPlotColor;
        }

        public ScottPlot.Color VoltageColor
        {
            get => Model.GreenScottPlotColor;
        }

        public ScottPlot.Color CurrentColor
        {
            get => Model.YellowScottPlotColor;
        }

        public ScottPlot.Color TempColor
        {
            get => Model.BlueScottPlotColor;
        }

        public ScottPlot.Color SocColor
        {
            get => Model.RedScottPlotColor;
        }

        public Scatter VoltageScatter
        {
            get => Model.VoltageScatter;
            set => Model.VoltageScatter = value;
        }

        public Scatter CurrentScatter
        {
            get => Model.CurrentScatter;
            set => Model.CurrentScatter = value;
        }

        public Scatter TempScatter
        {
            get => Model.TempScatter;
            set => Model.TempScatter = value;
        }

        public Scatter SocScatter
        {
            get => Model.SocScatter;
            set => Model.SocScatter = value;
        }

        public LeftAxis VoltageAxis
        {
            get => Model.VoltageAxis;
            set => Model.VoltageAxis = value;
        }

        public LeftAxis CurrentAxis
        {
            get => Model.CurrentAxis;
            set => Model.CurrentAxis = value;
        }

        public LeftAxis TempAxis
        {
            get => Model.TempAxis;
            set => Model.TempAxis = value;
        }

        public LeftAxis SocAxis
        {
            get => Model.SocAxis;
            set => Model.SocAxis = value;
        }

        public double[] DataNoArray
        {
            get => Model.DataNoList.ToArray();
        }

        public double[] DataVoltageArray
        {
            get => Model.DataVoltageList.ToArray();
        }

        public double[] DataCurrentArray
        {
            get => Model.DataCurrentList.ToArray();
        }

        public double[] DataTempArray
        {
            get => Model.DataTempList.ToArray();
        }

        public double[] DataSocArray
        {
            get => Model.DataSocList.ToArray();
        }
        #endregion

        public ViewModelMonitor_Graph()
        {
            _instance = this;

            InitializeModelDictionary();
        }

        private void InitializeModelDictionary()
        {
            // 기존 값 초기화
            ViewModelMonitor_Graph.Instance.ModelDictionary.Clear();

            // Discharger에서 관련 값 받아와 사용
            List<string> dischargerNameList = ViewModelDischarger.Instance.Model.DischargerNameList.ToList();
            List<DischargerInfo> dischargerInfoList = ViewModelDischarger.Instance.Model.DischargerInfos.ToList();

            for (int index = 0; index < dischargerNameList.Count; index++)
            {
                string dischargerName = dischargerNameList[index];

                ModelMonitor_Graph modelMonitor_Graph = new ModelMonitor_Graph();
                ViewModelMonitor_Graph.Instance.ModelDictionary.Add(dischargerName, modelMonitor_Graph);
            }
        }

        public void SetDischargerName(string dischargerName)
        {
            Model = ModelDictionary[dischargerName];
        }

        public void SetDischargeMode(string dischargerName, EDischargeMode eDischargeMode)
        {
            ModelDictionary[dischargerName].EDischargeMode = eDischargeMode;

            if (eDischargeMode == EDischargeMode.Preset)
            {
                ModelDictionary[dischargerName].IsCheckedSoc = true;
            }
            else
            {
                ModelDictionary[dischargerName].IsCheckedSoc = false;
            }
        }

        public void ClearReceiveData(string dischargerName)
        {
            ModelDictionary[dischargerName].DataNoList.Clear();
            ModelDictionary[dischargerName].DataVoltageList.Clear();
            ModelDictionary[dischargerName].DataCurrentList.Clear();
            ModelDictionary[dischargerName].DataTempList.Clear();
            ModelDictionary[dischargerName].DataSocList.Clear();

            if (Instance.GetDataChanged != null)
            {
                Instance.GetDataChanged(Instance, EventArgs.Empty);
            }
        }

        public void SetReceiveData(string dischargerName, DischargerDatas dischargerDatas, double receiveTemp = double.MaxValue)
        {
            try
            {
                ModelDictionary[dischargerName].DataNoList.Add(ModelDictionary[dischargerName].DataNoList.Count + 1);
                ModelDictionary[dischargerName].DataVoltageList.Add(dischargerDatas.ReceiveBatteryVoltage);
                ModelDictionary[dischargerName].DataCurrentList.Add(dischargerDatas.ReceiveDischargeCurrent);

                if (receiveTemp == double.MaxValue)
                {
                    ModelDictionary[dischargerName].DataTempList.Add(dischargerDatas.ReceiveDischargeTemp);
                }
                else
                {
                    ModelDictionary[dischargerName].DataTempList.Add(receiveTemp);
                }

                // Soc의 경우 Preset 모드에서만 표시
                if (ModelDictionary[dischargerName].IsCheckedSoc)
                {
                    ViewModelSetMode_Preset viewModelSetMode_Preset = ViewModelSetMode_Preset.Instance;

                    if (viewModelSetMode_Preset.ModelDictionary[dischargerName].SelectedBatteryType != null)
                    {
                        string batteryType = viewModelSetMode_Preset.ModelDictionary[dischargerName].SelectedBatteryType;
                        double getSoc = OCV_Table.getSOC(batteryType, dischargerDatas.ReceiveBatteryVoltage);

                        ModelDictionary[dischargerName].DataSocList.Add(getSoc);
                    }
                }

                if (Instance.GetDataChanged != null)
                {
                    Instance.GetDataChanged(Instance, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
