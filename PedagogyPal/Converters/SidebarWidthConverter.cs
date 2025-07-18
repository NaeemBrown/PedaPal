// Converters/SidebarWidthConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace PedagogyPal.Converters
{
    public class SidebarWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double windowWidth)
            {
                // Define sidebar width based on window width (e.g., 25% of window width)
                double calculatedWidth = windowWidth * 0.25;
                // Set minimum and maximum widths
                return Math.Max(200, Math.Min(calculatedWidth, 300));
            }

            return 270; // Default width if conversion fails
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
