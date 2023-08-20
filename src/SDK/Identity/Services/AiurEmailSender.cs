using System.Net;
using System.Net.Mail;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.Scanner.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Identity.Services;

public class AiurEmailSender : ITransientDependency
{
    private readonly DirectoryAppTokenService _directoryAppTokenService;
    private readonly ObserverService _eventService;
    private readonly ILogger<AiurEmailSender> _logger;
    private readonly string _mailPassword;
    private readonly string _mailServer;
    private readonly string _mailUser;

    public AiurEmailSender(
        IConfiguration configuration,
        ILogger<AiurEmailSender> logger,
        DirectoryAppTokenService directoryAppTokenService,
        ObserverService eventService)
    {
        _mailUser = configuration["MailUser"];
        _mailPassword = configuration["MailPassword"];
        _mailServer = configuration["MailServer"];
        _logger = logger;
        _directoryAppTokenService = directoryAppTokenService;
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
                _logger.LogError(e, "Failed to send an email");
                var accessToken = _directoryAppTokenService.GetAccessTokenAsync();
                await _eventService.LogExceptionAsync(await accessToken, e);
            }
            catch
            {
                // ignored
            }
        }
    }
}