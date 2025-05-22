using ScottPlot.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.LOG
{
    public class Log
    {
        public static void SaveFile_Day(List<string> listTitle, List<string> listContent, string path, string fileName = null)
        {
            try
            {
                if (fileName == null || fileName == string.Empty)
                {
                    fileName = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }
                else
                {
                    fileName = path + "\\" + fileName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }

                string titleAll = "";

                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                FileInfo fileInfo = new FileInfo(fileName);
                StreamWriter streamWriter;

                if (!fileInfo.Exists)
                {
                    foreach (string title in listTitle)
                    {
                        titleAll += title + ",";
                    }

                    streamWriter = new StreamWriter(fileName, true, Encoding.GetEncoding("UTF-8"));
                    streamWriter.WriteLine(titleAll);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                string oneLine = string.Empty;

                foreach (string content in listContent)
                {
                    oneLine += content + ",";
                }

                streamWriter = new StreamWriter(fileName, true, Encoding.GetEncoding("UTF-8"));
                streamWriter.WriteLine(oneLine);
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch
            {
                Debug.WriteLine("SAVE LOG FILE ERROR");
            }
        }

        public static void SaveFile_Hour(List<string> listTitle, List<string> listContent, string path, string fileName = null)
        {
            try
            {
                if (fileName == null || fileName == string.Empty)
                {
                    fileName = path + "\\" + DateTime.Now.ToString("yyyyMMdd_HH") + ".csv";
                }
                else
                {
                    fileName = path + "\\" + fileName + "_" + DateTime.Now.ToString("yyyyMMdd_HH") + ".csv";
                }

                string titleAll = "";

                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                FileInfo fileInfo = new FileInfo(fileName);
                StreamWriter streamWriter;

                if (!fileInfo.Exists)
                {
                    foreach (string title in listTitle)
                    {
                        titleAll += title + ",";
                    }

                    streamWriter = new StreamWriter(fileName, true, Encoding.GetEncoding("UTF-8"));
                    streamWriter.WriteLine(titleAll);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                string oneLine = string.Empty;

                foreach (string content in listContent)
                {
                    oneLine += content + ",";
                }

                streamWriter = new StreamWriter(fileName, true, Encoding.GetEncoding("UTF-8"));
                streamWriter.WriteLine(oneLine);
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch
            {
                Debug.WriteLine("SAVE LOG FILE ERROR");
            }
        }
    }
}
