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
        private int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                SetProperty(ref _index, value);
            }
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

        private ModelSetMode_Preset _modelSetMode_Preset = new ModelSetMode_Preset();
        public ModelSetMode_Preset ModelSetMode_Preset
        {
            get
            {
                return _modelSetMode_Preset;
            }
            set
            {
                SetProperty(ref _modelSetMode_Preset, value);
            }
        }

        private ModelSetMode_Step _modelSetMode_Step = new ModelSetMode_Step();
        public ModelSetMode_Step ModelSetMode_Step
        {
            get
            {
                return _modelSetMode_Step;
            }
            set
            {
                SetProperty(ref _modelSetMode_Step, value);
            }
        }

        private ModelSetMode_Simple _modelSetMode_Simple = new ModelSetMode_Simple();
        public ModelSetMode_Simple ModelSetMode_Simple
        {
            get
            {
                return _modelSetMode_Simple;
            }
            set
            {
                SetProperty(ref _modelSetMode_Simple, value);
            }
        }
    }
}