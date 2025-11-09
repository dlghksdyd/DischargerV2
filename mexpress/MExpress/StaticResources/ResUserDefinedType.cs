using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Prism.Mvvm;

namespace MExpress.Mex
{
    public class ColorSet_Button
    {
        public SolidColorBrush BorderDefault { get; set; }
        public SolidColorBrush BorderHover { get; set; }
        public SolidColorBrush BorderPressed { get; set; }
        public SolidColorBrush BorderDisabled { get; set; }

        public SolidColorBrush BackgroundDefault { get; set; }
        public SolidColorBrush BackgroundHover { get; set; }
        public SolidColorBrush BackgroundPressed { get; set; }
        public SolidColorBrush BackgroundDisabled { get; set; }

        public SolidColorBrush ForegroundDefault { get; set; }
        public SolidColorBrush ForegroundHover { get; set; }
        public SolidColorBrush ForegroundPressed { get; set; }
        public SolidColorBrush ForegroundDisabled { get; set; }
    }

    public class ColorSet_CheckBox
    {
        // Default
        public SolidColorBrush BorderDefault { get; set; }
        public SolidColorBrush BorderHover { get; set; }
        public SolidColorBrush BorderPressed { get; set; }
        public SolidColorBrush BorderDisabled { get; set; }

        public SolidColorBrush BackgroundDefault { get; set; }
        public SolidColorBrush BackgroundHover { get; set; }
        public SolidColorBrush BackgroundPressed { get; set; }
        public SolidColorBrush BackgroundDisabled { get; set; }

        public SolidColorBrush StrokeDefault { get; set; }
        public SolidColorBrush StrokeHover { get; set; }
        public SolidColorBrush StrokePressed { get; set; }
        public SolidColorBrush StrokeDisabled { get; set; }

        // Selected
        public SolidColorBrush BorderDefault_Selected { get; set; }
        public SolidColorBrush BorderHover_Selected { get; set; }
        public SolidColorBrush BorderPressed_Selected { get; set; }
        public SolidColorBrush BorderDisabled_Selected { get; set; }

        public SolidColorBrush BackgroundDefault_Selected { get; set; }
        public SolidColorBrush BackgroundHover_Selected { get; set; }
        public SolidColorBrush BackgroundPressed_Selected { get; set; }
        public SolidColorBrush BackgroundDisabled_Selected { get; set; }

        public SolidColorBrush StrokeDefault_Selected { get; set; }
        public SolidColorBrush StrokeHover_Selected { get; set; }
        public SolidColorBrush StrokePressed_Selected { get; set; }
        public SolidColorBrush StrokeDisabled_Selected { get; set; }
    }

    public class ColorSet_RadioButton
    {
        // Default
        public SolidColorBrush BorderDefault { get; set; }
        public SolidColorBrush BorderHover { get; set; }
        public SolidColorBrush BorderPressed { get; set; }
        public SolidColorBrush BorderDisabled { get; set; }

        public SolidColorBrush BackgroundDefault { get; set; }
        public SolidColorBrush BackgroundHover { get; set; }
        public SolidColorBrush BackgroundPressed { get; set; }
        public SolidColorBrush BackgroundDisabled { get; set; }

        public SolidColorBrush EllipseDefault { get; set; }
        public SolidColorBrush EllipseHover { get; set; }
        public SolidColorBrush EllipsePressed { get; set; }
        public SolidColorBrush EllipseDisabled { get; set; }

        // Selected
        public SolidColorBrush BorderDefault_Selected { get; set; }
        public SolidColorBrush BorderHover_Selected { get; set; }
        public SolidColorBrush BorderPressed_Selected { get; set; }
        public SolidColorBrush BorderDisabled_Selected { get; set; }

        public SolidColorBrush BackgroundDefault_Selected { get; set; }
        public SolidColorBrush BackgroundHover_Selected { get; set; }
        public SolidColorBrush BackgroundPressed_Selected { get; set; }
        public SolidColorBrush BackgroundDisabled_Selected { get; set; }

