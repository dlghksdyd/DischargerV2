using DischargerV2.MVVM.Models;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelMain : BindableBase
    {
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
        }
    }
}
