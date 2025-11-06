using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MExpress.Mex
{
    public class ImageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapSource imagePrimary = value as BitmapSource;
            SolidColorBrush targetColor = parameter as SolidColorBrush;

            if (imagePrimary == null)
                return new BitmapImage();

            System.Windows.Media.Color oldColor = Colors.Black;
            System.Windows.Media.Color newColor = targetColor.Color;

            FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap();
            formatConvertedBitmap.BeginInit();
            formatConvertedBitmap.Source = imagePrimary;
            formatConvertedBitmap.DestinationFormat = PixelFormats.Bgra32;
            formatConvertedBitmap.EndInit();

            Bitmap bitmap = new Bitmap(formatConvertedBitmap.PixelWidth, formatConvertedBitmap.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            formatConvertedBitmap.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var getColor = bitmap.GetPixel(i, j);

                    int a = getColor.A;
                    int r = 0;
                    int g = 0;
                    int b = 0;

                    int rDiff = newColor.R - oldColor.R;
                    int gDiff = newColor.G - oldColor.G;
                    int bDiff = newColor.B - oldColor.B;

                    if (getColor.A != 0)
                    {
                        // R
                        r = getColor.R + rDiff;
                        r = (r < 0) ? 0 : r;
                        r = (r > 255) ? 255 : r;
                        //G
                        g = getColor.G + gDiff;
                        g = (g < 0) ? 0 : g;
                        g = (g > 255) ? 255 : g;
                        //B
                        b = getColor.B + bDiff;
                        b = (b < 0) ? 0 : b;
                        b = (b > 255) ? 255 : b;
                    }

                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(a, r, g, b));
                }
            }

            BitmapImage convertImage;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;

                convertImage = new BitmapImage();
                convertImage.BeginInit();
                convertImage.StreamSource = memoryStream;
                convertImage.CacheOption = BitmapCacheOption.OnLoad;
                convertImage.EndInit();
                convertImage.Freeze();
            }
            return convertImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TextPaddingOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness textPadding)
            {
                if (double.TryParse(parameter.ToString(), out double offset))
                {
                    textPadding.Left += offset;
                    return textPadding;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FontSizeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double fontSize)
            {
                if (double.TryParse(parameter.ToString(), out double offset))
                {
                    fontSize += offset;
                    return fontSize;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SolidColorBrushAlphaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush solidColorBrush)
            {
                try
                {
                    byte alpha = byte.Parse(parameter.ToString(), NumberStyles.HexNumber);

                    var newSolidColorBrush = new SolidColorBrush();

                    newSolidColorBrush.Color = new System.Windows.Media.Color()
                    {
                        A = alpha,
                        R = solidColorBrush.Color.R,
                        G = solidColorBrush.Color.G,
                        B = solidColorBrush.Color.B
                    };

                    return newSolidColorBrush;
                }
                catch
                {
                    return solidColorBrush;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (double.TryParse(stringValue, out double doubleValue))
                {
                    return stringValue;
                }
                else if (stringValue == "-")
                {
                    return stringValue;
                }
                else
                {
                    if (stringValue.Length > 0)
                    {
                        return stringValue.Substring(0, stringValue.Length - 1);
                    }
                    else
                    {
                        return stringValue;
                    }
                }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (double.TryParse(stringValue, out double doubleValue))
                {
                    return stringValue;
                }
                else if (stringValue == "-")
                {
                    return stringValue;
                }
                else
                {
                    if (stringValue.Length > 0)
                    {
                        return stringValue.Substring(0, stringValue.Length - 1);
                    }
                    else
                    {
                        return stringValue;
                    }
                }
            }
            return Binding.DoNothing;
        }
    }

    public class IsInt32Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (Int32.TryParse(stringValue, out int inteValue))
                {
                    return stringValue;
                }
                else if (stringValue == "-")
                {
                    return stringValue;
                }
                else
                {
                    if (stringValue.Length > 0)
                    {
                        return stringValue.Substring(0, stringValue.Length - 1);
                    }
                    else
                    {
                        return stringValue;
                    }
                }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (Int32.TryParse(stringValue, out int inteValue))
                {
                    return stringValue;
                }
                else if (stringValue == "-")
                {
                    return stringValue;
                }
                else
                {
                    if (stringValue.Length > 0)
                    {
                        return stringValue.Substring(0, stringValue.Length - 1);
                    }
                    else
                    {
                        return stringValue;
                    }
                }
            }
            return Binding.DoNothing;
        }
    }

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

    public class BorderSizeToRectConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is double width)
            {
                if (value[1] is double height)
                {
                    return new Rect(0, 0, width, height);
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = (double)value;

            return new GridLength(doubleValue, GridUnitType.Pixel);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.ToUpper();
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
}

