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

namespace DischargerV2.MVVM.Enums
{
    public enum EMode
    {
        Preset, Step, Simple
    }
}

namespace DischargerV2.MVVM.Models
{
    public class ModelSetMode : BindableBase
    {
        private string _selectedDischargerName;
        public string SelectedDischargerName
        {
            get
            {
                return _selectedDischargerName;
            }
            set
            {
                SetProperty(ref _selectedDischargerName, value);
            }
        }

        private EMode mode = EMode.Preset;
        public EMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                SetProperty(ref mode, value);
            }
        }
    }
}