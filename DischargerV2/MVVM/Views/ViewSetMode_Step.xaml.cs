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
        private ViewModelSetMode_Step _viewModel = ViewModelSetMode_Step.Instance;

        public ViewSetMode_Step()
        {
            InitializeComponent();

            this.DataContext = _viewModel;

            _viewModel.Model.Content.CollectionChanged += Content_CollectionChanged;

            InitializeUI();
        }

        public void Content_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            string dischargerName = _viewModel.SelectedDischargerName;

            if (dischargerName == null || dischargerName == "")
                return;

            // 현재 값을 Dictionary Model에 넣기 위해 CollectionChanged 이벤트 발생되었을 때
            for (int index = 0; index < _viewModel.Model.Content.Count; index++)
            {
                if (_viewModel.Model.Content[index] == null || _viewModel.Model.Content[index].No == "")
                {
                    // 현재 값을 Dictionary Model에 넣음
                    ModelSetMode_Step model = new ModelSetMode_Step();

                    ObservableCollection<ModelSetMode_StepData> content = new ObservableCollection<ModelSetMode_StepData>();

                    foreach (var stepData in _viewModel.Model.Content)
                    {
                        content.Add(stepData);
                    }

                    model.DischargerName = dischargerName;
                    model.StandardCapacity = _viewModel.Model.StandardCapacity;
                    model.IsCompleteDischarge = _viewModel.Model.IsCompleteDischarge;
                    model.Content = content;

                    _viewModel.ModelDictionary[dischargerName] = model;
                    _viewModel.ModelDictionary[dischargerName].Content.RemoveAt(index);
                    _viewModel.ModelDictionary[dischargerName].Content.CollectionChanged += Content_CollectionChanged;

                    return;
                }
            }

            // ModelDictionary Model 다른 클래스에서 선언함에 따라 CollectionChanged Event 추가
            _viewModel.ModelDictionary[dischargerName].Content.CollectionChanged += Content_CollectionChanged;

            Application.Current.Dispatcher.Invoke(() =>
            {
                // UI 업데이트
                UpdateUI();
            });
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
                if (_viewModel.Model.Content[index] != null)
                {
                    ModelSetMode_StepData model = _viewModel.Model.Content[index];

                    ViewModelSetMode_StepData viewModel = new ViewModelSetMode_StepData()
                    {
                        No = (index + 1).ToString(),
                        Voltage = model.Voltage,
                        Current = model.Current,
                        CRate = model.CRate,
                        CRateEnabled = model.CRateEnabled,
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
            }
            _viewModel.Model.Content = content;
            _viewModel.Model.Content.CollectionChanged += Content_CollectionChanged;

            if (!(_viewModel.Model.Content.Count > 0))
            {
                _viewModel.Model.Content.Add(new ModelSetMode_StepData());
            }
        }

        private void xStandardCapacityTextBox_TextChanged(object sender, EventArgs e)
        {
            _viewModel.ChangeStandardCapacityText();
        }
    }
}
