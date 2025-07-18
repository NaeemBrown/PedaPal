// TaskManagerWindow.xaml.cs
using System.Windows;

namespace PedagogyPal.Windows
{
    public partial class TaskManagerWindow : Window
    {
        public TaskManagerWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the HomePage window if it's hidden
            if (this.Owner != null)
            {
                this.Owner.Show();
            }

            // Close the TaskManagerWindow
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
