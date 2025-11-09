using MExpress.Mex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;

namespace MExpress.Mex
{
    public class MexLabel : ContentControl
    {
        #region Property
        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty TextPaddingProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;

        public static readonly DependencyProperty ImageWidthProperty;
        public static readonly DependencyProperty ImageHeightProperty;
        public static readonly DependencyProperty ImageLeftProperty;
        public static readonly DependencyProperty ImageRightProperty;

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Thickness TextPadding
        {
            get { return (Thickness)GetValue(TextPaddingProperty); }
            set { SetValue(TextPaddingProperty, value); }
        }

        public FontSet FontSet
        {
            get { return (FontSet)GetValue(FontSetProperty); }
            set { SetValue(FontSetProperty, value); }
        }

        public double FontHeight
        {
            get { return (double)GetValue(FontHeightProperty); }
            set { SetValue(FontHeightProperty, value); }
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public BitmapSource ImageLeft
        {
            get { return (BitmapSource)GetValue(ImageLeftProperty); }
            set { SetValue(ImageLeftProperty, value); }
        }

        public BitmapSource ImageRight
        {
            get { return (BitmapSource)GetValue(ImageRightProperty); }
            set { SetValue(ImageRightProperty, value); }
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexLabel instance = d as MexLabel;

            FontSet value = (FontSet)e.NewValue;

            if (value == null) return;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;
        }

        static MexLabel()
        {
            FocusableProperty.OverrideMetadata(typeof(MexLabel), new FrameworkPropertyMetadata(false));
            BorderThicknessProperty.OverrideMetadata(typeof(MexLabel), new FrameworkPropertyMetadata(new Thickness(0)));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(MexLabel), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(MexLabel), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            ForegroundProperty.OverrideMetadata(typeof(MexLabel), new FrameworkPropertyMetadata(ResColor.text_body));
            BorderBrushProperty.OverrideMetadata(typeof(MexLabel), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent)));
            BackgroundProperty.OverrideMetadata(typeof(MexLabel), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent)));

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexLabel), new FrameworkPropertyMetadata(new CornerRadius(0)));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexLabel), new FrameworkPropertyMetadata(""));
            TextPaddingProperty = DependencyProperty.Register("TextPadding", typeof(Thickness), typeof(MexLabel), new FrameworkPropertyMetadata(new Thickness(0)));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexLabel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexLabel), new FrameworkPropertyMetadata());

            ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(MexLabel), new FrameworkPropertyMetadata(0.0));
            ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(MexLabel), new FrameworkPropertyMetadata(0.0));
            ImageLeftProperty = DependencyProperty.Register("ImageLeft", typeof(BitmapSource), typeof(MexLabel), new FrameworkPropertyMetadata(null));
            ImageRightProperty = DependencyProperty.Register("ImageRight", typeof(BitmapSource), typeof(MexLabel), new FrameworkPropertyMetadata(null));
        }
        #endregion

        public MexLabel()
        {
            Loaded += MexLabel_Loaded;
        }

        private void MexLabel_Loaded(object sender, RoutedEventArgs e)
        {
            if (FontSet == null)
            {
                FontSet = ResFontSet.body_md_regular;
            }
        }
    }
}
