// TasksPage.xaml.cs
using System.Windows;
using PedagogyPal.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace PedagogyPal.Windows
{
    public partial class TasksPage : Window
    {
        public TasksPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<TasksViewModel>();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the HomePage window if it's hidden
            if (this.Owner != null)
            {
                this.Owner.Show();
            }

            // Close the TasksPage
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Show the HomePage window if it's hidden and not already visible
            if (this.Owner != null && !this.Owner.IsVisible)
            {
                this.Owner.Show();
            }
        }
    }
}
