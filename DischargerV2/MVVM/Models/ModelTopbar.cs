using MExpress.Mex;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DischargerV2.MVVM.Models
{
    public class ModelTopbar : BindableBase
    {
        private SolidColorBrush _background_Minimize = ResColor.surface_page;
        public SolidColorBrush Background_Minimize
        {
            get
            {
                return _background_Minimize;
            }
            set
            {
                SetProperty(ref _background_Minimize, value);
            }
        }

        private SolidColorBrush _background_Close = ResColor.surface_page;
        public SolidColorBrush Background_Close
        {
            get
            {
                return _background_Close;
            }
            set
            {
                SetProperty(ref _background_Close, value);
            }
        }
    }
}