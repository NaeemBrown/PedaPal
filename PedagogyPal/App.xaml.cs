// App.xaml.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PedagogyPal.Services;
using PedagogyPal.ViewModels;
using PedagogyPal.Windows;
using System;
using System.IO;
using System.Windows;

namespace PedagogyPal
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loginPage = ServiceProvider.GetRequiredService<LoginRegisterPage>();
            loginPage.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Define the path to the appsettings.json inside the Data folder
            string configFileName = "appsettings.json";
            string configFolder = "Data";
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFolder, configFileName);

            // Check if the configuration file exists
            if (!File.Exists(configPath))
            {
                MessageBox.Show($"Configuration file not found at {configPath}", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFolder)) // Set base path to Data folder
                .AddJsonFile(configFileName, optional: false, reloadOnChange: true)
                .Build();

            // Get credential paths from configuration
            string googleCredentialsPath = configuration["GoogleDrive:CredentialsPath"];
            string firebaseCredentialsPath = configuration["Firebase:CredentialsPath"];

            // Validate credential paths
            string googleCredPathFull = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFolder, googleCredentialsPath);
            string firebaseCredPathFull = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFolder, firebaseCredentialsPath);

            if (!File.Exists(googleCredPathFull))
            {
                MessageBox.Show($"Google Drive credentials file not found at {googleCredPathFull}", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            if (!File.Exists(firebaseCredPathFull))
            {
                MessageBox.Show($"Firebase credentials file not found at {firebaseCredPathFull}", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // Get EmailService settings
            var emailConfig = configuration.GetSection("EmailService");
            string smtpHost = emailConfig["SmtpHost"];
            int smtpPort = int.Parse(emailConfig["SmtpPort"]);
            bool enableSsl = bool.Parse(emailConfig["EnableSsl"]);
            string smtpUsername = emailConfig["SmtpUsername"];
            string smtpPassword = emailConfig["SmtpPassword"];
            string fromEmail = emailConfig["FromEmail"];
            string fromName = emailConfig["FromName"];

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword) || string.IsNullOrEmpty(fromEmail))
            {
                MessageBox.Show("SMTP configuration is incomplete in appsettings.json.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // Register IConfiguration
            services.AddSingleton<IConfiguration>(configuration);

            // Register services
            services.AddSingleton<FirebaseService>();
            services.AddSingleton(new GoogleDriveService(googleCredPathFull));
            services.AddSingleton<EmailService>(); // EmailService is registered as Singleton
            services.AddSingleton<ReminderService>(); // Register ReminderService as Singleton

            // Register ViewModels
            services.AddSingleton<TasksViewModel>();
            services.AddTransient<TaskDetailsViewModel>();

            // Register Windows
            services.AddTransient<HomePage>();
            services.AddTransient<TasksPage>();
            services.AddTransient<PalPage>();
            services.AddTransient<DocumentsPage>();
            services.AddTransient<CalendarPage>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<LoginRegisterPage>();
            services.AddTransient<AddTaskWindow>();
            services.AddTransient<EditTaskWindow>();
        }
    }
}
