using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelSetMode_StepData : BindableBase
    {
        #region Command
        public DelegateCommand CalculateCurrentCommand { get; set; }
        public DelegateCommand CalculateCRateCommand { get; set; }
        #endregion

        #region Model
        public ModelSetMode_StepData Model { get; set; } = new ModelSetMode_StepData();
        #endregion

        #region Property
        public string No
        {
            get => Model.No;
            set => Model.No = value;
        }

        public string Voltage
        {
            get => Model.Voltage;
            set => Model.Voltage = value;
        }

        public string Current
        {
            get => Model.Current;
            set => Model.Current = value;
        }

        public string CRate
        {
            get => Model.CRate;
            set => Model.CRate = value;
        }
        #endregion

        public ViewModelSetMode_StepData()
        {
            CalculateCurrentCommand = new DelegateCommand(CalculateCurrent);
            CalculateCRateCommand = new DelegateCommand(CalculateCRate);
        }

        private void CalculateCurrent()
        {
            ViewModelSetMode_Step viewModelSetMode_Step = ViewModelSetMode_Step.Instance;

            if (Model.Current == null || Model.Current == "")
                return;

            if (viewModelSetMode_Step.Model.StandardCapacity == null || viewModelSetMode_Step.Model.StandardCapacity == "")
                return;

            double standardCapacity = Convert.ToDouble(viewModelSetMode_Step.Model.StandardCapacity);

            if (double.TryParse(Model.Current, out double current))
            {
                Model.CRate = (current / standardCapacity).ToString("F1");
            }
        }

        private void CalculateCRate()
        {
            ViewModelSetMode_Step viewModelSetMode_Step = ViewModelSetMode_Step.Instance;

            if (Model.CRate == null || Model.CRate == "")
                return;

            if (viewModelSetMode_Step.Model.StandardCapacity == null || viewModelSetMode_Step.Model.StandardCapacity == "")
            {
                Model.Current = "";
                Model.CRate = "";

                MessageBox.Show("Standard Capacity: 필수 정보입니다.");

                return;
            }

            double standardCapacity = Convert.ToDouble(viewModelSetMode_Step.Model.StandardCapacity);

            if (double.TryParse(Model.CRate, out double cRate))
            {
                Model.Current = (standardCapacity * cRate).ToString("F1");
            }
        }
    }
}
