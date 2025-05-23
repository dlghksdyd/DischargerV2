using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.ViewModels;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DischargerV2.MVVM.Models
{
    public class ModelStartDischarge : BindableBase
    {
        public class PhaseData
        {
            public double Voltage { get; set; }
            public double Current { get; set; }
            public double CRate { get; set; }
        }

        private string _dischargerName;
        public string DischargerName
        {
            get
            {
                return _dischargerName;
            }
            set
            {
                SetProperty(ref _dischargerName, value);
            }
        }

        private int _dischargerIndex;
        public int DischargerIndex
        {
            get
            {
                return _dischargerIndex;
            }
            set
            {
                SetProperty(ref _dischargerIndex, value);
            }
        }

        private EDischargeMode _mode = EDischargeMode.Preset;
        public EDischargeMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                SetProperty(ref _mode, value);
            }
        }

        private EDischargeTarget _target;
        public EDischargeTarget Target
        {
            get
            {
                return _target;
            }
            set
            {
                SetProperty(ref _target, value);
            }
        }

        private double _dvdq = 0;
        public double Dvdq
        {
            get
            {
                return _dvdq;
            }
            set
            {
                SetProperty(ref _dvdq, value);
            }
        }

        private List<PhaseData> _phaseDataList = new List<PhaseData>();
        public List<PhaseData> PhaseDataList
        {
            get
            {
                return _phaseDataList;
            }
            set
            {
                SetProperty(ref _phaseDataList, value);
            }
        }

        private bool _isEnterLastPhase = false;
        public bool IsEnterLastPhase
        {
            get
            {
                return _isEnterLastPhase;
            }
            set
            {
                SetProperty(ref _isEnterLastPhase, value);
            }
        }

        private int _phaseNo = 0;
        public int PhaseNo
        {
            get
            {
                return _phaseNo;
            }
            set
            {
                SetProperty(ref _phaseNo, value);
            }
        }
    }
}