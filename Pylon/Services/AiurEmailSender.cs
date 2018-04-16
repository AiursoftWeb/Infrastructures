using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class AiurEmailSender
    {
        private readonly IConfiguration _configuration;

        public AiurEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(string target, string subject, string content)
        {
            var client = new SmtpClient("smtp.mxhichina.com")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("service@aiursoft.com", _configuration["Emailpassword"])
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("service@aiursoft.com"),
                Body = content,
                Subject = subject,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };
            mailMessage.To.Add(target);
            await Task.Factory.StartNew(() => client.Send(mailMessage));
        }
    }
}
