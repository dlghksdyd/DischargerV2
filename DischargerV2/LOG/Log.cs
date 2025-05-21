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
        public static void SaveFile(List<string> listTitle, List<string> listContent, string path, out string contentAll)
        {
            contentAll = "";

            try
            {
                string fileName = path + "\\" + DateTime.Now.ToString("yyyyMMdd_HH") + ".csv";
                string titleAll = "";

                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                if (!directoryInfo.Exists)
                    directoryInfo.Create();

                FileInfo fileInfo = new FileInfo(fileName);
                StreamWriter streamWriter;

                if (!fileInfo.Exists)
                {
                    foreach (string title in listTitle)
                        titleAll += title + ",";

                    streamWriter = new StreamWriter(fileName, true, Encoding.GetEncoding("UTF-8"));
                    streamWriter.WriteLine(titleAll);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                else
                {
                    if (IsUsedFile(fileInfo))
                    {
                        fileName = path + "\\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

                        foreach (string title in listTitle)
                            titleAll += title + ",";

                        streamWriter = new StreamWriter(fileName, true, Encoding.GetEncoding("UTF-8"));
                        streamWriter.WriteLine(titleAll);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                foreach (string content in listContent)
                    contentAll += content + ",";

                streamWriter = new StreamWriter(fileName, true, Encoding.GetEncoding("UTF-8"));
                streamWriter.WriteLine(contentAll);
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch
            {
                Debug.WriteLine("SAVE LOG FILE ERROR");
            }
        }

        private static bool IsUsedFile(FileInfo fileInfo)
        {
            try
            {
                FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                fileStream.Close();
            }
            catch
            {
                return true;
            }
            return false;
        }
    }
}
