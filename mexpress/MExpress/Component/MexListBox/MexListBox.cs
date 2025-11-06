using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace MExpress.Mex
{
    public class MexListBox : ListBox
    {
        #region Property
        public static readonly DependencyProperty CornerRadiusProperty;

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        static MexListBox()
        {
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexListBox), new FrameworkPropertyMetadata());
        }
        #endregion

        public MexListBox()
        {
            // property default value
            BorderThickness = new Thickness(0);
            CornerRadius = new CornerRadius(8);
            Background = ResColor.surface_page;
            Padding = new Thickness(0);
        }
    }
}
