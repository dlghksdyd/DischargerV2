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
    public class ViewModelSetMode_SafetyCondition : BindableBase
    {
        #region Command
        #endregion

        #region Model
        private ModelSetMode_SafetyCondition _model = new ModelSetMode_SafetyCondition();
        public ModelSetMode_SafetyCondition Model
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

        private static ViewModelSetMode_SafetyCondition _instance = null;
        public static ViewModelSetMode_SafetyCondition Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelSetMode_SafetyCondition();
                }
                return _instance;
            }
        }

        private Dictionary<string, ModelSetMode_SafetyCondition> _modelDictionary = new Dictionary<string, ModelSetMode_SafetyCondition>();
        public Dictionary<string, ModelSetMode_SafetyCondition> ModelDictionary
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

        public ViewModelSetMode_SafetyCondition()
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
    }
}
