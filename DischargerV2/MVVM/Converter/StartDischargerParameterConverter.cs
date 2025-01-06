using DischargerV2.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DischargerV2.MVVM.Converter
{
    class StartDischargerParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type Target_Type, object Parameter, CultureInfo culture)
        {
            StartDischargerCommandParam param = new StartDischargerCommandParam();
            param.DischargerName = (string)Values[0];
            param.Voltage = (double)Values[1];
            param.Current = (double)Values[2];

            return param;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
