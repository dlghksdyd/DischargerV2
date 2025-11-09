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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MExpress.Example
{
    /// <summary>
    /// ExampleMexComboBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ExampleMexComboBoxHB : Window
    {
        public ExampleMexComboBoxHB()
        {
            InitializeComponent();
            ItemSourceTest();
        }

        private void ItemSourceTest()
        {
            List<string> list = new List<string>();

            for (int index = 0; index < 10; index++)
                list.Add("Placeholder" + (index + 1).ToString());

            xMexComboBox_1.ItemSource = list;
            xMexComboBox_2.ItemSource = list;
        }
    }
}
