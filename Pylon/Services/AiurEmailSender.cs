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
        public async Task SendEmail(string target, string subject, string content, string password)
        {
            SmtpClient client = new SmtpClient("smtp.mxhichina.com");
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("service@aiursoft.com", password);
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("service@aiursoft.com");
            mailMessage.To.Add(target);
            mailMessage.Body = content;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = Encoding.UTF8;
            await Task.Factory.StartNew(() => client.Send(mailMessage));
        }
    }
}
