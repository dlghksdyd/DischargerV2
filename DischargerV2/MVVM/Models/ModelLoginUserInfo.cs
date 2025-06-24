using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DischargerV2.MVVM.Models
{
    public class ModelLoginUserInfo : BindableBase
    {
        private bool _isLocalDb;
        public bool IsLocalDb
        {
            get
            {
                return _isLocalDb;
            }
            set
            {
                SetProperty(ref _isLocalDb, value);
            }
        }

        private string _serverIp;
        public string ServerIp
        {
            get
            {
                return _serverIp;
            }
            set
            {
                SetProperty(ref _serverIp, value);
            }
        }

        private string _serverPort;
        public string ServerPort
        {
            get
            {
                return _serverPort;
            }
            set
            {
                SetProperty(ref _serverPort, value);
            }
        }

        private string _serverName;
        public string ServerName
        {
            get
            {
                return _serverName;
            }
            set
            {
                SetProperty(ref _serverName, value);
            }
        }

        private Visibility _visibility;
        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                SetProperty(ref _visibility, value);
            }
        }

        private string _userId;
        public string UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                SetProperty(ref _userId, value);
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        private string _userName = "mintech";
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                SetProperty(ref _userName, value);
            }
        }

        private string _initial = "M";
        public string Initial
        {
            get
            {
                return _initial;
            }
            set
            {
                SetProperty(ref _initial, value);
            }
        }

        private string _permission = "User";
        public string Permission
        {
            get
            {
                return _permission;
            }
            set
            {
                SetProperty(ref _permission, value);
            }
        }

        private bool _isAdmin = false;
        public bool IsAdmin
        {
            get
            {
                return _isAdmin;
            }
            set
            {
                SetProperty(ref _isAdmin, value);
            }
        }
    }
}
