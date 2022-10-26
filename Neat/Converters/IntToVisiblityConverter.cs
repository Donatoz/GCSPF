using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Neat
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public sealed class IntToVisibilityConverter : IValueConverter
    {
        public Visibility ZeroValue { get; set; }
        public Visibility MoreThanZeroValue { get; set; }

        public IntToVisibilityConverter()
        {
            MoreThanZeroValue = Visibility.Visible;
            ZeroValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var modifier = true;
            if (parameter is bool p) modifier = p;

            if (!(value is int v))
                return null;
            return v > 0 ? (modifier ? MoreThanZeroValue : ZeroValue) : (modifier ? ZeroValue : MoreThanZeroValue);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
