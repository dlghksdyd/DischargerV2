using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace MExpress.Mex
{
    public partial class MexCheckBox : ContentControl
    {
        public event EventHandler IsCheckedChanged;

        #region Property
        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;

        public static readonly DependencyProperty IsCheckedProperty;

        public static readonly DependencyProperty CheckBoxSizeProperty;
        public static readonly DependencyProperty InnerCheckBoxSizeProperty;
        public static readonly DependencyProperty CheckBoxPathSizeProperty;

        public static readonly DependencyProperty PathDataProperty;

        public static readonly DependencyProperty ColorSetProperty;

        // private set property
        public static readonly DependencyProperty InnerCornerRadiusProperty;
        public static readonly DependencyProperty StrokeProperty;
        public static readonly DependencyProperty PathVisibilityProperty;

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
            private set { SetValue(FontHeightProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public double CheckBoxSize
        {
            get { return (double)GetValue(CheckBoxSizeProperty); }
            set { SetValue(CheckBoxSizeProperty, value); }
        }

        public double InnerCheckBoxSize
        {
            get { return (double)GetValue(InnerCheckBoxSizeProperty); }
            private set { SetValue(InnerCheckBoxSizeProperty, value); }
        }

        public double CheckBoxPathSize
        {
            get { return (double)GetValue(CheckBoxPathSizeProperty); }
            private set { SetValue(CheckBoxPathSizeProperty, value); }
        }

        public Geometry PathData
        {
            get { return (Geometry)GetValue(PathDataProperty); }
            private set { SetValue(PathDataProperty, value); }
        }

        public ColorSet_CheckBox ColorSet
        {
            get { return (ColorSet_CheckBox)GetValue(ColorSetProperty); }
            set { SetValue(ColorSetProperty, value); }
        }

        public CornerRadius InnerCornerRadius
        {
            get { return (CornerRadius)GetValue(InnerCornerRadiusProperty); }
            private set { SetValue(InnerCornerRadiusProperty, value); }
        }

        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            private set { SetValue(StrokeProperty, value); }
        }

        public Visibility PathVisibility
        {
            get { return (Visibility)GetValue(PathVisibilityProperty); }
            private set { SetValue(PathVisibilityProperty, value); }
        }

        private static void IsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexCheckBox instance = d as MexCheckBox;

            bool value = (bool)e.NewValue;

            instance._mouseState = (value) ? EMouseState.Default : EMouseState.Disabled;

            ApplyColorSet(instance);
        }

        private static void FlowDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexCheckBox instance = d as MexCheckBox;

            ApplyPathData(instance);
            ApplyColorSet(instance);
        }

        private static void CornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexCheckBox instance = d as MexCheckBox;

            CornerRadius value = (CornerRadius)e.NewValue;

            double[] arrCornerRadius = new double[4] {
                value.TopLeft, value.TopRight, value.BottomRight, value.BottomLeft};

            double cornerRadius = arrCornerRadius.Max();

            double offset = (cornerRadius > 1.5) ? -1.5 : 0;

            instance.CornerRadius = value;
            instance.InnerCornerRadius = new CornerRadius(
                value.TopLeft + offset, value.TopRight + offset, value.BottomRight + offset, value.BottomLeft + offset);
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexCheckBox instance = d as MexCheckBox;

            FontSet value = (FontSet)e.NewValue;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;
        }

        private static void IsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexCheckBox instance = d as MexCheckBox;

            ApplyColorSet(instance);

            // IsCheckedChanged Event 발생
            if (instance.IsCheckedChanged != null)
            {
                instance.IsCheckedChanged(instance, EventArgs.Empty);
            }
        }

        private static void CheckBoxSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexCheckBox instance = d as MexCheckBox;

            double value = (double)e.NewValue;

            instance.InnerCheckBoxSize = value - 3.5;
            instance.CheckBoxPathSize = value - 3.5;

            ApplyPathData(instance);
            ApplyColorSet(instance);
        }

        private static void ColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexCheckBox instance = d as MexCheckBox;

            ApplyColorSet(instance);
        }

        static MexCheckBox()
        {
            IsEnabledProperty.OverrideMetadata(typeof(MexCheckBox), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(IsEnabledPropertyChanged)));
            FlowDirectionProperty.OverrideMetadata(typeof(MexCheckBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(FlowDirectionPropertyChanged)));
            CursorProperty.OverrideMetadata(typeof(MexCheckBox), new FrameworkPropertyMetadata(null));
            BorderThicknessProperty.OverrideMetadata(typeof(MexCheckBox), new FrameworkPropertyMetadata(new Thickness(2)));
            ForegroundProperty.OverrideMetadata(typeof(MexCheckBox), new FrameworkPropertyMetadata(null));
            PaddingProperty.OverrideMetadata(typeof(MexCheckBox), new FrameworkPropertyMetadata(new Thickness(12, 0, 0, 0)));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(MexCheckBox), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(MexCheckBox), new FrameworkPropertyMetadata(VerticalAlignment.Center));

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexCheckBox), new FrameworkPropertyMetadata(new CornerRadius(4), new PropertyChangedCallback(CornerRadiusPropertyChanged)));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexCheckBox), new FrameworkPropertyMetadata(""));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexCheckBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexCheckBox), new FrameworkPropertyMetadata());

            IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(MexCheckBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsCheckedPropertyChanged)));

            CheckBoxSizeProperty = DependencyProperty.Register("CheckBoxSize", typeof(double), typeof(MexCheckBox), new FrameworkPropertyMetadata(double.MaxValue, new PropertyChangedCallback(CheckBoxSizePropertyChanged)));
            InnerCheckBoxSizeProperty = DependencyProperty.Register("InnerCheckBoxSize", typeof(double), typeof(MexCheckBox), new FrameworkPropertyMetadata(20.5));
            CheckBoxPathSizeProperty = DependencyProperty.Register("CheckBoxPathSize", typeof(double), typeof(MexCheckBox), new FrameworkPropertyMetadata(20.5));
            
            PathDataProperty = DependencyProperty.Register("PathData", typeof(Geometry), typeof(MexCheckBox), new FrameworkPropertyMetadata());

            ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(ColorSet_CheckBox), typeof(MexCheckBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorSetPropertyChanged)));

            InnerCornerRadiusProperty = DependencyProperty.Register("InnerCornerRadius", typeof(CornerRadius), typeof(MexCheckBox), new FrameworkPropertyMetadata());
            StrokeProperty = DependencyProperty.Register("Stroke", typeof(SolidColorBrush), typeof(MexCheckBox), new FrameworkPropertyMetadata(null));
            PathVisibilityProperty = DependencyProperty.Register("PathVisibility", typeof(Visibility), typeof(MexCheckBox), new FrameworkPropertyMetadata(Visibility.Hidden));
        }
        #endregion

        public enum EMouseState
        {
            Default, Hover, Disabled, Pressed
        }

        private EMouseState _mouseState = EMouseState.Default;

        public MexCheckBox()
        {
            // event
            MouseEnter += MexCheckBox_MouseEnter;
            MouseLeave += MexCheckBox_MouseLeave;
            MouseLeftButtonDown += MexCheckBox_MouseLeftButtonDown;
            MouseLeftButtonUp += MexCheckBox_MouseLeftButtonUp;
            Loaded += MexCheckBox_Loaded;
        }

        private void MexCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (Stroke == null)
            {
                Stroke = ResColorSet_CheckBox.Primary.StrokeDefault;
            }
            if (ColorSet == null)
            {
                ColorSet = ResColorSet_CheckBox.Primary;
            }
            if (FontSet == null)
            {
                FontSet = ResFontSet.body_lg_medium;
            }
            if (Cursor == null)
            {
                Cursor = Cursors.Hand;
            }
            if (Foreground == null)
            {
                Foreground = ResColor.text_body;
            }
            if (CheckBoxSize == double.MaxValue)
            {
                CheckBoxSize = 24.0;
            }
        }

        private void MexCheckBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseState = EMouseState.Hover;
            ApplyColorSet(this);
        }

        private void MexCheckBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseState = EMouseState.Default;
            ApplyColorSet(this);
        }

        private void MexCheckBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _mouseState = EMouseState.Hover;
            ApplyColorSet(this);
        }

        private void MexCheckBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;

            _mouseState = EMouseState.Pressed;
            ApplyColorSet(this);
        }

        private static void ApplyPathData(MexCheckBox mexCheckBox)
        {
            double x = mexCheckBox.CheckBoxPathSize / 4;
            string pathData;

            if (mexCheckBox.FlowDirection == FlowDirection.LeftToRight)
            {
                pathData = string.Format("M {0} {1} L {2} {3} M {4} {5} L {6} {7}",
                x, x * 3 - x / 2,
                x * 2 - x / 4, x * 3,
                x * 2 - x / 4, x * 3,
                x * 3, x + x / 4);
            }
            else //(mexCheckBox.FlowDirection == FlowDirection.RightToLeft)
            {
                pathData = string.Format("M {0} {1} L {2} {3} M {4} {5} L {6} {7}",
                mexCheckBox.CheckBoxPathSize - (x), x * 3 - x / 2,
                mexCheckBox.CheckBoxPathSize - (x * 2 - x / 4), x * 3,
                mexCheckBox.CheckBoxPathSize - (x * 2 - x / 4), x * 3,
                mexCheckBox.CheckBoxPathSize - (x * 3), x + x / 4);
            }

            mexCheckBox.PathData = Geometry.Parse(pathData);
        }

        private static void ApplyColorSet(MexCheckBox mexCheckBox)
        {
            if (mexCheckBox.ColorSet == null)
            {
                return;
            }

            if (mexCheckBox.IsChecked)
            {
                mexCheckBox.PathVisibility = Visibility.Visible;

                if (mexCheckBox._mouseState == EMouseState.Default)
                {
                    mexCheckBox.BorderBrush = mexCheckBox.ColorSet.BorderDefault_Selected;
                    mexCheckBox.Background = mexCheckBox.ColorSet.BackgroundDefault_Selected;
                    mexCheckBox.Stroke = mexCheckBox.ColorSet.StrokeDefault_Selected;
                }
                else if (mexCheckBox._mouseState == EMouseState.Hover)
                {
                    mexCheckBox.BorderBrush = mexCheckBox.ColorSet.BorderHover_Selected;
                    mexCheckBox.Background = mexCheckBox.ColorSet.BackgroundHover_Selected;
                    mexCheckBox.Stroke = mexCheckBox.ColorSet.StrokeHover_Selected;
                }
                else if (mexCheckBox._mouseState == EMouseState.Pressed)
                {
                    mexCheckBox.BorderBrush = mexCheckBox.ColorSet.BorderPressed_Selected;
                    mexCheckBox.Background = mexCheckBox.ColorSet.BackgroundPressed_Selected;
                    mexCheckBox.Stroke = mexCheckBox.ColorSet.StrokePressed_Selected;
                }
                else //(eMouseState == EMouseState.Disabled)
                {
                    mexCheckBox.BorderBrush = mexCheckBox.ColorSet.BorderDisabled_Selected;
                    mexCheckBox.Background = mexCheckBox.ColorSet.BackgroundDisabled_Selected;
                    mexCheckBox.Stroke = mexCheckBox.ColorSet.StrokeDisabled_Selected;
                }
            }
            else
            {
                mexCheckBox.PathVisibility = Visibility.Hidden;

                if (mexCheckBox._mouseState == EMouseState.Default)
                {
                    mexCheckBox.BorderBrush = mexCheckBox.ColorSet.BorderDefault;
                    mexCheckBox.Background = mexCheckBox.ColorSet.BackgroundDefault;
                    mexCheckBox.Stroke = mexCheckBox.ColorSet.StrokeDefault;
                }
                else if (mexCheckBox._mouseState == EMouseState.Hover)
                {
                    mexCheckBox.BorderBrush = mexCheckBox.ColorSet.BorderHover;
                    mexCheckBox.Background = mexCheckBox.ColorSet.BackgroundHover;
                    mexCheckBox.Stroke = mexCheckBox.ColorSet.StrokeHover;
                }
                else if (mexCheckBox._mouseState == EMouseState.Pressed)
                {
                    mexCheckBox.BorderBrush = mexCheckBox.ColorSet.BorderPressed;
                    mexCheckBox.Background = mexCheckBox.ColorSet.BackgroundPressed;
                    mexCheckBox.Stroke = mexCheckBox.ColorSet.StrokePressed;
                }
                else //(eMouseState == EMouseState.Disabled)
                {
                    mexCheckBox.BorderBrush = mexCheckBox.ColorSet.BorderDisabled;
                    mexCheckBox.Background = mexCheckBox.ColorSet.BackgroundDisabled;
                    mexCheckBox.Stroke = mexCheckBox.ColorSet.StrokeDisabled;
                }
            }
        }
    }
}
