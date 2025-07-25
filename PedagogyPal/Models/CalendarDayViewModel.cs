// ViewModels/CalendarViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using PedagogyPal.Models;

namespace PedagogyPal.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        private DateTime _currentDate;
        private DateTime? _selectedDate;

        public ObservableCollection<CalendarDay> CalendarDays { get; set; }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand SelectDateCommand { get; }

        public string MonthYearDisplay => _currentDate.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        public CalendarViewModel()
        {
            CalendarDays = new ObservableCollection<CalendarDay>();
            _currentDate = DateTime.Today;
            LoadCalendar(_currentDate);

            PreviousMonthCommand = new RelayCommand(GoToPreviousMonth);
            NextMonthCommand = new RelayCommand(GoToNextMonth);
            SelectDateCommand = new RelayCommand<CalendarDay>(SelectDate);
        }

        private void LoadCalendar(DateTime date)
        {
            CalendarDays.Clear();

            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;

            // Fill in the blanks for days of the previous month
            for (int i = 0; i < startDay; i++)
            {
                CalendarDays.Add(new CalendarDay
                {
                    Day = 0, // Representing empty cell
                    IsCurrentMonth = false
                });
            }

            // Populate the dates
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDay = new DateTime(date.Year, date.Month, day);
                CalendarDays.Add(new CalendarDay
                {
                    Day = day,
                    IsCurrentMonth = true,
                    HasEvent = CheckForEvents(currentDay),
                    IsToday = currentDay.Date == DateTime.Today
                });
            }

            // Fill the remaining cells to make 6 weeks (42 days)
            int totalCells = startDay + daysInMonth;
            int remainingCells = 42 - totalCells;
            for (int i = 0; i < remainingCells; i++)
            {
                CalendarDays.Add(new CalendarDay
                {
                    Day = 0, // Representing empty cell
                    IsCurrentMonth = false
                });
            }

            OnPropertyChanged(nameof(CalendarDays));
            OnPropertyChanged(nameof(MonthYearDisplay));
        }

        private bool CheckForEvents(DateTime date)
            return false; // Placeholder
        }

        private void GoToPreviousMonth()
        {
            _currentDate = _currentDate.AddMonths(-1);
            LoadCalendar(_currentDate);
        }

        private void GoToNextMonth()
        {
            _currentDate = _currentDate.AddMonths(1);
            LoadCalendar(_currentDate);
        }

        private void SelectDate(CalendarDay selectedDay)
        {
            if (selectedDay == null || !selectedDay.IsCurrentMonth)
                return;

            // Deselect previous selection
            if (_selectedDate.HasValue)
            {
                var previousSelected = CalendarDays.FirstOrDefault(d => d.Day == _selectedDate.Value.Day && d.IsCurrentMonth);
                if (previousSelected != null)
                {
                    previousSelected.IsSelected = false;
                }
            }

            // Select new date
            _selectedDate = new DateTime(_currentDate.Year, _currentDate.Month, selectedDay.Day);
            selectedDay.IsSelected = true;

            // Noti any bound properties or execute related logic
            OnPropertyChanged(nameof(SelectedDate));
        }

        public DateTime? SelectedDate => _selectedDate;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
