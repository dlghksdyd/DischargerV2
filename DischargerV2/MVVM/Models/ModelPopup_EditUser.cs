using MExpress.Mex;
using Prism.Mvvm;
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
    public class ModelPopup_EditUser : BindableBase
    {
        private bool _isAdmin;
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

        private string _id;
        public string Id
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

        private string _currentPassword;
        public string CurrentPassword
        {
            get
            {
                return _currentPassword;
            }
            set
            {
                SetProperty(ref _currentPassword, value);
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                SetProperty(ref _newPassword, value);
            }
        }

        private string _confirmCurrentPassword;
        public string ConfirmCurrentPassword
        {
            get
            {
                return _confirmCurrentPassword;
            }
            set
            {
                SetProperty(ref _confirmCurrentPassword, value);
            }
        }

        private string _confirmNewPassword;
        public string ConfirmNewPassword
        {
            get
            {
                return _confirmNewPassword;
            }
            set
            {
                SetProperty(ref _confirmNewPassword, value);
            }
        }

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
    }
}