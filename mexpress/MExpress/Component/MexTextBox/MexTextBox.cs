using MExpress.Mex;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MExpress.Mex
{
    /// <summary>
    /// MexTextBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public class MexTextBox : ContentControl
    {
        public event EventHandler TextChanged;

        #region Property
        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty WaterMarkProperty;
        public static readonly DependencyProperty WaterMarkForegroundProperty;
        public static readonly DependencyProperty WaterMarkVisibilityProperty;

        public static readonly DependencyProperty ImageWidthProperty;
        public static readonly DependencyProperty ImageHeightProperty;
        public static readonly DependencyProperty ImageLeftProperty;
        public static readonly DependencyProperty ImageRightProperty;
        public static readonly DependencyProperty ImageSetLeftProperty;
        public static readonly DependencyProperty ImageSetRightProperty;

        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty TextPaddingProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;
        public static readonly DependencyProperty TextWrappingProperty;
        public static readonly DependencyProperty AcceptsReturnProperty;

        public static readonly DependencyProperty ColorSetProperty;

        public static readonly DependencyProperty IsReadOnlyProperty;

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }

        public Brush WaterMarkForeground
        {
            get { return (Brush)GetValue(WaterMarkForegroundProperty); }
            set { SetValue(WaterMarkForegroundProperty, value); }
        }

        public Visibility WaterMarkVisibility
        {
            get { return (Visibility)GetValue(WaterMarkVisibilityProperty); }
            private set { SetValue(WaterMarkVisibilityProperty, value); }
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

        public ImageSet_Component ImageSetLeft
        {
            get { return (ImageSet_Component)GetValue(ImageSetLeftProperty); }
            set { SetValue(ImageSetLeftProperty, value); }
        }

        public ImageSet_Component ImageSetRight
        {
            get { return (ImageSet_Component)GetValue(ImageSetRightProperty); }
            set { SetValue(ImageSetRightProperty, value); }
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

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public bool AcceptsReturn
        {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        public ColorSet_TextBox ColorSet
        {
            get { return (ColorSet_TextBox)GetValue(ColorSetProperty); }
            set { SetValue(ColorSetProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void IsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBox instance = d as MexTextBox;

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

        private static void WaterMarkPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBox instance = d as MexTextBox;

            if (instance.Text == "" || instance.Text == null)
            {
                instance.WaterMarkVisibility = Visibility.Visible;
            }
            else
            {
                instance.WaterMarkVisibility = Visibility.Hidden;
            }
        }

        private static void ImageSetLeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBox instance = d as MexTextBox;

            ImageSet_Component value = (ImageSet_Component)e.NewValue;

            if (value == null) return;

            ApplyColorSet(instance);
        }

        private static void ImageSetRightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBox instance = d as MexTextBox;

            ImageSet_Component value = (ImageSet_Component)e.NewValue;

            if (value == null) return;

            ApplyColorSet(instance);
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBox instance = d as MexTextBox;

            string value = (string)e.NewValue;

            if (value == "" || value == null)
            {
                instance.WaterMarkVisibility = Visibility.Visible;
            }
            else
            {
                instance.WaterMarkVisibility = Visibility.Hidden;
                ApplyColorSet(instance);
            }

            // TextChanged Event 발생
            instance.TextChanged?.Invoke(instance, EventArgs.Empty);
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBox instance = d as MexTextBox;

            FontSet value = (FontSet)e.NewValue;

            if (value == null) return;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;
        }

        private static void ColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTextBox instance = d as MexTextBox;

            ColorSet_TextBox value = (ColorSet_TextBox)e.NewValue;

            if (value == null) return;

            ApplyColorSet(instance);
        }

        static MexTextBox()
        {
            PaddingProperty.OverrideMetadata(typeof(MexTextBox), new FrameworkPropertyMetadata(new Thickness(0)));
            BorderThicknessProperty.OverrideMetadata(typeof(MexTextBox), new FrameworkPropertyMetadata(new Thickness(1)));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(MexTextBox), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(MexTextBox), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            IsEnabledProperty.OverrideMetadata(typeof(MexTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsEnabledPropertyChanged)));
            FocusableProperty.OverrideMetadata(typeof(MexTextBox), new FrameworkPropertyMetadata(true));

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexTextBox), new FrameworkPropertyMetadata(new CornerRadius(8)));

            WaterMarkProperty = DependencyProperty.Register("WaterMark", typeof(string), typeof(MexTextBox), new FrameworkPropertyMetadata("", new PropertyChangedCallback(WaterMarkPropertyChanged)));
            WaterMarkForegroundProperty = DependencyProperty.Register("WaterMarkForeground", typeof(Brush), typeof(MexTextBox), new FrameworkPropertyMetadata());
            WaterMarkVisibilityProperty = DependencyProperty.Register("WaterMarkVisibility", typeof(Visibility), typeof(MexTextBox), new FrameworkPropertyMetadata(Visibility.Visible));

            ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(MexTextBox), new FrameworkPropertyMetadata((double)0));
            ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(MexTextBox), new FrameworkPropertyMetadata((double)0));
            ImageLeftProperty = DependencyProperty.Register("ImageLeft", typeof(BitmapSource), typeof(MexTextBox), new FrameworkPropertyMetadata(null));
            ImageRightProperty = DependencyProperty.Register("ImageRight", typeof(BitmapSource), typeof(MexTextBox), new FrameworkPropertyMetadata(null));
            ImageSetLeftProperty = DependencyProperty.Register("ImageSetLeft", typeof(ImageSet_Component), typeof(MexTextBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ImageSetLeftPropertyChanged)));
            ImageSetRightProperty = DependencyProperty.Register("ImageSetRight", typeof(ImageSet_Component), typeof(MexTextBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ImageSetRightPropertyChanged)));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexTextBox), new FrameworkPropertyMetadata("", new PropertyChangedCallback(TextPropertyChanged)));
            TextPaddingProperty = DependencyProperty.Register("TextPadding", typeof(Thickness), typeof(MexTextBox), new FrameworkPropertyMetadata(new Thickness(0)));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexTextBox), new FrameworkPropertyMetadata());
            TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(MexTextBox), new FrameworkPropertyMetadata(TextWrapping.NoWrap));
            AcceptsReturnProperty = DependencyProperty.Register("AcceptsReturn", typeof(bool), typeof(MexTextBox), new FrameworkPropertyMetadata(false));

            ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(ColorSet_TextBox), typeof(MexTextBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ColorSetPropertyChanged)));
        
            IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(MexTextBox), new FrameworkPropertyMetadata());
        }
        #endregion

        public enum EMouseState
        {
            Default, Hover, Selected, Disabled
        }

        public MexTextBox()
        {
            // event
            Loaded += MexTextBox_Loaded;
            MouseEnter += MexTextBox_MouseEnter;
            MouseLeave += MexTextBox_MouseLeave;
            GotFocus += MexTextBox_GotFocus;
            LostFocus += MexTextBox_LostFocus;
            KeyDown += MexTextBox_KeyDown;
        }

        private bool _isShiftTabKeyDown = false;
        private void MexTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                _isShiftTabKeyDown = true;
            }
        }

        private void MexTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (ColorSet == null)
            {
                ColorSet = ResColorSet_TextBox.Primary;
            }

            if (FontSet == null)
            {
                FontSet = ResFontSet.body_md_regular;
            }
        }

        private void MexTextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            MexTextBox instance = sender as MexTextBox;

            ApplyColorSet(instance, EMouseState.Hover);
        }

        private void MexTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            MexTextBox instance = sender as MexTextBox;

            ApplyColorSet(instance, EMouseState.Default);
        }

        private void MexTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            MexTextBox instance = sender as MexTextBox;

            ApplyColorSet(instance, EMouseState.Selected);

            TextBox xTextBox = instance.Template.FindName("xTextBox", this) as TextBox;
            if (!_isShiftTabKeyDown)
            {
                xTextBox.Focus();
            }
            else
            {
                MoveFocusToPrevious();
                _isShiftTabKeyDown = false;
            }
        }

        public void MoveFocusToPrevious()
        {
            // Shift + Tab 효과 (이전 컨트롤로 이동)
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
            this.MoveFocus(request);
        }

        public void SelectAll()
        {
            TextBox xTextBox = this.Template.FindName("xTextBox", this) as TextBox;
            xTextBox.SelectAll();
        }

        private void MexTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            MexTextBox instance = sender as MexTextBox;

            ApplyColorSet(instance, EMouseState.Default);
        }

        private static void ApplyColorSet(MexTextBox instance, EMouseState eMouseState = EMouseState.Default)
        {
            if (eMouseState == EMouseState.Default)
            {
                if (!instance.IsEnabled) return;

                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDefault;
                    instance.Background = instance.ColorSet.BackgroundDefault;
                    instance.Foreground = instance.ColorSet.ForegroundDefault;
                    instance.WaterMarkForeground = instance.ColorSet.WaterMarkForegroundDefault;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImageDefault;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImageDefault;
            }
            else if (eMouseState == EMouseState.Hover)
            {
                if (!instance.IsEnabled) return;

                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderHover;
                    instance.Background = instance.ColorSet.BackgroundHover;
                    instance.Foreground = instance.ColorSet.ForegroundHover;
                    instance.WaterMarkForeground = instance.ColorSet.WaterMarkForegroundHover;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImageHover;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImageHover;
            }
            else if (eMouseState == EMouseState.Selected)
            {
                if (!instance.IsEnabled) return;

                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderSelected;
                    instance.Background = instance.ColorSet.BackgroundSelected;
                    instance.Foreground = instance.ColorSet.ForegroundSelected;
                    instance.WaterMarkForeground = instance.ColorSet.WaterMarkForegroundSelected;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImagePressed;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImagePressed;
            }
            else //(eMouseState == EMouseState.Disabled)
            {
                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDisabled;
                    instance.Background = instance.ColorSet.BackgroundDisabled;
                    instance.Foreground = instance.ColorSet.ForegroundDisabled;
                    instance.WaterMarkForeground = instance.ColorSet.WaterMarkForegroundDisabled;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImageDisabled;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImageDisabled;
            }
        }
    }
}
