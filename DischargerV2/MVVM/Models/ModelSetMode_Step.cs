using DischargerV2.MVVM.ViewModels;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Mvvm;
using Sqlite.Common;
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
    public class StepInfo
    {
        public bool IsFixedCurrentUse;
        public double VoltPerModule;
        public double CratePerModule;
        public double FixedCurrent;
    }

    public class StepConfigure
    {
        public bool IsCompleteDischarge = true;
        public List<StepInfo> StepInfos = new List<StepInfo>();
    }

    public class ModelSetMode_Step : BindableBase
    {
        private bool _isCompleteDischarge;
        public bool IsCompleteDischarge
        {
            get
            {
                return _isCompleteDischarge;
            }
            set
            {
                SetProperty(ref _isCompleteDischarge, value);
            }
        }

        private ObservableCollection<ModelSetMode_StepData> _content = new ObservableCollection<ModelSetMode_StepData>();
        public ObservableCollection<ModelSetMode_StepData> Content
        {
            get
            {
                return _content;
            }
            set
            {
                SetProperty(ref _content, value);
            }
        }
    }
}