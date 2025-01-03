using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.MVVM.Models
{
    public class ModelLoginUserInfo : BindableBase
    {
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
    }
}
