using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public static class LogCsvFile
    {
        public static void CreateLogFile(string logFilePath, string logFileName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(logFilePath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            try
            {
                FileStream fileStream = new FileStream(logFilePath + logFileName + ".csv", FileMode.Create);
                StreamWriter file = new StreamWriter(fileStream);
                file.Flush();
                file.Close();
            }
            catch
            {

            }
        }

        public static void AppendLogFile(string logFilePath, string logFileName, List<string> stringList)
        {
            try
            {
                FileStream fileStream = new FileStream(logFilePath + logFileName + ".csv", FileMode.Append);
                StreamWriter file = new StreamWriter(fileStream);
                file.WriteLine(ConvertStringListToCsvOneLine(stringList));
                file.Flush();
                file.Close();
            }
            catch
            {

            }
        }

        public static void AppendLogFile(string logFilePath, string logFileName, string appendString)
        {
            try
            {
                FileStream fileStream = new FileStream(logFilePath + logFileName + ".csv", FileMode.Append);
                StreamWriter file = new StreamWriter(fileStream);
                file.WriteLine(appendString);
                file.Flush();
                file.Close();
            }
            catch
            {

            }
        }

        public static string ConvertStringListToCsvOneLine(List<string> stringList)
        {
            string stringData = "";

            for (int i = 0; i < stringList.Count; i++)
            {
                if (i == 0)
                {
                    stringData += stringList[0];
                }
                else
                {
                    stringData += "," + stringList[i];
                }
            }

            return stringData;
        }
    }
}
