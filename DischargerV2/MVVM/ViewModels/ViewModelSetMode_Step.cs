using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelSetMode_Step : BindableBase
    {
        #region Command
        #endregion

        #region Model
        public ModelSetMode_Step Model { get; set; } = new ModelSetMode_Step();
        #endregion

        public ViewModelSetMode_Step()
        {

        }
    }
}
