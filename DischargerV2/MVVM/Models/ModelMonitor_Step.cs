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
    public class ModelMonitor_Step : BindableBase
    {
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