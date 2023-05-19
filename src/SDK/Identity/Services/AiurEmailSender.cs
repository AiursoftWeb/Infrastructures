using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.Scanner.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Identity.Services;

public class AiurEmailSender : ITransientDependency
{
    private readonly AppsContainer _appsContainer;
    private readonly EventService _eventService;
    private readonly ILogger<AiurEmailSender> _logger;
    private readonly string _mailPassword;
    private readonly string _mailServer;
    private readonly string _mailUser;

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

    public async Task SendEmail(string fromDisplayName, string target, string targetSubject, string content)
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
                Subject = targetSubject,
                IsBodyHtml = true
            };
            mailMessage.To.Add(target);
            mailMessage.Bcc.Add(_mailUser);
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception e)
        {
            try
            {
                _logger.LogError(e, e.Message);
                var accessToken = _appsContainer.AccessTokenAsync();
                await _eventService.LogExceptionAsync(await accessToken, e);
            }
            catch
            {
                // ignored
            }
        }
    }
}