using DischargerV2.Communication.CommEthernetClient.CrevisClient;
using DischargerV2.Ini;
using DischargerV2.MVVM.Models;
using Prism.Mvvm;
using SqlClient.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static DischargerV2.MVVM.Models.ModelMain;
using System.Text;

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

        // Crevis
        private List<CrevisClient> _crevisClients = new List<CrevisClient>();

        public ViewModelMain()
        {
            _instance = this;

            IniDischarge.InitializeIniFile();
            IniCrevis.InitializeIniFile();

            InitializeModel();
            InitializePopup();
            InitializeCrevisClients();
        }

        private static string ReadIniValue(string path, string section, string key, string defaultValue)
        {
            try
            {
                var sb = new StringBuilder(1024);
                Ini.Ini.GetPrivateProfileString(section, key, defaultValue ?? string.Empty, sb, sb.Capacity, path);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ViewModelMain.ReadIniValue error: {ex.Message}");
                return defaultValue;
            }
        }

        private void InitializeCrevisClients()
        {
            try
            {
                string iniPath = IniCrevis.CrevisIniPath;

                // Create clients from INI (Enabled, IpAddress, TempChannelCount, VoltChannelCount, Calibration)
                _crevisClients = CrevisClient.InitializeFromIni(iniPath, null);

                // Apply Port to IpPort if specified in ini
                int deviceCount = 1;
                int.TryParse(ReadIniValue(iniPath, "General", "DeviceCount", "1"), out deviceCount);
                deviceCount = Math.Max(1, deviceCount);

                for (int i = 0; i < Math.Min(deviceCount, _crevisClients.Count); i++)
                {
                    string section = $"Device{i}";

                    // Port
                    int port = _crevisClients[i].IpPort; // default
                    int.TryParse(ReadIniValue(iniPath, section, "Port", port.ToString()), out port);
                    _crevisClients[i].IpPort = port;

                    // If enabled, start communication
                    if (_crevisClients[i].ControlEnabled)
                    {
                        var result = _crevisClients[i].StartTcp();
                        if (!result)
                        {
                            Debug.WriteLine($"Crevis TCP 시작 실패: Device={i}, Port={_crevisClients[i].IpPort}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ViewModelMain.InitializeCrevisClients error: {ex.Message}");
            }
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

        public void OpenPopup(EPopup setPopup)
        {
            Visibility[] popupVisibility = new Visibility[Enum.GetValues(typeof(EPopup)).Length];

            foreach (EPopup popup in Enum.GetValues(typeof(EPopup)))
            {
                popupVisibility[(int)popup] = (setPopup.Equals(popup)) ? 
                    Visibility.Visible : Visibility.Collapsed;
            }

            if (setPopup.Equals(EPopup.UserSetting))
            {
                Model.ViewModelPopup_UserSetting = new ViewModelPopup_UserSetting();
            }
            else if (setPopup.Equals(EPopup.DeviceRegister))
            {
                Model.ViewModelPopup_DeviceRegister = new ViewModelPopup_DeviceRegister();
            }
            else if (setPopup.Equals(EPopup.ModelRegiseter))
            {
                Model.ViewModelPopup_ModelRegister = new ViewModelPopup_ModelRegister();
            }

            Model.PopupVisibility = popupVisibility;
            Model.IsPopupOpen = true;
        }

        public void OpenNestedPopup(ENestedPopup setPopup)
        {
            Visibility[] popupVisibility = new Visibility[Enum.GetValues(typeof(ENestedPopup)).Length];

            foreach (ENestedPopup popup in Enum.GetValues(typeof(ENestedPopup)))
            {
                popupVisibility[(int)popup] = (setPopup.Equals(popup)) ? 
                    Visibility.Visible : Visibility.Collapsed;
            }

            Model.NestedPopupVisibility = popupVisibility;
            Model.IsNestedPopupOpen = true;
        }

        private void InitializePopup()
        {
            OffPopup();
            OffNestedPopup();
        }

        public void OffPopup()
        {
            Visibility[] popupVisibility = new Visibility[Enum.GetValues(typeof(EPopup)).Length];

            foreach (EPopup popup in Enum.GetValues(typeof(EPopup)))
            {
                popupVisibility[(int)popup] = Visibility.Collapsed;
            }

            Model.PopupVisibility = popupVisibility;
            Model.IsPopupOpen = false;
        }

        public void OffNestedPopup()
        {
            Visibility[] popupVisibility = new Visibility[Enum.GetValues(typeof(ENestedPopup)).Length];

            foreach (EPopup popup in Enum.GetValues(typeof(ENestedPopup)))
            {
                popupVisibility[(int)popup] = Visibility.Collapsed;
            }

            Model.ViewModelPopup_CreateNewUser = new ViewModelPopup_CreateNewUser();
            Model.ViewModelPopup_EditUser = new ViewModelPopup_EditUser();
            Model.ViewModelPopup_Warning = new ViewModelPopup_Warning();
            
            Model.NestedPopupVisibility = popupVisibility;
            Model.IsNestedPopupOpen = false;
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
