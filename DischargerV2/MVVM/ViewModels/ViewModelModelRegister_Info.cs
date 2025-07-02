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
using static DischargerV2.LOG.LogTrace;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelModelRegister_Info : BindableBase
    {
        #region Command
        public DelegateCommand OpenEditDataCommand { get; set; }
        public DelegateCommand OpenPopupDeleteCommand { get; set; }
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

        public ViewModelModelRegister_Info()
        {
            OpenEditDataCommand = new DelegateCommand(OpenEditData);
            OpenPopupDeleteCommand = new DelegateCommand(OpenPopupDelete);
        }

        private void OpenEditData()
        {
            ViewModelPopup_ModelRegister viewModelPopup_ModelRegister = new ViewModelPopup_ModelRegister()
            {
                SelectedId = Model.Id
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_ModelRegister(viewModelPopup_ModelRegister);
        }

        private void OpenPopupDelete()
        {
            ViewModelPopup_Warning viewModelPopup_Warning = new ViewModelPopup_Warning()
            {
                Title = string.Format("Delete Model '{0}'?", Model.DischargerModel),
                Comment = "Are you sure you want to delete this data?\r\n" +
                          "Once you confirm, this data will be permanetly deleted.",
                CallBackDelegate = DeleteDischargerModel,
            };

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.SetViewModelPopup_Warning(viewModelPopup_Warning);
            viewModelMain.OpenNestedPopup(ModelMain.ENestedPopup.Warning);
        }

        public void DeleteDischargerModel()
        {
            try
            {
                // 모델 정보 제거
                bool isOk = SqliteDischargerModel.DeleteData(Model.Id);

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
                    new LogTrace(ELogTrace.SYSTEM_OK_DELETE_MODEL, modelData);
                }
                else
                {
                    MessageBox.Show("모델 정보 삭제 실패");

                    new LogTrace(ELogTrace.SYSTEM_ERROR_DELETE_MODEL, modelData);
                }

                // 모델 정보 등록 화면 표시
                ViewModelMain viewModelMain = ViewModelMain.Instance;
                viewModelMain.OpenPopup(ModelMain.EPopup.ModelRegiseter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error 발생\n\n" +
                    $"ClassName: {this.GetType().Name}\n" +
                    $"Function: {System.Reflection.MethodBase.GetCurrentMethod().Name}\n" +
                    $"Exception: {ex.Message}");

                new LogTrace(ELogTrace.SYSTEM_ERROR_DELETE_MODEL, ex);
            }
        }
    }
}
