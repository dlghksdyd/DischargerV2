using DischargerV2.MVVM.Enums;
using MExpress.Mex;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Utility.Common
{
    public class BitmapToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bitmap = value as System.Drawing.Bitmap;
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RectangleInGraphWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Canvas graph = value as Canvas;

            return graph.ActualWidth / (int)(graph.ActualWidth / (graph.ActualHeight / 5));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RectangleInGraphHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Canvas graph = value as Canvas;

            return graph.ActualHeight / 5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter != null && parameter.ToString().ToUpper() == "INVERT")
                {
                    if (boolValue)
                    {
                        return FalseValue;
                    }
                    else
                    {
                        return TrueValue;
                    }
                }
                else
                {
                    if (boolValue)
                    {
                        return TrueValue;
                    }
                    else
                    {
                        return FalseValue;
                    }
                }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToAdminTextConverter : IValueConverter
    {
        public string TrueValue { get; set; } = "Admin";
        public string FalseValue { get; set; } = "User";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (boolValue)
                {
                    return TrueValue;
                }
                else
                {
                    return FalseValue;
                }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class IntToStringConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue.ToString();
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EModeToVisibilityConverter : IValueConverter
    {
        public Visibility SelectedValue { get; set; } = Visibility.Visible;
        public Visibility UnselectedValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EMode mode)
            {
                if (parameter != null && parameter.GetType() == typeof(EMode))
                {
                    if (parameter.ToString() == mode.ToString())
                    {
                        return SelectedValue;
                    }
                    else
                    {
                        return UnselectedValue;
                    }
                }
                return UnselectedValue;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EModeToForegroundConverter : IValueConverter
    {
        public SolidColorBrush SelectedValue { get; set; } = ResColor.text_action;
        public SolidColorBrush UnselectedValue { get; set; } = ResColor.text_body;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EMode mode)
            {
                if (parameter != null && parameter.GetType() == typeof(EMode))
                {
                    if (parameter.ToString() == mode.ToString())
                    {
                        return SelectedValue;
                    }
                    else
                    {
                        return UnselectedValue;
                    }
                }
                return UnselectedValue;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EModeToBorderBrushConverter : IValueConverter
    {
        public SolidColorBrush SelectedValue { get; set; } = ResColor.border_action_hover;
        public SolidColorBrush UnselectedValue { get; set; } = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EMode mode)
            {
                if (parameter != null && parameter.GetType() == typeof(EMode))
                {
                    if (parameter.ToString() == mode.ToString())
                    {
                        return SelectedValue;
                    }
                    else
                    {
                        return UnselectedValue;
                    }
                }
                return UnselectedValue;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EModeToBackgroundConverter : IValueConverter
    {
        public SolidColorBrush SelectedValue { get; set; } = ResColor.surface_action_hover2;
        public SolidColorBrush UnselectedValue { get; set; } = ResColor.surface_primary;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EMode mode)
            {
                if (parameter != null && parameter.GetType() == typeof(EMode))
                {
                    if (parameter.ToString() == mode.ToString())
                    {
                        return SelectedValue;
                    }
                    else
                    {
                        return UnselectedValue;
                    }
                }
                return UnselectedValue;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

