using DischargerV2.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace DischargerV2.Languages.Strings
{
    public class DynamicString : DynamicObject
    {
        private static ResourceManager _resourceManager;
        private static CultureInfo _cultureInfo = new CultureInfo("");

        public DynamicString()
        {
            _resourceManager = new ResourceManager(typeof(Strings));
        }

        public string GetDynamicString(string key)
        {
            string value = _resourceManager.GetString(key, _cultureInfo);
            if (string.IsNullOrEmpty(value))
            {
                value = key;
            }
            return value;
        }

        public string this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key)) return null;
                string value = _resourceManager.GetString(key, _cultureInfo);
                if (string.IsNullOrEmpty(value))
                {
                    value = key;
                }
                return value;
            }
        }

        public void ChangeLanguage(string languageCode)
        {
            _cultureInfo = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = _cultureInfo;

            foreach (Window window in Application.Current.Windows.Cast<Window>())
            {
                if (!window.AllowsTransparency)
                {
                    window.Language = XmlLanguage.GetLanguage(_cultureInfo.Name);
                }
            }
        }
    }
}
