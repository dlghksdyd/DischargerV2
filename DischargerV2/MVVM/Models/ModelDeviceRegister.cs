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
    public class ModelDeviceRegister : BindableBase
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        private string _ip;
        public string Ip
        {
            get
            {
                return _ip;
            }
            set
            {
                SetProperty(ref _ip, value);
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

        private List<string> _dischargerModelList = new List<string>();
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

        private List<string> _typeList = new List<string>();
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

        private List<string> _channelList = new List<string>();
        public List<string> ChannelList
        {
            get
            {
                return _channelList;
            }
            set
            {
                SetProperty(ref _channelList, value);
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

        private List<string> _voltSpecList = new List<string>();
        public List<string> VoltSpecList
        {
            get
            {
                return _voltSpecList;
            }
            set
            {
                SetProperty(ref _voltSpecList, value);
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

        private List<string> _currSpecList = new List<string>();
        public List<string> CurrSpecList
        {
            get
            {
                return _currSpecList;
            }
            set
            {
                SetProperty(ref _currSpecList, value);
            }
        }

        private string _comport;
        public string Comport
        {
            get
            {
                return _comport;
            }
            set
            {
                SetProperty(ref _comport, value);
            }
        }

        private string _moduleChannel;
        public string ModuleChannel
        {
            get
            {
                return _moduleChannel;
            }
            set
            {
                SetProperty(ref _moduleChannel, value);
            }
        }


        private string _tempChannel;
        public string TempChannel
        {
            get
            {
                return _tempChannel;
            }
            set
            {
                SetProperty(ref _tempChannel, value);
            }
        }
    }
}