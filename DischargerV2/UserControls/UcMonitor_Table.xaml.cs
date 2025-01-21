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
                MexTextBlock tbNo = new MexTextBlock();
                tbNo.Margin = new Thickness(16, 0, 16, 0);
                tbNo.Text = (i + 1).ToString();
                tbNo.Foreground = ResColor.text_body;
                tbNo.FontSet = ResFontSet.body_md_regular;
                colNo.Content = tbNo;
                row.Columns.Add(colNo);

                /// Discharger Name
                MexTableRowColumn colDischargerName = new MexTableRowColumn();
                MexTextBlock tbDischargerName = new MexTextBlock();
                tbDischargerName.Margin = new Thickness(16, 0, 16, 0);
                tbDischargerName.Text = dischargerName;
                tbDischargerName.Foreground = ResColor.text_body;
                tbDischargerName.FontSet = ResFontSet.body_md_regular;
                colDischargerName.Content = tbDischargerName;
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
                MexTextBlock tbChannel = new MexTextBlock();
                tbChannel.Margin = new Thickness(16, 0, 16, 0);
                tbChannel.Text = dischargerInfo.DischargerChannel.ToString();
                tbChannel.Foreground = ResColor.text_body;
                tbChannel.FontSet = ResFontSet.body_md_regular;
                colChannel.Content = tbChannel;
                row.Columns.Add(colChannel);

                /// Status
                MexTableRowColumn colStatus = new MexTableRowColumn();

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

                MexTextBlock tbStatus = new MexTextBlock();
                tbStatus.DataContext = _viewModelDischarger;
                tbStatus.Foreground = ResColor.text_body;
                tbStatus.FontSet = ResFontSet.body_md_regular;
                tbStatus.Padding = new Thickness(8, 0, 0, 0);
                BindingOperations.SetBinding(tbStatus, MexTextBlock.TextProperty,
                    new Binding("Model.DischargerStates[" + i + "]"));
                spStatus.Children.Add(tbStatus);

                row.Columns.Add(colStatus);

                /// Voltage
                MexTableRowColumn colVoltage = new MexTableRowColumn();
                MexTextBlock tbVoltage = new MexTextBlock();
                tbVoltage.DataContext = _viewModelDischarger;
                tbVoltage.Margin = new Thickness(16, 0, 16, 0);
                tbVoltage.Foreground = ResColor.text_body;
                tbVoltage.FontSet = ResFontSet.body_md_regular;
                BindingOperations.SetBinding(tbVoltage, MexTextBlock.TextProperty,
                    new Binding("Model.DischargerDatas[" + i + "].ReceiveBatteryVoltage"));
                colVoltage.Content = tbVoltage;
                row.Columns.Add(colVoltage);

                /// Current
                MexTableRowColumn colCurrent = new MexTableRowColumn();
                MexTextBlock tbCurrent = new MexTextBlock();
                tbCurrent.DataContext = _viewModelDischarger;
                tbCurrent.Margin = new Thickness(16, 0, 16, 0);
                tbCurrent.Foreground = ResColor.text_body;
                tbCurrent.FontSet = ResFontSet.body_md_regular;
                BindingOperations.SetBinding(tbCurrent, MexTextBlock.TextProperty,
                    new Binding("Model.DischargerDatas[" + i + "].ReceiveDischargeCurrent"));
                colCurrent.Content = tbCurrent;
                row.Columns.Add(colCurrent);

                /// Temperature
                MexTableRowColumn colTemperature = new MexTableRowColumn();
                MexTextBlock tbTemperature = new MexTextBlock();
                tbTemperature.DataContext = _viewModelTempModule;
                tbTemperature.Margin = new Thickness(16, 0, 16, 0);
                tbTemperature.Foreground = ResColor.text_body;
                tbTemperature.FontSet = ResFontSet.body_md_regular;
                int index = _viewModelTempModule.GetTempModuleDataIndex(dischargerInfo.TempModuleComPort);
                BindingOperations.SetBinding(tbTemperature, MexTextBlock.TextProperty,
                    new Binding("Model.TempDatas[" + index + "][" + dischargerInfo.TempChannel + "]"));
                colTemperature.Content = tbTemperature;
                row.Columns.Add(colTemperature);

                /// Progress Time
                MexTableRowColumn colProgressTime = new MexTableRowColumn();
                MexTextBlock tbProgressTime = new MexTextBlock();
                tbProgressTime.DataContext = _viewModelDischarger;
                tbProgressTime.Margin = new Thickness(16, 0, 16, 0);
                tbProgressTime.Foreground = ResColor.text_body;
                tbProgressTime.FontSet = ResFontSet.body_md_regular;
                BindingOperations.SetBinding(tbProgressTime, MexTextBlock.TextProperty,
                    new Binding("Model.ProgressTime[" + i + "]"));
                colProgressTime.Content = tbProgressTime;
                row.Columns.Add(colProgressTime);

                /// Info
                MexTableRowColumn colInfo = new MexTableRowColumn();
                Grid gridInfo = new Grid();
                colInfo.Content = gridInfo;

                /// Info -> Error
                StackPanel spError = new StackPanel();
                spError.Orientation = Orientation.Horizontal;
                spError.Margin = new Thickness(16, 0, 16, 0);
                gridInfo.Children.Add(spError);
                Image imageError = new Image();
                imageError.Width = 24;
                imageError.Height = 24;
                imageError.Margin = new Thickness(0, 0, 8, 0);
                ImageColorConverter imageErrorConverter = new ImageColorConverter();
                imageError.Source = (ImageSource)imageErrorConverter.Convert(ResImage.error_outline, null, ResColor.icon_error, null);
                spError.Children.Add(imageError);
                MexTextBlock tbError = new MexTextBlock();
                tbError.Foreground = ResColor.text_error;
                tbError.FontSet = ResFontSet.body_md_medium;
                tbError.TextDecorations = TextDecorations.Underline;
                spError.Children.Add(tbError);

                /// Info -> Reconnect
                StackPanel spReconnect = new StackPanel();
                spReconnect.Orientation = Orientation.Horizontal;
                spReconnect.Margin = new Thickness(16, 0, 16, 0);
                gridInfo.Children.Add(spReconnect);
                Image imageReconnect = new Image();
                imageReconnect.Width = 24;
                imageReconnect.Height = 24;
                imageReconnect.Margin = new Thickness(0, 0, 8, 0);
                ImageColorConverter imageReconnectConverter = new ImageColorConverter();
                imageReconnect.Source = (ImageSource)imageReconnectConverter.Convert(ResImage.refresh, null, ResColor.icon_infomation, null);
                spReconnect.Children.Add(imageReconnect);
                MexTextBlock tbReconnect = new MexTextBlock();
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
