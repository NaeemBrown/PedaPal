// EmailService.cs
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PedagogyPal.Services
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            var emailConfig = configuration.GetSection("EmailService");

            _fromEmail = emailConfig["FromEmail"];
            _fromName = emailConfig["FromName"];

            _smtpClient = new SmtpClient
            {
                Host = emailConfig["SmtpHost"],
                Port = int.Parse(emailConfig["SmtpPort"]),
                EnableSsl = bool.Parse(emailConfig["EnableSsl"]),
                Credentials = new NetworkCredential(
                    emailConfig["SmtpUsername"],
                    emailConfig["SmtpPassword"]
                )
            };
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = message,
                IsBodyHtml = false
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent to {toEmail} successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email to {toEmail}: {ex.Message}");
                // Optionally, implement retry logic or log the error appropriately
            }
        }
    }
}
