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
using static DischargerV2.MVVM.Models.ModelDeviceRegister_Add;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelModelRegister_Add : BindableBase
    {
        #region Command
        public DelegateCommand xAddButton_ClickCommand { get; set; }
        public DelegateCommand xCancelButton_ClickCommand { get; set; }
        #endregion

        #region Model
        public ModelModelRegister_Add Model { get; set; } = new ModelModelRegister_Add();
        #endregion

        private static ViewModelModelRegister_Add _instance = null;

        public static ViewModelModelRegister_Add Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelModelRegister_Add();
                }
                return _instance;
            }
        }

        public ViewModelModelRegister_Add()
        {
            _instance = this;

            xAddButton_ClickCommand = new DelegateCommand(xAddButton_Click);
            xCancelButton_ClickCommand = new DelegateCommand(xCancelButton_Click);

            LoadDischargerModelList();
        }

        private void xAddButton_Click()
        {
            if (Model.DischargerModel == null || Model.DischargerModel == "")
            {
                MessageBox.Show("Model: 필수 정보입니다.");
            }
            else if (Model.Type == null || Model.Type == "")
            {
                MessageBox.Show("Type: 필수 정보입니다.");
            }
            else if (Model.Channel == null || Model.Channel == "")
            {
                MessageBox.Show("Channel: 필수 정보입니다.");
            }
            else if (Model.VoltSpec == null || Model.VoltSpec == "")
            {
                MessageBox.Show("VoltSpec: 필수 정보입니다.");
            }
            else if (Model.CurrSpec == null || Model.CurrSpec == "")
            {
                MessageBox.Show("CurrSpec: 필수 정보입니다.");
            }
            else if (Model.VoltMax == null || Model.VoltMax == "")
            {
                MessageBox.Show("VoltMax: 필수 정보입니다.");
            }
            else if (Model.VoltMin == null || Model.VoltMin == "")
            {
                MessageBox.Show("VoltMin: 필수 정보입니다.");
            }
            else if (Model.CurrMax == null || Model.CurrMax == "")
            {
                MessageBox.Show("CurrMax: 필수 정보입니다.");
            }
            else if (Model.CurrMin == null || Model.CurrMin == "")
            {
                MessageBox.Show("CurrMin: 필수 정보입니다.");
            }
            else if (Model.TempMax == null || Model.TempMax == "")
            {
                MessageBox.Show("TempMax: 필수 정보입니다.");
            }
            else if (Model.TempMin == null || Model.TempMin == "")
            {
                MessageBox.Show("TempMin: 필수 정보입니다.");
            }
            else
            {
                InsertDischargerModel();
                Close();
            }
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

        private void InsertDischargerModel()
        {
            TableDischargerModel tableDischargerModel = new TableDischargerModel();

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

            SqliteDischargerModel.InsertData(tableDischargerModel);
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OpenPopup(ModelMain.EPopup.ModelRegiseter);
        }
    }
}
