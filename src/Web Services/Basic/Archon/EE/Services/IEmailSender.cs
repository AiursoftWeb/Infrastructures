using System.Threading.Tasks;

namespace Aiursoft.EE.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
