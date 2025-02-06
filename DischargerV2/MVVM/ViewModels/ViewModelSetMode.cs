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
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelSetMode : BindableBase
    {
        #region Command
        public DelegateCommand<string> SelectModeCommand { get; set; }
        #endregion

        #region Model
        private ModelSetMode _model = new ModelSetMode();
        public ModelSetMode Model
        {
            get => _model;
            set
            {
                SetProperty(ref _model, value);
            }
        }
        #endregion

        #region Property
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
        }

        private Dictionary<string, ModelSetMode> _modelDictionary = new Dictionary<string, ModelSetMode>();
        public Dictionary<string, ModelSetMode> ModelDictionary
        {
            get
            {
                return _modelDictionary;
            }
            set
            {
                SetProperty(ref _modelDictionary, value);
            }
        }
        #endregion

        public ViewModelSetMode()
        {
            _instance = this;

            SelectModeCommand = new DelegateCommand<string>(SelectMode);

            InitializeModelDictionary();
        }

        private void InitializeModelDictionary()
        {
            List<TableDischargerInfo> infos = SqliteDischargerInfo.GetData();

            ViewModelSetMode.Instance.ModelDictionary.Clear();
            ViewModelSetMode_Preset.Instance.ModelDictionary.Clear();
            ViewModelSetMode_Step.Instance.ModelDictionary.Clear();

            for (int index = 0; index < infos.Count; index++)
            {
                ModelSetMode modelSetMode = new ModelSetMode();
                modelSetMode.DischargerName = infos[index].DischargerName;
                ViewModelSetMode.Instance.ModelDictionary.Add(infos[index].DischargerName, modelSetMode);

                ModelSetMode_Preset modelSetMode_Preset = new ModelSetMode_Preset();
                modelSetMode_Preset.DischargerName = infos[index].DischargerName;
                ViewModelSetMode_Preset.Instance.ModelDictionary.Add(infos[index].DischargerName, modelSetMode_Preset);

                ModelSetMode_Step modelSetMode_Step = new ModelSetMode_Step();
                modelSetMode_Step.DischargerName = infos[index].DischargerName;
                modelSetMode_Step.Content.Add(new ModelSetMode_StepData());
                ViewModelSetMode_Step.Instance.ModelDictionary.Add(infos[index].DischargerName, modelSetMode_Step);
            }

            ViewModelSetMode_Preset.Instance.SetBatteryType();
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

        public void SetDischargerName(string dischargerName)
        {
            Model = ModelDictionary[dischargerName];

            ViewModelSetMode_Preset.Instance.SetDischargerName(dischargerName);
            ViewModelSetMode_Step.Instance.SetDischargerName(dischargerName);
        }
    }
}
