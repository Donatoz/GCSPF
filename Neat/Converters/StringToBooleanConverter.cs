using System;
using System.Globalization;
using System.Windows.Data;

namespace Neat
{
    [ValueConversion(typeof(string), typeof(bool))]
    public sealed class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var modifier = true;
            if (parameter is bool p) modifier = p;

            if (!(value is string v))
                return false;

            return modifier ? !string.IsNullOrWhiteSpace(v) : string.IsNullOrWhiteSpace(v);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
