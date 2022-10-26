using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Neat
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public sealed class NullToVisibilityConverter : IValueConverter
    {
        public Visibility NullValue { get; set; }
        public Visibility NonNullvalue { get; set; }

        public NullToVisibilityConverter()
        {
            NonNullvalue = Visibility.Visible;
            NullValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var modifier = true;
            if (parameter is bool p) modifier = p;

            return value != null ? (modifier ? NonNullvalue : NullValue) : (modifier ? NullValue : NonNullvalue);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
