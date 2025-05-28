using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DischargerV2.Ini
{
    partial class Ini
    {
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "Initialization.ini";

        public struct IniData
        {
            public string Section;
            public string Key;
            public string Value;

            public IniData(string section, string key, string value)
            {
                Section = section;
                Key = key;
                Value = value;
            }
        }

        public static void SetValue(string path, string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, path);
        }

        public static string GetValue(string Section, string Key = "Value", string Default = "")
        {
            StringBuilder data = new StringBuilder(255);

            GetPrivateProfileString(Section, Key, Default, data, 255, Path);

            if (data != null && data.Length > 0)
                return data.ToString();
            else
                return Default;
        }

        public static string GetValue(IniData iniData, string Default = "")
        {
            StringBuilder data = new StringBuilder(255);

            GetPrivateProfileString(iniData.Section, iniData.Key, Default, data, 255, Path);

            if (data != null && data.Length > 0)
                return data.ToString();
            else
                return Default;
        }
    }
}
