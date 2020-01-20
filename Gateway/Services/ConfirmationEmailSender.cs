using Aiursoft.Gateway.Controllers;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Services;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Services;
using Aiursoft.XelNaga.Models;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class ConfirmationEmailSender : ITransientDependency
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly AiurEmailSender _emailSender;
        public ConfirmationEmailSender(
            ServiceLocation serviceLocation,
            AiurEmailSender emailSender)
        {
            _serviceLocation = serviceLocation;
            _emailSender = emailSender;
        }

        public async Task SendConfirmation(string userId, string emailAddress, string token)
        {
            var callbackUrl = new AiurUrl(_serviceLocation.Gateway, "Password", nameof(PasswordController.EmailConfirm), new
            {
                userId,
                code = token
            });
            await _emailSender.SendEmail("Aiursoft Account Service", emailAddress, $"{Values.ProjectName} Account Email Confirmation",
                $"Please confirm your email by clicking <a href='{callbackUrl}'>here</a>");
        }

        public Task SendResetPassword(string code, string userId, string targetEmail)
        {
            var callbackUrl = new AiurUrl(_serviceLocation.Gateway, "Password", nameof(PasswordController.ResetPassword), new
            {
                Code = code,
                UserId = userId
            });
            return _emailSender.SendEmail("Aiursoft Account Service", targetEmail, "Reset Password",
                $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>");
        }
    }
}
