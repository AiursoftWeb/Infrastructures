using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class AuthLogger : IScopedDependency
    {
        private readonly GatewayDbContext _dbContext;

        public AuthLogger(
            GatewayDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task LogAuthRecord(string userId, HttpContext httpContext, bool success, string appId)
        {
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
                    Success = true,
                    AppId = appId
                };
                _dbContext.AuditLogs.Add(log);
                return _dbContext.SaveChangesAsync();
            }
        }
    }
}
