using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace MExpress.Mex
{
    [ContentProperty("Columns")]
    public class MexTableRow : ContentControl
    {
        #region Property 설정

        private static void HeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTableRow instance = d as MexTableRow;

            double value = (double)e.NewValue;

            instance._rowBorder.Height = value;
            instance._rowGrid.Height = value;
        }

        static MexTableRow()
        {
            HeightProperty.OverrideMetadata(typeof(MexTableRow), new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(HeightPropertyChanged)));
        }

        #endregion

        public MexTableRow()
        {
            InitializeComponent();

            _columns.CollectionChanged += OnColumnsChanged;
        }

        private void InitializeComponent()
        {
            AddChild(_rowBorder);
            _rowBorder.Child = _rowGrid;
            _rowBorder.BorderThickness = new Thickness(0, 0, 0, 1);
        }

        #region ObservableCollection

        private ObservableCollection<MexTableRowColumn> _columns = new ObservableCollection<MexTableRowColumn>();
        public ObservableCollection<MexTableRowColumn> Columns => _columns;

        private void OnColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_mexTable == null)
            {
                return;
            }

            _mexTable.UpdateRowsColumn(_mexTable.ColumnCount);
        }

        #endregion

        private MexTable _mexTable = null;

        private Border _rowBorder = new Border();
        public Border RowBorder => _rowBorder;

        private Grid _rowGrid = new Grid();
        public Grid RowGrid => _rowGrid;

        public int RowNum = 0;

        public void SetParameters(MexTable mexTable, int rowNum)
        {
            _mexTable = mexTable;
            RowNum = rowNum;
        }

        public List<GridLength> GetColumnLengthsFromHeader()
        {
            List<GridLength> widthList = new List<GridLength>();

            if (_mexTable == null)
            {
                return widthList;
            }

            if (_mexTable.Header == null)
            {
                return widthList;
            }

            ObservableCollection<MexTableHeaderColumn> headerColumns = _mexTable.Header.Columns;

            foreach (MexTableHeaderColumn column in headerColumns)
            {
                widthList.Add(column.Width);
            }

            return widthList;
        }

        public void UpdateColumns()
        {
            if (_mexTable == null)
            {
                return;
            }

            _rowGrid.Children.Clear();
            _rowGrid.Width = _mexTable.Width;

            if (_mexTable.Header == null)
            {
                return;
            }

            List<GridLength> columnWidths = GetColumnLengthsFromHeader();

            for (int column = 0; column < _mexTable.Header.Columns.Count; column++)
            {
                if (column >= _mexTable.ColumnCount)
                    break;

                if (column < Columns.Count)
                {
                    Columns[column].SetParameters(_mexTable);
                    _rowGrid.ColumnDefinitions[column].Width = columnWidths[column];
                    _rowGrid.Children.Add(Columns[column]);
                    Grid.SetColumn(Columns[column], column);

                    Columns[column].Foreground = _mexTable.SelectedStyle.RowForegroundDefault;
                }
                else
                {
                    _rowGrid.ColumnDefinitions[column].Width = columnWidths[column];
                }
            }
        }
    }

    public class MexTableRowColumn : MexLabel
    {
        public MexTableRowColumn()
        {
            this.Padding = new Thickness(0);
            this.Margin = new Thickness(0);
            this.BorderThickness = new Thickness(0);
            this.BorderBrush = new SolidColorBrush(Colors.Transparent);
            this.VerticalAlignment = VerticalAlignment.Center;
            this.HorizontalAlignment = HorizontalAlignment.Left;
        }

        private MexTable _mexTable = null;

        public void SetParameters(MexTable mexTable)
        {
            _mexTable = mexTable;
        }
    }
}
