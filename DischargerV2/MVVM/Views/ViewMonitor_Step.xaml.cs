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
using MExpress.Mex;
using DischargerV2.MVVM.ViewModels;
using DischargerV2.MVVM.Models;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewMonitor_Step.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewMonitor_Step : UserControl
    {
        private ViewModelSetMode_Step _viewModel = ViewModelSetMode_Step.Instance;

        public ViewMonitor_Step()
        {
            InitializeComponent();

            this.DataContext = _viewModel;
            this.IsVisibleChanged += ViewMonitor_Step_IsVisibleChanged;
        }

        private void ViewMonitor_Step_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Table 초기화
            xTable.Rows.Clear();

            ViewModelSetMode_Step viewModelSetMode_Step = ViewModelSetMode_Step.Instance;
            
            for (int index = 0; index < viewModelSetMode_Step.Model.Content.Count; index++)
            {
                ModelSetMode_StepData stepData = viewModelSetMode_Step.Model.Content[index];

                MexTableRow mexTableRow = new MexTableRow()
                {
                    Height = 52
                };

                // No
                mexTableRow.Columns.Add(new MexTableRowColumn()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = new MexTextBlock()
                    {
                        Margin = new Thickness(16, 0, 16, 0),
                        Text = (index + 1).ToString(),
                        Foreground = ResColor.text_body,
                        FontSet = ResFontSet.body_md_regular
                    }
                });

                // Mode
                if (index == viewModelSetMode_Step.Model.Content.Count - 1 &&
                    viewModelSetMode_Step.Model.IsCompleteDischarge)
                {
                    mexTableRow.Columns.Add(new MexTableRowColumn()
                    {
                        Content = new MexTextBlock()
                        {
                            Margin = new Thickness(16, 0, 16, 0),
                            Text = "CCCV",
                            Foreground = ResColor.text_body,
                            FontSet = ResFontSet.body_md_regular
                        }
                    });
                }
                else
                {
                    mexTableRow.Columns.Add(new MexTableRowColumn()
                    {
                        Content = new MexTextBlock()
                        {
                            Margin = new Thickness(16, 0, 16, 0),
                            Text = "CC",
                            Foreground = ResColor.text_body,
                            FontSet = ResFontSet.body_md_regular
                        }
                    });
                }

                // Voltage
                mexTableRow.Columns.Add(new MexTableRowColumn()
                {
                    Content = new MexTextBlock()
                    {
                        Margin = new Thickness(16, 0, 16, 0),
                        Text = stepData.Voltage,
                        Foreground = ResColor.text_body,
                        FontSet = ResFontSet.body_md_regular
                    }
                });

                // Current
                mexTableRow.Columns.Add(new MexTableRowColumn()
                {
                    Content = new MexTextBlock()
                    {
                        Margin = new Thickness(16, 0, 16, 0),
                        Text = stepData.Current,
                        Foreground = ResColor.text_body,
                        FontSet = ResFontSet.body_md_regular
                    }
                });

                xTable.Rows.Add(mexTableRow);
            }
        }
    }
}
