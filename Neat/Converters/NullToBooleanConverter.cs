using System;
using System.Globalization;
using System.Windows.Data;

namespace Neat
{
    [ValueConversion(typeof(object), typeof(bool))]
    public sealed class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var modifier = true;
            if (parameter is bool p) modifier = p;

            return value != null ? (modifier ? true : false) : (modifier ? false : true);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
