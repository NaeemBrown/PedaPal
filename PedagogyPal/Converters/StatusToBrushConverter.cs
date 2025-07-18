// Converters/StatusToBrushConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PedagogyPal.Converters
{
    public class StatusToBrushConverter : IValueConverter
    {
        // Define brushes for different statuses
        private static readonly SolidColorBrush CompletedBrush = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
        private static readonly SolidColorBrush PendingBrush = new SolidColorBrush(Color.FromRgb(255, 193, 7));   // Amber
        private static readonly SolidColorBrush OverdueBrush = new SolidColorBrush(Color.FromRgb(244, 67, 54));   // Red

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Ensure the input value is a string
            string status = value as string;
            if (status == null)
                return Brushes.Gray; // Default color

            // Return the corresponding brush based on the status
            switch (status)
            {
                case "Completed":
                    return CompletedBrush;
                case "Pending":
                    return PendingBrush;
                case "Overdue":
                    return OverdueBrush;
                default:
                    return Brushes.Gray; // Default color for unknown statuses
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Conversion back is not implemented
            throw new NotImplementedException();
        }
    }
}
