using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
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

        public async Task SendEmail(string target, string targetsubject, string content)
        {
            var key = _configuration["SendGridAPIKey"];
            var apiKey = key;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("service@aiursoft.com", "Aiursoft User Service");
            var subject = targetsubject;
            var to = new EmailAddress(target);
            var htmlContent = content;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
