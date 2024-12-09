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

namespace DischargerV2.UserControls
{
    /// <summary>
    /// UcModeTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcModeTab : UserControl
    {
        public UcModeTab()
        {
            InitializeComponent();
        }

        private void xModeTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MexTextBlock selectedTextBlock = sender as MexTextBlock;

            foreach (MexTextBlock mexTextBlock in xModeSelectStackPanel.Children)
            {
                if (mexTextBlock == selectedTextBlock)
                {
                    mexTextBlock.BorderBrush = ResColor.border_action_hover;
                    mexTextBlock.Background = ResColor.surface_action_hover2;
                }
                else
                {
                    mexTextBlock.BorderBrush = Brushes.Transparent;
                    mexTextBlock.Background = ResColor.surface_primary;
                }
            }
        }
    }
}