        public SolidColorBrush EllipseDefault_Selected { get; set; }
        public SolidColorBrush EllipseHover_Selected { get; set; }
        public SolidColorBrush EllipsePressed_Selected { get; set; }
        public SolidColorBrush EllipseDisabled_Selected { get; set; }
    }

    public class ColorSet_TextBox
    {
        public SolidColorBrush BorderDefault { get; set; }
        public SolidColorBrush BorderHover { get; set; }
        public SolidColorBrush BorderSelected { get; set; }
        public SolidColorBrush BorderDisabled { get; set; }

        public SolidColorBrush BackgroundDefault { get; set; }
        public SolidColorBrush BackgroundHover { get; set; }
        public SolidColorBrush BackgroundSelected { get; set; }
        public SolidColorBrush BackgroundDisabled { get; set; }

        public SolidColorBrush ForegroundDefault { get; set; }
        public SolidColorBrush ForegroundHover { get; set; }
        public SolidColorBrush ForegroundSelected { get; set; }
        public SolidColorBrush ForegroundDisabled { get; set; }

        public SolidColorBrush WaterMarkForegroundDefault { get; set; }
        public SolidColorBrush WaterMarkForegroundHover { get; set; }
        public SolidColorBrush WaterMarkForegroundSelected { get; set; }
        public SolidColorBrush WaterMarkForegroundDisabled { get; set; }
    }

    public class ColorSet_ComboBox
    {
        public SolidColorBrush BorderDefault { get; set; }
        public SolidColorBrush BorderHover { get; set; }
        public SolidColorBrush BorderSelected { get; set; }
        public SolidColorBrush BorderDisabled { get; set; }

        public SolidColorBrush BackgroundDefault { get; set; }
        public SolidColorBrush BackgroundHover { get; set; }
        public SolidColorBrush BackgroundSelected { get; set; }
        public SolidColorBrush BackgroundDisabled { get; set; }

        public SolidColorBrush WaterMarkForegroundDefault { get; set; }
        public SolidColorBrush WaterMarkForegroundHover { get; set; }
        public SolidColorBrush WaterMarkForegroundSelected { get; set; }
        public SolidColorBrush WaterMarkForegroundDisabled { get; set; }

        public SolidColorBrush ForegroundDefault { get; set; }
        public SolidColorBrush ForegroundHover { get; set; }
        public SolidColorBrush ForegroundSelected { get; set; }
        public SolidColorBrush ForegroundDisabled { get; set; }

        public ColorSet_ItemBox ItemBox { get; set; }
    }

    public class ColorSet_ComboBoxItem
    {
        public SolidColorBrush BackgroundDefault { get; set; }
        public SolidColorBrush BackgroundHover { get; set; }
        public SolidColorBrush BackgroundSelected { get; set; }
        public SolidColorBrush BackgroundDisabled { get; set; }

        public SolidColorBrush ForegroundDefault { get; set; }
        public SolidColorBrush ForegroundHover { get; set; }
        public SolidColorBrush ForegroundSelected { get; set; }
        public SolidColorBrush ForegroundDisabled { get; set; }
    }

    public class ColorSet_ItemBox
    {
        public SolidColorBrush BorderDefault { get; set; }
        public SolidColorBrush BorderHover { get; set; }
        public SolidColorBrush BorderSelected { get; set; }
        public SolidColorBrush BorderDisabled { get; set; }

        public SolidColorBrush BackgroundDefault { get; set; }
        public SolidColorBrush BackgroundHover { get; set; }
        public SolidColorBrush BackgroundSelected { get; set; }
        public SolidColorBrush BackgroundDisabled { get; set; }

        public SolidColorBrush ForegroundDefault { get; set; }
        public SolidColorBrush ForegroundHover { get; set; }
        public SolidColorBrush ForegroundSelected { get; set; }
        public SolidColorBrush ForegroundDisabled { get; set; }
    }

    public class ColorSet_DatePicker
    {
        public SolidColorBrush BorderDefault { get; set; }
        public SolidColorBrush BorderHover { get; set; }
        public SolidColorBrush BorderSelected { get; set; }
        public SolidColorBrush BorderDisabled { get; set; }

