using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelModelRegister_Edit : BindableBase
    {
        #region Command
        public DelegateCommand xEditButton_ClickCommand { get; set; }
        public DelegateCommand xCancelButton_ClickCommand { get; set; }
        #endregion

        #region Model
        public ModelModelRegister_Edit Model { get; set; } = new ModelModelRegister_Edit();

        public int Id
        {
            get => Model.Id;
            set => Model.Id = value;
        }

        public string DischargerModel
        {
            get => Model.DischargerModel;
            set => Model.DischargerModel = value;
        }

        public string Type
        {
            get => Model.Type;
            set => Model.Type = value;
        }

        public string Channel
        {
            get => Model.Channel;
            set => Model.Channel = value;
        }

        public string VoltSpec
        {
            get => Model.VoltSpec;
            set => Model.VoltSpec = value;
        }

        public string CurrSpec
        {
            get => Model.CurrSpec;
            set => Model.CurrSpec = value;
        }

        public string VoltMax
        {
            get => Model.VoltMax;
            set => Model.VoltMax = value;
        }

        public string VoltMin
        {
            get => Model.VoltMin;
            set => Model.VoltMin = value;
        }

        public string CurrMax
        {
            get => Model.CurrMax;
            set => Model.CurrMax = value;
        }

        public string CurrMin
        {
            get => Model.CurrMin;
            set => Model.CurrMin = value;
        }

        public string TempMax
        {
            get => Model.TempMax;
            set => Model.TempMax = value;
        }

        public string TempMin
        {
            get => Model.TempMin;
            set => Model.TempMin = value;
        }
        #endregion

        private static ViewModelModelRegister_Edit _instance = null;

        public static ViewModelModelRegister_Edit Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelModelRegister_Edit();
                }
                return _instance;
            }
        }

        public ViewModelModelRegister_Edit()
        {
            xEditButton_ClickCommand = new DelegateCommand(xEditButton_Click);
            xCancelButton_ClickCommand = new DelegateCommand(xCancelButton_Click);

            LoadDischargerModelList();
        }

        private void xEditButton_Click()
        {
            UpdateDischargerModel();
            Close();
        }

        private void xCancelButton_Click()
        {
            Close();
        }

        private void LoadDischargerModelList()
        {
            List<string> modelList = new List<string>();

            foreach (EDischargerModel eDischargerModel in Enum.GetValues(typeof(EDischargerModel)))
            {
                modelList.Add(eDischargerModel.ToString());
            }

            Model.DischargerModelList = modelList;

            List<string> typeList = new List<string>();

            foreach (EDischargeType eDischargerType in Enum.GetValues(typeof(EDischargeType)))
            {
                typeList.Add(eDischargerType.ToString());
            }

            Model.TypeList = typeList;
        }

        private void UpdateDischargerModel()
        {
            TableDischargerModel tableDischargerModel = new TableDischargerModel();

            tableDischargerModel.Id = Model.Id;

            foreach (EDischargerModel eDischargerModel in Enum.GetValues(typeof(EDischargerModel)))
            {
                if (Model.DischargerModel == eDischargerModel.ToString())
                {
                    tableDischargerModel.Model = eDischargerModel;
                }
            }

            foreach (EDischargeType eDischargerType in Enum.GetValues(typeof(EDischargeType)))
            {
                if (Model.Type == eDischargerType.ToString())
                {
                    tableDischargerModel.Type = eDischargerType;
                }
            }

            tableDischargerModel.Channel = Convert.ToInt32(Model.Channel);
            tableDischargerModel.SpecVoltage = Convert.ToDouble(Model.VoltSpec);
            tableDischargerModel.SpecCurrent = Convert.ToDouble(Model.CurrSpec);
            tableDischargerModel.SafetyVoltMax = Convert.ToDouble(Model.VoltMax);
            tableDischargerModel.SafetyVoltMin = Convert.ToDouble(Model.VoltMin);
            tableDischargerModel.SafetyCurrentMax = Convert.ToDouble(Model.CurrMax);
            tableDischargerModel.SafetyCurrentMin = Convert.ToDouble(Model.CurrMin);
            tableDischargerModel.SafetyTempMax = Convert.ToDouble(Model.TempMax);
            tableDischargerModel.SafetyTempMin = Convert.ToDouble(Model.TempMin);

            SqliteDischargerModel.UpdateData(tableDischargerModel);
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OpenPopup(ModelMain.EPopup.ModelRegiseter);
        }
    }
}
