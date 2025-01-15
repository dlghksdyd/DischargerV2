using DischargerV2.Database;
using DischargerV2.MVVM.Models;
using DischargerV2.MVVM.Views;
using MExpress.Mex;
using Prism.Commands;
using Prism.Mvvm;
using Sqlite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace DischargerV2.MVVM.ViewModels
{
    public class ViewModelUserSetting_Info : BindableBase
    {
        #region Command
        public DelegateCommand xEditImage_MouseLeftButtonUpCommand { get; set; }
        public DelegateCommand xDeleteImage_MouseLeftButtonUpCommand { get; set; }
        #endregion

        #region Model
        public ModelUserSetting_Info Model { get; set; } = new ModelUserSetting_Info();

        public bool IsAdmin
        {
            get
            {
                return Model.IsAdmin;
            }
            set
            {
                Model.IsAdmin = value;
            }
        }

        public string Initial
        {
            get
            {
                return Model.Initial;
            }
            set
            {
                Model.Initial = value;
            }
        }

        public string Id
        {
            get
            {
                return Model.Id;
            }
            set
            {
                Model.Id = value;
            }
        }

        public string Password
        {
            get
            {
                return Model.Password;
            }
            set
            {
                Model.Password = value;
            }
        }

        public string Name
        {
            get
            {
                return Model.Name;
            }
            set
            {
                Model.Name = value;
            }
        }
        #endregion

        public ViewModelUserSetting_Info()
        {
            xEditImage_MouseLeftButtonUpCommand = new DelegateCommand(xEditImage_MouseLeftButtonUp);
            xDeleteImage_MouseLeftButtonUpCommand = new DelegateCommand(xDeleteImage_MouseLeftButtonUp);
        }

        private void xEditImage_MouseLeftButtonUp()
        {

        }

        private void xDeleteImage_MouseLeftButtonUp()
        {
            DatabaseContext.DeleteUserInfo(Id);

            ViewModelMain viewModelMain = ViewModelMain.Instance;
            viewModelMain.Model.IsPopupOpen = true;
            viewModelMain.Model.PopupContent = new ViewPopup_UserSetting();
        }
    }
}
