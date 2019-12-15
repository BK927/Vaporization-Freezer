using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VF.Converter
{
    class SlideToTime : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO: Fix CoerceValueCallback
            //values[0] == Tick, values[1] == TimePerTick
            int totalMin = (int)(((double)values[0] + 1) * ((double)values[1]));
            int hour = totalMin / 60;
            int min = totalMin % 60;

            return hour.ToString() + "h " + min.ToString() + "min";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
