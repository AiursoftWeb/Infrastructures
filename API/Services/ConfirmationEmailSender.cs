using Aiursoft.API.Controllers;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using System.Threading.Tasks;

namespace Aiursoft.API.Services
{
    public class ConfirmationEmailSender
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
            var callbackUrl = new AiurUrl(_serviceLocation.API, "Password", nameof(PasswordController.EmailConfirm), new
            {
                userId,
                code = token
            });
            await _emailSender.SendEmail(emailAddress, $"{Values.ProjectName} Account Email Confirmation",
                $"Please confirm your email by clicking <a href='{callbackUrl}'>here</a>");
        }

        public Task SendResetPassword(string code, string userId, string targetEmail)
        {
            var callbackUrl = new AiurUrl(_serviceLocation.API, "User", nameof(PasswordController.ResetPassword), new
            {
                Code = code,
                UserId = userId
            });
            return _emailSender.SendEmail(targetEmail, "Reset Password",
                $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>");
        }
    }
}
