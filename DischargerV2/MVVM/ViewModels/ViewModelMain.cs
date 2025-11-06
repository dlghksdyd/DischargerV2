using DischargerV2.Ini;
using DischargerV2.MVVM.Models;
using Prism.Mvvm;
using SqlClient.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static DischargerV2.MVVM.Models.ModelMain;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelMain : BindableBase
    {
        public event EventHandler UpdateDischargerInfoTableEvent;

        #region Command
        #endregion

        #region Model
        public ModelMain Model { get; set; } = new ModelMain();
        #endregion

        private static ViewModelMain _instance = null;
        public static ViewModelMain Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelMain();
                }
                return _instance;
            }
        }

        public ViewModelMain()
        {
            _instance = this;

            IniDischarge.InitializeIniFile();

            InitializeModel();
        }

        public void InitializeModel()
        {
            // 기존 값 초기화
            Model.IsStartedArray.Clear();

            // Discharger에서 관련 값 받아와 사용
            List<string> dischargerNameList = ViewModelDischarger.Instance.Model.ToList().ConvertAll(x => x.DischargerName);

            for (int index = 0; index < dischargerNameList.Count; index++)
            {
                Model.IsStartedArray.Add(false);
            }
        }

        public void UpdateDischarger()
        {
            string machineCode = ViewModelLogin.Instance.Model.MachineCode;
            SqlClientStatus.DeleteData(machineCode);

            // 방전기 연결
            ViewModelDischarger.Instance.Initialize();

            // 방전기 테이블 갱신
            UpdateDischargerInfoTableEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// SetMode ↔ Monitor 화면 전환
        /// </summary>
        /// <param name="dischargerIndex"></param>
        /// <param name="setIsStarted"></param>
        public void SetIsStartedArray(bool setIsStarted)
        {
            ObservableCollection<bool> isStartedArray = new ObservableCollection<bool>();

            foreach (var isStarted in Model.IsStartedArray)
            {
                isStartedArray.Add(isStarted);
            }

            isStartedArray[Model.DischargerIndex] = setIsStarted;

            Model.IsStartedArray = isStartedArray;
        }

        public void SetViewModelPopup_DeviceRegister(ViewModelPopup_DeviceRegister viewModelPopup_DeviceRegister)
        {
            Model.ViewModelPopup_DeviceRegister = viewModelPopup_DeviceRegister;
        }

        public void SetViewModelPopup_ModelRegister(ViewModelPopup_ModelRegister viewModelPopup_ModelRegister)
        {
            Model.ViewModelPopup_ModelRegister = viewModelPopup_ModelRegister;
        }

        public void SetViewModelPopup_Info(ViewModelPopup_Info viewModelPopup_Info)
        {
            Model.ViewModelPopup_Info = viewModelPopup_Info;
        }

        public void SetViewModelPopup_Error(ViewModelPopup_Error viewModelPopup_Error)
        {
            Model.ViewModelPopup_Error = viewModelPopup_Error;
        }

        public void SetViewModelPopup_EditUser(ViewModelPopup_EditUser viewModelPopup_EditUser)
        {
            Model.ViewModelPopup_EditUser = viewModelPopup_EditUser;
        }

        public void SetViewModelPopup_Warning(ViewModelPopup_Warning viewModelPopup_Warning)
        {
            Model.ViewModelPopup_Warning = viewModelPopup_Warning;
        }

        public void SetViewModelPopup_Waiting(ViewModelPopup_Waiting viewModelPopup_Waiting)
        {
            Model.ViewModelPopup_Waiting = viewModelPopup_Waiting;
        }

        public void SetViewModelPopup_SetLogFileName(ViewModelPopup_SetLogFileName viewModelPopup_SetLogFileNa)
        {
            Model.ViewModelPopup_SetLogFileName = viewModelPopup_SetLogFileNa;
        }

        public string GetLogFileName()
        {
            return Model.ViewModelPopup_SetLogFileName.Comment;
        }
    }
}
