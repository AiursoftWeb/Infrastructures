using Aiursoft.Archon.SDK.Services;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Repositories
{
    public class AppRepo
    {
        private readonly ACTokenManager _acTokenManager;
        private readonly ProbeDbContext _dbContext;

        public AppRepo(
            ACTokenManager acTokenManager,
            ProbeDbContext dbContext)
        {
            _acTokenManager = acTokenManager;
            _dbContext = dbContext;
        }

        public async Task<string> GetAppId(string accessToken)
        {
            var appid = await _acTokenManager.ValidateAccessToken(accessToken);
            var appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.AppId == appid);
            if (appLocal == null)
            {
                appLocal = new ProbeApp
                {
                    AppId = appid
                };
                _dbContext.Apps.Add(appLocal);
                await _dbContext.SaveChangesAsync();
            }
            return appLocal.AppId;
        }
    }
}
