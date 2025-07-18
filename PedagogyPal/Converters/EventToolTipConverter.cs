using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PedagogyPal.Converters
{
    public class EventToolTipConverter : IValueConverter
    {
        public string HasEventToolTip { get; set; } = "You have tasks on this day.";
        public string NoEventToolTip { get; set; } = "No tasks.";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasEvent = (bool)value;
            return hasEvent ? HasEventToolTip : NoEventToolTip;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
