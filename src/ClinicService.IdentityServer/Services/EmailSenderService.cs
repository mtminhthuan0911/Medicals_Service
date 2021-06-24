using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ClinicService.IdentityServer.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string clientEmailAddress, string subject, string body)
        {
            var fromAddress = new MailAddress(_configuration.GetValue<string>("EmailSenderSettings:SenderAddress"), _configuration.GetValue<string>("EmailSenderSettings:SenderName"));
            var toAddress = new MailAddress(clientEmailAddress, "User Client");
            var fromPassword = _configuration.GetValue<string>("EmailSenderSettings:SenderPassword");

            var smtp = new SmtpClient
            {
                Host = _configuration.GetValue<string>("EmailSenderSettings:SmtpAddress"),
                Port = _configuration.GetValue<int>("EmailSenderSettings:SmtpPort"),
                EnableSsl = _configuration.GetValue<bool>("EmailSenderSettings:EnableSsl"),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}
