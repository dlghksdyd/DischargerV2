using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Sqlite.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DischargerV2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                Thread.Sleep(1000);

                Application.Current.Dispatcher.Invoke(() =>
                {

                    var main = new ViewMain();
                    main.Loaded += Main_Loaded;
                    main.Show();
                });

            });
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
