// Controls/CustomCalendarControl.xaml.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PedagogyPal.Controls
{
    /// <summary>
    /// Interaction logic for CustomCalendarControl.xaml
    /// </summary>
    public partial class CustomCalendarControl : UserControl
    {
        private DateTime _currentDate;

        public CustomCalendarControl()
        {
            InitializeComponent();
            _currentDate = DateTime.Today;
            DisplayCalendar(_currentDate);
        }

        private void DisplayCalendar(DateTime date)
        {
            // Update the Month and Year display
            MonthYearText.Text = date.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

            // Clear previous dates
            DatesGrid.Children.Clear();

            // Determine the first day of the month
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;

            // Fill in the blanks for days of the previous month
            for (int i = 0; i < startDay; i++)
            {
                DatesGrid.Children.Add(new TextBlock()); // Empty cell
            }

            // Populate the dates
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDay = new DateTime(date.Year, date.Month, day);
                Button dayButton = new Button
                {
                    Content = day.ToString(),
                    Tag = currentDay,
                    Background = Brushes.White,
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Normal,
                    Style = (Style)FindResource("CustomCalendarDayButtonStyle") // Apply custom day button style
                };

                // Highlight today's date
                if (currentDay.Date == DateTime.Today)
                {
                    dayButton.Background = Brushes.LightBlue;
                }

                dayButton.Click += DayButton_Click;
                DatesGrid.Children.Add(dayButton);
            }

            // Fill the remaining cells of the grid to maintain structure
            int totalCells = startDay + daysInMonth;
            int remainingCells = (7 - (totalCells % 7)) % 7;
            for (int i = 0; i < remainingCells; i++)
            {
                DatesGrid.Children.Add(new TextBlock()); // Empty cell
            }
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(-1);
            DisplayCalendar(_currentDate);
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(1);
            DisplayCalendar(_currentDate);
        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DateTime selectedDate)
            {
                // Raise an event to notify the parent window/viewmodel
                DateSelected?.Invoke(this, new DateSelectedEventArgs(selectedDate));
            }
        }

        // Event to notify when a date is selected
        public event EventHandler<DateSelectedEventArgs> DateSelected;
    }

    // Custom EventArgs to pass the selected date
    public class DateSelectedEventArgs : EventArgs
    {
        public DateTime SelectedDate { get; }

        public DateSelectedEventArgs(DateTime selectedDate)
        {
            SelectedDate = selectedDate;
        }
    }
}
