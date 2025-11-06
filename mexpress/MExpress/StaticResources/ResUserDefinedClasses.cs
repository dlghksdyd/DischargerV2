using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace MExpress.Mex
{
    public class ColorSet_Button
    {
        public Thickness BorderThickness { get; set; }

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
        public SolidColorBrush BorderDisabled { get; set; }
        public SolidColorBrush BorderPressed { get; set; }

        public SolidColorBrush BackgroundDefault { get; set; }
        public SolidColorBrush BackgroundHover { get; set; }
        public SolidColorBrush BackgroundDisabled { get; set; }
        public SolidColorBrush BackgroundPressed { get; set; }

        public SolidColorBrush StrokeDefault { get; set; }
        public SolidColorBrush StrokeHover { get; set; }
        public SolidColorBrush StrokeDisabled { get; set; }
        public SolidColorBrush StrokePressed { get; set; }

        // Selected
        public SolidColorBrush BorderDefault_Selected { get; set; }
        public SolidColorBrush BorderHover_Selected { get; set; }
        public SolidColorBrush BorderDisabled_Selected { get; set; }
        public SolidColorBrush BorderPressed_Selected { get; set; }

        public SolidColorBrush BackgroundDefault_Selected { get; set; }
        public SolidColorBrush BackgroundHover_Selected { get; set; }
        public SolidColorBrush BackgroundDisabled_Selected { get; set; }
        public SolidColorBrush BackgroundPressed_Selected { get; set; }

        public SolidColorBrush StrokeDefault_Selected { get; set; }
        public SolidColorBrush StrokeHover_Selected { get; set; }
        public SolidColorBrush StrokeDisabled_Selected { get; set; }
        public SolidColorBrush StrokePressed_Selected { get; set; }
    }

    public class ColorSet_RadioButton
    {
        public SolidColorBrush ActiveDefaultLargeEllipse { get; set; }
        public SolidColorBrush ActiveDefaultMiddleEllipse { get; set; }
        public SolidColorBrush ActiveDefaultSmallEllipse { get; set; }

        public SolidColorBrush ActiveHoverLargeEllipse { get; set; }
        public SolidColorBrush ActiveHoverMiddleEllipse { get; set; }
        public SolidColorBrush ActiveHoverSmallEllipse { get; set; }

        public SolidColorBrush ActiveDisabledLargeEllipse { get; set; }
        public SolidColorBrush ActiveDisabledMiddleEllipse { get; set; }
        public SolidColorBrush ActiveDisabledSmallEllipse { get; set; }

        public SolidColorBrush ActivePressedLargeEllipse { get; set; }
        public SolidColorBrush ActivePressedMiddleEllipse { get; set; }
        public SolidColorBrush ActivePressedSmallEllipse { get; set; }

        public SolidColorBrush InactiveDefaultLargeEllipse { get; set; }
        public SolidColorBrush InactiveDefaultMiddleEllipse { get; set; }
        public SolidColorBrush InactiveDefaultSmallEllipse { get; set; }

        public SolidColorBrush InactiveHoverLargeEllipse { get; set; }
        public SolidColorBrush InactiveHoverMiddleEllipse { get; set; }
        public SolidColorBrush InactiveHoverSmallEllipse { get; set; }

        public SolidColorBrush InactiveDisabledLargeEllipse { get; set; }
        public SolidColorBrush InactiveDisabledMiddleEllipse { get; set; }
        public SolidColorBrush InactiveDisabledSmallEllipse { get; set; }

        public SolidColorBrush InactivePressedLargeEllipse { get; set; }
        public SolidColorBrush InactivePressedMiddleEllipse { get; set; }
        public SolidColorBrush InactivePressedSmallEllipse { get; set; }
    }

    public class ColorSet_TextBox
    {
        public SolidColorBrush BorderDefault { get; set; }
        public SolidColorBrush BorderHover { get; set; }
        public SolidColorBrush BorderSelected { get; set; }
        public SolidColorBrush BorderDisabled { get; set; }
    }

    public class FontSet
    {
        public double FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public double FontHeight { get; set; }
    }

    public class ImageSet_Component
    {
        public BitmapImage Default { get; set; }
        public BitmapImage Hover { get; set; }
        public BitmapImage Pressed { get; set; }
        public BitmapImage Disabled { get; set; }
    }

    public class ImageSet_Toggle
    {
        public BitmapImage True { get; set; }
        public BitmapImage False { get; set; }
    }

    public class IconSet
    {
        public Bitmap Default { get; set; }
    }
}
