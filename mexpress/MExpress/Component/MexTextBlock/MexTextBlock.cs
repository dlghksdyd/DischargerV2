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
    [ContentProperty("Text")]
    public class MexTextBlock : ContentControl
    {
        #region Event
        public event EventHandler TextChanged;
        #endregion

        #region Property
        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;
        public static readonly DependencyProperty TextDecorationsProperty;
        public static readonly DependencyProperty TextWrappingProperty;

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

        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBlock instance = d as MexTextBlock;

            instance.TextChanged?.Invoke(instance, EventArgs.Empty);
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBlock instance = d as MexTextBlock;

            FontSet value = (FontSet)e.NewValue;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;
        }

        static MexTextBlock()
        {
            PaddingProperty.OverrideMetadata(typeof(MexTextBlock), new FrameworkPropertyMetadata(new Thickness(0)));
            ForegroundProperty.OverrideMetadata(typeof(MexTextBlock), new FrameworkPropertyMetadata(ResColor.text_body));
            FocusableProperty.OverrideMetadata(typeof(MexTextBlock), new FrameworkPropertyMetadata(false));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(MexTextBlock), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(MexTextBlock), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            BorderBrushProperty.OverrideMetadata(typeof(MexTextBlock), new FrameworkPropertyMetadata(null));
            BorderThicknessProperty.OverrideMetadata(typeof(MexTextBlock), new FrameworkPropertyMetadata(new Thickness(0)));    
            BackgroundProperty.OverrideMetadata(typeof(MexTextBlock), new FrameworkPropertyMetadata(null));

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexTextBlock), new FrameworkPropertyMetadata(new CornerRadius(0)));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexTextBlock), new FrameworkPropertyMetadata("", new PropertyChangedCallback(TextPropertyChanged)));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexTextBlock), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexTextBlock), new FrameworkPropertyMetadata());
            TextDecorationsProperty = DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(MexTextBlock), new FrameworkPropertyMetadata());
            TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(MexTextBlock), new FrameworkPropertyMetadata());
        }
        #endregion

        public MexTextBlock()
        {
            Loaded += MexTextBlock_Loaded;
        }

        private void MexTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (BorderBrush == null)
            {
                BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            if (Background == null)
            {
                Background = new SolidColorBrush(Colors.Transparent);
            }

            if (FontSet == null)
            {
                FontSet = ResFontSet.body_md_regular;
            }
        }
    }
}
