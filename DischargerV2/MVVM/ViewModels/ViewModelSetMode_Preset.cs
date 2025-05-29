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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Common;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelSetMode_Preset : BindableBase
    {
        #region Model
        private ModelSetMode_Preset _model = new ModelSetMode_Preset();
        public ModelSetMode_Preset Model
        {
            get => _model;
            set
            {
                SetProperty(ref _model, value);
            }
        }
        #endregion

        #region Property
        public string SelectedDischargerName;

        private static ViewModelSetMode_Preset _instance = null;
        public static ViewModelSetMode_Preset Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelSetMode_Preset();
                }
                return _instance;
            }
        }

        private Dictionary<string, ModelSetMode_Preset> _modelDictionary = new Dictionary<string, ModelSetMode_Preset>();
        public Dictionary<string, ModelSetMode_Preset> ModelDictionary
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

        public ViewModelSetMode_Preset()
        {
            _instance = this;
        }

        public void SetDischargerName(string dischargerName)
        {
            if (SelectedDischargerName != null && SelectedDischargerName != "")
            {
                // 현재 값을 ModelDictionary에 넣기 
                ModelDictionary[SelectedDischargerName] = Model;
            }

            SelectedDischargerName = dischargerName;

            // ModelDictionary 값 가져오기
            Model = ModelDictionary[dischargerName];
        }

        public void SetBatteryType()
        {
            List<string> batteryTypeList = new List<string>();

            OCV_Table.initOcvTable();

            foreach (var data in OCV_Table.ocvList)
            {
                batteryTypeList.Add(data.battTypeName);
            }

            foreach (var model in ModelDictionary.Values)
            {
                if (!(model.BatteryTypeList.Count > 0))
                {
                    model.BatteryTypeList = batteryTypeList;
                    model.SelectedBatteryType = model.BatteryTypeList[0]; // 기본값 설정
                }
            }
        }

        public void GetCurrentSoc(ModelDischarger modelDischarger)
        {
            if (modelDischarger.DischargerState == EDischargerState.Ready)
            {
                if (modelDischarger.DischargerData.ReceiveBatteryVoltage == 0)
                {
                    Thread.Sleep(1000);
                }

                var modelSetMode_Preset = Instance.ModelDictionary[modelDischarger.DischargerName];
                var dischargerIndex = modelDischarger.DischargerIndex;

                GetCurrentSoc(modelSetMode_Preset, dischargerIndex);
            }
        }

        public void GetCurrentSoc(ModelSetMode_Preset modelSetMode_Preset, int dischargerIndex = 0)
        {
            ModelDischarger modelDischarger;

            if (dischargerIndex >= 0)
            {
                modelDischarger = ViewModelDischarger.Instance.Model[dischargerIndex];
            }
            else
            {
                modelDischarger = ViewModelDischarger.Instance.SelectedModel;
            }

            double receiveBatteryVoltage = modelDischarger.DischargerData.ReceiveBatteryVoltage;
            string batteryType = modelSetMode_Preset.SelectedBatteryType;
            string currentSoC = OCV_Table.getSOC(batteryType, receiveBatteryVoltage).ToString();

            modelSetMode_Preset.CurrentSoC = currentSoC;
        }
    }
}
