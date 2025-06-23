using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DischargerV2.Ini
{
    partial class IniDischarge : Ini
    {
        public enum EIniData 
        { 
            IsLocalDb, ServerIp, ServerPort, ServerName, 
            Sound, MaxSampleNum 
        }

        public enum ESound { On, Off }

        public static Dictionary<EIniData, IniData> DicIniData = new Dictionary<EIniData, IniData>()
        {
            { EIniData.IsLocalDb, new IniData { Section = "IsLocalDb", Key = "Value", Value = "True" } },
            { EIniData.ServerIp, new IniData { Section = "ServerIp", Key = "Value", Value = "127.0.0.1" } },
            { EIniData.ServerPort, new IniData { Section = "ServerPort", Key = "Value", Value = "1433" } },
            { EIniData.ServerName, new IniData { Section = "ServerName", Key = "Value", Value = "MINDIMS" } },
            { EIniData.Sound, new IniData { Section = "Sound", Key = "Value", Value = "On" } },
            { EIniData.MaxSampleNum, new IniData { Section = "Graph", Key = "MaxSampleNum", Value = "1600" } },
        };

        public static void InitializeIniFile()
        {
            FileInfo fileInfo = new FileInfo(Ini.Path);

            if (!fileInfo.Exists)
            {
                foreach (var iniData in DicIniData)
                {
                    SetIniData(iniData.Key, iniData.Value.Value);
                }
            }
        }

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
