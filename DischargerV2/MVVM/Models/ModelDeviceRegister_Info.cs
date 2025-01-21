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
    public class ModelDeviceRegister_Info : BindableBase
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

        private EDischargerModel _eModel;
        public EDischargerModel EModel
        {
            get
            { 
                return _eModel; 
            }
            set
            {
                SetProperty(ref _eModel, value);
            }
        }

        private EDischargeType _eType;
        public EDischargeType EType
        {
            get
            {
                return _eType;
            }
            set
            {
                SetProperty(ref _eType, value);
            }
        }

        private short _channel;
        public short Channel
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

        private double _voltSpec;
        public double VoltSpec
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

        private double _currSpec;
        public double CurrSpec
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

        private int _moduleChannel;
        public int ModuleChannel
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


        private int _tempChannel;
        public int TempChannel
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