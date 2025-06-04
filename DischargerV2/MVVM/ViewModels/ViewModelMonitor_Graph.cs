using DischargerV2.Ini;
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
        public event EventHandler DischargerChanged;
        public event EventHandler DischargeModeChanged;
        public event EventHandler GetDataChanged;
        #endregion

        #region Model
        public ModelMonitor_Graph Model { get; set; }
        #endregion

        #region Property
        public string SelectedDischargerName;

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

        public LeftAxis VoltageAxis { get; set; }
        public LeftAxis CurrentAxis { get; set; }
        public LeftAxis TempAxis { get; set; }
        public LeftAxis SocAxis { get; set; }

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

        public bool IsCheckedVoltage
        {
            get => Model.IsCheckedVoltage;
            set => Model.IsCheckedVoltage = value;
        }

        public bool IsCheckedCurrent
        {
            get => Model.IsCheckedCurrent;
            set => Model.IsCheckedCurrent = value;
        }

        public bool IsCheckedTemp
        {
            get => Model.IsCheckedTemp;
            set => Model.IsCheckedTemp = value;
        }

        public bool IsCheckedSoc
        {
            get => Model.IsCheckedSoc;
            set => Model.IsCheckedSoc = value;
        }

        public Visibility VisibilitySoc
        {
            get => Model.VisibilitySoc;
            set => Model.VisibilitySoc = value;
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

        public void InitializeModelDictionary()
        {
            // 기존 값 초기화
            ViewModelMonitor_Graph.Instance.ModelDictionary.Clear();

            // Discharger에서 관련 값 받아와 사용
            List<string> dischargerNameList = ViewModelDischarger.Instance.Model.ToList().ConvertAll(x => x.DischargerName);

            for (int index = 0; index < dischargerNameList.Count; index++)
            {
                string dischargerName = dischargerNameList[index];

                ModelMonitor_Graph modelMonitor_Graph = new ModelMonitor_Graph();
                ViewModelMonitor_Graph.Instance.ModelDictionary.Add(dischargerName, modelMonitor_Graph);
            }
        }

        public void SetDischargerName(string dischargerName)
        {
            if (SelectedDischargerName != null && SelectedDischargerName != "")
            {
                // 현재 값을 ModelDictionary에 넣기 
                ModelDictionary[SelectedDischargerName] = Model;
            }

            SelectedDischargerName = dischargerName;

            // ModelDictionary 값 가져오기
            Model = ModelDictionary[dischargerName];

            DischargerChanged?.Invoke(Instance, EventArgs.Empty);
        }

        public void SetDischargeMode(string dischargerName, EDischargeMode eDischargeMode)
        {
            ModelDictionary[dischargerName].EDischargeMode = eDischargeMode;

            ModelDictionary[dischargerName].IsCheckedVoltage = true;
            ModelDictionary[dischargerName].IsCheckedCurrent = true;
            ModelDictionary[dischargerName].IsCheckedTemp = true;

            if (eDischargeMode == EDischargeMode.Preset)
            {
                ModelDictionary[dischargerName].IsCheckedSoc = true;
                ModelDictionary[dischargerName].VisibilitySoc = Visibility.Visible;
            }
            else
            {
                ModelDictionary[dischargerName].IsCheckedSoc = false;
                ModelDictionary[dischargerName].VisibilitySoc = Visibility.Collapsed;
            }

            Model = ModelDictionary[dischargerName];

            DischargeModeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearReceiveData(string dischargerName)
        {
            try
            {
                ModelDictionary[dischargerName].DataNoList.Clear();
                ModelDictionary[dischargerName].DataVoltageList.Clear();
                ModelDictionary[dischargerName].DataCurrentList.Clear();
                ModelDictionary[dischargerName].DataTempList.Clear();
                ModelDictionary[dischargerName].DataSocList.Clear();
                ModelDictionary[dischargerName].ActiveCount = 1;
                ModelDictionary[dischargerName].ReceiveCount = 0;

                GetDataChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");
            }
        }

        public void SetReceiveData(string dischargerName, DischargerDatas dischargerDatas, double receiveTemp = double.MaxValue)
        {
            try
            {
                ModelDictionary[dischargerName].ReceiveCount += 1;
                if (ModelDictionary[dischargerName].ReceiveCount < ModelDictionary[dischargerName].ActiveCount)
                {
                    return;
                }
                ModelDictionary[dischargerName].ReceiveCount = 0;

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
                if (ModelDictionary[dischargerName].VisibilitySoc == Visibility.Visible)
                {
                    ViewModelSetMode_Preset viewModelSetMode_Preset = ViewModelSetMode_Preset.Instance;

                    if (viewModelSetMode_Preset.ModelDictionary[dischargerName].SelectedBatteryType != null)
                    {
                        string batteryType = viewModelSetMode_Preset.ModelDictionary[dischargerName].SelectedBatteryType;
                        double getSoc = OCV_Table.getSOC(batteryType, dischargerDatas.ReceiveBatteryVoltage);

                        ModelDictionary[dischargerName].DataSocList.Add(getSoc);
                    }
                }

                // DataNoList가 특정 개수가 되면 데이터 절반으로 줄임
                int dataCount = ModelDictionary[dischargerName].DataNoList.Count;
                int activeCount = ModelDictionary[dischargerName].ActiveCount;
                int limitCount = IniDischarge.GetIniData<int>(IniDischarge.EIniData.MaxSampleNum);
                while (true)
                {
                    if (activeCount > limitCount)
                    {
                        limitCount = activeCount;
                        break;
                    }

                    activeCount += ModelDictionary[dischargerName].ActiveCount;
                }
                if (dataCount >= limitCount)
                {
                    ModelDictionary[dischargerName].ActiveCount *= 2;

                    // 짝수 인덱스 항목만 삭제
                    for (int i = (dataCount - 1); i >= 0; i--)
                    {
                        if ((i % 2) == 0)
                        {
                            ModelDictionary[dischargerName].DataNoList.RemoveAt(i);
                            ModelDictionary[dischargerName].DataVoltageList.RemoveAt(i);
                            ModelDictionary[dischargerName].DataCurrentList.RemoveAt(i);
                            ModelDictionary[dischargerName].DataTempList.RemoveAt(i);
                            if (ModelDictionary[dischargerName].VisibilitySoc == Visibility.Visible)
                            {
                                ModelDictionary[dischargerName].DataSocList.RemoveAt(i);
                            }
                        }
                    }

                    // DataNoList 값 갱신
                    for (int i = 0; i < ModelDictionary[dischargerName].DataNoList.Count; i++)
                    {
                        ModelDictionary[dischargerName].DataNoList[i] = i + 1;
                    }
                }

                GetDataChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");
            }
        }
    }
}
