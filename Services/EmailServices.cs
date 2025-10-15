using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace PermohonanSystemMVC.Services
{
    public interface IEmailService
    {
        Task SendAsync(string subject, string htmlBody, string? attachmentPath = null);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config) => _config = config;

        public async Task SendAsync(string subject, string htmlBody, string? attachmentPath = null)
        {
            var settings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings["FromName"], settings["User"]));
            message.To.Add(MailboxAddress.Parse(settings["AdminTo"]));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };

            if (!string.IsNullOrWhiteSpace(attachmentPath) && System.IO.File.Exists(attachmentPath))
            {
                builder.Attachments.Add(attachmentPath);
            }

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(settings["Smtp"], int.Parse(settings["Port"]!), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(settings["User"], settings["Pass"]);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
