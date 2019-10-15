using Aiursoft.Pylon.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class AiurEmailSender : ITransientDependency
    {
        private readonly string _mailFrom;
        private readonly string _mailUser;
        private readonly string _mailPassword;
        private readonly string _mailServer;

        public AiurEmailSender(IConfiguration configuration)
        {
            _mailFrom = configuration["MailFrom"];
            _mailUser = configuration["MailUser"];
            _mailPassword = configuration["MailPassword"];
            _mailServer = configuration["MailServer"];
        }

        public async Task SendEmail(string target, string targetsubject, string content)
        {
            var client = new SmtpClient(_mailServer)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_mailUser, _mailPassword),
                EnableSsl = true,
                Port = 587
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_mailFrom),
                Body = content,
                Subject = targetsubject,
                IsBodyHtml = true
            };
            mailMessage.To.Add(target);
            await client.SendMailAsync(mailMessage);
        }
    }
}
