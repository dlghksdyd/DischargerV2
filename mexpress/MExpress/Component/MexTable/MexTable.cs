using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace MExpress.Mex
{
    [Localizability(LocalizationCategory.Ignore)]
    [DefaultProperty("Content")]
    [ContentProperty("Content")]
    public class MexTable : ContentControl
    {
        #region ColorStyle
        public enum EColorStyle
        {
            Primary,
        }

        public class CStyle
        {
            public SolidColorBrush BorderBrush;
            public SolidColorBrush HeaderBackground;
            public SolidColorBrush ContentBackground;

            public SolidColorBrush RowForegroundDefault;
            public SolidColorBrush RowForegroundHover;
            public SolidColorBrush RowForegroundSelected;

            public SolidColorBrush RowBackgroundDefault;
            public SolidColorBrush RowBackgroundHover;
            public SolidColorBrush RowBackgroundSelected;
            public SolidColorBrush RowLineColor;

            public FontSet HeaderFontSet;
            public FontSet ContentFontSet;
        }

        private Dictionary<string, CStyle> StyleDict = new Dictionary<string, CStyle>();
        public CStyle SelectedStyle = null;

        private void InitializeStyle()
        {
            CStyle primary = new CStyle();
            primary.BorderBrush = ResColor.border_primary;
            primary.HeaderBackground = ResColor.surface_disabled;
            primary.ContentBackground = ResColor.transparent;
            primary.RowForegroundDefault = ResColor.text_body;
            primary.RowForegroundHover = ResColor.text_body;
            primary.RowForegroundSelected = ResColor.text_headings;
            primary.RowBackgroundDefault = ResColor.transparent;
            primary.RowBackgroundHover = ResColor.table_action_hover;
            primary.RowBackgroundSelected = ResColor.table_selected;
            primary.RowLineColor = ResColor.border_primary;
            primary.HeaderFontSet = ResFontSet.body_lg_medium;
            primary.ContentFontSet = ResFontSet.body_md_regular;
            StyleDict["Primary"] = primary;

            SelectedStyle = StyleDict["Primary"];
        }

        #endregion

        #region Property 선언
        public static readonly DependencyProperty IsReadOnlyProperty;
        public static readonly DependencyProperty ColorStyleProperty;
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty HeaderProperty;
        public static readonly DependencyProperty ColumnCountProperty;
        public static readonly DependencyProperty SelectedIndexProperty;

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public EColorStyle ColorStyle
        {
            get { return (EColorStyle)GetValue(ColorStyleProperty); }
            set { SetValue(ColorStyleProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public MexTableHeader Header
        {
            get { return (MexTableHeader)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public uint ColumnCount
        {
            get { return (uint)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        private static void ColorStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTable instance = d as MexTable;

            EColorStyle value = (EColorStyle)e.NewValue;

            instance.SelectedStyle = instance.StyleDict[value.ToString()];
        }

        private static void CornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTable instance = d as MexTable;

            CornerRadius value = (CornerRadius)e.NewValue;

            instance.xBorder.CornerRadius = new CornerRadius(
                value.TopLeft - 0.5, value.TopRight - 0.5, value.BottomRight - 0.5, value.BottomLeft - 0.5);
            instance.xRectangleGeometry.RadiusX = value.TopRight;
            instance.xRectangleGeometry.RadiusY = value.TopRight;
        }

        private static void HeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTable instance = d as MexTable;

            MexTableHeader value = (MexTableHeader)e.NewValue;

            if (value == null)
            {
                instance.xHeaderRowDefinition.Height = new GridLength(0, GridUnitType.Pixel);
                instance.xHeaderGrid.Height = 0.0;
                instance.UpdateRowsColumn(instance.ColumnCount);
            }
            else
            {
                instance.xHeaderRowDefinition.Height = new GridLength(instance.Header.Height, GridUnitType.Pixel);
                instance.xHeaderGrid.Height = instance.Header.Height;
                instance.Header.SetMexTableComponent(instance);
                instance.UpdateHeaderColumn(instance.ColumnCount);
                instance.UpdateRowsColumn(instance.ColumnCount);
            }
        }

        private static void ColumnCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTable instance = d as MexTable;

            uint value = (uint)e.NewValue;

            instance.UpdateHeaderColumn(value);
            instance.UpdateRowsColumn(value);
        }

        public void UpdateHeaderColumn(uint columnCount)
        {
            /// 헤더 업데이트
            xHeaderGrid.ColumnDefinitions.Clear();
            for (int column = 0; column < columnCount; column++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                xHeaderGrid.ColumnDefinitions.Add(columnDefinition);
            }
            Header?.UpdateColumns();

            UpdateContentHeight(Height);
        }

        public void UpdateRowsColumn(uint columnCount)
        {
            for (uint row = 0; row < _rows.Count; row++)
            {
                UpdateRowColumn(row, columnCount);
            }

            UpdateContentHeight(Height);
        }

        public void UpdateRowColumn(uint rowNum, uint columnCount)
        {
            _rows[(int)rowNum].RowBorder.BorderBrush = SelectedStyle.RowLineColor;
            _rows[(int)rowNum].MouseEnter += Row_MouseEnter;
            _rows[(int)rowNum].MouseLeave += Row_MouseLeave;
            _rows[(int)rowNum].MouseLeftButtonDown += Row_MouseLeftButtonDown;
            _rows[(int)rowNum].RowGrid.ColumnDefinitions.Clear();

            for (int column = 0; column < columnCount; column++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                _rows[(int)rowNum].RowGrid.ColumnDefinitions.Add(columnDefinition);
            }
            _rows[(int)rowNum]?.UpdateColumns();
        }

        private void Row_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MexTableRow mexTableRow = sender as MexTableRow;

            if (IsReadOnly && e != null) return;

            int selectedIndex = -1;

            for (int row = 0; row < _rows.Count; row++)
            {
                if (ReferenceEquals(_rows[row], mexTableRow))
                {
                    selectedIndex = row;
                }
            }

            /// 기존에 선택된 row 색상 원래대로 복구
            if (SelectedIndex != -1)
            {
                /// background
                _rows[this.SelectedIndex].RowBorder.Background = SelectedStyle.RowBackgroundDefault;
                /// foreground
                Grid beforeGrid = _rows[this.SelectedIndex].RowBorder.Child as Grid;
                for (int column = 0; column < beforeGrid.Children.Count; column++)
                {
                    MexTableRowColumn label = beforeGrid.Children[column] as MexTableRowColumn;
                    label.Foreground = SelectedStyle.RowForegroundDefault;
                }
            }

            /// 새로 선택된 row 색상 변경
            /// background
            mexTableRow.RowBorder.Background = SelectedStyle.RowBackgroundSelected;
            /// foreground
            Grid afterGrid = mexTableRow.RowBorder.Child as Grid;
            for (int column = 0; column < afterGrid.Children.Count; column++)
            {
                MexTableRowColumn label = afterGrid.Children[column] as MexTableRowColumn;
                label.Foreground = SelectedStyle.RowForegroundSelected;
            }
            this.SelectedIndex = selectedIndex;
        }

        private void Row_MouseLeave(object sender, MouseEventArgs e)
        {
            MexTableRow mexTableRow = sender as MexTableRow;

            if (mexTableRow.RowBorder.Background != SelectedStyle.RowBackgroundSelected)
            {
                /// background
                mexTableRow.RowBorder.Background = SelectedStyle.RowBackgroundDefault;

                /// foreground
                Grid afterGrid = mexTableRow.RowBorder.Child as Grid;
                for (int column = 0; column < afterGrid.Children.Count; column++)
                {
                    MexTableRowColumn label = afterGrid.Children[column] as MexTableRowColumn;
                    label.Foreground = SelectedStyle.RowForegroundDefault;
                }
            }
        }

        private void Row_MouseEnter(object sender, MouseEventArgs e)
        {
            MexTableRow mexTableRow = sender as MexTableRow;

            if (mexTableRow.RowBorder.Background != SelectedStyle.RowBackgroundSelected)
            {
                mexTableRow.RowBorder.Background = SelectedStyle.RowBackgroundHover;

                /// foreground
                Grid afterGrid = mexTableRow.RowBorder.Child as Grid;
                for (int column = 0; column < afterGrid.Children.Count; column++)
                {
                    MexTableRowColumn label = afterGrid.Children[column] as MexTableRowColumn;
                    label.Foreground = SelectedStyle.RowForegroundHover;
                }
            }
        }

        private static void BorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTable instance = d as MexTable;

            Thickness value = (Thickness)e.NewValue;

            instance.xBorder.BorderThickness = value;
        }

        private static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTable instance = d as MexTable;

            instance.Content = instance.xTopGrid;
        }

        private static void WidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTable instance = d as MexTable;

            double value = (double)e.NewValue;

            instance.xHeaderGrid.Width = value;
            instance.xContentScrollViewer.Width = value;
            instance.xRectangleGeometry.Rect = new Rect(
                0, 0, value, instance.Height);

            instance.UpdateHeaderColumn(instance.ColumnCount);
            instance.UpdateRowsColumn(instance.ColumnCount);
        }

        private static void HeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTable instance = d as MexTable;

            double value = (double)e.NewValue;

            instance.xRectangleGeometry.Rect = new Rect(
                0, 0, instance.Width, value);

            instance.UpdateContentHeight(value);

            instance.UpdateHeaderColumn(instance.ColumnCount);
            instance.UpdateRowsColumn(instance.ColumnCount);
        }

        public void UpdateContentHeight(double height)
        {
            if (height < xHeaderGrid.ActualHeight)
            {
                xContentScrollViewer.Height = 0;
            }
            else
            {
                xContentScrollViewer.Height = height - xHeaderRowDefinition.Height.Value;
            }
        }

        static MexTable()
        {
            IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(MexTable), new PropertyMetadata(false));
            ColorStyleProperty = DependencyProperty.Register("ColorStyle", typeof(EColorStyle), typeof(MexTable), new FrameworkPropertyMetadata(EColorStyle.Primary, (PropertyChangedCallback)ColorStylePropertyChanged));
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexTable), new FrameworkPropertyMetadata(new CornerRadius(0), (PropertyChangedCallback)CornerRadiusPropertyChanged));
            HeaderProperty = DependencyProperty.Register("Header", typeof(MexTableHeader), typeof(MexTable), new FrameworkPropertyMetadata(null, (PropertyChangedCallback)HeaderPropertyChanged));
            ColumnCountProperty = DependencyProperty.Register("ColumnNum", typeof(uint), typeof(MexTable), new FrameworkPropertyMetadata((uint)0, (PropertyChangedCallback)ColumnCountPropertyChanged));
            SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(MexTable), new FrameworkPropertyMetadata(-1));

            BorderThicknessProperty.OverrideMetadata(typeof(MexTable), new FrameworkPropertyMetadata(new Thickness(1), (PropertyChangedCallback)BorderThicknessPropertyChanged));
            ContentProperty.OverrideMetadata(typeof(MexTable), new FrameworkPropertyMetadata((PropertyChangedCallback)ContentPropertyChanged));
            WidthProperty.OverrideMetadata(typeof(MexTable), new FrameworkPropertyMetadata(double.NaN, (PropertyChangedCallback)WidthPropertyChanged));
            HeightProperty.OverrideMetadata(typeof(MexTable), new FrameworkPropertyMetadata(double.NaN, (PropertyChangedCallback)HeightPropertyChanged));
        }

        #endregion

        #region Component 초기화

        /// <summary>
        /// UI 컴포넌트들
        /// </summary>
        public Grid xTopGrid;
        public RectangleGeometry xRectangleGeometry;

        public RowDefinition xHeaderRowDefinition;
        public RowDefinition xContentRowDefinition;

        public Grid xHeaderGrid;
        public MexScrollViewer xContentScrollViewer;
        public Border xBorder;

        public StackPanel xContentStackPanel;

        private void InitializeComponent()
        {
            xTopGrid = new Grid();
            xRectangleGeometry = new RectangleGeometry();

            xHeaderRowDefinition = new RowDefinition();
            xContentRowDefinition = new RowDefinition();

            xHeaderGrid = new Grid();
            xContentScrollViewer = new MexScrollViewer();
            xBorder = new Border();

            xContentStackPanel = new StackPanel();

            /// 상하 관계 설정
            Content = xTopGrid;
            xTopGrid.Clip = xRectangleGeometry;
            xTopGrid.RowDefinitions.Add(xHeaderRowDefinition);
            xTopGrid.RowDefinitions.Add(xContentRowDefinition);
            xTopGrid.Children.Add(xHeaderGrid);
            Grid.SetRow(xHeaderGrid, 0);
            xTopGrid.Children.Add(xContentScrollViewer);
            Grid.SetRow(xContentScrollViewer, 1);
            xTopGrid.Children.Add(xBorder);
            Grid.SetRow(xBorder, 0);
            Grid.SetRowSpan(xBorder, 100);
            xContentScrollViewer.Content = xContentStackPanel;

            /// 초기 값 설정
            xRectangleGeometry.Rect = new Rect(0, 0, Width, Height);
            xHeaderRowDefinition.Height = new GridLength(0);
            xContentRowDefinition.Height = GridLength.Auto;
            xHeaderGrid.Background = SelectedStyle.HeaderBackground;
            xContentScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            xContentScrollViewer.Background = SelectedStyle.ContentBackground;
            xBorder.BorderBrush = SelectedStyle.BorderBrush;
            xBorder.BorderThickness = new Thickness(1);
        }

        #endregion

        public MexTable()
        {
            InitializeStyle();

            InitializeComponent();

            _rows.CollectionChanged += OnRowsChanged;
        }

        public void SelectRow(MexTableRow sender)
        {
            Row_MouseLeftButtonDown(sender, null);
        }

        public void ScrollToVerticalOffset(double offset)
        {
            xContentScrollViewer.ScrollToVerticalOffset(offset);
        }

        private ObservableCollection<MexTableRow> _rows = new ObservableCollection<MexTableRow>();
        public ObservableCollection<MexTableRow> Rows => _rows;

        private void OnRowsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (MexTableRow item in e.NewItems)
                    {
                        item.SetParameters(this, e.NewStartingIndex);

                        xContentStackPanel.Children.Insert(e.NewStartingIndex, item);

                        UpdateRowColumn((uint)e.NewStartingIndex, ColumnCount);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (MexTableRow item in e.OldItems)
                    {
                        xContentStackPanel.Children.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    xContentStackPanel.Children.Clear();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (MexTableRow item in e.NewItems)
                    {
                        item.SetParameters(this, e.NewStartingIndex);

                        xContentStackPanel.Children.Insert(e.NewStartingIndex, item);

                        UpdateRowColumn((uint)e.NewStartingIndex, ColumnCount);
                    }
                    foreach (MexTableRow item in e.OldItems)
                    {
                        xContentStackPanel.Children.Remove(item);
                    }
                    break;
            }
        }
    }
}
