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
    public class ViewModelModelRegister_Add : BindableBase
    {
        #region Command
        public DelegateCommand InsertNewDataCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        #endregion

        #region Model
        public ModelModelRegister Model { get; set; } = new ModelModelRegister();
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

            InsertNewDataCommand = new DelegateCommand(InsertNewData);
            CloseCommand = new DelegateCommand(Close);

            LoadDischargerModelList();
        }

        private void InsertNewData()
        {
            if (!(CheckData() < 0))
            {
                InsertDischargerModel();
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

        private void InsertDischargerModel()
        {
            try
            {
                // 모델 정보 추가
                TableDischargerModel tableDischargerModel = new TableDischargerModel();

                // Id 생성
                var datas = SqliteDischargerModel.GetData().OrderByDescending(x => x.Id).ToList();
                if (datas.Count == 0)
                {
                    tableDischargerModel.Id = 0;
                }
                else
                {
                    tableDischargerModel.Id = datas.First().Id + 1;
                }
                Model.Id = tableDischargerModel.Id;

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

                bool isOk = SqliteDischargerModel.InsertData(tableDischargerModel);

                // 모델 정보 추가 Trace Log 저장
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
                    new LogTrace(ELogTrace.TRACE_ADD_MODEL, modelData);
                }
                else
                {
                    new LogTrace(ELogTrace.ERROR_ADD_MODEL, modelData);
                }
            }
            catch (Exception ex)
            {
                new LogTrace(ELogTrace.ERROR_ADD_MODEL, ex);
            }
        }
    }
}
