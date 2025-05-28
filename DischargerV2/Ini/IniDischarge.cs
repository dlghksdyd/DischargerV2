using System;
using System.Collections.Generic;
using System.IO;

namespace DischargerV2.Ini
{
    partial class IniDischarge : Ini
    {
        public enum EIniData { Sound }

        public enum ESound { On, Off }

        public static Dictionary<EIniData, IniData> DicIniData = new Dictionary<EIniData, IniData>()
        {
            // 사용자 정보
            { EIniData.Sound, new IniData { Section = "Sound", Key = "Value", Value = "On" } },
        };


        public static void SetIniData(EIniData eIniData, string value)
        {
            IniData iniData = DicIniData[eIniData];

            WritePrivateProfileString(iniData.Section, iniData.Key, value, Ini.Path);
        }

        public static string GetIniData(EIniData eIniData)
        {
            string iniData;

            try
            {
                FileInfo fileInfo = new FileInfo(Ini.Path);

                if (fileInfo.Exists)
                    iniData = GetValue(DicIniData[eIniData]);
                else
                    iniData = DicIniData[eIniData].Value;
            }
            catch
            {
                Console.WriteLine(eIniData.ToString() + " 설정 값 확인이 필요합니다.");
                iniData = string.Empty;
            }

            return iniData;
        }

        public static void SetIniData<T>(EIniData eIniData, T value)
        {
            SetIniData(eIniData, value.ToString());
        }

        public static T GetIniData<T>(EIniData eIniData)
        {
            T data;
            Type type = typeof(T);

            if (type.Equals(typeof(int)))
            {
                data = (T)(object)Convert.ToInt32(GetIniData(eIniData));
            }
            else if (type.Equals(typeof(double)))
            {
                data = (T)(object)Convert.ToDouble(GetIniData(eIniData));
            }
            else if (type.Equals(typeof(bool)))
            {
                data = (T)(object)Convert.ToBoolean(GetIniData(eIniData));
            }
            else if (type.Equals(typeof(ESound)))
            {
                data = (T)Enum.Parse(typeof(ESound), GetIniData(eIniData));
            }
            else //if (type.Equals(typeof(string)))
            {
                data = (T)(object)GetIniData(eIniData);
            }

            return data;
        }
    }
}
