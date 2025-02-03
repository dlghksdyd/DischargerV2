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
    public class ViewModelSetMode_Step : BindableBase
    {
        #region Command
        public DelegateCommand LoadStepInfoListCommand { get; set; }
        public DelegateCommand SaveStepInfoListCommand { get; set; }
        public DelegateCommand AddStepInfoCommand { get; set; }
        public DelegateCommand<object> DeleteStepInfoCommand { get; set; }
        #endregion

        #region Model
        public ModelSetMode_Step Model { get; set; } = new ModelSetMode_Step();
        #endregion

        private static ViewModelSetMode_Step _instance = null;

        public static ViewModelSetMode_Step Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelSetMode_Step();
                }
                return _instance;
            }
        }

        public ViewModelSetMode_Step()
        {
            _instance = this;

            LoadStepInfoListCommand = new DelegateCommand(LoadStepInfoList);
            SaveStepInfoListCommand = new DelegateCommand(SaveStepInfoList);
            AddStepInfoCommand = new DelegateCommand(AddStepInfo);
            DeleteStepInfoCommand = new DelegateCommand<object>(DeleteStepInfo);
        }

        private void LoadStepInfoList()
        {

        }

        private void SaveStepInfoList()
        {

        }

        private void AddStepInfo()
        {
            Model.Content.Add(new ModelSetMode_StepData());
        }

        private void DeleteStepInfo(object obj)
        {
            if (obj is ModelSetMode_StepData modelSetMode_StepData) 
            {
                Model.Content.Remove(modelSetMode_StepData);
            }
        }
    }
}
