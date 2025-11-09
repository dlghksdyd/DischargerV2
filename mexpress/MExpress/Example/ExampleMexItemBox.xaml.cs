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
using System.Windows.Shapes;

namespace MExpress.Example
{
    /// <summary>
    /// ExampleMexItemBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ExampleMexItemBox : Window
    {
        public ExampleMexItemBox()
        {
            InitializeComponent();

            List<string> list = new List<string>();

            for (int i = 1; i <= 10; i++)
            {
                list.Add("ItemBox Test " + i.ToString());
            }

            xMexItemBox.ItemSource = list;
        }
    }
}
