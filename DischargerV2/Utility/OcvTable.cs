using DischargerV2.MVVM.Enums;
using MExpress.Mex;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Utility.Common
{
    public struct OCVSTRUCT
    {
        public string battTypeName;
        public List<double> socToVoltage;
        public double capacity;
        public double phase2Volt;

        public OCVSTRUCT(string[] data)
        {
            if (data.Length == 104)
            {
                battTypeName = data[0];
                socToVoltage = new List<double>();
                for (int i = 1; i < 102; i++)
                {

                    socToVoltage.Add(double.Parse(data[i]));
                }
                capacity = double.Parse(data[102]);
                phase2Volt = double.Parse(data[103]);
            }
            else
            {
                battTypeName = "";
                socToVoltage = null;
                capacity = 0;
                phase2Volt = 0;
            }
        }

        public void setData(string[] data)
        {
            if (data.Length == 104)
            {
                battTypeName = data[0];
                socToVoltage = new List<double>();
                for (int i = 1; i < 102; i++)
                {

                    socToVoltage.Add(double.Parse(data[i]));
                }
                capacity = double.Parse(data[102]);
                phase2Volt = double.Parse(data[103]);
            }
        }
    }

    public static class OCV_Table
    { 
        // OCV temp 25
        public static List<OCVSTRUCT> ocvList;

        public static void initOcvTable()
        {
            string fileName = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\Database\\OCV.csv";
            StreamReader sr;
            try
            {
                sr = new StreamReader(fileName);
            }
            catch
            {
                return;
            }

            string s = null;
            ocvList = new List<OCVSTRUCT>();
            while ((s = sr.ReadLine()) != null)
            {
                string[] strarr = s.Split(',');
                ocvList.Add(new OCVSTRUCT(strarr));
            }
        }

        public static double getCapcity(string name)
        {
            double ret = ocvList.Find(element => element.battTypeName.Contains(name)).capacity;
            return ret;
        }

        public static double getPhase2Volt(string name)
        {
            double ret = ocvList.Find(element => element.battTypeName.Contains(name)).phase2Volt;
            return ret;
        }

        public static int getSOC(string name, double currentVolt)
        {
            String battName = name;
            double min = double.MaxValue;
            double[] data = ocvList.Find(element => element.battTypeName.Contains(battName)).socToVoltage.ToArray();
            double target = currentVolt;        //target과 가까운 값
            int near = 0;     //가까운 값: 27

            //③ 처리: NEAR
            for (int i = 0; i < data.Length; i++)
            {
                double abs = Math.Abs(data[i] - target); //차이 값의 절댓값
                if (abs < min)
                {
                    min = abs;                  //MIN: 최솟값 알고리즘
                    near = i;          //NEAR: 차이 값의 절댓값 중 최솟값일 때 값
                }
            }
            return near;
        }

        public static double getTargetVolt(string name, int SOC)
        {
            String battName = name;
            double[] data = ocvList.Find(element => element.battTypeName.Contains(battName)).socToVoltage.ToArray();

            return data[SOC];
        }
    }
}

