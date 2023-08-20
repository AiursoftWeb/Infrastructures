using Aiursoft.AiurProtocol;
using Aiursoft.Directory.Controllers;
using Aiursoft.Identity.Services;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.SDK;

namespace Aiursoft.Directory.Services;

public class ConfirmationEmailSender : ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly AiurEmailSender _emailSender;

    public ConfirmationEmailSender(
        // TODO: Deprecate the IConfiguration.
        IConfiguration configuration,
        AiurEmailSender emailSender)
    {
        _configuration = configuration;
        _emailSender = emailSender;
    }

    public async Task SendConfirmation(string userId, string emailAddress, string token)
    {
        var callbackUrl = new AiurApiEndpoint(_configuration["DirectoryEndpoint"], "Password", nameof(PasswordController.EmailConfirm), new
        {
            userId,
            code = token
        });
        await _emailSender.SendEmail("Aiursoft Account Service", emailAddress,
            $"{Values.ProjectName} Account Email Confirmation",
            $"Please confirm your email by clicking <a href='{callbackUrl}'>here</a>");
    }

    public Task SendResetPassword(string code, string userId, string targetEmail)
    {
        var callbackUrl = new AiurApiEndpoint(_configuration["DirectoryEndpoint"], "Password", nameof(PasswordController.ResetPassword),
            new
            {
                Code = code,
                UserId = userId
            });
        return _emailSender.SendEmail("Aiursoft Account Service", targetEmail, "Reset Password",
            $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>");
    }
}