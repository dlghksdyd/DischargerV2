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
    public class ModelSetMode : BindableBase
    {
        public enum EMode
        {
            Preset, Step, Simple
        }

        private Visibility[] _modeVisibility = new Visibility[3]
        {
            Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed
        };
        public Visibility[] ModeVisibility
        {
            get
            {
                return _modeVisibility;
            }
            set
            {
                SetProperty(ref _modeVisibility, value);
            }
        }
    }
}