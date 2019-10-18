using Aiursoft.Pylon.Interfaces;
using Aiursoft.Pylon.Models.Status;
using Aiursoft.Pylon.Services.ToStatusServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Services
{
    public class AiurEmailSender : ITransientDependency
    {
        private readonly string _mailUser;
        private readonly string _mailPassword;
        private readonly string _mailServer;
        private readonly ILogger<AiurEmailSender> _logger;
        private readonly AppsContainer _appsContainer;
        private readonly EventService _eventService;

        public AiurEmailSender(
            IConfiguration configuration,
            ILogger<AiurEmailSender> logger,
            AppsContainer appsContainer,
            EventService eventService)
        {
            _mailUser = configuration["MailUser"];
            _mailPassword = configuration["MailPassword"];
            _mailServer = configuration["MailServer"];
            _logger = logger;
            _appsContainer = appsContainer;
            _eventService = eventService;
        }

        public async Task SendEmail(string fromDisplayName, string target, string targetsubject, string content)
        {
            try
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
                    From = new MailAddress(_mailUser, fromDisplayName),
                    Body = content,
                    Subject = targetsubject,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(target);
                mailMessage.Bcc.Add(_mailUser);
                await client.SendMailAsync(mailMessage);
                return;
            }
            catch (Exception e)
            {
                try
                {
                    _logger.LogError(e, e.Message);
                    var accessToken = _appsContainer.AccessToken();
                    await _eventService.LogAsync(await accessToken, e.Message, e.StackTrace, EventLevel.Exception);
                }
                catch { }
            }
        }
    }
}
