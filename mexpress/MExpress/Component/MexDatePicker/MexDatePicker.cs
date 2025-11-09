using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MExpress.Mex
{
    public class MexDatePicker : Control
    {
        public event EventHandler SelectedDateChanged;

        #region Property
        public static readonly double MinCalendarSize = 300;

        public static string StringFormat = "yyyy-MM-dd";

        public static readonly DependencyProperty ClipRadiusProperty;
        public static readonly DependencyProperty ClipRectProperty;
        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty WaterMarkProperty;
        public static readonly DependencyProperty WaterMarkForegroundProperty;
        public static readonly DependencyProperty WaterMarkVisibilityProperty;

        public static readonly DependencyProperty SelectedDateProperty;

        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;
        public static readonly DependencyProperty DateFormatProperty;

        public static readonly DependencyProperty ButtonWidthProperty;
        public static readonly DependencyProperty ButtonColorProperty;
        public static readonly DependencyProperty ButtonImageWidthProperty;
        public static readonly DependencyProperty ButtonImageHeightProperty;

        public static readonly DependencyProperty CalendarClipRectProperty;
        public static readonly DependencyProperty CalendarSizeProperty;
        public static readonly DependencyProperty CalendarStaysOpenProperty;
        public static readonly DependencyProperty IsCalendarOpenProperty;

        public static readonly DependencyProperty ColorSetProperty;
        public static readonly DependencyProperty BorderBrushDefaultProperty;

        public double ClipRadius
        {
            get { return (double)GetValue(ClipRadiusProperty); }
            private set { SetValue(ClipRadiusProperty, value); }
        }

        public Rect ClipRect
        {
            get { return (Rect)GetValue(ClipRectProperty); }
            private set { SetValue(ClipRectProperty, value); }
        }

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

        public SolidColorBrush WaterMarkForeground
        {
            get { return (SolidColorBrush)GetValue(WaterMarkForegroundProperty); }
            set { SetValue(WaterMarkForegroundProperty, value); }
        }

        public Visibility WaterMarkVisibility
        {
            get { return (Visibility)GetValue(WaterMarkVisibilityProperty); }
            set { SetValue(WaterMarkVisibilityProperty, value); }
        }

        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            private set { SetValue(TextProperty, value); }
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

        public string DateFormat
        {
            get { return (string)GetValue(DateFormatProperty); }
            set { SetValue(DateFormatProperty, value); }
        }

        public double ButtonWidth
        {
            get { return (double)GetValue(ButtonWidthProperty); }
            set { SetValue(ButtonWidthProperty, value); }
        }

        public SolidColorBrush ButtonColor
        {
            get { return (SolidColorBrush)GetValue(ButtonColorProperty); }
            set { SetValue(ButtonColorProperty, value); }
        }

        public double ButtonImageWidth
        {
            get { return (double)GetValue(ButtonImageWidthProperty); }
            set { SetValue(ButtonImageWidthProperty, value); }
        }

        public double ButtonImageHeight
        {
            get { return (double)GetValue(ButtonImageHeightProperty); }
            set { SetValue(ButtonImageHeightProperty, value); }
        }

        public Rect CalendarClipRect
        {
            get { return (Rect)GetValue(CalendarClipRectProperty); }
            private set { SetValue(CalendarClipRectProperty, value); }
        }

        /// <summary>
        /// MinCalendarSize = 300
        /// </summary>
        public double CalendarSize
        {
            get { return (double)GetValue(CalendarSizeProperty); }
            set { SetValue(CalendarSizeProperty, value); }
        }

        public bool CalendarStaysOpen
        {
            get { return (bool)GetValue(CalendarStaysOpenProperty); }
            set { SetValue(CalendarStaysOpenProperty, value); }
        }

        public bool IsCalendarOpen
        {
            get { return (bool)GetValue(IsCalendarOpenProperty); }
            set { SetValue(IsCalendarOpenProperty, value); }
        }

        public ColorSet_DatePicker ColorSet
        {
            get { return (ColorSet_DatePicker)GetValue(ColorSetProperty); }
            set { SetValue(ColorSetProperty, value); }
        }

        public SolidColorBrush BorderBrushDefault
        {
            get { return (SolidColorBrush)GetValue(BorderBrushDefaultProperty); }
            set { SetValue(BorderBrushDefaultProperty, value); }
        }

        private static void WidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            double value = (double)e.NewValue;

            Rect setRect = instance.ClipRect;
            setRect.Width = value;

            instance.ClipRect = setRect;
        }

        private static void HeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            double value = (double)e.NewValue;

            Rect setRect = instance.ClipRect;
            setRect.Height = value;

            instance.ClipRect = setRect;
        }

        private static void IsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            bool value = (bool)e.NewValue;

            if (value)
            {
                ApplyColorSet(instance, EMouseState.Default);
            }
            else
            {
                ApplyColorSet(instance, EMouseState.Disabled);
            }
        }

        private static void CornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            CornerRadius value = (CornerRadius)e.NewValue;

            instance.ClipRadius = value.TopRight + 1.0;
        }

        private static void SelectedDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            if (e.NewValue is DateTime dateTime)
            {
                if (dateTime == null) return;

                if (dateTime == DateTime.MinValue)
                {
                    instance.Text = "";
                }
                else
                {
                    instance.Text = dateTime.ToString(StringFormat);
                }
                
                instance.IsCalendarOpen = false;
            }

            // SelectedDateChanged Event 발생
            instance.SelectedDateChanged?.Invoke(instance, EventArgs.Empty);
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            string value = (string)e.NewValue;

            if (value == "")
            {
                instance.WaterMarkVisibility = Visibility.Visible;
            }
            else
            {
                instance.WaterMarkVisibility = Visibility.Collapsed;
            }
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            FontSet value = (FontSet)e.NewValue;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;
        }

        private static void DateFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            string value = (string)e.NewValue;

            StringFormat = value;

            if (instance.SelectedDate is DateTime selectedDate)
            {
                if (selectedDate == null) return;

                if (selectedDate == DateTime.MinValue)
                {
                    instance.WaterMark = StringFormat;
                    return;
                }

                instance.Text = selectedDate.ToString(StringFormat);
            }
        }

        private static void CalendarSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            double value = (double)e.NewValue;

            if (value < MinCalendarSize)
            {
                instance.CalendarSize = MinCalendarSize;
                return;
            }

            Rect setRect = instance.ClipRect;
            setRect.Width = value;
            setRect.Height = value;

            instance.CalendarClipRect = setRect;
        }

        private static void ColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexDatePicker instance = d as MexDatePicker;

            ColorSet_DatePicker value = (ColorSet_DatePicker)e.NewValue;

            if (value == null) return;

            ApplyColorSet(instance);
        }

        static MexDatePicker()
        {
            WidthProperty.OverrideMetadata(typeof(MexDatePicker), new FrameworkPropertyMetadata(new PropertyChangedCallback(WidthPropertyChanged)));
            HeightProperty.OverrideMetadata(typeof(MexDatePicker), new FrameworkPropertyMetadata(40.0, new PropertyChangedCallback(HeightPropertyChanged)));
            BorderThicknessProperty.OverrideMetadata(typeof(MexDatePicker), new FrameworkPropertyMetadata(new Thickness(1)));
            PaddingProperty.OverrideMetadata(typeof(MexDatePicker), new FrameworkPropertyMetadata(new Thickness(12, 0, 0, 0)));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(MexDatePicker), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(MexDatePicker), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            IsEnabledProperty.OverrideMetadata(typeof(MexDatePicker), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsEnabledPropertyChanged)));

            ClipRadiusProperty = DependencyProperty.Register("ClipRadius", typeof(double), typeof(MexDatePicker), new FrameworkPropertyMetadata(9.0));
            ClipRectProperty = DependencyProperty.Register("ClipRect", typeof(Rect), typeof(MexDatePicker), new FrameworkPropertyMetadata());
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexDatePicker), new FrameworkPropertyMetadata(new CornerRadius(8), new PropertyChangedCallback(CornerRadiusPropertyChanged)));

            WaterMarkProperty = DependencyProperty.Register("WaterMark", typeof(string), typeof(MexDatePicker), new FrameworkPropertyMetadata(StringFormat));
            WaterMarkForegroundProperty = DependencyProperty.Register("WaterMarkForeground", typeof(SolidColorBrush), typeof(MexDatePicker), new FrameworkPropertyMetadata(ResColor.text_placeholder));
            WaterMarkVisibilityProperty = DependencyProperty.Register("WaterMarkVisibility", typeof(Visibility), typeof(MexDatePicker), new FrameworkPropertyMetadata(Visibility.Visible));

            SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(MexDatePicker), new FrameworkPropertyMetadata(DateTime.MinValue, new PropertyChangedCallback(SelectedDatePropertyChanged)));
            DateFormatProperty = DependencyProperty.Register("DateFormat", typeof(string), typeof(MexDatePicker), new FrameworkPropertyMetadata(StringFormat, new PropertyChangedCallback(DateFormatPropertyChanged)));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexDatePicker), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(TextPropertyChanged)));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexDatePicker), new FrameworkPropertyMetadata(ResFontSet.body_md_regular, new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexDatePicker), new FrameworkPropertyMetadata(24.0));

            ButtonWidthProperty = DependencyProperty.Register("ButtonWidth", typeof(double), typeof(MexDatePicker), new FrameworkPropertyMetadata(48.0));
            ButtonColorProperty = DependencyProperty.Register("ButtonColor", typeof(SolidColorBrush), typeof(MexDatePicker), new FrameworkPropertyMetadata(ResColor.surface_action));
            ButtonImageWidthProperty = DependencyProperty.Register("ButtonImageWidth", typeof(double), typeof(MexDatePicker), new FrameworkPropertyMetadata(24.0));
            ButtonImageHeightProperty = DependencyProperty.Register("ButtonImageHeight", typeof(double), typeof(MexDatePicker), new FrameworkPropertyMetadata(24.0));

            CalendarClipRectProperty = DependencyProperty.Register("CalendarClipRect", typeof(Rect), typeof(MexDatePicker), new FrameworkPropertyMetadata());
            CalendarSizeProperty = DependencyProperty.Register("CalendarSize", typeof(double), typeof(MexDatePicker), new FrameworkPropertyMetadata(300.0, new PropertyChangedCallback(CalendarSizePropertyChanged))); ;
            CalendarStaysOpenProperty = DependencyProperty.Register("CalendarStaysOpen", typeof(bool), typeof(MexDatePicker), new FrameworkPropertyMetadata(false));
            IsCalendarOpenProperty = DependencyProperty.Register("IsCalendarOpen", typeof(bool), typeof(MexDatePicker), new FrameworkPropertyMetadata(false));

            ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(ColorSet_DatePicker), typeof(MexDatePicker), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ColorSetPropertyChanged)));
            BorderBrushDefaultProperty = DependencyProperty.Register("BorderBrushDefault", typeof(SolidColorBrush), typeof(MexDatePicker), new FrameworkPropertyMetadata());
        }
        #endregion

        public enum EMouseState
        {
            Default, Hover, Selected, Disabled
        }

        public MexDatePicker()
        {
            Loaded += MexDatePicker_Loaded;
            SizeChanged += MexDatePicker_SizeChanged;

            MouseLeave += MexDatePicker_MouseLeave;
            MouseEnter += MexDatePicker_MouseEnter;
            MouseLeftButtonDown += MexDatePicker_MouseLeftButtonDown;
            MouseLeftButtonUp += MexDatePicker_MouseLeftButtonUp;
            MouseRightButtonUp += MexDatePicker_MouseRightButtonUp;
        }

        private void MexDatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            ClipRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
            CalendarClipRect = new Rect(0, 0, this.CalendarSize, this.CalendarSize);

            ApplyColorSet(this);
        }

        private void MexDatePicker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClipRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
        }

        private void MexDatePicker_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CalendarStaysOpen = false;

            ApplyColorSet(this, EMouseState.Default);
        }

        private void MexDatePicker_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CalendarStaysOpen = true;

            ApplyColorSet(this, EMouseState.Hover);
        }

        private void MexDatePicker_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplyColorSet(this, EMouseState.Selected);
        }

        private void MexDatePicker_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsCalendarOpen = !IsCalendarOpen;

            ApplyColorSet(this, EMouseState.Default);
        }

        private void MexDatePicker_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SelectedDate = DateTime.MinValue;
        }

        private static void ApplyColorSet(MexDatePicker instance, EMouseState eMouseState = EMouseState.Default)
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
                    instance.ButtonColor = instance.ColorSet.ButtonDefault;
                }
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
                    instance.ButtonColor = instance.ColorSet.ButtonHover;
                }
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
                    instance.ButtonColor = instance.ColorSet.ButtonSelected;
                }
            }
            else //(eMouseState == EMouseState.Disabled)
            {
                if (instance.ColorSet != null)
                {
                    instance.BorderBrush = instance.ColorSet.BorderDisabled;
                    instance.Background = instance.ColorSet.BackgroundDisabled;
                    instance.Foreground = instance.ColorSet.ForegroundDisabled;
                    instance.WaterMarkForeground = instance.ColorSet.WaterMarkForegroundDisabled;
                    instance.ButtonColor = instance.ColorSet.ButtonDisabled;
                }
            }
        }
    }
}
