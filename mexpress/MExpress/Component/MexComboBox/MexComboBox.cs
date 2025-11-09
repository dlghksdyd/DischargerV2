using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace MExpress.Mex
{
    public class MexComboBox : Control
    {
        #region Event
        public event EventHandler SelectedIndexChanged;
        public event EventHandler SelectedItemChanged;
        #endregion

        #region Command
        public DelegateCommand OpenDropDownCommand { get; set; }
        public DelegateCommand ComboBoxMouseLeaveCommand { get; set; }
        public DelegateCommand ComboBoxMouseEnterCommand { get; set; }
        #endregion

        #region Property
        public static readonly DependencyProperty ClipRadiusProperty;
        public static readonly DependencyProperty ClipRectProperty;
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty IsDropDownOpenProperty;
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty WaterMarkProperty;
        public static readonly DependencyProperty WaterMarkForegroundProperty;
        public static readonly DependencyProperty WaterMarkVisibilityProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;
        public static readonly DependencyProperty ItemSourceProperty;
        public static readonly DependencyProperty ItemSourceInternalProperty;
        public static readonly DependencyProperty StaysOpenProperty;
        public static readonly DependencyProperty DropDownImageSizeProperty;
        public static readonly DependencyProperty DropDownImageSetProperty;
        public static readonly DependencyProperty DropDownImageProperty;
        public static readonly DependencyProperty DropDownMaxHeightProperty;
        public static readonly DependencyProperty ColorSetProperty;
        public static readonly DependencyProperty ColorSetItemProperty;
        public static readonly DependencyProperty SelectedIndexProperty;
        public static readonly DependencyProperty SelectedItemProperty;
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

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
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

        public SolidColorBrush WaterMarkForground
        {
            get { return (SolidColorBrush)GetValue(WaterMarkForegroundProperty); }
            set { SetValue(WaterMarkForegroundProperty, value); }
        }

        public Visibility WaterMarkVisibility
        {
            get { return (Visibility)GetValue(WaterMarkVisibilityProperty); }
            set { SetValue(WaterMarkVisibilityProperty, value); }
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

        public List<string> ItemSource
        {
            get { return (List<string>)GetValue(ItemSourceProperty); }
            set
            {
                SetValue(ItemSourceProperty, value);
            }
        }

        public bool StaysOpen
        {
            get { return (bool)GetValue(StaysOpenProperty); }
            set { SetValue(StaysOpenProperty, value); }
        }

        public double DropDownImageSize
        {
            get { return (double)GetValue(DropDownImageSizeProperty); }
            set { SetValue(DropDownImageSizeProperty, value); }
        }

        public ImageSet_Toggle DropDownImageSet
        {
            get { return (ImageSet_Toggle)GetValue(DropDownImageSetProperty); }
            set { SetValue(DropDownImageSetProperty, value); }
        }

        public BitmapSource DropDownImage
        {
            get { return (BitmapSource)GetValue(DropDownImageProperty); }
            set { SetValue(DropDownImageProperty, value); }
        }

        public double DropDownMaxHeight
        {
            get { return (double)GetValue(DropDownMaxHeightProperty); }
            set { SetValue(DropDownMaxHeightProperty, value); }
        }

        public ColorSet_ComboBox ColorSet
        {
            get { return (ColorSet_ComboBox)GetValue(ColorSetProperty); }
            set { SetValue(ColorSetProperty, value); }
        }

        public ColorSet_ComboBoxItem ColorSetItem
        {
            get { return (ColorSet_ComboBoxItem)GetValue(ColorSetItemProperty); }
            set { SetValue(ColorSetItemProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public string SelectedItem
        {
            get { return (string)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public SolidColorBrush BorderBrushDefault
        {
            get { return (SolidColorBrush)GetValue(BorderBrushDefaultProperty); }
            set { SetValue(BorderBrushDefaultProperty, value); }
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBox instance = d as MexComboBox;

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

        private static void IsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBox instance = d as MexComboBox;

            instance.SetComboBoxColor(EColorState.Default);
        }

        private static void CornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBox instance = d as MexComboBox;

            CornerRadius value = (CornerRadius)e.NewValue;

            instance.ClipRadius = value.TopRight + 1.0;
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBox instance = d as MexComboBox;

            FontSet value = (FontSet)e.NewValue;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;

            instance.ChangeFontSetInDropDownTextBlock();
        }

        private static void ItemSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBox instance = d as MexComboBox;

            List<string> temp = e.NewValue as List<string>;

            if (instance.ItemSource.SequenceEqual(temp) == false)
            {
                instance.ItemSource = temp;
            }

            instance.MakeDropdownComponents(instance.ItemSource.ToList());
        }

        private static void DropDownImageSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBox instance = d as MexComboBox;

            ImageSet_Toggle value = (ImageSet_Toggle)e.NewValue;

            if (value != null)
            {
                instance.DropDownImage = instance.DropDownImageSet.ImageFalse;
            }
        }

        private static void ColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBox instance = d as MexComboBox;

            ColorSet_ComboBox value = (ColorSet_ComboBox)e.NewValue;

            if (value != null)
            {
                instance.SetComboBoxColor(EColorState.Default);
                instance.ChangeBorderBrushInDropDownRectangle();
            }
        }

        private static void SelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBox instance = d as MexComboBox;

            int value = (int)e.NewValue;

            if (value == -1)
            {
                instance.SelectedItem = "";
                instance.Text = "";
            }
            else
            {
                instance.SelectedItem = instance.ItemSource[value];
                instance.Text = instance.ItemSource[value];
            }

            instance.SelectedIndexChanged?.Invoke(instance, EventArgs.Empty);
        }

        private static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBox instance = d as MexComboBox;

            string value = (string)e.NewValue;

            instance.SelectedIndex = instance.ItemSource.ToList().FindIndex(x => x == value);

            if (instance.SelectedIndex == -1)
            {
                instance.Text = "";
            }
            else
            {
                instance.Text = value;
            }

            instance.SelectedItemChanged?.Invoke(instance, EventArgs.Empty);
        }

        static MexComboBox()
        {
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(MexComboBox), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(MexComboBox), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            PaddingProperty.OverrideMetadata(typeof(MexComboBox), new FrameworkPropertyMetadata(new Thickness(12, 8, 12, 8)));
            BorderBrushProperty.OverrideMetadata(typeof(MexComboBox), new FrameworkPropertyMetadata(ResColor.border_primary));
            BorderThicknessProperty.OverrideMetadata(typeof(MexComboBox), new FrameworkPropertyMetadata(new Thickness(1)));
            FontSizeProperty.OverrideMetadata(typeof(MexComboBox), new FrameworkPropertyMetadata(ResFontSet.body_md_regular.FontSize));
            FontWeightProperty.OverrideMetadata(typeof(MexComboBox), new FrameworkPropertyMetadata(ResFontSet.body_md_regular.FontWeight));
            IsEnabledProperty.OverrideMetadata(typeof(MexComboBox), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(IsEnabledPropertyChanged)));

            ClipRadiusProperty = DependencyProperty.Register("ClipRadius", typeof(double), typeof(MexComboBox), new FrameworkPropertyMetadata());
            ClipRectProperty = DependencyProperty.Register("ClipRect", typeof(Rect), typeof(MexComboBox), new FrameworkPropertyMetadata());
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexComboBox), new FrameworkPropertyMetadata("", new PropertyChangedCallback(TextPropertyChanged)));
            IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(MexComboBox), new FrameworkPropertyMetadata(false));
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexComboBox), new FrameworkPropertyMetadata(ResCornerRadius.radius_lg, new PropertyChangedCallback(CornerRadiusPropertyChanged)));
            WaterMarkProperty = DependencyProperty.Register("WaterMark", typeof(string), typeof(MexComboBox), new FrameworkPropertyMetadata(""));
            WaterMarkForegroundProperty = DependencyProperty.Register("WaterMarkForeground", typeof(SolidColorBrush), typeof(MexComboBox), new FrameworkPropertyMetadata(ResColor.text_placeholder));
            WaterMarkVisibilityProperty = DependencyProperty.Register("WaterMarkVisibility", typeof(Visibility), typeof(MexComboBox), new FrameworkPropertyMetadata(Visibility.Visible));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexComboBox), new FrameworkPropertyMetadata(ResFontSet.body_md_regular, new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexComboBox), new FrameworkPropertyMetadata(24.0));
            ItemSourceProperty = DependencyProperty.Register("ItemSource", typeof(List<string>), typeof(MexComboBox), new FrameworkPropertyMetadata(new List<string>(), new PropertyChangedCallback(ItemSourcePropertyChanged)));
            ItemSourceInternalProperty = DependencyProperty.Register("ItemSourceInternal", typeof(List<string>), typeof(MexComboBox), new FrameworkPropertyMetadata(new List<string>()));
            StaysOpenProperty = DependencyProperty.Register("StaysOpen", typeof(bool), typeof(MexComboBox), new FrameworkPropertyMetadata(false));
            DropDownImageSizeProperty = DependencyProperty.Register("DropDownImageSize", typeof(double), typeof(MexComboBox), new FrameworkPropertyMetadata(24.0));
            DropDownImageSetProperty = DependencyProperty.Register("DropDownImageSet", typeof(ImageSet_Toggle), typeof(MexComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(DropDownImageSetPropertyChanged)));
            DropDownImageProperty = DependencyProperty.Register("DropDownImage", typeof(BitmapSource), typeof(MexComboBox), new FrameworkPropertyMetadata());
            DropDownMaxHeightProperty = DependencyProperty.Register("DropDownMaxHeight", typeof(double), typeof(MexComboBox), new FrameworkPropertyMetadata(200.0));
            ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(ColorSet_ComboBox), typeof(MexComboBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorSetPropertyChanged)));
            ColorSetItemProperty = DependencyProperty.Register("ColorSetItem", typeof(ColorSet_ComboBoxItem), typeof(MexComboBox), new FrameworkPropertyMetadata(ResColorSet_ComboBoxItem.Primary));
            SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(MexComboBox), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(SelectedIndexPropertyChanged)));
            SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(MexComboBox), new FrameworkPropertyMetadata("", new PropertyChangedCallback(SelectedItemPropertyChanged)));
            BorderBrushDefaultProperty = DependencyProperty.Register("BorderBrushDefault", typeof(SolidColorBrush), typeof(MexComboBox), new FrameworkPropertyMetadata());
        }

        #endregion

        public MexComboBox()
        {
            Loaded += MexComboBoxNew_Loaded;

            OpenDropDownCommand = new DelegateCommand(OpenDropDown);
            ComboBoxMouseLeaveCommand = new DelegateCommand(ComboBoxMouseLeave);
            ComboBoxMouseEnterCommand = new DelegateCommand(ComboBoxMouseEnter);
        }

        private void MexComboBoxNew_Loaded(object sender, RoutedEventArgs e)
        {
            if (DropDownImageSet == null)
            {
                DropDownImageSet = ResImageSet_Toggle.DropdownOpen;
            }
            if (ColorSet == null)
            {
                ColorSet = ResColorSet_ComboBox.Primary;
            }
            if (ItemSource == null)
            {
                ItemSource = new List<string>();
            }

            SetComboBoxColor(EColorState.Default);
        }

        private StackPanel _dropDownStackPanel = new StackPanel();
        private void MakeDropdownComponents(List<string> itemsSource)
        {
            double totalheight = 0;

            _dropDownStackPanel.Children.Clear();
            _dropDownStackPanel.Orientation = Orientation.Vertical;

            for (int i = 0; i < itemsSource.Count; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = itemsSource[i];
                textBlock.Width = this.Width;
                textBlock.Height = this.FontHeight + this.Padding.Top + this.Padding.Bottom;
                textBlock.Padding = this.Padding;
                textBlock.FontSize = this.FontSize;
                textBlock.FontWeight = this.FontWeight;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Loaded += TextBlock_Loaded;
                textBlock.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
                textBlock.MouseEnter += TextBlock_MouseEnter;
                textBlock.MouseLeave += TextBlock_MouseLeave;

                _dropDownStackPanel.Children.Add(textBlock);
                totalheight += textBlock.Height;

                if (i != (itemsSource.Count - 1))
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Width = this.Width;
                    rectangle.Height = 1;
                    rectangle.Fill = this.BorderBrushDefault;
                    rectangle.StrokeThickness = 0;

                    _dropDownStackPanel.Children.Add(rectangle);
                    totalheight += rectangle.Height;
                }
            }

            _dropDownStackPanel.Height = totalheight;
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if (textBlock.Text != SelectedItem)
            {
                SetTextBlockColorToDefault(textBlock);
            }
            else
            {
                SetTextBlockColorToSelected(textBlock);
            }
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if (textBlock.Text != SelectedItem)
            {
                SetTextBlockColorToHover(textBlock);
            }
            else
            {
                SetTextBlockColorToSelected(textBlock);
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            Text = textBlock.Text;

            SelectedIndex = ItemSource.ToList().FindIndex(x => x == Text);
            SelectedItem = Text;

            OpenDropDown();
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            if (textBlock.Text != SelectedItem)
            {
                SetTextBlockColorToDefault(textBlock);
            }
            else
            {
                SetTextBlockColorToSelected(textBlock);
            }
        }

        private void ChangeBorderBrushInDropDownRectangle()
        {
            foreach (var item in _dropDownStackPanel.Children)
            {
                if (item is Rectangle rectangle)
                {
                    rectangle.Fill = this.BorderBrushDefault;
                }
            }
        }

        private void ChangeFontSetInDropDownTextBlock()
        {
            foreach (var item in _dropDownStackPanel.Children)
            {
                if (item is TextBlock textBlock)
                {
                    textBlock.FontSize = this.FontSize;
                    textBlock.FontWeight = this.FontWeight;
                    textBlock.Height = this.FontHeight + this.Padding.Top + this.Padding.Bottom;
                }
            }
        }

        private void OpenDropDown()
        {
            if (IsDropDownOpen == false)
            {
                DropDownImage = DropDownImageSet.ImageTrue;
                SetClipRectAndClipRadius();
                InsertStackPanelInDropDown();
            }
            else
            {
                DropDownImage = DropDownImageSet.ImageFalse;
            }

            IsDropDownOpen = !IsDropDownOpen;
        }

        private void SetClipRectAndClipRadius()
        {
            Popup popup = this.Template.FindName("xPopup", this) as Popup;
            Rect setRect = ClipRect;
            setRect.Width = popup.Width;
            setRect.Height = _dropDownStackPanel.Height;
            ClipRect = setRect;

            ClipRadius = CornerRadius.TopRight + 1.0;
        }

        private void InsertStackPanelInDropDown()
        {
            MexScrollViewer scrollViewer = this.Template.FindName("xDropDownScrollViewer", this) as MexScrollViewer;

            scrollViewer.Content = _dropDownStackPanel;
        }

        private enum EColorState
        {
            Default,
            Hover,
            Selected,
        }

        private void SetComboBoxColor(EColorState colorState)
        {
            if (IsEnabled == true)
            {
                if (colorState == EColorState.Default)
                {
                    BorderBrush = ColorSet.BorderDefault;
                    BorderBrushDefault = ColorSet.BorderDefault;
                    Background = ColorSet.BackgroundDefault;
                    Foreground = ColorSet.ForegroundDefault;
                    WaterMarkForground = ColorSet.WaterMarkForegroundDefault;
                }
                else if (colorState == EColorState.Hover)
                {
                    BorderBrush = ColorSet.BorderHover;
                    Background = ColorSet.BackgroundHover;
                    Foreground = ColorSet.ForegroundHover;
                    WaterMarkForground = ColorSet.WaterMarkForegroundHover;
                }
                else if (colorState == EColorState.Selected)
                {
                    BorderBrush = ColorSet.BorderSelected;
                    Background = ColorSet.BackgroundSelected;
                    Foreground = ColorSet.ForegroundSelected;
                    WaterMarkForground = ColorSet.WaterMarkForegroundSelected;
                }
            }
            else
            {
                BorderBrush = ColorSet.BorderDisabled;
                Background = ColorSet.BackgroundDisabled;
                Foreground = ColorSet.ForegroundDisabled;
                WaterMarkForground = ColorSet.WaterMarkForegroundDisabled;
            }
        }

        private void ComboBoxMouseLeave()
        {
            StaysOpen = false;

            if (IsDropDownOpen == false)
            {
                SetComboBoxColor(EColorState.Default);
            }
        }

        private void ComboBoxMouseEnter()
        {
            StaysOpen = true;

            SetComboBoxColor(EColorState.Hover);
        }

        private void SetTextBlockColorToDefault(TextBlock textBlock)
        {
            textBlock.Background = ColorSetItem.BackgroundDefault;
            textBlock.Foreground = ColorSetItem.ForegroundDefault;
        }

        private void SetTextBlockColorToHover(TextBlock textBlock)
        {
            textBlock.Background = ColorSetItem.BackgroundHover;
            textBlock.Foreground = ColorSetItem.ForegroundHover;
        }

        private void SetTextBlockColorToSelected(TextBlock textBlock)
        {
            textBlock.Background = ColorSetItem.BackgroundSelected;
            textBlock.Foreground = ColorSetItem.ForegroundSelected;
        }
    }
}
