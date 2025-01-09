using DischargerV2.MVVM.Models;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelTopmenu : BindableBase
    {
        private ModelTopmenu Model = null;

        public ViewModelTopmenu()
        {
            Model = new ModelTopmenu();
        }
    }
}
