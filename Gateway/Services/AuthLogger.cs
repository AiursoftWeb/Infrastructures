using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class AuthLogger : IScopedDependency
    {
        private readonly GatewayDbContext _dbContext;

        public AuthLogger(GatewayDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task LogAuthRecord(string userId, HttpContext httpContext, bool success, string appId)
        {
            if (httpContext.AllowTrack())
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
                return Task.CompletedTask;
            }
        }
    }
}