        public SolidColorBrush BackgroundDefault { get; set; }
        public SolidColorBrush BackgroundHover { get; set; }
        public SolidColorBrush BackgroundSelected { get; set; }
        public SolidColorBrush BackgroundDisabled { get; set; }

        public SolidColorBrush ForegroundDefault { get; set; }
        public SolidColorBrush ForegroundHover { get; set; }
        public SolidColorBrush ForegroundSelected { get; set; }
        public SolidColorBrush ForegroundDisabled { get; set; }

        public SolidColorBrush WaterMarkForegroundDefault { get; set; }
        public SolidColorBrush WaterMarkForegroundHover { get; set; }
        public SolidColorBrush WaterMarkForegroundSelected { get; set; }
        public SolidColorBrush WaterMarkForegroundDisabled { get; set; }

        public SolidColorBrush ButtonDefault { get; set; }
        public SolidColorBrush ButtonHover { get; set; }
        public SolidColorBrush ButtonSelected { get; set; }
        public SolidColorBrush ButtonDisabled { get; set; }
    }

    public class FontSet
    {
        public System.Windows.Media.FontFamily FontFamily { get; set; }
        public double FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public double FontHeight { get; set; }
    }

    public class ImageSet_Component : DependencyObject
    {
        public static readonly DependencyProperty ImageDefaultProperty;
        public static readonly DependencyProperty ImageHoverProperty;
        public static readonly DependencyProperty ImagePressedProperty;
        public static readonly DependencyProperty ImageDisabledProperty;

        static ImageSet_Component()
        {
            ImageDefaultProperty = DependencyProperty.Register("ImageDefault", typeof(BitmapImage), typeof(ImageSet_Component), new FrameworkPropertyMetadata());
            ImageHoverProperty = DependencyProperty.Register("ImageHover", typeof(BitmapImage), typeof(ImageSet_Component), new FrameworkPropertyMetadata());
            ImagePressedProperty = DependencyProperty.Register("ImagePressed", typeof(BitmapImage), typeof(ImageSet_Component), new FrameworkPropertyMetadata());
            ImageDisabledProperty = DependencyProperty.Register("ImageDisabled", typeof(BitmapImage), typeof(ImageSet_Component), new FrameworkPropertyMetadata());
        }

        public BitmapImage ImageDefault
        {
            get { return (BitmapImage)GetValue(ImageDefaultProperty); }
            set { SetValue(ImageDefaultProperty, value); }
        }
        public BitmapImage ImageHover
        {
            get { return (BitmapImage)GetValue(ImageHoverProperty); }
            set { SetValue(ImageHoverProperty, value); }
        }
        public BitmapImage ImagePressed
        {
            get { return (BitmapImage)GetValue(ImagePressedProperty); }
            set { SetValue(ImagePressedProperty, value); }
        }
        public BitmapImage ImageDisabled
        {
            get { return (BitmapImage)GetValue(ImageDisabledProperty); }
            set { SetValue(ImageDisabledProperty, value); }
        }
    }

    public class ImageSet_Toggle : DependencyObject
    {
        public static readonly DependencyProperty ImageTrueProperty;
        public static readonly DependencyProperty ImageFalseProperty;

        static ImageSet_Toggle()
        {
            ImageTrueProperty = DependencyProperty.Register("ImageTrue", typeof(BitmapImage), typeof(ImageSet_Toggle), new FrameworkPropertyMetadata());
            ImageFalseProperty = DependencyProperty.Register("ImageFalse", typeof(BitmapImage), typeof(ImageSet_Toggle), new FrameworkPropertyMetadata());
        }

        public BitmapImage ImageTrue
        {
            get { return (BitmapImage)GetValue(ImageTrueProperty); }
            set { SetValue(ImageTrueProperty, value); }
        }
        public BitmapImage ImageFalse
        {
            get { return (BitmapImage)GetValue(ImageFalseProperty); }
            set { SetValue(ImageFalseProperty, value); }
        }
    }

    public class IconSet
    {
        public BitmapSource Default { get; set; }
    }
}
