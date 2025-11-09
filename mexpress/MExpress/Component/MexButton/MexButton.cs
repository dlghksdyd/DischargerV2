using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MExpress.Mex
{
    public class MexButton : Button
    {
        #region Property
        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty ImageWidthProperty;
        public static readonly DependencyProperty ImageHeightProperty;
        public static readonly DependencyProperty ImageLeftProperty;
        public static readonly DependencyProperty ImageTopProperty;
        public static readonly DependencyProperty ImageRightProperty;
        public static readonly DependencyProperty ImageBottomProperty;
        public static readonly DependencyProperty ImageSetLeftProperty;
        public static readonly DependencyProperty ImageSetTopProperty;
        public static readonly DependencyProperty ImageSetRightProperty;
        public static readonly DependencyProperty ImageSetBottomProperty;
        
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty TextPaddingProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;

        public static readonly DependencyProperty ColorSetProperty;

        public static readonly DependencyProperty IsShadowProperty;

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
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

        public BitmapSource ImageTop
        {
            get { return (BitmapSource)GetValue(ImageTopProperty); }
            set { SetValue(ImageTopProperty, value); }
        }

        public BitmapSource ImageRight
        {
            get { return (BitmapSource)GetValue(ImageRightProperty); }
            set { SetValue(ImageRightProperty, value); }
        }

        public BitmapSource ImageBottom
        {
            get { return (BitmapSource)GetValue(ImageBottomProperty); }
            set { SetValue(ImageBottomProperty, value); }
        }

        public ImageSet_Component ImageSetLeft
        {
            get { return (ImageSet_Component)GetValue(ImageSetLeftProperty); }
            set { SetValue(ImageSetLeftProperty, value); }
        }

        public ImageSet_Component ImageSetTop
        {
            get { return (ImageSet_Component)GetValue(ImageSetTopProperty); }
            set { SetValue(ImageSetTopProperty, value); }
        }

        public ImageSet_Component ImageSetRight
        {
            get { return (ImageSet_Component)GetValue(ImageSetRightProperty); }
            set { SetValue(ImageSetRightProperty, value); }
        }

        public ImageSet_Component ImageSetBottom
        {
            get { return (ImageSet_Component)GetValue(ImageSetBottomProperty); }
            set { SetValue(ImageSetBottomProperty, value); }
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

        public ColorSet_Button ColorSet
        {
            get { return (ColorSet_Button)GetValue(ColorSetProperty); }
            set { SetValue(ColorSetProperty, value); }
        }

        public bool IsShadow
        {
            get { return (bool)GetValue(IsShadowProperty); }
            set { SetValue(IsShadowProperty, value); }
        }

        private static void IsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexButton instance = d as MexButton;

            bool value = (bool)e.NewValue;

            if (value == true)
            {
                ApplyColorSet(instance, EMouseState.Default);
            }
            else
            {
                ApplyColorSet(instance, EMouseState.Disabled);
            }
        }

        private static void ImageSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexButton instance = d as MexButton;

            ImageSet_Component value = (ImageSet_Component)e.NewValue;

            if (value == null) return;

            ApplyColorSet(instance);
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexButton instance = d as MexButton;

            FontSet value = (FontSet)e.NewValue;

            if (value == null) return;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontHeight = value.FontHeight;
            instance.FontWeight = value.FontWeight;
        }

        private static void ColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexButton instance = d as MexButton;

            ColorSet_Button value = (ColorSet_Button)e.NewValue;

            if (value == null) return;

            ApplyColorSet(instance);
        }

        static MexButton()
        {
            IsEnabledProperty.OverrideMetadata(typeof(MexButton), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(IsEnabledPropertyChanged)));
            BorderThicknessProperty.OverrideMetadata(typeof(MexButton), new FrameworkPropertyMetadata(new Thickness(1)));
            PaddingProperty.OverrideMetadata(typeof(MexButton), new FrameworkPropertyMetadata(new Thickness(0)));
            HorizontalAlignmentProperty.OverrideMetadata(typeof(MexButton), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
            VerticalAlignmentProperty.OverrideMetadata(typeof(MexButton), new FrameworkPropertyMetadata(VerticalAlignment.Stretch));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(MexButton), new FrameworkPropertyMetadata(HorizontalAlignment.Center));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(MexButton), new FrameworkPropertyMetadata(VerticalAlignment.Center));

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexButton), new FrameworkPropertyMetadata(new CornerRadius(8)));

            ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(MexButton), new FrameworkPropertyMetadata(0.0));
            ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(MexButton), new FrameworkPropertyMetadata(0.0));
            ImageLeftProperty = DependencyProperty.Register("ImageLeft", typeof(BitmapSource), typeof(MexButton), new FrameworkPropertyMetadata());
            ImageTopProperty = DependencyProperty.Register("ImageTop", typeof(BitmapSource), typeof(MexButton), new FrameworkPropertyMetadata());
            ImageRightProperty = DependencyProperty.Register("ImageRight", typeof(BitmapSource), typeof(MexButton), new FrameworkPropertyMetadata());
            ImageBottomProperty = DependencyProperty.Register("ImageBottom", typeof(BitmapSource), typeof(MexButton), new FrameworkPropertyMetadata());
            ImageSetLeftProperty = DependencyProperty.Register("ImageSetLeft", typeof(ImageSet_Component), typeof(MexButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(ImageSetPropertyChanged)));
            ImageSetTopProperty = DependencyProperty.Register("ImageSetTop", typeof(ImageSet_Component), typeof(MexButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(ImageSetPropertyChanged)));
            ImageSetRightProperty = DependencyProperty.Register("ImageSetRight", typeof(ImageSet_Component), typeof(MexButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(ImageSetPropertyChanged)));
            ImageSetBottomProperty = DependencyProperty.Register("ImageSetBottom", typeof(ImageSet_Component), typeof(MexButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(ImageSetPropertyChanged)));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexButton), new FrameworkPropertyMetadata(""));
            TextPaddingProperty = DependencyProperty.Register("TextPadding", typeof(Thickness), typeof(MexButton), new FrameworkPropertyMetadata(new Thickness(0)));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexButton), new FrameworkPropertyMetadata());

            ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(ColorSet_Button), typeof(MexButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ColorSetPropertyChanged)));

            IsShadowProperty = DependencyProperty.Register("IsShadow", typeof(bool), typeof(MexButton), new FrameworkPropertyMetadata(false));
        }
        #endregion

        public enum EMouseState
        {
            Default, Hover, Pressed, Disabled
        }

        public MexButton()
        {
            // event
            MouseEnter += MexButton_MouseEnter;
            MouseLeave += MexButton_MouseLeave;
            PreviewMouseLeftButtonDown += MexButton_MouseLeftButtonDown;
            PreviewMouseLeftButtonUp += MexButton_MouseLeftButtonUp;
            Loaded += MexButton_Loaded;
        }

        private void MexButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (FontSet == null)
            {
                FontSet = ResFontSet.body_md_medium;
            }

            if (ColorSet == null)
            {
                ColorSet = ResColorSet_Button.Primary;
            }
        }

        private void MexButton_MouseEnter(object sender, MouseEventArgs e)
        {
            MexButton instance = sender as MexButton;

            if (!instance.IsEnabled) return;

            ApplyColorSet(instance, EMouseState.Hover);
        }

        private void MexButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MexButton instance = sender as MexButton;

            if (!instance.IsEnabled) return;

            ApplyColorSet(instance, EMouseState.Default);
        }

        private void MexButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MexButton instance = sender as MexButton;

            if (!instance.IsEnabled) return;

            ApplyColorSet(instance, EMouseState.Pressed);
        }

        private void MexButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MexButton instance = sender as MexButton;

            if (!instance.IsEnabled) return;

            ApplyColorSet(instance, EMouseState.Hover);
        }

        private static void ApplyColorSet(MexButton instance, EMouseState eMouseState = EMouseState.Default)
        {
            if (eMouseState == EMouseState.Default)
            {
                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDefault;
                    instance.Background = instance.ColorSet.BackgroundDefault;
                    instance.Foreground = instance.ColorSet.ForegroundDefault;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImageDefault;
                if (instance.ImageSetTop != null) instance.ImageTop = instance.ImageSetTop.ImageDefault;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImageDefault;
                if (instance.ImageSetBottom != null) instance.ImageBottom = instance.ImageSetBottom.ImageDefault;
            }
            else if (eMouseState == EMouseState.Hover)
            {
                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderHover;
                    instance.Background = instance.ColorSet.BackgroundHover;
                    instance.Foreground = instance.ColorSet.ForegroundHover;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImageHover;
                if (instance.ImageSetTop != null) instance.ImageTop = instance.ImageSetTop.ImageHover;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImageHover;
                if (instance.ImageSetBottom != null) instance.ImageBottom = instance.ImageSetBottom.ImageHover;
            }
            else if (eMouseState == EMouseState.Pressed)
            {
                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderPressed;
                    instance.Background = instance.ColorSet.BackgroundPressed;
                    instance.Foreground = instance.ColorSet.ForegroundPressed;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImagePressed;
                if (instance.ImageSetTop != null) instance.ImageTop = instance.ImageSetTop.ImagePressed;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImagePressed;
                if (instance.ImageSetBottom != null) instance.ImageBottom = instance.ImageSetBottom.ImagePressed;
            }
            else //(eMouseState == EMouseState.Disabled)
            {
                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDisabled;
                    instance.Background = instance.ColorSet.BackgroundDisabled;
                    instance.Foreground = instance.ColorSet.ForegroundDisabled;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImageDisabled;
                if (instance.ImageSetTop != null) instance.ImageTop = instance.ImageSetTop.ImageDisabled;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImageDisabled;
                if (instance.ImageSetBottom != null) instance.ImageBottom = instance.ImageSetBottom.ImageDisabled;
            }
        }
    }
}
