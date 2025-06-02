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

        }

        private bool _isCRateCalculating = false;
        public void CalculateCRate()
        {
            if (_isCurrentCalculating) return;

            _isCRateCalculating = true;

            ViewModelSetMode_Step viewModelSetMode_Step = ViewModelSetMode_Step.Instance;

            if (Model.Current == null || Model.Current == "")
            {
                _isCRateCalculating = false;
                return;
            }

            if (viewModelSetMode_Step.Model.StandardCapacity == null || viewModelSetMode_Step.Model.StandardCapacity == "")
            {
                _isCRateCalculating = false;
                return;
            }

            double standardCapacity = Convert.ToDouble(viewModelSetMode_Step.Model.StandardCapacity);

            if (double.TryParse(Model.Current, out double current))
            {
                Model.CRate = (current / standardCapacity).ToString("F1");
            }

            _isCRateCalculating = false;
        }

        private bool _isCurrentCalculating = false;
        public void CalculateCurrent()
        {
            if (_isCRateCalculating) return;

            _isCurrentCalculating = true;

            ViewModelSetMode_Step viewModelSetMode_Step = ViewModelSetMode_Step.Instance;

            if (Model.CRate == null || Model.CRate == "")
            {
                _isCurrentCalculating = false;
                return;
            }

            if (viewModelSetMode_Step.Model.StandardCapacity == null || viewModelSetMode_Step.Model.StandardCapacity == "")
            {
                Model.Current = "";
                Model.CRate = "";

                MessageBox.Show("Standard Capacity: 필수 정보입니다.");

                _isCurrentCalculating = false;

                return;
            }

            double standardCapacity = Convert.ToDouble(viewModelSetMode_Step.Model.StandardCapacity);

            if (double.TryParse(Model.CRate, out double cRate))
            {
                Model.Current = (standardCapacity * cRate).ToString("F1");
            }

            _isCurrentCalculating = false;
        }
    }
}
