using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using Ethernet.Client.Discharger;
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
            // 기존 값 초기화
            ViewModelSetMode.Instance.ModelDictionary.Clear();
            ViewModelSetMode_Preset.Instance.ModelDictionary.Clear();
            ViewModelSetMode_Step.Instance.ModelDictionary.Clear();
            ViewModelSetMode_Simple.Instance.ModelDictionary.Clear();
            ViewModelSetMode_SafetyCondition.Instance.ModelDictionary.Clear();

            // Discharger에서 관련 값 받아와 사용
            List<string> dischargerNameList = ViewModelDischarger.Instance.Model.DischargerNameList.ToList();
            List<DischargerInfo> dischargerInfoList = ViewModelDischarger.Instance.Model.DischargerInfos.ToList();

            for (int index = 0; index < dischargerNameList.Count; index++)
            {
                string dischargerName = dischargerNameList[index];
                DischargerInfo dischargerInfo = dischargerInfoList[index];

                ModelSetMode modelSetMode = new ModelSetMode();
                modelSetMode.DischargerIndex = index;
                modelSetMode.DischargerName = dischargerName;
                ViewModelSetMode.Instance.ModelDictionary.Add(dischargerName, modelSetMode);

                ModelSetMode_Preset modelSetMode_Preset = new ModelSetMode_Preset();
                modelSetMode_Preset.DischargerName = dischargerName;
                ViewModelSetMode_Preset.Instance.ModelDictionary.Add(dischargerName, modelSetMode_Preset);

                ModelSetMode_Step modelSetMode_Step = new ModelSetMode_Step();
                modelSetMode_Step.DischargerName = dischargerName;
                modelSetMode_Step.Content.Add(new ModelSetMode_StepData());
                ViewModelSetMode_Step.Instance.ModelDictionary.Add(dischargerName, modelSetMode_Step);

                ModelSetMode_Simple modelSetMode_Simple = new ModelSetMode_Simple();
                modelSetMode_Simple.DischargerName = dischargerName;
                ViewModelSetMode_Simple.Instance.ModelDictionary.Add(dischargerName, modelSetMode_Simple);

                ModelSetMode_SafetyCondition modelSetMode_SafetyCondition = new ModelSetMode_SafetyCondition();
                modelSetMode_SafetyCondition.DischargerName = dischargerName;
                modelSetMode_SafetyCondition.VoltageMin = dischargerInfo.SafetyVoltageMin.ToString();
                modelSetMode_SafetyCondition.VoltageMax = dischargerInfo.SafetyVoltageMax.ToString();
                modelSetMode_SafetyCondition.CurrentMin = dischargerInfo.SafetyCurrentMin.ToString();
                modelSetMode_SafetyCondition.CurrentMax = dischargerInfo.SafetyCurrentMax.ToString();
                modelSetMode_SafetyCondition.TempMin = dischargerInfo.SafetyTempMin.ToString();
                modelSetMode_SafetyCondition.TempMax = dischargerInfo.SafetyTempMax.ToString();
                ViewModelSetMode_SafetyCondition.Instance.ModelDictionary.Add(dischargerName, modelSetMode_SafetyCondition);
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
            ViewModelSetMode_Simple.Instance.SetDischargerName(dischargerName);
            ViewModelSetMode_SafetyCondition.Instance.SetDischargerName(dischargerName);
        }
    }
}
