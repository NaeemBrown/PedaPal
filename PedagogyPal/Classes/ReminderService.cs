// ReminderService.cs
using PedagogyPal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PedagogyPal.Services
{
    public class ReminderService : IDisposable
    {
        private readonly FirebaseService _firebaseService;
        private readonly EmailService _emailService;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour
        private Timer _timer;

        // Reminder intervals in days
        private readonly List<int> _reminderDays = new List<int> { 14, 7, 4, 2 };

        public ReminderService(FirebaseService firebaseService, EmailService emailService)
        {
            _firebaseService = firebaseService;
            _emailService = emailService;
        }

        public void Start()
        {
            _timer = new Timer(async _ => await CheckAndSendRemindersAsync(), null, TimeSpan.Zero, _checkInterval);
            Console.WriteLine("ReminderService started.");
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0);
            Console.WriteLine("ReminderService stopped.");
        }

        private async Task CheckAndSendRemindersAsync()
        {
            try
            {
                var tasks = await _firebaseService.GetTasksAsync();
                var today = DateTime.Today;

                foreach (var task in tasks)
                {
                    var daysUntilDue = (task.DueDate.Date - today).Days;

                    if (_reminderDays.Contains(daysUntilDue))
                    {
                        // Get user email associated with the task using UserId
                        string userEmail = await _firebaseService.GetUserEmailAsync(task.UserId);

                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            string subject = $"Reminder: Task '{task.Title}' is due in {daysUntilDue} day(s)";
                            string message = $"Hello,\n\nThis is a reminder that your task '{task.Title}' is due on {task.DueDate.ToShortDateString()} ({daysUntilDue} day(s) remaining).\n\nBest regards,\nPedagogyPal Team";

                            await _emailService.SendEmailAsync(userEmail, subject, message);
                        }
                        else
                        {
                            Console.WriteLine($"No email found for UserId: {task.UserId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle accordingly
                Console.WriteLine($"Error in CheckAndSendRemindersAsync: {ex.Message}");
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    _timer?.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer.
                // Set large fields to null.

                disposedValue = true;
            }
        }

        // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources.
        // ~ReminderService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
