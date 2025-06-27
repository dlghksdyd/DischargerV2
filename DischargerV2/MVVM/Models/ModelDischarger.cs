using Ethernet.Client.Discharger;
using MExpress.Mex;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DischargerV2.MVVM.Models
{
    /// <summary>
    /// 방전기로부터 수신 받은 데이터 모음
    /// </summary>
    public class ModelDischarger : BindableBase
    {
        private int _dischargerIndex = 0;
        public int DischargerIndex
        {
            get { return _dischargerIndex; }
            set
            {
                SetProperty(ref _dischargerIndex, value);
            }
        }

        private SolidColorBrush _foreground = ResColor.text_body;
        public SolidColorBrush Foreground
        {
            get { return _foreground; }
            set
            {
                SetProperty(ref _foreground, value);
            }
        }

        private SolidColorBrush _background = ResColor.surface_primary;
        public SolidColorBrush Background
        {
            get { return _background; }
            set
            {
                SetProperty(ref _background, value);
            }
        }

        private string _no = string.Empty;
        public string No
        {
            get { return _no; }
            set
            {
                SetProperty(ref _no, value);
            }
        }

        private string _machineCode = string.Empty;
        public string MachineCode
        {
            get { return _machineCode; }
            set
            {
                SetProperty(ref _machineCode, value);
            }
        }

        private string _dischargerName = string.Empty;
        public string DischargerName
        {
            get { return _dischargerName; }
            set
            {
                SetProperty(ref _dischargerName, value);
            }
        }

        private DischargerDatas _dischargerData = new DischargerDatas();
        public DischargerDatas DischargerData
        {
            get { return _dischargerData; }
            set
            {
                SetProperty(ref _dischargerData, value);
            }
        }

        private DischargerInfo _dischargerInfo = new DischargerInfo();
        public DischargerInfo DischargerInfo
        {
            get { return _dischargerInfo; }
            set
            {
                SetProperty(ref _dischargerInfo, value);
            }
        }

        private EDischargerState _dischargerState = EDischargerState.None;
        public EDischargerState DischargerState
        {
            get { return _dischargerState; }
            set
            {
                SetProperty(ref _dischargerState, value);
            }
        }

        private SolidColorBrush _stateColor = ResColor.icon_disabled;
        public SolidColorBrush StateColor
        {
            get {  return _stateColor; }
            set
            {
                SetProperty(ref _stateColor, value);
            }
        }

        private string _progressTime = "00:00:00";
        public string ProgressTime
        {
            get { return _progressTime; }
            set
            {
                SetProperty(ref _progressTime, value);
            }
        }

        private Visibility _shortAvailableVisibility = Visibility.Hidden;
        public Visibility ShortAvailableVisibility
        {
            get { return _shortAvailableVisibility; }
            set
            {
                SetProperty(ref _shortAvailableVisibility, value);
            }
        }

        private Visibility _tempReconnectVisibility = Visibility.Hidden;
        public Visibility TempReconnectVisibility
        {
            get { return _tempReconnectVisibility; }
            set
            {
                SetProperty(ref _tempReconnectVisibility, value);
            }
        }

        private Visibility _reconnectVisibility = Visibility.Hidden;
        public Visibility ReconnectVisibility
        {
            get { return _reconnectVisibility; }
            set
            {
                SetProperty(ref _reconnectVisibility, value);
            }
        }

        private Visibility _errorVisibility = Visibility.Hidden;
        public Visibility ErrorVisibility
        {
            get { return _errorVisibility; }
            set
            {
                SetProperty(ref _errorVisibility, value);
            }
        }

        /// 모니터링 프로퍼티
        private Visibility _resumeButtonVisibility = Visibility.Hidden;
        public Visibility ResumeButtonVisibility
        {
            get { return _resumeButtonVisibility; }
            set
            {
                SetProperty(ref _resumeButtonVisibility, value);
            }
        }
        private Visibility _errorDetailButtonVisibility = Visibility.Collapsed;
        public Visibility ErrorDetailButtonVisibility
        {
            get { return _errorDetailButtonVisibility; }
            set
            {
                SetProperty(ref _errorDetailButtonVisibility, value);
            }
        }
    }
}
