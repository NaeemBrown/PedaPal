using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PedagogyPal.Converters
{ 
    public class EventIndicatorConverter : IValueConverter
    {
        public Brush EventBrush { get; set; } = Brushes.Red;
        public Brush NoEventBrush { get; set; } = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasEvent = (bool)value;
            return hasEvent ? EventBrush : NoEventBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
