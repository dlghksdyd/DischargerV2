using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
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
using static System.Net.Mime.MediaTypeNames;

namespace MExpress.Mex
{
    public class MexPasswordBox : ContentControl, INotifyPropertyChanged
    {
        #region MVVM
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <param name="onChanged">Action that is called after the property value has been changed.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="args">The PropertyChangedEventArgs</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        public DelegateCommand<TextBox> OnTextBoxLoadedCommand { get; set; }
        public DelegateCommand<PasswordBox> OnPasswordBoxLoadedCommand { get; set; }

        public DelegateCommand OnTextChangedCommand { get; set; }
        public DelegateCommand OnPasswordChangedCommand { get; set; }

        public DelegateCommand OnIsHidePasswordChangedCommand { get; set; }

        #endregion

        #region Property
        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty WaterMarkProperty;
        public static readonly DependencyProperty WaterMarkForgroundProperty;
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

        public static readonly DependencyProperty TextBoxVisibilityProperty;
        public static readonly DependencyProperty PasswordBoxVisibilityProperty;

        public static readonly DependencyProperty IsHidePasswordProperty;

        public static readonly DependencyProperty ImageSizeHidePasswordProperty;
        public static readonly DependencyProperty ImageHidePasswordProperty;
        public static readonly DependencyProperty ImageSetHidePasswordProperty;

        public static readonly DependencyProperty ColorSetProperty;

        public static readonly DependencyProperty ViewModelProperty;

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

        public SolidColorBrush WaterMarkForground
        {
            get { return (SolidColorBrush)GetValue(WaterMarkForgroundProperty); }
            set { SetValue(WaterMarkForgroundProperty, value); }
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

        public Visibility TextBoxVisibility
        {
            get { return (Visibility)GetValue(TextBoxVisibilityProperty); }
            private set { SetValue(TextBoxVisibilityProperty, value); }
        }

        public Visibility PasswordBoxVisibility
        {
            get { return (Visibility)GetValue(PasswordBoxVisibilityProperty); }
            private set { SetValue(PasswordBoxVisibilityProperty, value); }
        }

        public bool IsHidePassword
        {
            get { return (bool)GetValue(IsHidePasswordProperty); }
            set { SetValue(IsHidePasswordProperty, value); }
        }

        public double ImageSizeHidePassword
        {
            get { return (double)GetValue(ImageSizeHidePasswordProperty); }
            set { SetValue(ImageSizeHidePasswordProperty, value); }
        }

        public BitmapSource ImageHidePassword
        {
            get { return (BitmapSource)GetValue(ImageHidePasswordProperty); }
            set { SetValue(ImageHidePasswordProperty, value); }
        }

        public ImageSet_Toggle ImageSetHidePassword
        {
            get { return (ImageSet_Toggle)GetValue(ImageSetHidePasswordProperty); }
            set { SetValue(ImageSetHidePasswordProperty, value); }
        }

        public ColorSet_TextBox ColorSet
        {
            get { return (ColorSet_TextBox)GetValue(ColorSetProperty); }
            set { SetValue(ColorSetProperty, value); }
        }

        public MexPasswordBox ViewModel
        {
            get { return (MexPasswordBox)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private static void WaterMarkPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexPasswordBox instance = d as MexPasswordBox;

            if (instance.Text == "" || instance.Text == null)
            {
                instance.WaterMarkVisibility = Visibility.Visible;
            }
            else
            {
                instance.WaterMarkVisibility = Visibility.Hidden;
            }
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexPasswordBox instance = d as MexPasswordBox;

            string value = (string)e.NewValue;

            if (value == "" || value == null)
            {
                instance.WaterMarkVisibility = Visibility.Visible;

                if (instance.ViewTextBox != null) instance.ViewTextBox.Text = "";
                if (instance.ViewPasswordBox != null) instance.ViewPasswordBox.Password = "";
            }
            else
            {
                instance.WaterMarkVisibility = Visibility.Hidden;
            }
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexPasswordBox instance = d as MexPasswordBox;

            FontSet value = (FontSet)e.NewValue;

            if (value == null) return;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;
        }

        private static void ImageSetLeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexPasswordBox instance = d as MexPasswordBox;

            ImageSet_Component value = (ImageSet_Component)e.NewValue;

            if (value == null) return;

            ApplyColorSet(instance);
        }

        private static void ImageSetRightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexPasswordBox instance = d as MexPasswordBox;

            ImageSet_Component value = (ImageSet_Component)e.NewValue;

            if (value == null) return;

            ApplyColorSet(instance);
        }

        private static void IsHidePasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexPasswordBox instance = d as MexPasswordBox;

            bool value = (bool)e.NewValue;

            if (instance.ImageSetHidePassword == null) return;

            if (value)
            {
                instance.ImageHidePassword = instance.ImageSetHidePassword.ImageTrue;

                instance.TextBoxVisibility = Visibility.Hidden;
                instance.PasswordBoxVisibility = Visibility.Visible;
            }
            else
            {
                instance.ImageHidePassword = instance.ImageSetHidePassword.ImageFalse;

                instance.TextBoxVisibility = Visibility.Visible;
                instance.PasswordBoxVisibility = Visibility.Hidden;
            }
        }

