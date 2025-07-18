// SettingsPage.xaml.cs
using System.Windows;

namespace PedagogyPal.Windows
{
    public partial class SettingsPage : Window
    {
        public SettingsPage()
        {
            InitializeComponent();
            // Optionally, load existing settings here
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Implement logic to load existing settings into the UI elements
            // Example:
            // UsernameTextBox.Text = SettingsService.GetUsername();
            // EmailTextBox.Text = SettingsService.GetEmail();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement logic to save the settings
            // Example:
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;

            // Validate inputs as needed
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter both username and email.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save settings using a hypothetical SettingsService
            // SettingsService.SaveUsername(username);
            // SettingsService.SaveEmail(email);

            MessageBox.Show("Settings have been saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Optionally, close the window after saving
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Optionally, confirm cancellation
            var result = MessageBox.Show("Are you sure you want to cancel without saving?", "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the HomePage window if it's hidden
            if (this.Owner != null)
            {
                this.Owner.Show();
            }

            // Close the SettingsPage
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
