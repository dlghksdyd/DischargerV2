using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DischargerV2.Ini
{
    // Create and initialize Crevis.ini without parsing System_Setting.xml (reference only).
    // If Crevis.ini exists, keep it as-is. If not, create with built-in defaults.
    partial class IniCrevis : Ini
    {
        public static readonly string CrevisIniPath = System.AppDomain.CurrentDomain.BaseDirectory + "Crevis.ini";

        private class CrevisDevice
        {
            public int No { get; set; }
            public bool Enabled { get; set; }
            public string Ip { get; set; }
            public int Port { get; set; } = 502;
            public int TempChannelCount { get; set; }
            public List<int> Calibrations { get; set; } = new List<int>();
        }

        public static void InitializeIniFile()
        {
            var iniFile = new FileInfo(CrevisIniPath);
            if (iniFile.Exists)
                return; // Use existing file

            // Create with default configuration (do not parse System_Setting.xml)
            var devices = GetDefaultDevices();
            WriteDevicesToIni(devices);
        }

        private static List<CrevisDevice> GetDefaultDevices()
        {
            return new List<CrevisDevice>
            {
                new CrevisDevice
                {
                    No = 0,
                    Enabled = true,
                    Ip = "192.168.1.61",
                    Port = 502,
                    TempChannelCount = 2,
                    Calibrations = new List<int> { 0, 0 }
                }
            };
        }

        private static void WriteDevicesToIni(List<CrevisDevice> devices)
        {
            // [General]
            SetValue(CrevisIniPath, "General", "DeviceCount", devices.Count.ToString(CultureInfo.InvariantCulture));

            for (int i = 0; i < devices.Count; i++)
            {
                var d = devices[i];
                string section = $"Device{i}";

                SetValue(CrevisIniPath, section, "No", d.No.ToString(CultureInfo.InvariantCulture));
                SetValue(CrevisIniPath, section, "Enabled", d.Enabled.ToString());
                SetValue(CrevisIniPath, section, "IpAddress", d.Ip ?? string.Empty);
                SetValue(CrevisIniPath, section, "Port", d.Port.ToString(CultureInfo.InvariantCulture));
                SetValue(CrevisIniPath, section, "TempChannelCount", d.TempChannelCount.ToString(CultureInfo.InvariantCulture));

                if (d.Calibrations != null && d.Calibrations.Count > 0)
                {
                    for (int c = 0; c < d.Calibrations.Count; c++)
                    {
                        SetValue(CrevisIniPath, section, $"Calibration_{c + 1}", d.Calibrations[c].ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
        }
    }
}
