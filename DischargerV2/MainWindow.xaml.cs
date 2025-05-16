using System.Windows;
using Sqlite.Common;

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

            SqliteUtility.InitializeDatabases();

            Instance = this;
        }
    }
}
