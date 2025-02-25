using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
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
    public class ViewModelSetMode_Step : BindableBase
    {
        public event EventHandler SelectedDischargerChanged;

        #region Command
        public DelegateCommand EnterStandardCapacityCommand { get; set; }
        public DelegateCommand LoadStepInfoListCommand { get; set; }
        public DelegateCommand SaveStepInfoListCommand { get; set; }
        public DelegateCommand AddStepInfoCommand { get; set; }
        public DelegateCommand<object> DeleteStepInfoCommand { get; set; }
        #endregion

        #region Model
        private ModelSetMode_Step _model = new ModelSetMode_Step();
        public ModelSetMode_Step Model
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

        private Dictionary<string, ModelSetMode_Step> _modelDictionary = new Dictionary<string, ModelSetMode_Step>();
        public Dictionary<string, ModelSetMode_Step> ModelDictionary
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

        public ViewModelSetMode_Step()
        {
            _instance = this;

            EnterStandardCapacityCommand = new DelegateCommand(EnterStandardCapacity);
            LoadStepInfoListCommand = new DelegateCommand(LoadStepInfoList);
            SaveStepInfoListCommand = new DelegateCommand(SaveStepInfoList);
            AddStepInfoCommand = new DelegateCommand(AddStepInfo);
            DeleteStepInfoCommand = new DelegateCommand<object>(DeleteStepInfo);
        }

        public void SetDischargerName(string dischargerName)
        {
            // 현재 값을 ModelDictionary에 넣기 : CollectionChanged 이벤트에서 처리
            if (SelectedDischargerName != null && SelectedDischargerName != "")
            {
                Model.Content.Add(null);
            }

            SelectedDischargerName = dischargerName;

            // ModelDictionary 값 가져오기
            Model.StandardCapacity = ModelDictionary[dischargerName].StandardCapacity;
            Model.IsCompleteDischarge = ModelDictionary[dischargerName].IsCompleteDischarge;
            SetModelContent(Model, ModelDictionary[dischargerName]);

            if (SelectedDischargerChanged != null)
            {
                this.SelectedDischargerChanged(this, EventArgs.Empty);
            }
        }

        private void EnterStandardCapacity()
        {
            if (Model.StandardCapacity == null || Model.StandardCapacity == "")
                return;

            foreach (var stepData in Model.Content)
            {
                stepData.Current = "";
                stepData.CRate = "";
            }
        }

        private void LoadStepInfoList()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory() + "\\StepConfigure",
                IsFolderPicker = false
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            try
            {
                FileStream fileStream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.None, 4096);
                byte[] readBuffer = new byte[fileStream.Length];
                fileStream.Read(readBuffer, 0, readBuffer.Length);
                fileStream.Dispose();

                StepConfigure stepConfigure = JsonConvert.DeserializeObject<StepConfigure>(readBuffer.FromByteArrayToString(Encoding.UTF8));
                List<StepInfo> stepInfoList = stepConfigure.StepInfos;
                ObservableCollection<ModelSetMode_StepData> content = new ObservableCollection<ModelSetMode_StepData>();

                foreach (var stepInfo in stepInfoList)
                {
                    content.Add(new ModelSetMode_StepData()
                    {
                        Voltage = stepInfo.VoltPerModule.ToString(),
                        Current = stepInfo.FixedCurrent.ToString(),
                        CRate = stepInfo.CratePerModule.ToString()
                    });
                }

                // Step 설정 값
                Model.Content.Clear();

                foreach (var data in content)
                {
                    Model.Content.Add(data);
                }

                Model.Content.RemoveAt(0);

                // 완전 방전 진행 여부 설정 값
                Model.IsCompleteDischarge = stepConfigure.IsCompleteDischarge;
            }
            catch
            {
                MessageBox.Show("환경 설정 파일 구조가 잘못 되었습니다.");
            }
        }

        private void SaveStepInfoList()
        {
            if (Directory.Exists("./StepConfigure") == false)
            {
                Directory.CreateDirectory("./StepConfigure");
            }

            StepConfigure stepConfigure = CreateStepConfigure();

            if (stepConfigure == null)
            {
                MessageBox.Show("각 스텝 값의 누락이 없어야 하고, 각 값은 숫자여야 합니다.");
                return;
            }

            SaveFileDialog saveFiledialog = new SaveFileDialog
            {
                Filter = "환경 설정 파일 (*.json)|*.json",
                Title = "Save Configuration File"
            };

            if (saveFiledialog.ShowDialog() == true)
            {
                string jsonStr = "";

                /// JSON byte array 생성
                jsonStr += JsonConvert.SerializeObject(stepConfigure, Formatting.Indented);
                byte[] jsonByteArray = jsonStr.FromStringToByteArray(Encoding.UTF8);

                /// 파일에 저장
                FileStream fileStream = new FileStream(saveFiledialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096);
                fileStream.Write(jsonByteArray, 0, jsonByteArray.Length);
                fileStream.Dispose();
            }
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

        private void SetModelContent(ModelSetMode_Step targetModel, ModelSetMode_Step model)
        {
            ObservableCollection<ModelSetMode_StepData> content = new ObservableCollection<ModelSetMode_StepData>();

            foreach (var stepData in model.Content)
            {
                content.Add(stepData);
            }

            targetModel.Content.Clear();

            foreach (var data in content)
            {
                targetModel.Content.Add(data);
            }

            targetModel.Content.RemoveAt(0);
            targetModel.DischargerName = model.DischargerName;
        }

        public StepConfigure CreateStepConfigure()
        {
            StepConfigure stepConfigure = new StepConfigure();

            foreach (var modelSetMode_StepData in Model.Content)
            {
                if (!double.TryParse(modelSetMode_StepData.Voltage, out double voltage))
                {
                    return null;
                }
                if (!double.TryParse(modelSetMode_StepData.Current, out double current))
                {
                    return null;
                }
                if ((!double.TryParse(modelSetMode_StepData.CRate, out double cRate)) &&
                    (Model.StandardCapacity != null && Model.StandardCapacity != ""))
                {
                    return null;
                }

                StepInfo stepInfo = new StepInfo()
                {
                    IsFixedCurrentUse = (current != 0) ? true : false,
                    VoltPerModule = voltage,
                    FixedCurrent = current,
                    CratePerModule = cRate
                };

                stepConfigure.IsCompleteDischarge = Model.IsCompleteDischarge;
                stepConfigure.StepInfos.Add(stepInfo);
            }

            if (stepConfigure.StepInfos.Count == 0)
            {
                return null;
            }

            return stepConfigure;
        }
    }
}
