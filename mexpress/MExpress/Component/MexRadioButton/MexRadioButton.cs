using System;
using System.Collections.Generic;
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
    public class MexRadioButton : RadioButton
    {
        public event EventHandler IsCheckedChanged;

        #region Property
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty TextPaddingProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;

        public static readonly DependencyProperty ButtonSizeProperty;
        public static readonly DependencyProperty ButtonInnerSizeProperty;
        public static readonly DependencyProperty ButtonBorderThicknessProperty;
        public static readonly DependencyProperty CheckSizeProperty;
        public static readonly DependencyProperty CheckFillProperty;

        public static readonly DependencyProperty ColorSetProperty;

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

        public double ButtonSize
        {
            get { return (double)GetValue(ButtonSizeProperty); }
            set { SetValue(ButtonSizeProperty, value); }
        }

        public double ButtonInnerSize
        {
            get { return (double)GetValue(ButtonInnerSizeProperty); }
            set { SetValue(ButtonInnerSizeProperty, value); }
        }

        public double CheckSize
        {
            get { return (double)GetValue(CheckSizeProperty); }
            set { SetValue(CheckSizeProperty, value); }
        }

        public SolidColorBrush CheckFill
        {
            get { return (SolidColorBrush)GetValue(CheckFillProperty); }
            set { SetValue(CheckFillProperty, value); }
        }

        public Thickness ButtonBorderThickness
        {
            get { return (Thickness)GetValue(ButtonBorderThicknessProperty); }
            set { SetValue(ButtonBorderThicknessProperty, value); }
        }

        public ColorSet_RadioButton ColorSet
        {
            get { return (ColorSet_RadioButton)GetValue(ColorSetProperty); }
            set { SetValue(ColorSetProperty, value); }
        }

        private static void IsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexRadioButton instance = d as MexRadioButton;

            instance._mouseState = EMouseState.Default;
            ApplyColorSet(instance);

            // IsCheckedChanged Event 발생
            instance.IsCheckedChanged?.Invoke(instance, EventArgs.Empty);
        }

        private static void IsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexRadioButton instance = d as MexRadioButton;

            bool value = (bool)e.NewValue;

            instance._mouseState = value ? EMouseState.Default : EMouseState.Disabled;

            ApplyColorSet(instance);
        }

        private static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexRadioButton instance = d as MexRadioButton;

            string value = (string)e.NewValue;

            if (value == null) return;

            instance.Text = value;
            instance.Content = null;
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexRadioButton instance = d as MexRadioButton;

            FontSet value = (FontSet)e.NewValue;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;
        }

        private static void ButtonSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexRadioButton instance = d as MexRadioButton;

            double value = (double)e.NewValue;

            instance.ButtonInnerSize = value - (instance.ButtonBorderThickness.Left * 2);
        }

        private static void ButtonBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexRadioButton instance = d as MexRadioButton;

            Thickness value = (Thickness)e.NewValue;

            instance.ButtonInnerSize = instance.ButtonSize - (value.Left * 2);
        }

        private static void ColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexRadioButton instance = d as MexRadioButton;

            ApplyColorSet(instance);
        }

        static MexRadioButton()
        {
            // property default value
            IsCheckedProperty.OverrideMetadata(typeof(MexRadioButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsCheckedPropertyChanged)));
            IsEnabledProperty.OverrideMetadata(typeof(MexRadioButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsEnabledPropertyChanged)));
            ContentProperty.OverrideMetadata(typeof(MexRadioButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(ContentPropertyChanged)));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(MexRadioButton), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(MexRadioButton), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            FlowDirectionProperty.OverrideMetadata(typeof(MexRadioButton), new FrameworkPropertyMetadata(FlowDirection.LeftToRight));
            ForegroundProperty.OverrideMetadata(typeof(MexRadioButton), new FrameworkPropertyMetadata(ResColor.text_body));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexRadioButton), new FrameworkPropertyMetadata(""));
            TextPaddingProperty = DependencyProperty.Register("TextPadding", typeof(Thickness), typeof(MexRadioButton), new FrameworkPropertyMetadata(new Thickness(10, 0, 0, 0)));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexRadioButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexRadioButton), new FrameworkPropertyMetadata());

            ButtonSizeProperty = DependencyProperty.Register("ButtonSize", typeof(double), typeof(MexRadioButton), new FrameworkPropertyMetadata(24.0, new PropertyChangedCallback(ButtonSizePropertyChanged)));
            ButtonInnerSizeProperty = DependencyProperty.Register("ButtonInnerSize", typeof(double), typeof(MexRadioButton), new FrameworkPropertyMetadata(20.0));
            ButtonBorderThicknessProperty = DependencyProperty.Register("ButtonBorderThickness", typeof(Thickness), typeof(MexRadioButton), new FrameworkPropertyMetadata(new Thickness(2.0), new PropertyChangedCallback(ButtonBorderThicknessPropertyChanged)));
            CheckSizeProperty = DependencyProperty.Register("CheckSize", typeof(double), typeof(MexRadioButton), new FrameworkPropertyMetadata(12.0));
            CheckFillProperty = DependencyProperty.Register("CheckFill", typeof(SolidColorBrush), typeof(MexRadioButton), new FrameworkPropertyMetadata());

            ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(ColorSet_RadioButton), typeof(MexRadioButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ColorSetPropertyChanged)));
        }
        #endregion

        public enum EMouseState
        {
            Default, Hover, Pressed, Disabled
        }

        private EMouseState _mouseState;

        public MexRadioButton()
        {
            MouseEnter += MexRadioButton_MouseEnter;
            MouseLeave += MexRadioButton_MouseLeave;
            MouseLeftButtonDown += MexRadioButton_MouseLeftButtonDown;
            MouseLeftButtonUp += MexRadioButton_MouseLeftButtonUp;
            Loaded += MexRadioButton_Loaded;
        }

        private void MexRadioButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (FontSet == null)
            {
                FontSet = ResFontSet.body_lg_medium;
            }
            if (ColorSet == null)
            {
                ColorSet = ResColorSet_RadioButton.Primary;
            }
        }

        private void MexRadioButton_MouseEnter(object sender, MouseEventArgs e)
        {
            MexRadioButton instance = sender as MexRadioButton;

            instance._mouseState = EMouseState.Hover;
            ApplyColorSet(instance);
        }

        private void MexRadioButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MexRadioButton instance = sender as MexRadioButton;

            instance._mouseState = EMouseState.Default;
            ApplyColorSet(instance);
        }

        private void MexRadioButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MexRadioButton instance = sender as MexRadioButton;

            if (instance.IsChecked == false) instance.IsChecked = true;

            instance._mouseState = EMouseState.Pressed;
            ApplyColorSet(instance);
        }

        private void MexRadioButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MexRadioButton instance = sender as MexRadioButton;

            instance._mouseState = EMouseState.Hover;
            ApplyColorSet(instance);
        }

        private static void ApplyColorSet(MexRadioButton instance)
        {
            if (instance.ColorSet == null)
            {
                return;
            }

            if ((bool)instance.IsChecked)
            {
                if (instance._mouseState == EMouseState.Default)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDefault_Selected;
                    instance.Background = instance.ColorSet.BackgroundDefault_Selected;
                    instance.CheckFill = instance.ColorSet.EllipseDefault_Selected;
                }
                else if (instance._mouseState == EMouseState.Hover)
                {
                    instance.BorderBrush = instance.ColorSet.BorderHover_Selected;
                    instance.Background = instance.ColorSet.BackgroundHover_Selected;
                    instance.CheckFill = instance.ColorSet.EllipseHover_Selected;
                }
                else if (instance._mouseState == EMouseState.Pressed)
                {
                    instance.BorderBrush = instance.ColorSet.BorderPressed_Selected;
                    instance.Background = instance.ColorSet.BackgroundPressed_Selected;
                    instance.CheckFill = instance.ColorSet.EllipsePressed_Selected;
                }
                else //(eMouseState == EMouseState.Disabled)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDisabled_Selected;
                    instance.Background = instance.ColorSet.BackgroundDisabled_Selected;
                    instance.CheckFill = instance.ColorSet.EllipseDisabled_Selected;
                }
            }
            else
            {
                if (instance._mouseState == EMouseState.Default)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDefault;
                    instance.Background = instance.ColorSet.BackgroundDefault;
                    instance.CheckFill = instance.ColorSet.EllipseDefault;
                }
                else if (instance._mouseState == EMouseState.Hover)
                {
                    instance.BorderBrush = instance.ColorSet.BorderHover;
                    instance.Background = instance.ColorSet.BackgroundHover;
                    instance.CheckFill = instance.ColorSet.EllipseHover;
                }
                else if (instance._mouseState == EMouseState.Pressed)
                {
                    instance.BorderBrush = instance.ColorSet.BorderPressed;
                    instance.Background = instance.ColorSet.BackgroundPressed;
                    instance.CheckFill = instance.ColorSet.EllipsePressed;
                }
                else //(eMouseState == EMouseState.Disabled)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDisabled;
                    instance.Background = instance.ColorSet.BackgroundDisabled;
                    instance.CheckFill = instance.ColorSet.EllipseDisabled;
                }
            }
        }
    }
}
