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

            _viewModelDischarger = ViewModelDischarger.Instance();
            _viewModelTempModule = new ViewModelTempModule();

            InitializeUI();
        }

        public void InitializeUI()
        {
            xTable.Rows.Clear();

            for (int i = 0; i < _viewModelDischarger.Model.DischargerNameList.Count; i++)
            {
                string dischargerName = _viewModelDischarger.Model.DischargerNameList[i];
                TableDischargerInfo dischargerInfo = SqliteDischargerInfo.GetData().Find(x => x.DischargerName == dischargerName);

                MexTableRow row = new MexTableRow();
                
                MexTableRowColumn column0 = new MexTableRowColumn();
                MexTextBlock textBlock0 = new MexTextBlock();
                textBlock0.Margin = new Thickness(16, 0, 16, 0);
                textBlock0.Text = (i + 1).ToString();
                textBlock0.Foreground = ResColor.text_body;
                textBlock0.FontSet = ResFontSet.body_md_regular;
                column0.Content = textBlock0;
                row.Columns.Add(column0);

                MexTableRowColumn column1 = new MexTableRowColumn();
                MexTextBlock textBlock1 = new MexTextBlock();
                textBlock1.Margin = new Thickness(16, 0, 16, 0);
                BindingOperations.SetBinding(textBlock1, MexTextBlock.TextProperty, 
                    new Binding("Model.DischargerNameList[" + i + "]"));
                textBlock1.Foreground = ResColor.text_body;
                textBlock1.FontSet = ResFontSet.body_md_regular;
                column1.Content = textBlock1;
                row.Columns.Add(column1);

                MexTableRowColumn column2 = new MexTableRowColumn();
                MexTextBlock textBlock2 = new MexTextBlock();
                column2.Content = textBlock2;
                row.Columns.Add(column2);

                /// channel
                MexTableRowColumn column3 = new MexTableRowColumn();
                MexTextBlock textBlock3 = new MexTextBlock();
                column3.Content = textBlock3;
                row.Columns.Add(column3);

                /// status
                MexTableRowColumn column4 = new MexTableRowColumn();
                MexTextBlock textBlock4 = new MexTextBlock();
                column3.Content = textBlock4;
                row.Columns.Add(column4);

                /// Voltage
                MexTableRowColumn column5 = new MexTableRowColumn();
                MexTextBlock textBlock5 = new MexTextBlock();
                BindingOperations.SetBinding(textBlock5, MexTextBlock.TextProperty,
                    new Binding("Model.DischargerDatas[" + i + "].ReceiveBatteryVoltage"));
                column5.Content = textBlock5;
                row.Columns.Add(column5);

                /// Current
                MexTableRowColumn column6 = new MexTableRowColumn();
                MexTextBlock textBlock6 = new MexTextBlock();
                BindingOperations.SetBinding(textBlock6, MexTextBlock.TextProperty,
                    new Binding("Model.DischargerDatas[" + i + "].ReceiveDischargeCurrent"));
                column6.Content = textBlock6;
                row.Columns.Add(column6);

                /// Temperature
                int tempModuleDataIndex = _viewModelTempModule.GetTempModuleDataIndex(dischargerInfo.TempModuleComPort);
                MexTableRowColumn column7 = new MexTableRowColumn();
                MexTextBlock textBlock7 = new MexTextBlock();
                textBlock7.DataContext = _viewModelTempModule;
                BindingOperations.SetBinding(textBlock7, MexTextBlock.TextProperty,
                    new Binding("Model.TempDatas[" + tempModuleDataIndex + "]" + "[" + dischargerInfo.TempChannel + "]"));
                column7.Content = textBlock7;
                row.Columns.Add(column7);

                xTable.Rows.Add(row);
            }
        }
    }
}
