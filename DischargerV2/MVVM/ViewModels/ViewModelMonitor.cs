using DischargerV2.MVVM.Enums;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using Ethernet.Client.Discharger;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Common;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelMonitor : BindableBase
    {
        #region Command
        #endregion

        #region Model
        private ModelMonitor _model = new ModelMonitor();
        public ModelMonitor Model
        {
            get => _model;
            set
            {
                SetProperty(ref _model, value);
            }
        }
        #endregion

        #region Property
        public int DischargerIndex;
        public string SelectedDischargerName;

        private static ViewModelMonitor _instance = new ViewModelMonitor();
        public static ViewModelMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModelMonitor();
                }
                return _instance;
            }
        }

        private Dictionary<string, ModelMonitor> _modelDictionary = new Dictionary<string, ModelMonitor>();
        public Dictionary<string, ModelMonitor> ModelDictionary
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

        public ViewModelMonitor()
        {
            _instance = this;

            InitializeModelDictionary();
        }

        public void SetDischargerName(string dischargerName, int dischargerIndex)
        {
            DischargerIndex = dischargerIndex;
            SelectedDischargerName = dischargerName;

            Model = ModelDictionary[dischargerName];
        }

        private void InitializeModelDictionary()
        {
            // 기존 값 초기화
            ViewModelMonitor.Instance.ModelDictionary.Clear();

            // Discharger에서 관련 값 받아와 사용
            List<string> dischargerNameList = ViewModelDischarger.Instance.Model.DischargerNameList.ToList();

            for (int index = 0; index < dischargerNameList.Count; index++)
            {
                string dischargerName = dischargerNameList[index];

                ModelMonitor modelMonitor = new ModelMonitor();
                ViewModelMonitor.Instance.ModelDictionary.Add(dischargerName, modelMonitor);
            }
        }
    }
}
