using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public class MexComboBoxHB : ContentControl
    {
        public DelegateCommand OnIsOpenItemBoxChangedCommand { get; set; }
        public DelegateCommand ItemBoxMouseLeaveCommand { get; set; }
        public DelegateCommand ItemBoxMouseEnterCommand { get; set; }
        public DelegateCommand ComponentLoadedCommand { get; set; }

        public event EventHandler SelectedIndexChanged;
        public event EventHandler SelectedItemChanged;

        #region Property
        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty WaterMarkProperty;
        public static readonly DependencyProperty WaterMarkForgroundProperty;
        public static readonly DependencyProperty WaterMarkVisibilityProperty;

        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;

        public static readonly DependencyProperty IsOpenItemBoxProperty;
        public static readonly DependencyProperty StaysOpenProperty;

        public static readonly DependencyProperty ItemBoxHeightProperty;
        public static readonly DependencyProperty ItemBoxBorderBrushProperty;
        public static readonly DependencyProperty ItemBoxBackgroundProperty;
        public static readonly DependencyProperty ItemBoxForgroundProperty;

        public static readonly DependencyProperty ItemSourceHeightProperty;
        public static readonly DependencyProperty ItemSourceViewCountProperty;
        public static readonly DependencyProperty ItemSourceProperty;
        public static readonly DependencyProperty ItemSourceInternalProperty;
        public static readonly DependencyProperty ItemSourcePaddingProperty;
        public static readonly DependencyProperty ItemSourceFontSetProperty;

        public static readonly DependencyProperty SelectedIndexProperty;
        public static readonly DependencyProperty SelectedItemProperty;

        public static readonly DependencyProperty ImageSizeDropDownProperty;
        public static readonly DependencyProperty ImageDropDownProperty;
        public static readonly DependencyProperty ImageSetDropDownProperty;

        public static readonly DependencyProperty ColorSetProperty;

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
            set { SetValue(WaterMarkVisibilityProperty, value); }
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

        public bool IsOpenItemBox
        {
            get { return (bool)GetValue(IsOpenItemBoxProperty); }
            set { SetValue(IsOpenItemBoxProperty, value); }
        }

        public bool StaysOpen
        {
            get { return (bool)GetValue(StaysOpenProperty); }
            set { SetValue(StaysOpenProperty, value); }
        }

        public double ItemBoxHeight
        {
            get { return (double)GetValue(ItemBoxHeightProperty); }
            set { SetValue(ItemBoxHeightProperty, value); }
        }

        public SolidColorBrush ItemBoxBorderBrush
        {
            get { return (SolidColorBrush)GetValue(ItemBoxBorderBrushProperty); }
            set { SetValue(ItemBoxBorderBrushProperty, value); }
        }

        public SolidColorBrush ItemBoxBackground
        {
            get { return (SolidColorBrush)GetValue(ItemBoxBackgroundProperty); }
            set { SetValue(ItemBoxBackgroundProperty, value); }
        }

        public SolidColorBrush ItemBoxForground
        {
            get { return (SolidColorBrush)GetValue(ItemBoxForgroundProperty); }
            set { SetValue(ItemBoxForgroundProperty, value); }
        }

        public double ItemSourceHeight
        {
            get { return (double)GetValue(ItemSourceHeightProperty); }
            set { SetValue(ItemSourceHeightProperty, value); }
        }

        public int ItemSourceViewCount
        {
            get { return (int)GetValue(ItemSourceViewCountProperty); }
            set { SetValue(ItemSourceViewCountProperty, value); }
        }

        public List<string> ItemSource
        {
            get { return (List<string>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public List<string> ItemSourceInternal
        {
            get { return (List<string>)GetValue(ItemSourceInternalProperty); }
            set { SetValue(ItemSourceInternalProperty, value); }
        }

        public Thickness ItemSourcePadding
        {
            get { return (Thickness)GetValue(ItemSourcePaddingProperty); }
            set { SetValue(ItemSourcePaddingProperty, value); }
        }

        public FontSet ItemSourceFontSet
        {
            get { return (FontSet)GetValue(ItemSourceFontSetProperty); }
            set { SetValue(ItemSourceFontSetProperty, value); }
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

        public double ImageSizeDropDown
        {
            get { return (double)GetValue(ImageSizeDropDownProperty); }
            set { SetValue(ImageSizeDropDownProperty, value); }
        }

        public BitmapSource ImageDropDown
        {
            get { return (BitmapSource)GetValue(ImageDropDownProperty); }
            set { SetValue(ImageDropDownProperty, value); }
        }

        public ImageSet_Toggle ImageSetDropDown
        {
            get { return (ImageSet_Toggle)GetValue(ImageSetDropDownProperty); }
            set { SetValue(ImageSetDropDownProperty, value); }
        }

        public ColorSet_ComboBox ColorSet
        {
            get { return (ColorSet_ComboBox)GetValue(ColorSetProperty); }
            set { SetValue(ColorSetProperty, value); }
        }

        private static void WaterMarkPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB instance = d as MexComboBoxHB;

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
            MexComboBoxHB instance = d as MexComboBoxHB;

            string value = (string)e.NewValue;

            if (value == "" || value == null)
            {
                instance.WaterMarkVisibility = Visibility.Visible;
            }
            else
            {
                instance.WaterMarkVisibility = Visibility.Hidden;
            }
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB instance = d as MexComboBoxHB;

            FontSet value = (FontSet)e.NewValue;

            if (value == null) return;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;

            instance.ItemSourceFontSet = value;
        }

        private static void ImageSetDropDownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB instance = d as MexComboBoxHB;

            ImageSet_Toggle value = (ImageSet_Toggle)e.NewValue;

            if (value == null) return;

            instance.ImageDropDown = (instance.IsOpenItemBox) ? value.ImageTrue : value.ImageFalse;
        }

        private static void IsOpenItemBoxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB instance = d as MexComboBoxHB;

            bool value = (bool)e.NewValue;

            instance.ImageDropDown = (value) ? instance.ImageSetDropDown.ImageTrue : instance.ImageSetDropDown.ImageFalse;

            ApplyColorSet(instance, EMouseState.Selected);
        }

        private static void ItemSourceHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB instance = d as MexComboBoxHB;

            double value = (double)e.NewValue;

            instance.ItemBoxHeight = instance.ItemSourceViewCount * value;
        }

        private static void ItemSourceViewCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB instance = d as MexComboBoxHB;

            int value = (int)e.NewValue;

            instance.ItemBoxHeight = instance.ItemSourceHeight * value;
        }

        private static void ItemSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB instance = d as MexComboBoxHB;

            List<string> value = e.NewValue as List<string>;

            instance.ItemSourceInternal = value;
        }

        private static void SelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB instance = d as MexComboBoxHB;

            if (e.NewValue == e.OldValue) return;
            if (instance.IsOpenItemBox) instance.IsOpenItemBox = false;

            if (e.NewValue is int selectedIndex)
            {
                if (selectedIndex < 0)
                {
                    if (instance.SelectedItem != "")
                    {
                        instance.SelectedItem = "";
                    }
                    return;
                }

                // SelectedIndexChanged Event 발생
                instance.SelectedIndexChanged?.Invoke(instance, EventArgs.Empty);
            }
            else
            {
                if (instance.SelectedItem != "")
                {
                    instance.SelectedItem = "";
                }
            }
        }

        private static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB instance = d as MexComboBoxHB;

            if (e.NewValue == e.OldValue) return;
            if (instance.IsOpenItemBox) instance.IsOpenItemBox = false;

            if (e.NewValue is string selectedItem)
            {
                if (selectedItem == "" || selectedItem.ToLower() == "null")
                {
                    instance.Text = "";

                    if (!(instance.SelectedIndex < 0))
                    {
                        instance.SelectedIndex = -1;
                    }
                    return;
                }

                // SelectedItemChanged Event 발생
                if (instance.Text != selectedItem)
                {
                    instance.Text = selectedItem;

                    instance.SelectedItemChanged?.Invoke(instance, EventArgs.Empty);
                }
            }
            else
            {
                instance.Text = "";

                if (!(instance.SelectedIndex < 0))
                {
                    instance.SelectedIndex = -1;
                }
            }
        }

        private static void ColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexComboBoxHB mexComboBox = d as MexComboBoxHB;

            ApplyColorSet(mexComboBox);
        }

        static MexComboBoxHB()
        {
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());

            WaterMarkProperty = DependencyProperty.Register("WaterMark", typeof(string), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(WaterMarkPropertyChanged)));
            WaterMarkForgroundProperty = DependencyProperty.Register("WaterMarkForground", typeof(SolidColorBrush), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());
            WaterMarkVisibilityProperty = DependencyProperty.Register("WaterMarkVisibility", typeof(Visibility), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());

            ImageSizeDropDownProperty = DependencyProperty.Register("ImageSizeDropDown", typeof(double), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());
            ImageDropDownProperty = DependencyProperty.Register("ImageDropDown", typeof(BitmapSource), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());
            ImageSetDropDownProperty = DependencyProperty.Register("ImageSetDropDown", typeof(ImageSet_Toggle), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(ImageSetDropDownPropertyChanged)));

            IsOpenItemBoxProperty = DependencyProperty.Register("IsOpenItemBox", typeof(bool), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsOpenItemBoxPropertyChanged)));
            StaysOpenProperty = DependencyProperty.Register("StaysOpen", typeof(bool), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());

            ItemBoxHeightProperty = DependencyProperty.Register("ItemBoxHeight", typeof(double), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());
            ItemBoxBorderBrushProperty = DependencyProperty.Register("ItemBoxBorderBrush", typeof(SolidColorBrush), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());
            ItemBoxBackgroundProperty = DependencyProperty.Register("ItemBoxBackground", typeof(SolidColorBrush), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());
            ItemBoxForgroundProperty = DependencyProperty.Register("ItemBoxForground", typeof(SolidColorBrush), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());

            ItemSourceHeightProperty = DependencyProperty.Register("ItemSourceHeight", typeof(double), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(ItemSourceHeightPropertyChanged)));
            ItemSourceViewCountProperty = DependencyProperty.Register("ItemSourceViewCount", typeof(int), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(ItemSourceViewCountPropertyChanged)));
            ItemSourceProperty = DependencyProperty.Register("ItemSource", typeof(List<string>), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new List<string>(), new PropertyChangedCallback(ItemSourcePropertyChanged)));
            ItemSourceInternalProperty = DependencyProperty.Register("ItemSourceInternal", typeof(List<string>), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new List<string>()));
            ItemSourcePaddingProperty = DependencyProperty.Register("ItemSourcePadding", typeof(Thickness), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());
            ItemSourceFontSetProperty = DependencyProperty.Register("ItemSourceFontSet", typeof(FontSet), typeof(MexComboBoxHB), new FrameworkPropertyMetadata());

            SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedIndexPropertyChanged)));
            SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedItemPropertyChanged)));

            ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(ColorSet_ComboBox), typeof(MexComboBoxHB), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorSetPropertyChanged)));
        }
        #endregion

        public enum EMouseState
        {
            Default, Hover, Selected, Disabled
        }

        public MexComboBoxHB()
        {
            DataContext = this;

            OnIsOpenItemBoxChangedCommand = new DelegateCommand(OnIsOpenItemBoxChanged);
            ItemBoxMouseLeaveCommand = new DelegateCommand(ItemBoxMouseLeave);
            ItemBoxMouseEnterCommand = new DelegateCommand(ItemBoxMouseEnter);
            ComponentLoadedCommand = new DelegateCommand(ComponentLoaded);

            // property default value
            BorderThickness = new Thickness(1);
            CornerRadius = new CornerRadius(8);

            WaterMark = "Water Mark";
            WaterMarkVisibility = Visibility.Visible;

            Text = "";
            FontSet = ResFontSet.body_md_regular;

            ImageSizeDropDown = 24;
            ImageSetDropDown = ResImageSet_Toggle.DropdownOpen;

            IsOpenItemBox = false;
            StaysOpen = true;

            ItemBoxHeight = 192;

            ItemSourceHeight = 48;
            ItemSourceViewCount = 4;
            ItemSourcePadding = new Thickness(16, 12, 16, 12);
            FontSet = ResFontSet.body_md_medium;

            SelectedIndex = -1;
            SelectedItem = "";

            ColorSet = ResColorSet_ComboBox.Primary;

            Padding = new Thickness(12, 8, 12, 8);
            HorizontalContentAlignment = HorizontalAlignment.Left;
            VerticalContentAlignment = VerticalAlignment.Center;

            // event
            MouseEnter += MexComboBox_MouseEnter;
            MouseLeave += MexComboBox_MouseLeave;
        }

        private void ComponentLoaded()
        {
            MexItemBox mexItemBox = this.Template.FindName("xItemBox", this) as MexItemBox;

            mexItemBox.ItemSource = this.ItemSourceInternal;
        }

        private void OnIsOpenItemBoxChanged()
        {
            IsOpenItemBox = !IsOpenItemBox;
        }

        private void ItemBoxMouseLeave()
        {
            StaysOpen = false;
        }

        private void ItemBoxMouseEnter()
        {
            StaysOpen = true;
        }

        private void MexComboBox_MouseEnter(object sender, MouseEventArgs e)
        {
            MexComboBoxHB instance = sender as MexComboBoxHB;

            ApplyColorSet(instance, EMouseState.Hover);
        }

        private void MexComboBox_MouseLeave(object sender, MouseEventArgs e)
        {
            MexComboBoxHB instance = sender as MexComboBoxHB;
            
            if (instance.IsOpenItemBox)
            {
                ApplyColorSet(instance, EMouseState.Selected);
            }
            else
            {
                ApplyColorSet(instance, EMouseState.Default);
            }
        }

        private static void ApplyColorSet(MexComboBoxHB mexComboBox, EMouseState eMouseState = EMouseState.Default)
        {
            if (eMouseState == EMouseState.Default)
            {
                mexComboBox.BorderBrush = mexComboBox.ColorSet.BorderDefault;
                mexComboBox.Background = mexComboBox.ColorSet.BackgroundDefault;
                mexComboBox.Foreground = mexComboBox.ColorSet.ForegroundDefault;
                mexComboBox.WaterMarkForground = mexComboBox.ColorSet.WaterMarkForegroundDefault;

                mexComboBox.ItemBoxBorderBrush = mexComboBox.ColorSet.ItemBox.BorderDefault;
                mexComboBox.ItemBoxBackground = mexComboBox.ColorSet.ItemBox.BackgroundDefault;
                mexComboBox.ItemBoxForground = mexComboBox.ColorSet.ItemBox.ForegroundDefault;
            }
            else if (eMouseState == EMouseState.Hover)
            {
                mexComboBox.BorderBrush = mexComboBox.ColorSet.BorderHover;
                mexComboBox.Background = mexComboBox.ColorSet.BackgroundHover;
                mexComboBox.Foreground = mexComboBox.ColorSet.ForegroundHover;
                mexComboBox.WaterMarkForground = mexComboBox.ColorSet.WaterMarkForegroundHover;

                mexComboBox.ItemBoxBorderBrush = mexComboBox.ColorSet.ItemBox.BorderHover;
                mexComboBox.ItemBoxBackground = mexComboBox.ColorSet.ItemBox.BackgroundHover;
                mexComboBox.ItemBoxForground = mexComboBox.ColorSet.ItemBox.ForegroundHover;
            }
            else if (eMouseState == EMouseState.Selected)
            {
                mexComboBox.BorderBrush = mexComboBox.ColorSet.BorderSelected;
                mexComboBox.Background = mexComboBox.ColorSet.BackgroundSelected;
                mexComboBox.Foreground = mexComboBox.ColorSet.ForegroundSelected;
                mexComboBox.WaterMarkForground = mexComboBox.ColorSet.WaterMarkForegroundSelected;

                mexComboBox.ItemBoxBorderBrush = mexComboBox.ColorSet.ItemBox.BorderDefault;
                mexComboBox.ItemBoxBackground = mexComboBox.ColorSet.ItemBox.BackgroundDefault;
                mexComboBox.ItemBoxForground = mexComboBox.ColorSet.ItemBox.ForegroundDefault;
            }
            else //(eMouseState == EMouseState.Disabled)
            {
                mexComboBox.BorderBrush = mexComboBox.ColorSet.BorderDisabled;
                mexComboBox.Background = mexComboBox.ColorSet.BackgroundDisabled;
                mexComboBox.Foreground = mexComboBox.ColorSet.ForegroundDisabled;
                mexComboBox.WaterMarkForground = mexComboBox.ColorSet.WaterMarkForegroundDisabled;

                mexComboBox.ItemBoxBorderBrush = mexComboBox.ColorSet.ItemBox.BorderDisabled;
                mexComboBox.ItemBoxBackground = mexComboBox.ColorSet.ItemBox.BackgroundDisabled;
                mexComboBox.ItemBoxForground = mexComboBox.ColorSet.ItemBox.ForegroundDisabled;
            }
        }
    }
}
