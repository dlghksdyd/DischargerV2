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
        private readonly ResourceManager _resourceManager;
        private CultureInfo _cultureInfo = new CultureInfo("");

        public DynamicString()
        {
            _resourceManager = new ResourceManager(typeof(Strings));
        }

        public string this[string id]
        {
            get
            {
                if (string.IsNullOrEmpty(id)) return null;
                string value = _resourceManager.GetString(id, _cultureInfo);
                if (string.IsNullOrEmpty(value))
                {
                    value = id;
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
