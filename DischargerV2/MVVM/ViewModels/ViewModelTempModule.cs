using DischargerV2.MVVM.Models;
using Prism.Commands;
using Serial.Client.TempModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelTempModule
    {
        #region Command


        #endregion

        public ModelTempModule Model = new ModelTempModule();

        private System.Timers.Timer OneSecondTimer { get; set; } = null;

        /// <summary>
        /// Key: ComPortString (e.g. COM3)
        /// </summary>
        private Dictionary<string, SerialClientTempModule> _clients = new Dictionary<string, SerialClientTempModule>();

        private static ViewModelTempModule _instance = null;

        public static ViewModelTempModule Instance()
        {
            return _instance;
        }

        public ViewModelTempModule()
        {
            _instance = this;
        }

        private void InitializeTempModuleClients()
        {
            SerialClientTempModuleStart parameters = new SerialClientTempModuleStart();

            OneSecondTimer?.Stop();
            OneSecondTimer = new System.Timers.Timer();
            OneSecondTimer.Elapsed += CopyDataFromTempModuleClientToModel;
            OneSecondTimer.Interval = 1000;
            OneSecondTimer.Start();
        }

        private void CopyDataFromTempModuleClientToModel(object sender, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < Model.TempModuleComportList.Count; i++)
            {
                //_clients[Model.TempModuleComportList[i]].GetDatas().TempDatas.ForEach(x => Model.TempDatas);
            }
        }
    }
}
