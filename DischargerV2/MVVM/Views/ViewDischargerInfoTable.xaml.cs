using DischargerV2.MVVM.ViewModels;
using MExpress.Mex;
using Microsoft.Xaml.Behaviors;
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
using Utility.Common;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewDischargerInfoTable.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewDischargerInfoTable : UserControl
    {
        private ViewModelDischarger _viewModelDischarger = null;
        private ViewModelTempModule _viewModelTempModule = null;

        public ViewDischargerInfoTable()
        {
            InitializeComponent();

            _viewModelDischarger = ViewModelDischarger.Instance;
            _viewModelTempModule = ViewModelTempModule.Instance;

            InitializeUi();
        }

        public void UpdateUi()
        {
            _viewModelDischarger.InitializeDischarger();

            InitializeUi();
        }

        private void InitializeUi()
        {
            foreach (var row in xTable.Rows)
            {
                foreach(var column in row.Columns)
                {
                    if (!string.IsNullOrEmpty(column.Name))
                    {
                        UnregisterName(column.Name);
                    }
                }
            }

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
                MexTableRowColumn tempRowColumn = new MexTableRowColumn();
                tempRowColumn.Name = "xTemp" + i;
                this.RegisterName(tempRowColumn.Name, tempRowColumn);

                StackPanel tempStackPanel = new StackPanel();
                tempStackPanel.DataContext = _viewModelTempModule;
                tempStackPanel.Orientation = Orientation.Horizontal;
                tempStackPanel.Margin = new Thickness(16, 0, 16, 0);

                if (dischargerInfo.IsTempModule)
                {
                    MexTextBlock tempTextBlock = new MexTextBlock();
                    Binding foregroundBinding = new Binding("Foreground");
                    foregroundBinding.ElementName = "xTemp" + i;
                    tempTextBlock.SetBinding(MexTextBlock.ForegroundProperty, foregroundBinding);
                    tempTextBlock.FontSet = ResFontSet.body_md_regular;
                    int comportIndex = _viewModelTempModule.Model.TempModuleDictionary[dischargerName].ComportIndex;
                    BindingOperations.SetBinding(tempTextBlock, MexTextBlock.TextProperty,
                       new Binding("Model.TempDatas[" + comportIndex + "][" + dischargerInfo.TempChannel + "]"));

                    tempStackPanel.Children.Add(tempTextBlock);

                    Image reconnectTempImage = new Image();
                    reconnectTempImage.Width = 24;
                    reconnectTempImage.Height = 24;
                    reconnectTempImage.Margin = new Thickness(8, 0, 0, 0);
                    reconnectTempImage.Cursor = Cursors.Hand;
                    reconnectTempImage.Source = (ImageSource)new ImageColorConverter().Convert(ResImage.refresh, null, ResColor.icon_infomation, null);

                    BindingOperations.SetBinding(reconnectTempImage, Image.VisibilityProperty,
                        new Binding("Model.ReconnectVisibility[" + comportIndex + "]"));

                    var reconnectTempModuleTriggerCollection = Interaction.GetTriggers(reconnectTempImage);
                    var reconnectTempModuleEventTrigger = new Microsoft.Xaml.Behaviors.EventTrigger();
                    reconnectTempModuleEventTrigger.EventName = "MouseLeftButtonUp";
                    var reconnectTempModuleAction = new InvokeCommandAction();
                    reconnectTempModuleAction.PassEventArgsToCommand = false;
                    reconnectTempModuleAction.CommandParameter = dischargerName;
                    BindingOperations.SetBinding(reconnectTempModuleAction, InvokeCommandAction.CommandProperty,
                        new Binding("ReconnectTempModuleCommand"));
                    reconnectTempModuleEventTrigger.Actions.Add(reconnectTempModuleAction);
                    reconnectTempModuleTriggerCollection.Add(reconnectTempModuleEventTrigger);

                    tempStackPanel.Children.Add(reconnectTempImage);
                }
                else
                {
                    MexTextBlock tempTextBlock = new MexTextBlock();
                    tempTextBlock.DataContext = _viewModelDischarger;
                    Binding foregroundBinding = new Binding("Foreground");
                    foregroundBinding.ElementName = "xTemp" + i;
                    tempTextBlock.SetBinding(MexTextBlock.ForegroundProperty, foregroundBinding);
                    tempTextBlock.FontSet = ResFontSet.body_md_regular;
                    BindingOperations.SetBinding(tempTextBlock, MexTextBlock.TextProperty,
                       new Binding("Model.DischargerDatas[" + i + "].ReceiveDischargeTemp"));

                    tempStackPanel.Children.Add(tempTextBlock);
                }

                tempRowColumn.Content = tempStackPanel;

                row.Columns.Add(tempRowColumn);

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

                var errorTriggerCollection = Interaction.GetTriggers(spError);
                var errorEventTrigger = new Microsoft.Xaml.Behaviors.EventTrigger();
                errorEventTrigger.EventName = "MouseLeftButtonUp";
                var errorAction = new InvokeCommandAction();
                errorAction.PassEventArgsToCommand = false;
                errorAction.CommandParameter = dischargerName;
                BindingOperations.SetBinding(errorAction,
                    InvokeCommandAction.CommandProperty,
                    new Binding("OpenPopupErrorCommand"));
                errorEventTrigger.Actions.Add(errorAction);
                errorTriggerCollection.Add(errorEventTrigger);

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

                var reconnectTriggerCollection = Interaction.GetTriggers(spReconnect);
                var reconnectEventTrigger = new Microsoft.Xaml.Behaviors.EventTrigger();
                reconnectEventTrigger.EventName = "MouseLeftButtonUp";
                var reconnectAction = new InvokeCommandAction();
                reconnectAction.PassEventArgsToCommand = false;
                reconnectAction.CommandParameter = dischargerName;
                BindingOperations.SetBinding(reconnectAction,
                    InvokeCommandAction.CommandProperty,
                    new Binding("ReconnectDischargerCommand"));
                reconnectEventTrigger.Actions.Add(reconnectAction);
                reconnectTriggerCollection.Add(reconnectEventTrigger);

                gridInfo.Children.Add(spReconnect);

                Image imageReconnect = new Image();
                imageReconnect.Width = 24;
                imageReconnect.Height = 24;
                imageReconnect.Margin = new Thickness(0, 0, 8, 0);
                imageReconnect.Source = (ImageSource)new ImageColorConverter().Convert(ResImage.refresh, null, ResColor.icon_infomation, null);
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

            // 첫 번째 방전기 선택
            xTable.SelectRow(xTable.Rows[0]);

            if (_viewModelDischarger.Model.DischargerNameList.Count > 0)
            {
                string dischargerName = _viewModelDischarger.Model.DischargerNameList[0];

                _viewModelDischarger.Model.SelectedDischargerName = dischargerName;

                ViewModelMain.Instance.Model.DischargerIndex = 0;
                ViewModelMain.Instance.Model.SelectedDischargerName = dischargerName;
                ViewModelSetMode.Instance.SetDischargerName(dischargerName);
            }
        }
    }
}
