// HomePage.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using PedagogyPal.Services;
using PedagogyPal.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace PedagogyPal.Windows
{
    public partial class HomePage : Window
    {
        private readonly FirebaseService _firebaseService;
        private readonly TasksViewModel _tasksViewModel;

        private bool isSidebarOpen = false;

        public HomePage()
            : this(
                App.ServiceProvider.GetRequiredService<FirebaseService>(),
                App.ServiceProvider.GetRequiredService<TasksViewModel>())
        {
        }

        public HomePage(TasksViewModel tasksViewModel)
        {
            InitializeComponent();
            _tasksViewModel = tasksViewModel;
            this.DataContext = _tasksViewModel;
        }

        public HomePage(FirebaseService firebaseService, TasksViewModel tasksViewModel)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
            _tasksViewModel = tasksViewModel;
            this.DataContext = _tasksViewModel;
            Console.WriteLine("HomePage DataContext set to TasksViewModel.");
        }

        #region Sidebar Toggle

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isSidebarOpen)
            {
                SlideInSidebar();
            }
            else
            {
                SlideOutSidebar();
            }
        }

        private void SlideInSidebar()
        {
            Storyboard slideIn = (Storyboard)FindResource("SlideInSidebar");
            slideIn.Begin();
            isSidebarOpen = true;
        }

        private void SlideOutSidebar()
        {
            Storyboard slideOut = (Storyboard)FindResource("SlideOutSidebar");
            slideOut.Begin();
            isSidebarOpen = false;
        }

        private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isSidebarOpen)
            {
                SlideOutSidebar();
            }
        }

        #endregion

        #region Navigation

        private void NavigateTasks_Click(object sender, RoutedEventArgs e)
        {
            var tasksPage = App.ServiceProvider.GetRequiredService<TasksPage>();
            tasksPage.Owner = this;
            tasksPage.Show();
            this.Hide();
        }

        private void NavigatePal_Click(object sender, RoutedEventArgs e)
        {
            var palPage = App.ServiceProvider.GetRequiredService<PalPage>();
            palPage.Owner = this;
            palPage.Show();
            this.Hide();
        }

        private void NavigateDocuments_Click(object sender, RoutedEventArgs e)
        {
            var documentsPage = App.ServiceProvider.GetRequiredService<DocumentsPage>();
            documentsPage.Owner = this;
            documentsPage.Show();
            this.Hide();
        }

        private void NavigateCalendar_Click(object sender, RoutedEventArgs e)
        {
            var calendarPage = App.ServiceProvider.GetRequiredService<CalendarPage>();
            calendarPage.Owner = this;
            calendarPage.Show();
            this.Hide();
        }

        private void NavigateSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsPage = App.ServiceProvider.GetRequiredService<SettingsPage>();
            settingsPage.Owner = this;
            settingsPage.Show();
            this.Hide();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _firebaseService.Logout();
            var loginPage = App.ServiceProvider.GetRequiredService<LoginRegisterPage>();
            loginPage.Show();
            this.Close();
        }

        #endregion

        #region Add Task

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var addTaskWindow = App.ServiceProvider.GetRequiredService<AddTaskWindow>();
            addTaskWindow.Owner = this;
            bool? result = addTaskWindow.ShowDialog();
            if (result == true)
            {
                _tasksViewModel.LoadTasksAsync();
            }
        }

        #endregion

        #region Today Button

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            _tasksViewModel.SelectedDate = DateTime.UtcNow.Date;
            _tasksViewModel.DisplayDate = DateTime.UtcNow.Date;
        }

        #endregion
    }
}
