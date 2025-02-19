using DischargerV2.MVVM.Enums;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Utility.Common
{
    #region IValueConverter
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
            if (value is EDischargeMode mode)
            {
                if (parameter != null && parameter.GetType() == typeof(EDischargeMode))
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
            if (value is EDischargeMode mode)
            {
                if (parameter != null && parameter.GetType() == typeof(EDischargeMode))
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
            if (value is EDischargeMode mode)
            {
                if (parameter != null && parameter.GetType() == typeof(EDischargeMode))
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
            if (value is EDischargeMode mode)
            {
                if (parameter != null && parameter.GetType() == typeof(EDischargeMode))
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

    public class EDischargeTargetToBoolConverter : IValueConverter
    {
        public bool SelectedValue { get; set; } = true;
        public bool UnselectedValue { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EDischargeTarget mode)
            {
                if (parameter != null && parameter.GetType() == typeof(EDischargeTarget))
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
            if (parameter != null && parameter.GetType() == typeof(EDischargeTarget))
            {
                return Enum.Parse(targetType, parameter.ToString());
            }
            return Binding.DoNothing;
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
    #endregion

    #region IMultiValueConverter
    public class EDischargerStateToBoolConverter : IMultiValueConverter
    {
        public bool ReadyValue { get; set; } = true;
        public bool NotReadyValue { get; set; } = false;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<EDischargerState> dischargerStates)
            {
                if (value[1] is int selectedIndex)
                {
                    EDischargerState eDischargerState = dischargerStates[selectedIndex];

                    if (eDischargerState == EDischargerState.Ready)
                    {
                        return ReadyValue;
                    }
                    else
                    {
                        return NotReadyValue;
                    }
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsStartedToVisibilityConverter : IMultiValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<bool> isStartedArray)
            {
                if (value[1] is int selectedIndex)
                {
                    if (parameter != null && parameter.ToString().ToUpper() == "INVERT")
                    {
                        if (isStartedArray[selectedIndex])
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
                        if (isStartedArray[selectedIndex])
                        {
                            return TrueValue;
                        }
                        else
                        {
                            return FalseValue;
                        }
                    }
                }
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class EDischargeTargetToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is EDischargeTarget mode)
            {
                if (mode == EDischargeTarget.Full)
                {
                    return "Full Discharge";
                }
                else if (mode == EDischargeTarget.Zero)
                {
                    return "0V Discharge";
                }
                else if (mode == EDischargeTarget.Voltage)
                {
                    if (value[1] is string target)
                    {
                        return string.Format("Target Voltage ({0}V)", target);
                    }
                    else
                    {
                        return "Target Voltage (V)";
                    }
                }
                else if (mode == EDischargeTarget.SoC)
                {
                    if (value[1] is string target)
                    {
                        return string.Format("Target SoC ({0}%)", target);
                    }
                    else
                    {
                        return "Target SoC (%)";
                    }
                }
                return string.Empty;
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}