        private static void HidePasswordImageSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexPasswordBox instance = d as MexPasswordBox;

            ImageSet_Toggle value = (ImageSet_Toggle)e.NewValue;

            if (value == null) return;

            instance.ImageHidePassword = (instance.IsHidePassword) ? value.ImageTrue : value.ImageFalse;
        }

        private static void ColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexPasswordBox instance = d as MexPasswordBox;

            ColorSet_TextBox value = (ColorSet_TextBox)e.NewValue;

            if (value == null) return;

            ApplyColorSet(instance);
        }

        static MexPasswordBox()
        {
            PaddingProperty.OverrideMetadata(typeof(MexPasswordBox), new FrameworkPropertyMetadata(null));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(MexPasswordBox), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(MexPasswordBox), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            BorderThicknessProperty.OverrideMetadata(typeof(MexPasswordBox), new FrameworkPropertyMetadata(new Thickness(1)));

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexPasswordBox), new FrameworkPropertyMetadata(new CornerRadius(8)));

            WaterMarkProperty = DependencyProperty.Register("WaterMark", typeof(string), typeof(MexPasswordBox), new FrameworkPropertyMetadata("Water Mark", new PropertyChangedCallback(WaterMarkPropertyChanged)));
            WaterMarkForgroundProperty = DependencyProperty.Register("WaterMarkForground", typeof(SolidColorBrush), typeof(MexPasswordBox), new FrameworkPropertyMetadata());
            WaterMarkVisibilityProperty = DependencyProperty.Register("WaterMarkVisibility", typeof(Visibility), typeof(MexPasswordBox), new FrameworkPropertyMetadata(Visibility.Visible));

            ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(MexPasswordBox), new FrameworkPropertyMetadata());
            ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(MexPasswordBox), new FrameworkPropertyMetadata());
            ImageLeftProperty = DependencyProperty.Register("ImageLeft", typeof(BitmapSource), typeof(MexPasswordBox), new FrameworkPropertyMetadata(null));
            ImageRightProperty = DependencyProperty.Register("ImageRight", typeof(BitmapSource), typeof(MexPasswordBox), new FrameworkPropertyMetadata(null));
            ImageSetLeftProperty = DependencyProperty.Register("ImageSetLeft", typeof(ImageSet_Component), typeof(MexPasswordBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ImageSetLeftPropertyChanged)));
            ImageSetRightProperty = DependencyProperty.Register("ImageSetRight", typeof(ImageSet_Component), typeof(MexPasswordBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ImageSetRightPropertyChanged)));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexPasswordBox), new FrameworkPropertyMetadata("", new PropertyChangedCallback(TextPropertyChanged)));
            TextPaddingProperty = DependencyProperty.Register("TextPadding", typeof(Thickness), typeof(MexPasswordBox), new FrameworkPropertyMetadata(new Thickness(0)));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexPasswordBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexPasswordBox), new FrameworkPropertyMetadata());

            TextBoxVisibilityProperty = DependencyProperty.Register("TextBoxVisibility", typeof(Visibility), typeof(MexPasswordBox), new FrameworkPropertyMetadata(Visibility.Hidden));
            PasswordBoxVisibilityProperty = DependencyProperty.Register("PasswordBoxVisibility", typeof(Visibility), typeof(MexPasswordBox), new FrameworkPropertyMetadata(Visibility.Visible));

            IsHidePasswordProperty = DependencyProperty.Register("IsHidePassword", typeof(bool), typeof(MexPasswordBox), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(IsHidePasswordPropertyChanged)));

            ImageSizeHidePasswordProperty = DependencyProperty.Register("ImageSizeHidePassword", typeof(double), typeof(MexPasswordBox), new FrameworkPropertyMetadata(20.0));
            ImageHidePasswordProperty = DependencyProperty.Register("ImageHidePassword", typeof(BitmapSource), typeof(MexPasswordBox), new FrameworkPropertyMetadata());
            ImageSetHidePasswordProperty = DependencyProperty.Register("ImageSetHidePassword", typeof(ImageSet_Toggle), typeof(MexPasswordBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(HidePasswordImageSetPropertyChanged)));

            ColorSetProperty = DependencyProperty.Register("ColorSet_TextBox", typeof(ColorSet_TextBox), typeof(MexPasswordBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorSetPropertyChanged)));

            ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(MexPasswordBox), typeof(MexPasswordBox), new FrameworkPropertyMetadata());
        }
        #endregion

        public enum EMouseState
        {
            Default, Hover, Selected, Disabled
        }

        public MexPasswordBox()
        {
            // MVVM
            ViewModel = this;

            OnTextBoxLoadedCommand = new DelegateCommand<TextBox>(OnTextBoxLoaded);
            OnPasswordBoxLoadedCommand = new DelegateCommand<PasswordBox>(OnPasswordBoxLoaded);

            OnTextChangedCommand = new DelegateCommand(OnTextChanged);
            OnPasswordChangedCommand = new DelegateCommand(OnPasswordChanged);

            OnIsHidePasswordChangedCommand = new DelegateCommand(OnIsHidePasswordChanged);

            // event
            Loaded += MexPasswordBox_Loaded;
            MouseEnter += MexPasswordBox_MouseEnter;
            MouseLeave += MexPasswordBox_MouseLeave;
            GotFocus += MexPasswordBox_GotFocus;
            LostFocus += MexPasswordBox_LostFocus;
            KeyDown += MexPasswordBox_KeyDown;
        }

        private bool _isShiftTabKeyDown = false;
        private void MexPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                _isShiftTabKeyDown = true;
            }
        }

        private void MexPasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            // property default value
            if (FontSet == null)
            {
                FontSet = ResFontSet.body_md_regular;
            }
            if (ColorSet == null)
            {
                ColorSet = ResColorSet_TextBox.Primary;
            }
            if (Padding == null)
            {
                Padding = new Thickness(12, 8, 12, 8);
            }
        }

        private void MexPasswordBox_MouseEnter(object sender, MouseEventArgs e)
        {
            MexPasswordBox instance = sender as MexPasswordBox;

            ApplyColorSet(instance, EMouseState.Hover);
        }

        private void MexPasswordBox_MouseLeave(object sender, MouseEventArgs e)
        {
            MexPasswordBox instance = sender as MexPasswordBox;

            ApplyColorSet(instance, EMouseState.Default);
        }

        private void MexPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            MexPasswordBox instance = sender as MexPasswordBox;

            ApplyColorSet(instance, EMouseState.Selected);

            if (instance.TextBoxVisibility == Visibility.Visible)
            {
                TextBox xPasswordTextBox = instance.Template.FindName("xPasswordTextBox", this) as TextBox;
                if (!_isShiftTabKeyDown)
                {
                    xPasswordTextBox.Focus();
                }
                else
                {
                    MoveFocusToPrevious();
                    _isShiftTabKeyDown = false;
                }
            }
            else
            {
                PasswordBox xPasswordBox = instance.Template.FindName("xPasswordBox", this) as PasswordBox;
                if (!_isShiftTabKeyDown)
                {
                    xPasswordBox.Focus();
                }
                else
                {
                    MoveFocusToPrevious();
                    _isShiftTabKeyDown = false;
                }
            }
        }

        public void MoveFocusToPrevious()
        {
            // Shift + Tab 효과 (이전 컨트롤로 이동)
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
            this.MoveFocus(request);
        }

        private void MexPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            MexPasswordBox instance = sender as MexPasswordBox;

            ApplyColorSet(instance, EMouseState.Default);
        }

        private static void ApplyColorSet(MexPasswordBox instance, EMouseState eMouseState = EMouseState.Default)
        {
            if (eMouseState == EMouseState.Default)
            {
                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDefault;
                    instance.Background = instance.ColorSet.BackgroundDefault;
                    instance.Foreground = instance.ColorSet.ForegroundDefault;
                    instance.WaterMarkForground = instance.ColorSet.WaterMarkForegroundDefault;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImageDefault;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImageDefault;
            }
            else if (eMouseState == EMouseState.Hover)
            {
                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderHover;
                    instance.Background = instance.ColorSet.BackgroundHover;
                    instance.Foreground = instance.ColorSet.ForegroundHover;
                    instance.WaterMarkForground = instance.ColorSet.WaterMarkForegroundHover;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImageHover;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImageHover;
            }
            else if (eMouseState == EMouseState.Selected)
            {
                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderSelected;
                    instance.Background = instance.ColorSet.BackgroundSelected;
                    instance.Foreground = instance.ColorSet.ForegroundSelected;
                    instance.WaterMarkForground = instance.ColorSet.WaterMarkForegroundSelected;
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
                    instance.WaterMarkForground = instance.ColorSet.WaterMarkForegroundDisabled;
                }

                if (instance.ImageSetLeft != null) instance.ImageLeft = instance.ImageSetLeft.ImageDisabled;
                if (instance.ImageSetRight != null) instance.ImageRight = instance.ImageSetRight.ImageDisabled;
            }
        }

        // MVVM
        private TextBox ViewTextBox;
        private PasswordBox ViewPasswordBox;

        private void OnTextBoxLoaded(TextBox textBox)
        {
            ViewTextBox = textBox;
        }

        private void OnPasswordBoxLoaded(PasswordBox passwordBox)
        {
            ViewPasswordBox = passwordBox;
        }

        private void OnTextChanged()
        {
            Text = ViewTextBox.Text;
        }

        private void OnPasswordChanged()
        {
            Text = ViewPasswordBox.Password;
        }

        private void OnIsHidePasswordChanged()
        {
            if (IsHidePassword == true)
            {
                ViewTextBox.Text = ViewPasswordBox.Password;
                Text = ViewPasswordBox.Password;
            }
            else
            {
                ViewPasswordBox.Password = ViewTextBox.Text;
                Text = ViewTextBox.Text;
            }

            IsHidePassword = !IsHidePassword;
        }
    }
}
