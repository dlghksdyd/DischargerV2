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
using System.Windows.Media.Animation;

namespace DischargerV2.MVVM.Views
{
    /// <summary>
    /// ViewPopup_Waiting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewPopup_Waiting : UserControl
    {
        public ViewPopup_Waiting()
        {
            InitializeComponent();

            this.DataContext = new ViewModelPopup_Waiting();

            Loaded += ViewPopup_Waiting_Loaded;
        }

        private void ViewPopup_Waiting_Loaded(object sender, RoutedEventArgs e)
        {
            if (xRotate != null)
            {
                var rotateAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 360,
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                xRotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }
        }
    }
}
