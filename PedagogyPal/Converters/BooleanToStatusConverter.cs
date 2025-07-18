using System.Windows.Data;
using System;

namespace PedagogyPal.Converters
{
    public class BooleanToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted ? "Completed" : "Pending";
            }
            return "Pending";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
