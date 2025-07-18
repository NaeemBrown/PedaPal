// Models/CalendarDay.cs
using System.ComponentModel;

namespace PedagogyPal.Models
{
    public class CalendarDay : INotifyPropertyChanged
    {
        private bool _isSelected;
        private bool _isToday;

        public int Day { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool HasEvent { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public bool IsToday
        {
            get => _isToday;
            set
            {
                if (_isToday != value)
                {
                    _isToday = value;
                    OnPropertyChanged(nameof(IsToday));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
