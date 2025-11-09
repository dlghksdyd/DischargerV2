using MExpress.Properties;
using Prism.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MExpress.Mex
{
    public class MexItemBox : ContentControl
    {
        private static readonly int ViewCountDefault = 4;

        #region Property
        public static readonly DependencyProperty IsParentProperty;

        public static readonly DependencyProperty ClipRadiusProperty;
        public static readonly DependencyProperty ClipRectProperty;

        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty ItemSourceHeightProperty;
        public static readonly DependencyProperty ItemSourceViewCountProperty;
        public static readonly DependencyProperty ItemSourceProperty;
        public static readonly DependencyProperty ItemSourcePaddingProperty;
        public static readonly DependencyProperty FontSetProperty;
        public static readonly DependencyProperty FontHeightProperty;

        public static readonly DependencyProperty SelectedObjectProperty;
        public static readonly DependencyProperty SelectedIndexProperty;
        public static readonly DependencyProperty SelectedItemProperty;

        public static readonly DependencyProperty ColorSetProperty;

        public bool IsParent
        {
            get { return (bool)GetValue(IsParentProperty); }
            set { SetValue(IsParentProperty, value); }
        }

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
            get
            {
                List<string> itemSource = new List<string>();

                foreach (MexTextBlock mexTextBlock in DicItemSource.Values)
                {
                    itemSource.Add(mexTextBlock.Text);
                }

                ItemSource = itemSource;

                return (List<string>)GetValue(ItemSourceProperty);
            }
            set { SetValue(ItemSourceProperty, value); }
        }

        public Thickness ItemSourcePadding
        {
            get { return (Thickness)GetValue(ItemSourcePaddingProperty); }
            set { SetValue(ItemSourcePaddingProperty, value); }
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

        public object SelectedObject
        {
            get { return (object)GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
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

        public ColorSet_ItemBox ColorSet
        {
            get { return (ColorSet_ItemBox)GetValue(ColorSetProperty); }
            set { SetValue(ColorSetProperty, value); }
        }

        private static void WidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            double value = (double)e.NewValue;

            Rect setRect = instance.ClipRect;
            setRect.Width = value;

            instance.ClipRect = setRect;
        }

        private static void HeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            double value = (double)e.NewValue;

            Rect setRect = instance.ClipRect;
            setRect.Height = value;

            instance.ClipRect = setRect;
        }

        private static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            if (e.NewValue == null)
            {
                MexScrollViewer getScrollViewer = instance.Content as MexScrollViewer;

                SetScrollViewerContent(getScrollViewer);
            }
            else if (e.NewValue.GetType() == typeof(MexScrollViewer))
            {
                MexScrollViewer getScrollViewer = (MexScrollViewer)e.NewValue;

                // .xaml 에서 추가할 경우 Content Children 변경에 Changed Event 발생하지 않음에 따른 처리 부분
                if (getScrollViewer.Content == null)
                {
                    getScrollViewer.SizeChanged += GetScrollViewer_SizeChanged;
                    return;
                }

                // 값이 변경되지 않았을 경우 return
                if (IsChangedContent(e)) return;

                if (getScrollViewer.Content.GetType() == typeof(StackPanel))
                {
                    SetScrollViewerContent(getScrollViewer);
                }
            }
        }

        private static void IsParentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            bool value = (bool)e.NewValue;

            if (!value)
            {
                InitializeComponent(instance);
            }
        }

        private static void CornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            CornerRadius value = (CornerRadius)e.NewValue;

            instance.ClipRadius = value.TopRight + 0.5;
        }

        private static void ItemSourceHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            double value = (double)e.NewValue;

            if (value == 0) return;
            if (instance.ItemSourceViewCount == 0) return;

            instance.Height = instance.ItemSourceViewCount * value;
        }

        private static void ItemSourceViewCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            int value = (int)e.NewValue;

            if (value == 0) return;
            if (instance.ItemSourceHeight == 0) return;

            instance.Height = instance.ItemSourceHeight * value;
        }

        private static void ItemSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is List<string> newValue && e.OldValue is List<string> oldValue)
            {
                if (Enumerable.SequenceEqual(newValue, oldValue)) return;
            }

            try
            {
                MexItemBox instance = d as MexItemBox;

                List<string> value = (List<string>)e.NewValue;

                if (value == null || value.Count == 0) return;

                StackPanel setStackPanel = new StackPanel();

                // .xaml 에서 추가한 것과 같이 변환
                SetStackPanelContent(instance, ref setStackPanel, value);

                MexScrollViewer setScrollViewer = new MexScrollViewer();
                setScrollViewer.Content = setStackPanel;
                setScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                // ItemSource 수량이 ViewCount보다 작을 경우
                if (instance.DicItemSource.Count < ViewCountDefault)
                {
                    instance.ItemSourceViewCount = instance.DicItemSource.Count;
                }

                instance.Content = setScrollViewer;

                // 변경된 ItemSource에 이전 선택된 아이템이 있을 경우, 해당 아이템 선택될 수 있도록
                if (instance.SelectedObject is MexTextBlock selectedObject)
                {
                    if (instance.DicItemSource.ContainsKey(selectedObject.Text))
                    {
                        instance.SelectedObject = instance.DicItemSource[selectedObject.Text];
                    }
                    else
                    {
                        instance.SelectedObject = null;
                    }
                }
                else
                {
                    if (instance.DicItemSource.ContainsKey(instance.SelectedItem))
                    {
                        instance.SelectedObject = instance.DicItemSource[instance.SelectedItem];
                    }
                    else
                    {
                        instance.SelectedObject = null;
                    }
                }
            }
            catch
            {
                Debug.WriteLine("중복 아이템 선언 불가");
            }
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            FontSet value = (FontSet)e.NewValue;

            if (value == null) return;

            instance.FontFamily = value.FontFamily;
            instance.FontSize = value.FontSize;
            instance.FontWeight = value.FontWeight;
            instance.FontHeight = value.FontHeight;
        }

        private static void SelectedObjectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;

            MexItemBox instance = d as MexItemBox;
            if (instance == null)
            {
                instance.SelectedIndex = -1;
                instance.SelectedItem = "";
                return;
            }

            MexScrollViewer scrollViewer = instance.Content as MexScrollViewer;
            if (scrollViewer == null) return;

            StackPanel stackPanel = scrollViewer.Content as StackPanel;
            if (stackPanel == null) return;

            if (e.NewValue is MexTextBlock selectedObject)
            {
                for (int index = 0; index < stackPanel.Children.Count; index++)
                {
                    MexTextBlock mexTextBlock = stackPanel.Children[index] as MexTextBlock;

                    if (mexTextBlock == selectedObject)
                    {
                        instance.SelectedIndex = index;
                        instance.SelectedItem = selectedObject.Text;

                        ApplyColorSetectedItem(selectedObject);

                        if (instance.IsParent)
                        {
                            ApplyScrollOffsetSetectedItem(selectedObject);
                        }
                    }
                }
            }
        }

        private static void SelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            MexItemBox instance = d as MexItemBox;
            if (e.NewValue == e.OldValue) return;

            if (e.NewValue is int selectedIndex)
            {
                if (selectedIndex < 0)
                {
                    if (instance.SelectedObject != null)
                    {
                        instance.SelectedObject = null;
                    }
                    return;
                }

                MexScrollViewer scrollViewer = instance.Content as MexScrollViewer;
                if (scrollViewer == null) return;

                StackPanel stackPanel = scrollViewer.Content as StackPanel;
                if (stackPanel == null) return;

                MexTextBlock selectedObject = stackPanel.Children[selectedIndex] as MexTextBlock;
                
                if (instance.SelectedObject != selectedObject)
                {
                    instance.SelectedObject = selectedObject;
                }
            }
            else
            {
                if (instance.SelectedObject != null)
                {
                    instance.SelectedObject = null;
                }
            }
        }

        private static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            if (e.NewValue == e.OldValue) return;

            if (e.NewValue is string selectedItem)
            {
                if (selectedItem == "" || selectedItem.ToLower() == "null")
                {
                    if (instance.SelectedObject != null)
                    {
                        instance.SelectedObject = null;
                    }
                    return;
                }

                MexScrollViewer scrollViewer = instance.Content as MexScrollViewer;
                if (scrollViewer == null) return;

                StackPanel stackPanel = scrollViewer.Content as StackPanel;
                if (stackPanel == null) return;

                if (instance.DicItemSource.ContainsKey(selectedItem))
                {
                    MexTextBlock selectedObject = instance.DicItemSource[selectedItem];

                    if (instance.SelectedObject != selectedObject)
                    {
                        instance.SelectedObject = selectedObject;
                    }
                }
            }
            else
            {
                if (instance.SelectedObject != null)
                {
                    instance.SelectedObject = null;
                }
            }
        }

        private static void ColorSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexItemBox instance = d as MexItemBox;

            ColorSet_ItemBox value = (ColorSet_ItemBox)e.NewValue;

            if (value == null) return;

            instance.BorderBrush = value.BorderDefault;
            instance.Background = value.BackgroundDefault;
            instance.Foreground = value.ForegroundDefault;
        }

        static MexItemBox()
        {
            // property default value
            WidthProperty.OverrideMetadata(typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(WidthPropertyChanged)));
            HeightProperty.OverrideMetadata(typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(HeightPropertyChanged)));
            BorderBrushProperty.OverrideMetadata(typeof(MexItemBox), new FrameworkPropertyMetadata());
            BorderThicknessProperty.OverrideMetadata(typeof(MexItemBox), new FrameworkPropertyMetadata());
            BackgroundProperty.OverrideMetadata(typeof(MexItemBox), new FrameworkPropertyMetadata());
            ContentProperty.OverrideMetadata(typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ContentPropertyChanged)));

            IsParentProperty = DependencyProperty.Register("IsParent", typeof(bool), typeof(MexItemBox), new PropertyMetadata(true, IsParentPropertyChanged));

            ClipRadiusProperty = DependencyProperty.Register("ClipRadius", typeof(double), typeof(MexItemBox), new FrameworkPropertyMetadata());
            ClipRectProperty = DependencyProperty.Register("ClipRect", typeof(Rect), typeof(MexItemBox), new FrameworkPropertyMetadata());

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(CornerRadiusPropertyChanged)));

            ItemSourceHeightProperty = DependencyProperty.Register("ItemSourceHeight", typeof(double), typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ItemSourceHeightPropertyChanged)));
            ItemSourceViewCountProperty = DependencyProperty.Register("ItemSourceViewCount", typeof(int), typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ItemSourceViewCountPropertyChanged)));
            ItemSourceProperty = DependencyProperty.Register("ItemSource", typeof(List<string>), typeof(MexItemBox), new FrameworkPropertyMetadata(new List<string>(), new PropertyChangedCallback(ItemSourcePropertyChanged)));
            ItemSourcePaddingProperty = DependencyProperty.Register("ItemSourcePadding", typeof(Thickness), typeof(MexItemBox), new FrameworkPropertyMetadata());
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(FontSetPropertyChanged)));
            FontHeightProperty = DependencyProperty.Register("FontHeight", typeof(double), typeof(MexItemBox), new FrameworkPropertyMetadata());

            SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(MexItemBox), new FrameworkPropertyMetadata(SelectedObjectPropertyChanged));
            SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedIndexPropertyChanged)));
            SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectedItemPropertyChanged)));

            ColorSetProperty = DependencyProperty.Register("ColorSet", typeof(ColorSet_ItemBox), typeof(MexItemBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ColorSetPropertyChanged)));
        }
        #endregion

        public static double ScrollOffsetPrev = 0;
        public static double ScrollOffset = 0;

        private Dictionary<string, MexTextBlock> DicItemSource = new Dictionary<string, MexTextBlock>();

        public MexItemBox()
        {
            if (IsParent) return;

            BorderThickness = new Thickness(1);
            CornerRadius = new CornerRadius(8);

            ClipRadius = 8.5;

            ItemSourceHeight = 48;
            ItemSourceViewCount = ViewCountDefault;
            ItemSourcePadding = new Thickness(16, 12, 16, 12);
            FontSet = ResFontSet.body_md_medium;

            SelectedIndex = -1;
            SelectedItem = "";

            ColorSet = ResColorSet_ItemBox.Primary;

            HorizontalContentAlignment = HorizontalAlignment.Left;
            VerticalContentAlignment = VerticalAlignment.Center;
        }

        private static void InitializeComponent(MexItemBox instance)
        {
            instance.BorderThickness = new Thickness(1);
            instance.CornerRadius = new CornerRadius(8);

            instance.ClipRadius = 8.5;

            instance.ItemSourceHeight = 48;
            instance.ItemSourceViewCount = ViewCountDefault;
            instance.ItemSource = new List<string>();
            instance.ItemSourcePadding = new Thickness(16, 12, 16, 12);
            instance.FontSet = ResFontSet.body_md_medium;

            instance.SelectedIndex = -1;
            instance.SelectedItem = "";

            instance.ColorSet = ResColorSet_ItemBox.Primary;

            instance.HorizontalContentAlignment = HorizontalAlignment.Left;
            instance.VerticalContentAlignment = VerticalAlignment.Center;
        }

        private static void SetScrollViewerContent(MexScrollViewer getScrollViewer)
        {
            try
            {
                MexItemBox instance = getScrollViewer.Parent as MexItemBox;
                StackPanel getStackPanel = getScrollViewer.Content as StackPanel;

                if (instance == null) return;
                if (getStackPanel == null) return;

                if (getStackPanel.Children.Count > 0)
                {
                    SetStackPanelContent(instance, ref getStackPanel);
                }
            }
            catch
            {
                MessageBox.Show("중복 아이템 선언 불가");
            }
        }

        private static void SetStackPanelContent(MexItemBox instance, ref StackPanel stackPanel, List<string> itemList = null)
        {
            instance.DicItemSource = new Dictionary<string, MexTextBlock>();

            if (itemList == null)
            {
                foreach (var item in stackPanel.Children)
                {
                    if (item.GetType() == typeof(MexTextBlock))
                    {
                        MexTextBlock getTextBlock = (MexTextBlock)item;

                        getTextBlock.Height = instance.ItemSourceHeight;
                        getTextBlock.BorderBrush = instance.BorderBrush;
                        getTextBlock.BorderThickness = new Thickness(0, 0, 0, instance.BorderThickness.Bottom);
                        getTextBlock.Background = new SolidColorBrush(Colors.Transparent);
                        getTextBlock.Foreground = instance.Foreground;
                        getTextBlock.FontSize = instance.FontSize;
                        getTextBlock.FontWeight = instance.FontWeight;
                        getTextBlock.FontHeight = instance.FontHeight;
                        getTextBlock.Padding = instance.ItemSourcePadding;
                        getTextBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                        getTextBlock.HorizontalContentAlignment = instance.HorizontalContentAlignment;
                        getTextBlock.VerticalAlignment = VerticalAlignment.Stretch;
                        getTextBlock.VerticalContentAlignment = instance.VerticalContentAlignment;
                        getTextBlock.MouseLeftButtonUp += MexTextBlock_MouseLeftButtonUp;

                        instance.DicItemSource.Add(getTextBlock.Text, getTextBlock);
                    }
                }
            }
            else
            {
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.SnapsToDevicePixels = true;

                foreach (var item in itemList)
                {
                    MexTextBlock setTextBlock = new MexTextBlock();

                    setTextBlock.Text = item;
                    setTextBlock.Height = instance.ItemSourceHeight;
                    setTextBlock.BorderBrush = instance.BorderBrush;
                    setTextBlock.BorderThickness = new Thickness(0, 0, 0, instance.BorderThickness.Bottom);
                    setTextBlock.Background = new SolidColorBrush(Colors.Transparent);
                    setTextBlock.Foreground = instance.Foreground;
                    setTextBlock.FontSize = instance.FontSize;
                    setTextBlock.FontWeight = instance.FontWeight;
                    setTextBlock.FontHeight = instance.FontHeight;
                    setTextBlock.Padding = instance.ItemSourcePadding;
                    setTextBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                    setTextBlock.HorizontalContentAlignment = instance.HorizontalContentAlignment;
                    setTextBlock.VerticalAlignment = VerticalAlignment.Stretch;
                    setTextBlock.VerticalContentAlignment = instance.VerticalContentAlignment;
                    setTextBlock.MouseLeftButtonUp += MexTextBlock_MouseLeftButtonUp;

                    instance.DicItemSource.Add(setTextBlock.Text, setTextBlock);

                    stackPanel.Children.Add(setTextBlock);
                }
            }
        }

        private static void GetScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MexScrollViewer getScrollViewer = sender as MexScrollViewer;

            if (getScrollViewer.Content == null) return;

            if (getScrollViewer.Content.GetType() == typeof(StackPanel))
            {
                MexItemBox mexItemBox = getScrollViewer.Parent as MexItemBox;
                ContentPropertyChanged(mexItemBox, new DependencyPropertyChangedEventArgs());
            }
        }

        private static bool IsChangedContent(DependencyPropertyChangedEventArgs e)
        {
            ContentControl newContentControl = e.NewValue as ContentControl;
            ContentControl oldContentControl = e.OldValue as ContentControl;

            if (oldContentControl == null) return false;

            StackPanel newStackPanel = newContentControl.Content as StackPanel;
            StackPanel oldStackPanel = oldContentControl.Content as StackPanel;

            if (newStackPanel.Children.Count == oldStackPanel.Children.Count)
            {
                for (int index = 0; index < newStackPanel.Children.Count; index++)
                {
                    MexTextBlock newMexTextBlock = newStackPanel.Children[index] as MexTextBlock;
                    MexTextBlock oldMexTextBlock = oldStackPanel.Children[index] as MexTextBlock;

                    if (newMexTextBlock.Text == oldMexTextBlock.Text)
                        return true;
                }
            }
            return false;
        }

        public static void MexTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MexTextBlock mexTextBlock = sender as MexTextBlock;
            StackPanel getStackPanel = mexTextBlock.Parent as StackPanel;
            MexScrollViewer getScrollViewer = getStackPanel.Parent as MexScrollViewer;
            MexItemBox instance = getScrollViewer.Parent as MexItemBox;

            if (getStackPanel == null) return;
            if (getScrollViewer == null) return;
            if (instance == null) return;

            instance.SelectedObject = mexTextBlock;
        }

        private static void ApplyColorSetectedItem(MexTextBlock mexTextBlock)
        {
            StackPanel getStackPanel = mexTextBlock.Parent as StackPanel;
            MexScrollViewer getScrollViewer = getStackPanel.Parent as MexScrollViewer;
            MexItemBox instance = getScrollViewer.Parent as MexItemBox;

            if (getStackPanel == null) return;
            if (getScrollViewer == null) return;
            if (instance == null) return;

            foreach (var textBlock in getStackPanel.Children.OfType<MexTextBlock>())
            {
                if (mexTextBlock == textBlock)
                {
                    textBlock.Background = instance.ColorSet.BackgroundSelected;
                    textBlock.Foreground = instance.ColorSet.ForegroundSelected;
                }
                else
                {
                    textBlock.Background = Brushes.Transparent;
                    textBlock.Foreground = instance.ColorSet.ForegroundDefault;
                }
            }
        }

        private static void ApplyScrollOffsetSetectedItem(MexTextBlock mexTextBlock)
        {
            StackPanel getStackPanel = mexTextBlock.Parent as StackPanel;
            MexScrollViewer getScrollViewer = getStackPanel.Parent as MexScrollViewer;
            MexItemBox instance = getScrollViewer.Parent as MexItemBox;

            if (instance == null) return;

            double offset = instance.SelectedIndex * mexTextBlock.Height;

            getScrollViewer.ScrollToVerticalOffset(offset);
        }
    }
}
