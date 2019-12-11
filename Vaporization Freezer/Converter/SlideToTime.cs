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
    class SlideToTime : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            int totalMin = (int)((double)value) * (Int32)Application.Current.Resources["TimePerTick_DEBUG"];
#else
            int totalMin = (int)((double)value) * (Int32)Application.Current.Resources["TimePerTick"];
#endif

            int hour = totalMin / 60;
            int min = totalMin % 60;

            return hour.ToString() + "h " + min.ToString() + "min";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
