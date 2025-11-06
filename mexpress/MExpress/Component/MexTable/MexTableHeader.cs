using MExpress.Mex;
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

namespace MExpress.Mex
{
    [ContentProperty("Columns")]
    public class MexTableHeader : DependencyObject
    {
        public static readonly DependencyProperty HeightProperty;
        public static readonly DependencyProperty FontSetProperty;

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public FontSet FontSet
        {
            get { return (FontSet)GetValue(FontSetProperty); }
            set { SetValue(FontSetProperty, value); }
        }

        private static void HeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTableHeader instance = d as MexTableHeader;

            double value = (double)e.NewValue;

            if (instance._mexTable != null)
            {
                instance._mexTable.xHeaderRowDefinition.Height = new GridLength(value, GridUnitType.Pixel);
                instance._mexTable.xHeaderGrid.Height = value;
            }
        }

        private static void FontSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTableHeader instance = d as MexTableHeader;

            FontSet value = (FontSet)e.NewValue;

            if (instance._mexTable != null)
            {
                for (int column = 0; column < instance.Columns.Count; column++)
                {
                    instance.Columns[column].ContentLabel.FontSize = value.FontSize;
                    instance.Columns[column].ContentLabel.Height = value.FontHeight;
                    instance.Columns[column].ContentLabel.FontWeight = value.FontWeight;
                }
            }
        }

        static MexTableHeader()
        {
            HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(MexTableHeader), new FrameworkPropertyMetadata(30.0, (PropertyChangedCallback)HeightPropertyChanged));
            FontSetProperty = DependencyProperty.Register("FontSet", typeof(FontSet), typeof(MexTableHeader), new FrameworkPropertyMetadata(ResFontSet.body_lg_medium, (PropertyChangedCallback)FontSetPropertyChanged));
        }

        public MexTableHeader()
        {
            _columns.CollectionChanged += OnColumnsChanged;
        }

        private MexTable _mexTable = null;

        private ObservableCollection<MexTableHeaderColumn> _columns = new ObservableCollection<MexTableHeaderColumn>();
        public ObservableCollection<MexTableHeaderColumn> Columns => _columns;

        private void OnColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_mexTable == null)
            {
                return;
            }

            _mexTable.UpdateHeaderColumn(_mexTable.ColumnCount);
            _mexTable.UpdateRowsColumn(_mexTable.ColumnCount);
        }

        public void SetMexTableComponent(MexTable mexTable)
        {
            _mexTable = mexTable;
        }

        public void UpdateColumns()
        {
            if (_mexTable == null)
            {
                return;
            }

            _mexTable.xHeaderGrid.Children.Clear();

            for (int column = 0; column < _mexTable.ColumnCount; column++)
            {
                if (column >= Columns.Count)
                    break;

                Columns[column].SetParameters(_mexTable);

                _mexTable.xHeaderGrid.ColumnDefinitions[column].Width = Columns[column].Width;
                _mexTable.xHeaderGrid.Children.Add(Columns[column].ContentLabel);
                Grid.SetColumn(Columns[column].ContentLabel, column);
            }
        }
    }

    [ContentProperty("Content")]
    public class MexTableHeaderColumn : DependencyObject
    {
        #region Property 설정

        public static readonly DependencyProperty ContentProperty;
        public static readonly DependencyProperty WidthProperty;
        public static readonly DependencyProperty HorizontalContentAlignmentProperty;

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public GridLength Width
        {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        private static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTableHeaderColumn instance = d as MexTableHeaderColumn;

            object value = (object)e.NewValue;

            instance._contentLabel.Content = value;
        }

        private static void WidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTableHeaderColumn instance = d as MexTableHeaderColumn;

            if (instance._mexTable != null)
            {
                instance._mexTable.UpdateHeaderColumn(instance._mexTable.ColumnCount);
                instance._mexTable.UpdateRowsColumn(instance._mexTable.ColumnCount);
            }
        }

        private static void HorizontalContentAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MexTableHeaderColumn instance = d as MexTableHeaderColumn;

            instance._contentLabel.HorizontalContentAlignment = (HorizontalAlignment)e.NewValue;

            if (instance._mexTable != null)
            {
                instance._mexTable.UpdateHeaderColumn(instance._mexTable.ColumnCount);
                instance._mexTable.UpdateRowsColumn(instance._mexTable.ColumnCount);
            }
        }

        static MexTableHeaderColumn()
        {
            ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(MexTableHeaderColumn), new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(ContentPropertyChanged)));
            WidthProperty = DependencyProperty.Register("Width", typeof(GridLength), typeof(MexTableHeaderColumn), new FrameworkPropertyMetadata(new GridLength(1.0, GridUnitType.Star), new PropertyChangedCallback(WidthPropertyChanged)));
            HorizontalContentAlignmentProperty = DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(MexTableHeaderColumn), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, new PropertyChangedCallback(HorizontalContentAlignmentPropertyChanged)));
        }

        #endregion

        public MexTableHeaderColumn()
        {
            _contentLabel.Padding = new Thickness(0);
            _contentLabel.Margin = new Thickness(0);
            _contentLabel.BorderThickness = new Thickness(0,0,0,1);
            _contentLabel.BorderBrush = new SolidColorBrush(ResColor.border_primary.Color);
            _contentLabel.VerticalContentAlignment = VerticalAlignment.Center;
        }

        private Label _contentLabel = new Label();
        public Label ContentLabel => _contentLabel;

        private MexTable _mexTable = null;

        public void SetParameters(MexTable mexTable)
        {
            _mexTable = mexTable;
        }
    }
}
