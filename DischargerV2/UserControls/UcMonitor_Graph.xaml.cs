using MExpress.Mex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace DischargerV2.UserControls
{
    /// <summary>
    /// UcMonitor_Graph.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcMonitor_Graph : UserControl
    {
        public UcMonitor_Graph()
        {
            InitializeComponent();

            Loaded += UcGraph_Loaded;
        }

        private void UcGraph_Loaded(object sender, RoutedEventArgs e)
        {
            Rect rect = new Rect(0, 0, 0, 0);

            double width = xGraphCanvas.ActualWidth;
            double height = xGraphCanvas.ActualHeight;

            rect.Height = 0.2;
            rect.Width = 1 / Math.Ceiling(width / (height / 5));

            xGraphVisualBrush.Viewport = rect;
        }
    }
}
