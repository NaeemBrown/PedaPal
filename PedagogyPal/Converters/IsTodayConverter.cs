using System;
using System.Globalization;
using System.Windows.Data;

namespace PedagogyPal.Converters
{
    public class IsTodayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Assuming 'value' is a DateTime or a string representing a date
            if (value is DateTime date)
            {
                return date.Date == DateTime.Today;
            }
            else if (DateTime.TryParse(value?.ToString(), out DateTime parsedDate))
            {
                return parsedDate.Date == DateTime.Today;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Typically not needed for one-way bindings
            throw new NotImplementedException();
        }
    }
}
