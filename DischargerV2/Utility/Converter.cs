using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.Models;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
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
    public class EDischargerDataToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<DischargerDatas> dischargerDatas)
            {
                if (value[1] is int selectedIndex)
                {
                    if (value[2] is EDischargerData eDischargerData)
                    {
                        DischargerDatas dischargerData = dischargerDatas[selectedIndex];

                        if (eDischargerData == EDischargerData.Voltage)
                        {
                            return dischargerData.ReceiveBatteryVoltage.ToString("F1");
                        }
                        else if (eDischargerData == EDischargerData.Current)
                        {
                            return dischargerData.ReceiveDischargeCurrent.ToString("F1");
                        }
                        else if (eDischargerData == EDischargerData.Temp)
                        {
                            return dischargerData.ReceiveDischargeTemp.ToString("F1");
                        }
                        else if (eDischargerData == EDischargerData.SoC)
                        {
                            if (value[3] is Dictionary<string, ModelSetMode_Preset> modelDictionary)
                            {
                                if (value[4] is string dischargerName)
                                {
                                    string batteryType = modelDictionary[dischargerName].SelectedBatteryType;

                                    return OCV_Table.getSOC(batteryType, dischargerData.ReceiveBatteryVoltage).ToString("F1");
                                }
                            }
                        }
                        else if (eDischargerData == EDischargerData.SafetyVoltageMin)
                        {
                            return (dischargerData.SafetyVoltageMin + EthernetClientDischarger.SafetyMarginVoltage).ToString("F1");
                        }
                        else if (eDischargerData == EDischargerData.SafetyVoltageMax)
                        {
                            return (dischargerData.SafetyVoltageMax - EthernetClientDischarger.SafetyMarginVoltage).ToString("F1");
                        }
                        else if (eDischargerData == EDischargerData.SafetyCurrentMin)
                        {
                            return (dischargerData.SafetyCurrentMin + EthernetClientDischarger.SafetyMarginCurrent).ToString("F1");
                        }
                        else if (eDischargerData == EDischargerData.SafetyCurrentMax)
                        {
                            return (dischargerData.SafetyCurrentMax - EthernetClientDischarger.SafetyMarginCurrent).ToString("F1");
                        }
                        else if (eDischargerData == EDischargerData.SafetyTempMin)
                        {
                            return dischargerData.SafetyTempMin.ToString("F1");
                        }
                        else if (eDischargerData == EDischargerData.SafetyTempMax)
                        {
                            return dischargerData.SafetyTempMax.ToString("F1");
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

    public class TempDataToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<ObservableCollection<double>> tempDatas)
            {
                if (!(tempDatas.Count > 0))
                    return Binding.DoNothing;

                if (value[1] is int tempModuleIndex)
                {
                    if (value[2] is int tempModuleChannel)
                    {
                        return tempDatas[tempModuleIndex][tempModuleChannel].ToString("F1");
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

    public class EDischargerStateToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<EDischargerState> dischargerStates)
            {
                if (value[1] is int selectedIndex)
                {
                    EDischargerState eDischargerState = dischargerStates[selectedIndex];

                    return eDischargerState.ToString();
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// if (EDischarggerState == ConverterParameter) return true;
    /// ConverterParameter "READY"
    /// </summary>
    public class EDischargerStateToBoolConverter : IMultiValueConverter
    {
        public bool TrueValue { get; set; } = true;
        public bool FalseValue { get; set; } = false;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<EDischargerState> dischargerStates)
            {
                if (value[1] is int selectedIndex)
                {
                    EDischargerState eDischargerState = dischargerStates[selectedIndex];

                    if (parameter != null)
                    {
                        if (parameter.ToString().ToUpper() == "READY")
                        {
                            if (eDischargerState == EDischargerState.Ready)
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
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// ConverterParameter "ERROR"
    /// </summary>
    public class EDischargerStateToForegroundConverter : IMultiValueConverter
    {
        public SolidColorBrush ErrorValue { get; set; } = ResColor.text_error;
        public SolidColorBrush NoneErrorValue { get; set; } = ResColor.text_body;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<EDischargerState> dischargerStates)
            {
                if (value[1] is int selectedIndex)
                {
                    EDischargerState eDischargerState = dischargerStates[selectedIndex];

                    if (parameter != null)
                    {
                        if (parameter.ToString().ToUpper() == "ERROR")
                        {
                            if (eDischargerState >= EDischargerState.SafetyOutOfRange)
                            {
                                return ErrorValue;
                            }
                            else
                            {
                                return NoneErrorValue;
                            }
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

    /// <summary>
    /// if (EDischarggerState == ConverterParameter) return Visibility.Visible;
    /// ConverterParameter "ERROR", "PAUSE"
    /// </summary>
    public class EDischargerStateToVisibilityConverter : IMultiValueConverter
    {
        public Visibility ErrorValue { get; set; } = Visibility.Visible;
        public Visibility NoneErrorValue { get; set; } = Visibility.Collapsed;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<EDischargerState> dischargerStates)
            {
                if (value[1] is int selectedIndex)
                {
                    EDischargerState eDischargerState = dischargerStates[selectedIndex];

                    if (parameter != null)
                    {
                        if (parameter.ToString().ToUpper() == "ERROR")
                        {
                            if (eDischargerState >= EDischargerState.SafetyOutOfRange)
                            {
                                return ErrorValue;
                            }
                            else
                            {
                                return NoneErrorValue;
                            }
                        }
                        else if (parameter.ToString().ToUpper() == "PAUSE")
                        {
                            if (eDischargerState == EDischargerState.Pause)
                            {
                                return ErrorValue;
                            }
                            else
                            {
                                return NoneErrorValue;
                            }
                        }
                        return ErrorValue;
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

    public class EDischargerStateToImageConverter : IMultiValueConverter
    {
        public BitmapSource dischargingImage { get; set; } = ResImage.downloading;
        public BitmapSource pauseImage { get; set; } = ResImage.pause;
        public BitmapSource errorImage { get; set; } = ResImage.warning_amber;

        public SolidColorBrush discharingImageColor { get; set; } = ResColor.icon_primary;
        public SolidColorBrush pauseImageColor { get; set; } = ResColor.icon_primary;
        public SolidColorBrush errorImageColor { get; set; } = ResColor.icon_error;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<EDischargerState> dischargerStates)
            {
                if (value[1] is int selectedIndex)
                {
                    EDischargerState eDischargerState = dischargerStates[selectedIndex];
                    BitmapSource imagePrimary;
                    SolidColorBrush targetColor;

                    if (eDischargerState == EDischargerState.Discharging)
                    {
                        imagePrimary = dischargingImage;
                        targetColor = ResColor.icon_primary;
                    }
                    else if (eDischargerState == EDischargerState.Pause)
                    {
                        imagePrimary = pauseImage;
                        targetColor = ResColor.icon_primary;
                    }
                    else if (eDischargerState >= EDischargerState.SafetyOutOfRange)
                    {
                        imagePrimary = errorImage;
                        targetColor = ResColor.icon_error;
                    }
                    else
                    {
                        return Binding.DoNothing;
                    }

                    // ImageColorConverter와 동일
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
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// ConverterParameter "ERROR" 
    /// </summary>
    public class EDischargerStateToBorderBrushConverter : IMultiValueConverter
    {
        public SolidColorBrush ErrorValue { get; set; } = ResColor.border_error;
        public SolidColorBrush NoneErrorValue { get; set; } = ResColor.border_primary;

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ObservableCollection<EDischargerState> dischargerStates)
            {
                if (value[1] is int selectedIndex)
                {
                    EDischargerState eDischargerState = dischargerStates[selectedIndex];

                    if (parameter != null)
                    {
                        if (parameter.ToString().ToUpper() == "ERROR")
                        {
                            if (eDischargerState >= EDischargerState.SafetyOutOfRange)
                            {
                                return ErrorValue;
                            }
                            else
                            {
                                return NoneErrorValue;
                            }
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
                    if (value[2] is string target)
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

