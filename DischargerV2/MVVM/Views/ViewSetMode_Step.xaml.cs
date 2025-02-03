using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.ViewModels;
using MExpress.Mex;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
    /// ViewSetMode_Step.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewSetMode_Step : UserControl
    {
        private ViewModelSetMode_Step _viewModel = new ViewModelSetMode_Step();

        public ViewSetMode_Step()
        {
            InitializeComponent();

            this.DataContext = _viewModel;

            _viewModel.Model.Content.CollectionChanged += Content_CollectionChanged;

            InitializeUI();
        }

        private void Content_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateUI();
        }

        private void InitializeUI()
        {
            _viewModel.Model.Content.Add(new ModelSetMode_StepData());
        }

        private void UpdateUI()
        {
            xContentPanel.Children.Clear();

            ObservableCollection<ModelSetMode_StepData> content = new ObservableCollection<ModelSetMode_StepData>();

            for (int index = 0; index < _viewModel.Model.Content.Count; index++)
            {
                ModelSetMode_StepData model = _viewModel.Model.Content[index];

                ViewModelSetMode_StepData viewModel = new ViewModelSetMode_StepData()
                {
                    No = (index + 1).ToString(),
                    IsFixedCurrent = model.IsFixedCurrent,
                    Voltage = model.Voltage,
                    Current = model.Current,
                    CRate = model.CRate,
                };

                ViewSetMode_StepData view = new ViewSetMode_StepData();
                view.DataContext = viewModel;

                xContentPanel.Children.Add(view);

                content.Add(viewModel.Model);

                if (index < _viewModel.Model.Content.Count - 1)
                {
                    xContentPanel.Children.Add(new Grid() { Height = 12 });
                }
            }
            _viewModel.Model.Content = content;
            _viewModel.Model.Content.CollectionChanged += Content_CollectionChanged;
        }
    }
}
