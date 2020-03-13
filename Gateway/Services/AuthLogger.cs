using Aiursoft.Gateway.Bots;
using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.WebTools;
using Kahla.SDK.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class AuthLogger : IScopedDependency
    {
        private readonly GatewayDbContext _dbContext;
        private readonly SecurityBot _securityBot;

        public AuthLogger(
            GatewayDbContext dbContext,
            SecurityBot securityBot)
        {
            _dbContext = dbContext;
            _securityBot = securityBot;
        }

        public Task LogAuthRecord(string userId, HttpContext httpContext, bool success, string appId)
        {
            var _ = ShowBotAlert(userId, success).ConfigureAwait(false);
            if (httpContext.AllowTrack() || success == false)
            {
                var log = new AuditLogLocal
                {
                    UserId = userId,
                    IPAddress = httpContext.Connection.RemoteIpAddress.ToString(),
                    Success = success,
                    AppId = appId
                };
                _dbContext.AuditLogs.Add(log);
                return _dbContext.SaveChangesAsync();
            }
            else
            {
                var log = new AuditLogLocal
                {
                    UserId = userId,
                    IPAddress = "Unknown(because of `dnt` policy)",
                    Success = success,
                    AppId = appId
                };
                _dbContext.AuditLogs.Add(log);
                return _dbContext.SaveChangesAsync();
            }
        }

        public async Task ShowBotAlert(string userId, bool success)
        {
            var conversations = await _securityBot.ConversationService.AllAsync();
            var conversation = conversations
                .Items
                .Where(t => t.Discriminator == nameof(PrivateConversation))
                .FirstOrDefault(t => t.UserId == userId);
            if (conversation == null)
            {
                // Not my friend in Kahla. Skip.
                return;
            }
            var alertText = $"Your account is signing in!\r\nSuccess: {success}\r\n\r\nIf you are not the one who logged in, or this login is suspicious and you believe that someone else may have accessed your account, please change your password now!";
            await _securityBot.SendMessage(alertText, conversation.ConversationId, conversation.AesKey);
        }
    }
}
