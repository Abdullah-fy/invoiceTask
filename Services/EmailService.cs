using System.Net.Mail;
using itRoot.Services.IServices;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;

namespace itRoot.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailConfirmationAsync(string email, string fullName, string confirmationUrl)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_configuration["Email:FromName"], _configuration["Email:FromAddress"]));
                message.To.Add(new MailboxAddress(fullName, email));
                message.Subject = "Confirm Your Email Address";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                        <body>
                            <p>Hi {fullName},</p>
                            <p>Thank you for registering with us. Please click the link below to confirm your email address:</p>
                            <p><a href='{confirmationUrl}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px;'>Confirm Email</a></p>
                            <p>If you didn't create an account, please ignore this email.</p>
                            <p>This link will expire in 24 hours.</p>
                            <br>
                            <p>Best regards</p>
                        </body>
                        </html>",
                    TextBody = $@"
                        
                        Hi {fullName},
                        
                        Thank you for registering with us. Please visit the following link to confirm your email address:
                        {confirmationUrl}
                        
                        If you didn't create an account, please ignore this email.
                        This link will expire in 24 hours.
                        
                        Best regards"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new MailKit.Net.Smtp.SmtpClient();

                await client.ConnectAsync(
                    _configuration["Email:SmtpHost"],
                    int.Parse(_configuration["Email:SmtpPort"]),
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                );

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email confirmation sent successfully to {email}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email confirmation to {email}");
                return false;
            }
        }
    }
}