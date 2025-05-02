using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.BLL.StaticData;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HealthMonitoring.BLL.Services
{
    public class MailingService : IMailingService
    {
        private readonly MailSettings _mailesettings;

        public MailingService(IOptions<MailSettings> mailesettings)
        {
            _mailesettings = mailesettings.Value;
        }
        public async Task SendEmailAsync(string mailto, string Subject, string Body, IList<IFormFile>? attachments = null)
        {
            try
            {
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_mailesettings.Email),
                    Subject = Subject
                };
                email.To.Add(MailboxAddress.Parse(mailto));
                email.From.Add(new MailboxAddress(_mailesettings.DisplayName, _mailesettings.Email));

                var builder = new BodyBuilder
                {
                    HtmlBody = Body
                };

                if (attachments != null && attachments.Any())
                {
                    foreach (var file in attachments)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }

                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_mailesettings.Host, _mailesettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailesettings.Email, _mailesettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}
