using DischargerV2.MVVM.ViewModels;
using MExpress.Mex;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DischargerV2.UserControls
{
    /// <summary>
    /// UcMonitor_Table.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcMonitor_Table : UserControl
    {
        private ViewModelDischarger _viewModelDischarger = null;
        private ViewModelTempModule _viewModelTempModule = null;

        public UcMonitor_Table()
        {
            InitializeComponent();

            _viewModelDischarger = ViewModelDischarger.Instance;
            _viewModelTempModule = ViewModelTempModule.Instance;

            InitializeUi();
        }

        private void InitializeUi()
        {
            xTable.Rows.Clear();

            for (int i = 0; i < _viewModelDischarger.Model.DischargerNameList.Count; i++)
            {
                string dischargerName = _viewModelDischarger.Model.DischargerNameList[i];
                TableDischargerInfo dischargerInfo = SqliteDischargerInfo.GetData().Find(x => x.DischargerName == dischargerName);

                MexTableRow row = new MexTableRow();
                row.Height = 40;

                /// No.
                MexTableRowColumn colNo = new MexTableRowColumn();
                colNo.Margin = new Thickness(16, 0, 16, 0);
                colNo.FontSet = ResFontSet.body_md_regular;
                colNo.Content = (i + 1).ToString(); ;
                row.Columns.Add(colNo);

                /// Discharger Name
                MexTableRowColumn colDischargerName = new MexTableRowColumn();
                colDischargerName.Margin = new Thickness(16, 0, 16, 0);
                colDischargerName.FontSet = ResFontSet.body_md_regular;
                colDischargerName.Content = dischargerName;
                row.Columns.Add(colDischargerName);

                /// Short Available
                MexTableRowColumn colShortAvailable = new MexTableRowColumn();
                MexLabel lbShortAvailable = new MexLabel();
                lbShortAvailable.Visibility = Visibility.Visible;
                lbShortAvailable.Margin = new Thickness(-8, 0, 16, 0);
                lbShortAvailable.CornerRadius = new CornerRadius(4);
                lbShortAvailable.Background = ResColor.surface_success;
                lbShortAvailable.Padding = new Thickness(8, 0, 8, 0);
                lbShortAvailable.Text = "Short Available";
                lbShortAvailable.Foreground = ResColor.text_success;
                lbShortAvailable.TextPadding = new Thickness(4, 0, 0, 0);
                lbShortAvailable.FontSet = ResFontSet.body_xs_regular;
                lbShortAvailable.ImageWidth = 16;
                lbShortAvailable.ImageHeight = 16;
                ImageColorConverter imageColorConverter = new ImageColorConverter();
                lbShortAvailable.ImageLeft = (BitmapSource)imageColorConverter.Convert(ResImage.electric_bolt, null, ResColor.icon_success, null);
                colShortAvailable.Content = lbShortAvailable;
                row.Columns.Add(colShortAvailable);

                /// Channel
                MexTableRowColumn colChannel = new MexTableRowColumn();
                colChannel.Margin = new Thickness(16, 0, 16, 0);
                colChannel.FontSet = ResFontSet.body_md_regular;
                colChannel.Content = dischargerInfo.DischargerChannel.ToString(); ;
                row.Columns.Add(colChannel);

                /// Status
                MexTableRowColumn colStatus = new MexTableRowColumn();
                colStatus.Name = "xStatus" + i;
                this.RegisterName(colStatus.Name, colStatus);

                StackPanel spStatus = new StackPanel();
                spStatus.Orientation = Orientation.Horizontal;
                spStatus.Margin = new Thickness(16, 0, 16, 0);
                colStatus.Content = spStatus;

                Ellipse elStatus = new Ellipse();
                elStatus.DataContext = _viewModelDischarger;
                elStatus.Width = 12;
                elStatus.Height = 12;
                BindingOperations.SetBinding(elStatus, Ellipse.FillProperty,
                    new Binding("Model.StateColor[" + i + "]"));
                spStatus.Children.Add(elStatus);

                Binding bdStatus = new Binding("Foreground");
                bdStatus.ElementName = "xStatus" + i;
                MexTextBlock tbStatus = new MexTextBlock();
                tbStatus.SetBinding(MexTextBlock.ForegroundProperty, bdStatus);
                tbStatus.DataContext = _viewModelDischarger;
                tbStatus.FontSet = ResFontSet.body_md_regular;
                tbStatus.Padding = new Thickness(8, 0, 0, 0);
                BindingOperations.SetBinding(tbStatus, MexTextBlock.TextProperty,
                    new Binding("Model.DischargerStates[" + i + "]"));
                spStatus.Children.Add(tbStatus);

                row.Columns.Add(colStatus);

                /// Voltage
                MexTableRowColumn colVoltage = new MexTableRowColumn();
                colVoltage.DataContext = _viewModelDischarger;
                colVoltage.Margin = new Thickness(16, 0, 16, 0);
                colVoltage.FontSet = ResFontSet.body_md_regular;
                BindingOperations.SetBinding(colVoltage, MexTableRowColumn.ContentProperty,
                    new Binding("Model.DischargerDatas[" + i + "].ReceiveBatteryVoltage"));
                row.Columns.Add(colVoltage);

                /// Current
                MexTableRowColumn colCurrent = new MexTableRowColumn();
                colCurrent.DataContext = _viewModelDischarger;
                colCurrent.Margin = new Thickness(16, 0, 16, 0);
                colCurrent.FontSet = ResFontSet.body_md_regular;
                BindingOperations.SetBinding(colCurrent, MexTableRowColumn.ContentProperty,
                    new Binding("Model.DischargerDatas[" + i + "].ReceiveDischargeCurrent"));
                row.Columns.Add(colCurrent);

                /// Temperature
                MexTableRowColumn colTemperature = new MexTableRowColumn();
                colTemperature.DataContext = _viewModelTempModule;
                colTemperature.Margin = new Thickness(16, 0, 16, 0);
                colTemperature.FontSet = ResFontSet.body_md_regular;
                int index = _viewModelTempModule.GetTempModuleDataIndex(dischargerInfo.TempModuleComPort);
                BindingOperations.SetBinding(colTemperature, MexTextBlock.ContentProperty,
                    new Binding("Model.TempDatas[" + index + "][" + dischargerInfo.TempChannel + "]"));
                row.Columns.Add(colTemperature);

                /// Progress Time
                MexTableRowColumn colProgressTime = new MexTableRowColumn();
                colProgressTime.DataContext = _viewModelDischarger;
                colProgressTime.Margin = new Thickness(16, 0, 16, 0);
                colProgressTime.FontSet = ResFontSet.body_md_regular;
                BindingOperations.SetBinding(colProgressTime, MexTableRowColumn.ContentProperty,
                    new Binding("Model.ProgressTime[" + i + "]"));
                row.Columns.Add(colProgressTime);

                /// Info
                MexTableRowColumn colInfo = new MexTableRowColumn();
                Grid gridInfo = new Grid();
                colInfo.Content = gridInfo;

                /// Info -> Error
                StackPanel spError = new StackPanel();
                spError.DataContext = _viewModelDischarger;
                spError.Orientation = Orientation.Horizontal;
                spError.Margin = new Thickness(16, 0, 16, 0);
                BindingOperations.SetBinding(spError, StackPanel.VisibilityProperty,
                    new Binding("Model.ErrorVisibility[" + i + "]"));
                gridInfo.Children.Add(spError);
                Image imageError = new Image();
                imageError.Width = 24;
                imageError.Height = 24;
                imageError.Margin = new Thickness(0, 0, 8, 0);
                ImageColorConverter imageErrorConverter = new ImageColorConverter();
                imageError.Source = (ImageSource)imageErrorConverter.Convert(ResImage.error_outline, null, ResColor.icon_error, null);
                spError.Children.Add(imageError);
                MexTextBlock tbError = new MexTextBlock();
                tbError.Text = "Error";
                tbError.Foreground = ResColor.text_error;
                tbError.FontSet = ResFontSet.body_md_medium;
                tbError.TextDecorations = TextDecorations.Underline;
                spError.Children.Add(tbError);

                /// Info -> Reconnect
                StackPanel spReconnect = new StackPanel();
                spReconnect.DataContext = _viewModelDischarger;
                spReconnect.Orientation = Orientation.Horizontal;
                spReconnect.Margin = new Thickness(16, 0, 16, 0);
                BindingOperations.SetBinding(spReconnect, StackPanel.VisibilityProperty,
                    new Binding("Model.ReconnectVisibility[" + i + "]"));
                gridInfo.Children.Add(spReconnect);
                Image imageReconnect = new Image();
                imageReconnect.Width = 24;
                imageReconnect.Height = 24;
                imageReconnect.Margin = new Thickness(0, 0, 8, 0);
                ImageColorConverter imageReconnectConverter = new ImageColorConverter();
                imageReconnect.Source = (ImageSource)imageReconnectConverter.Convert(ResImage.refresh, null, ResColor.icon_infomation, null);
                spReconnect.Children.Add(imageReconnect);
                MexTextBlock tbReconnect = new MexTextBlock();
                tbReconnect.Text = "Reconnect";
                tbReconnect.Foreground = ResColor.text_infomation;
                tbReconnect.FontSet = ResFontSet.body_md_medium;
                tbReconnect.TextDecorations = TextDecorations.Underline;
                spReconnect.Children.Add(tbReconnect);

                row.Columns.Add(colInfo);

                xTable.Rows.Add(row);
            }
        }
    }
}
