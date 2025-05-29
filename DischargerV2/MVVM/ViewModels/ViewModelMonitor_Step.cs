using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
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
    public class ViewModelMonitor_Step : BindableBase
    {
        private static ViewModelMonitor_Step _instance = new ViewModelMonitor_Step();
        public static ViewModelMonitor_Step Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelMonitor_Step();
                }
                return _instance;
            }
        }

        #region Defines
        public class PhaseDataBinding : BindableBase
        {
            private string _no;
            public string No
            {
                get => _no;
                set => SetProperty(ref _no, value);
            }

            private string _mode;
            public string Mode
            {
                get => _mode;
                set => SetProperty(ref _mode, value);
            }

            private double _voltage;
            public double Voltage
            {
                get => _voltage;
                set => SetProperty(ref _voltage, value);
            }

            private double _current;
            public double Current
            {
                get => _current;
                set => SetProperty(ref _current, value);
            }

            private SolidColorBrush _background = ResColor.transparent;
            public SolidColorBrush Background
            {
                get => _background;
                set => SetProperty(ref _background, value);
            }
        }
        #endregion

        #region Property
        private ObservableCollection<PhaseDataBinding> _phaseData = new ObservableCollection<PhaseDataBinding>();
        public ObservableCollection<PhaseDataBinding> PhaseData
        {
            get => _phaseData;
            set => SetProperty(ref _phaseData, value);
        }
        #endregion

        private MexScrollViewer _scrollViewer = null;

        private object _phaseDataLock = new object();

        public ViewModelMonitor_Step()
        {

        }

        public void SetScrollViewer(MexScrollViewer scrollViewer)
        {
            _scrollViewer = scrollViewer;
        }

        public void UpdatePhaseData(string dischargerName)
        {
            // 현재 선택된 방전기의 PhaseData를 UI에 반영
            var startDischarge = ViewModelSetMode.Instance.StartDischargeDictionary[dischargerName];
            var phaseDataList = startDischarge.Model.PhaseDataList;

            lock (_phaseDataLock)
            {
                PhaseData.Clear();
                foreach (var phaseData in phaseDataList)
                {
                    PhaseDataBinding binding = new PhaseDataBinding
                    {
                        No = phaseData.No,
                        Mode = phaseData.Mode,
                        Voltage = phaseData.Voltage,
                        Current = phaseData.Current,
                        Background = ResColor.transparent
                    };
                    PhaseData.Add(binding);
                }
            }

            UpdatePhaseIndex();
        }

        public void UpdatePhaseIndex()
        {
            // 현재 선택된 방전기의 PhaseIndex를 UI에 반영
            lock (_phaseDataLock)
            {
                var selectedDischargerName = ViewModelSetMode.Instance.SelectedDischargerName;
                var selectedStartDischarge = ViewModelSetMode.Instance.StartDischargeDictionary[selectedDischargerName];

                if (selectedStartDischarge.PhaseIndex >= PhaseData.Count) return;

                for (int i = 0; i < PhaseData.Count; i++)
                {
                    if (i == selectedStartDischarge.PhaseIndex)
                    {
                        PhaseData[i].Background = ResColor.table_selected;
                    }
                    else
                    {
                        PhaseData[i].Background = ResColor.transparent;
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (selectedStartDischarge.PhaseIndex >= 3)
                    {
                        _scrollViewer.ScrollToVerticalOffset(((double)selectedStartDischarge.PhaseIndex - 2) * 52);
                    }
                    else
                    {
                        _scrollViewer.ScrollToVerticalOffset(0);
                    }
                });
            }
        }
    }
}
