using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DischargerV2.MVVM.Models
{
    public class ModelTopmenu : BindableBase
    {
        public bool isPopupOpen;
        public bool isPopupStaysOpen = true;
    }
}