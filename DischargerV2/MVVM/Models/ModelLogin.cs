using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DischargerV2.MVVM.Models
{
    public class ModelLogin : BindableBase
    {
        public Visibility visibility;
        public string userId;
        public string password;
    }
}