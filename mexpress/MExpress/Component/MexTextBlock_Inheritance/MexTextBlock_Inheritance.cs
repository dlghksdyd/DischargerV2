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
    public class MexTextBlock_Inheritance : TextBlock
    {
        #region Property
        public static readonly DependencyProperty FontSetProperty;

        public FontSet FontSet
        {
            get { return (FontSet)GetValue(FontSetProperty); }
            set { SetValue(FontSetProperty, value); }
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBlock_Inheritance instance = d as MexTextBlock_Inheritance;

            FontSet value = (FontSet)e.NewValue;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.LineHeight = value.FontHeight;
        }

        static MexTextBlock_Inheritance()
        {
            ForegroundProperty.OverrideMetadata(typeof(MexTextBlock_Inheritance), new FrameworkPropertyMetadata(ResColor.text_body));
            FocusableProperty.OverrideMetadata(typeof(MexTextBlock_Inheritance), new FrameworkPropertyMetadata(false));
         
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexTextBlock_Inheritance), new FrameworkPropertyMetadata(ResFontSet.body_md_regular, new PropertyChangedCallback(FontSetPropertyChanged)));
        }
        #endregion
    }
}
