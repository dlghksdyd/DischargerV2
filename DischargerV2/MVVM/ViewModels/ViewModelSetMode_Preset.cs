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
using Utility.Common;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelSetMode_Preset : BindableBase
    {
        #region Command
        public DelegateCommand SelectBatteryTypeCommand { get; set; }
        #endregion

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

            SelectBatteryTypeCommand = new DelegateCommand(SelectBatteryType);
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

            foreach (var model in ModelDictionary)
            {
                if (!(model.Value.BatteryTypeList.Count > 0))
                {
                    model.Value.BatteryTypeList = batteryTypeList;
                    model.Value.SelectedBatteryType = batteryTypeList[0];
                }
            }
        }

        private void SelectBatteryType()
        {
            ModelDischarger modelDischarger = ViewModelDischarger.Instance.Model;

            double receiveBatteryVoltage = modelDischarger.DischargerData[Model.DischargerIndex].ReceiveBatteryVoltage;
            string batteryType = Model.SelectedBatteryType;
            string currentSoC = OCV_Table.getSOC(batteryType, receiveBatteryVoltage).ToString();

            Model.CurrentSoC = currentSoC;
        }
    }
}
