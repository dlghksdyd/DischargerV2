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
        #endregion

        #region Model
        public ModelSetMode_StepData Model { get; set; } = new ModelSetMode_StepData();

        public string No
        {
            get => Model.No;
            set => Model.No = value;
        }

        public bool IsFixedCurrent
        {
            get => Model.IsFixedCurrent;
            set => Model.IsFixedCurrent = value;
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
    }
}
