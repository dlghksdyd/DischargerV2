using MExpress.Mex;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DischargerV2.MVVM.Models
{
    public class ModelModelRegister : BindableBase
    {
        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        private string _dischargerModel = "";
        public string DischargerModel
        {
            get
            {
                return _dischargerModel;
            }
            set
            {
                SetProperty(ref _dischargerModel, value);
            }
        }

        private List<string> _dischargerModelList;
        public List<string> DischargerModelList
        {
            get
            {
                return _dischargerModelList;
            }
            set
            {
                SetProperty(ref _dischargerModelList, value);
            }
        }

        private string _type = "";
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                SetProperty(ref _type, value);
            }
        }

        private List<string> _typeList;
        public List<string> TypeList
        {
            get
            {
                return _typeList;
            }
            set
            {
                SetProperty(ref _typeList, value);
            }
        }

        private string _channel = "";
        public string Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                SetProperty(ref _channel, value);
            }
        }

        private string _voltSpec = "";
        public string VoltSpec
        {
            get
            {
                return _voltSpec;
            }
            set
            {
                SetProperty(ref _voltSpec, value);
            }
        }

        private string _currSpec = "";
        public string CurrSpec
        {
            get
            {
                return _currSpec;
            }
            set
            {
                SetProperty(ref _currSpec, value);
            }
        }

        private string _voltMax;
        public string VoltMax
        {
            get
            {
                return _voltMax;
            }
            set
            {
                SetProperty(ref _voltMax, value);
            }
        }

        private string _voltMin;
        public string VoltMin
        {
            get
            {
                return _voltMin;
            }
            set
            {
                SetProperty(ref _voltMin, value);
            }
        }

        private string _currMax;
        public string CurrMax
        {
            get
            {
                return _currMax;
            }
            set
            {
                SetProperty(ref _currMax, value);
            }
        }

        private string _currMin;
        public string CurrMin
        {
            get
            {
                return _currMin;
            }
            set
            {
                SetProperty(ref _currMin, value);
            }
        }

        private string _tempMax;
        public string TempMax
        {
            get
            {
                return _tempMax;
            }
            set
            {
                SetProperty(ref _tempMax, value);
            }
        }

        private string _tempMin;
        public string TempMin
        {
            get
            {
                return _tempMin;
            }
            set
            {
                SetProperty(ref _tempMin, value);
            }
        }
    }
}