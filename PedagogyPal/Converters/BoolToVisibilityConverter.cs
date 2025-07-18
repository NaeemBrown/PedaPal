using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PedagogyPal.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        // Converts bool to Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        // Converts back Visibility to bool
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
                return visibility == Visibility.Visible;
            return false;
        }
    }
}
