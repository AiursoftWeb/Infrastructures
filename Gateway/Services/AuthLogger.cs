using Aiursoft.Gateway.Data;
using Aiursoft.Gateway.Models;
using Aiursoft.Pylon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task LogAuthRecord(string userId, string ip, bool success, string appId)
        {
            var log = new AuditLogLocal
            {
                UserId = userId,
                IPAddress = ip,
                Success = success,
                AppId = appId
            };
            _dbContext.AuditLogs.Add(log);
            return _dbContext.SaveChangesAsync();
        }
    }
}
