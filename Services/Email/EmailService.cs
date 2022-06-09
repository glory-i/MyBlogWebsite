using Blog3.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;
        private readonly SmtpClient _client;

        public EmailService(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
            _client = new SmtpClient(_settings.Server)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            };
        }
        public Task SendEmail(string email, string subject, string message)
        {
            //throw new NotImplementedException();
            var mailMessage = new MailMessage("Blog-awesome mail", email, subject, message);
           

            return _client.SendMailAsync(mailMessage);
        }
    }
}
