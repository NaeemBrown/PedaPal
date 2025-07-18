// LoginRegisterPage.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using PedagogyPal.Services;
using PedagogyPal.Windows;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PedagogyPal.Windows
{
    public partial class LoginRegisterPage : Window
    {
        private readonly FirebaseService _firebaseService;
        private readonly GoogleDriveService _googleDriveService;

        // Only constructor with dependencies. Removed the parameterless constructor.
        public LoginRegisterPage(FirebaseService firebaseService, GoogleDriveService googleDriveService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
            _googleDriveService = googleDriveService;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            bool rememberMe = RememberMeCheckBox.IsChecked == true;

            // Input Validations
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter your email.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var loginResult = await _firebaseService.LoginUserAsync(email, password, rememberMe);
            if (loginResult.Success)
            {
                // Navigate to HomePage
                var homePage = App.ServiceProvider.GetRequiredService<HomePage>();
                homePage.Owner = this;
                homePage.Show();
                this.Hide();
            }
            else
            {
                // Display specific error message
                MessageBox.Show(loginResult.Message, "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            bool rememberMe = RememberMeCheckBox.IsChecked == true;

            // Input Validations
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter your email.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var registerResult = await _firebaseService.RegisterUserAsync(email, password, rememberMe);
            if (registerResult.Success)
            {
                MessageBox.Show(registerResult.Message, "Registered", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navigate to HomePage after registration
                var homePage = App.ServiceProvider.GetRequiredService<HomePage>();
                homePage.Owner = this;
                homePage.Show();
                this.Hide();
            }
            else
            {
                // Display specific error message
                MessageBox.Show(registerResult.Message, "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Override OnContentRendered to attempt auto-login
        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            var autoLoginResult = await _firebaseService.AutoLoginAsync();
            if (autoLoginResult.Success)
            {
                // Navigate to HomePage
                var homePage = App.ServiceProvider.GetRequiredService<HomePage>();
                homePage.Show();
                this.Close();
            }
        }

        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    // Define classes to handle Firebase error responses
    public class AuthErrorResponse
    {
        public AuthError error { get; set; }
    }

    public class AuthError
    {
        public string code { get; set; }
        public string message { get; set; }
        public List<AuthErrorDetail> errors { get; set; }
    }

    public class AuthErrorDetail
    {
        public string message { get; set; }
        public string domain { get; set; }
        public string reason { get; set; }
    }
}
