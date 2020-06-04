using Aiursoft.Gateway.Controllers;
using Aiursoft.Gateway.SDK.Services;
using Aiursoft.Identity.Services;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK;
using Aiursoft.XelNaga.Models;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class ConfirmationEmailSender : ITransientDependency
    {
        private readonly GatewayLocator _serviceLocation;
        private readonly AiurEmailSender _emailSender;
        public ConfirmationEmailSender(
            GatewayLocator serviceLocation,
            AiurEmailSender emailSender)
        {
            _serviceLocation = serviceLocation;
            _emailSender = emailSender;
        }

        public async Task SendConfirmation(string userId, string emailAddress, string token)
        {
            var callbackUrl = new AiurUrl(_serviceLocation.Endpoint, "Password", nameof(PasswordController.EmailConfirm), new
            {
                userId,
                code = token
            });
            await _emailSender.SendEmail("Aiursoft Account Service", emailAddress, $"{Values.ProjectName} Account Email Confirmation",
                $"Please confirm your email by clicking <a href='{callbackUrl}'>here</a>");
        }

        public Task SendResetPassword(string code, string userId, string targetEmail)
        {
            var callbackUrl = new AiurUrl(_serviceLocation.Endpoint, "Password", nameof(PasswordController.ResetPassword), new
            {
                Code = code,
                UserId = userId
            });
            return _emailSender.SendEmail("Aiursoft Account Service", targetEmail, "Reset Password",
                $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>");
        }
    }
}
