using DischargerV2.MVVM.Enums;
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
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static DischargerV2.MVVM.Models.ModelMain;
using static DischargerV2.MVVM.Models.ModelSetMode;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelSetMode : BindableBase
    {
        #region Command
        public DelegateCommand<string> SelectModeCommand { get; set; }
        #endregion

        #region Model
        public ModelSetMode Model { get; set; } = new ModelSetMode();

        public string SelectedDischargerName
        {
            get => Model.SelectedDischargerName;
            set => Model.SelectedDischargerName = value;
        }
        #endregion

        private static ViewModelSetMode _instance = new ViewModelSetMode();

        public static ViewModelSetMode Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelSetMode();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public ViewModelSetMode()
        {
            _instance = this;

            SelectModeCommand = new DelegateCommand<string>(SelectMode);
        }

        private void SelectMode(string mode)
        {
            foreach (EMode eMode in Enum.GetValues(typeof(EMode)))
            {
                if (mode.ToString() == eMode.ToString())
                {
                    Model.Mode = eMode;
                }
            }
        }
    }
}
