using DischargerV2.LOG;
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
        public DelegateCommand LoadDischargerModelListCommand { get; set; }
        public DelegateCommand UpdateEditDataCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        #endregion

        #region Model
        public ModelModelRegister Model { get; set; } = new ModelModelRegister();

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
            LoadDischargerModelListCommand = new DelegateCommand(LoadDischargerModelList);
            UpdateEditDataCommand = new DelegateCommand(UpdateEditData);
            CloseCommand = new DelegateCommand(Close);
        }

        public void SetModelData(ModelModelRegister setModel)
        {
            Model.Id = setModel.Id;
            Model.DischargerModel = setModel.DischargerModel;
            Model.Type = setModel.Type;
            Model.Channel = setModel.Channel;
            Model.VoltSpec = setModel.VoltSpec;
            Model.CurrSpec = setModel.CurrSpec;
            Model.VoltMax = setModel.VoltMax;
            Model.VoltMin = setModel.VoltMin;
            Model.CurrMax = setModel.CurrMax;
            Model.CurrMin = setModel.CurrMin;
            Model.TempMax = setModel.TempMax;
            Model.TempMin = setModel.TempMin;

            LoadDischargerModelList();
        }

        private void UpdateEditData()
        {
            if (!(CheckData() < 0))
            {
                UpdateDischargerModel();
                Close();
            }
        }

        private void Close()
        {
            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.OpenPopup(ModelMain.EPopup.ModelRegiseter);
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

        private int CheckData()
        {
            if (Model.DischargerModel == null || Model.DischargerModel == "")
            {
                MessageBox.Show("Model: 필수 정보입니다.");
                return -1;
            }
            if (Model.Type == null || Model.Type == "")
            {
                MessageBox.Show("Type: 필수 정보입니다.");
                return -1;
            }
            if (Model.Channel == null || Model.Channel == "")
            {
                MessageBox.Show("Channel: 필수 정보입니다.");
                return -1;
            }
            if (!Int32.TryParse(Model.Channel, out Int32 channel))
            {
                MessageBox.Show("Channel: 데이터 형식이 잘못되었습니다.");
                return -1;
            }
            if (Model.VoltSpec == null || Model.VoltSpec == "")
            {
                MessageBox.Show("VoltSpec: 필수 정보입니다.");
                return -1;
            }
            if (!double.TryParse(Model.VoltSpec, out double voltSpec))
            {
                MessageBox.Show("VoltSpec: 데이터 형식이 잘못되었습니다.");
                return -1;
            }
            if (Model.CurrSpec == null || Model.CurrSpec == "")
            {
                MessageBox.Show("CurrSpec: 필수 정보입니다.");
                return -1;
            }
            if (!double.TryParse(Model.CurrSpec, out double surrSpec))
            {
                MessageBox.Show("CurrSpec: 데이터 형식이 잘못되었습니다.");
                return -1;
            }
            if (Model.VoltMax == null || Model.VoltMax == "")
            {
                MessageBox.Show("VoltMax: 필수 정보입니다.");
                return -1;
            }
            if (!double.TryParse(Model.VoltMax, out double voltMax))
            {
                MessageBox.Show("VoltMax: 데이터 형식이 잘못되었습니다.");
                return -1;
            }
            if (Model.VoltMin == null || Model.VoltMin == "")
            {
                MessageBox.Show("VoltMin: 필수 정보입니다.");
                return -1;
            }
            if (!double.TryParse(Model.VoltMin, out double voltMin))
            {
                MessageBox.Show("VoltMin: 데이터 형식이 잘못되었습니다.");
                return -1;
            }
            if (Model.CurrMax == null || Model.CurrMax == "")
            {
                MessageBox.Show("CurrMax: 필수 정보입니다.");
                return -1;
            }
            if (!double.TryParse(Model.CurrMax, out double currMax))
            {
                MessageBox.Show("CurrMax: 데이터 형식이 잘못되었습니다.");
                return -1;
            }
            if (Model.CurrMin == null || Model.CurrMin == "")
            {
                MessageBox.Show("CurrMin: 필수 정보입니다.");
                return -1;
            }
            if (!double.TryParse(Model.CurrMin, out double currMin))
            {
                MessageBox.Show("CurrMin: 데이터 형식이 잘못되었습니다.");
                return -1;
            }
            if (Model.TempMax == null || Model.TempMax == "")
            {
                MessageBox.Show("TempMax: 필수 정보입니다.");
                return -1;
            }
            if (!double.TryParse(Model.TempMax, out double tempMax))
            {
                MessageBox.Show("TempMax: 데이터 형식이 잘못되었습니다.");
                return -1;
            }
            if (Model.TempMin == null || Model.TempMin == "")
            {
                MessageBox.Show("TempMin: 필수 정보입니다.");
                return -1;
            }
            if (!double.TryParse(Model.TempMin, out double tempMin))
            {
                MessageBox.Show("TempMin: 데이터 형식이 잘못되었습니다.");
                return -1;
            }
            return 0;
        }

        private void UpdateDischargerModel()
        {
            try
            {
                // 모델 정보 수정
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

                bool isOk = SqliteDischargerModel.UpdateData(tableDischargerModel);

                // 모델 정보 수정 Trace Log 저장
                ModelData modelData = new ModelData()
                {
                    Id = Model.Id,
                    Model = Model.DischargerModel,
                    Type = Model.Type,
                    Channel = Model.Channel,
                    SpecVoltage = Model.VoltSpec,
                    SpecCurrent = Model.CurrSpec,
                    SafetyVoltMax = Model.VoltMax,
                    SafetyVoltMin = Model.VoltMin,
                    SafetyCurrentMax = Model.CurrMax,
                    SafetyCurrentMin = Model.CurrMin,
                    SafetyTempMax = Model.TempMax,
                    SafetyTempMin = Model.TempMin
                };

                if (isOk)
                {
                    new LogTrace(ELogTrace.TRACE_EDIT_MODEL, modelData);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_EDIT_MODEL, modelData);
                }
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_EDIT_MODEL, ex);
            }
        }
    }
}
