using Aiursoft.Archon.SDK.Services;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Repositories
{
    public class AppRepo : IScopedDependency
    {
        private readonly ACTokenManager _acTokenManager;
        private readonly ProbeDbContext _dbContext;
        private readonly SiteRepo _siteRepo;
        private readonly AiurCache _cache;

        public AppRepo(
            ACTokenManager acTokenManager,
            ProbeDbContext dbContext,
            SiteRepo siteRepo,
            AiurCache cache)
        {
            _acTokenManager = acTokenManager;
            _dbContext = dbContext;
            _siteRepo = siteRepo;
            _cache = cache;
        }

        public async Task<string> GetAppId(string accessToken)
        {
            return (await GetApp(accessToken)).AppId;
        }

        public async Task<ProbeApp> GetApp(string accessToken, bool fromCache = true)
        {
            var appid = await _acTokenManager.ValidateAccessToken(accessToken);
            ProbeApp appLocal = null;
            if (fromCache)
            {
                appLocal = await _cache.GetAndCache($"app_object_{appid}", () => _dbContext.Apps.SingleOrDefaultAsync(t => t.AppId == appid));
            }
            else
            {
                appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.AppId == appid);
            }
            if (appLocal == null)
            {
                appLocal = new ProbeApp
                {
                    AppId = appid
                };
                _dbContext.Apps.Add(appLocal);
                await _dbContext.SaveChangesAsync();
            }
            return appLocal;
        }

        public async Task DeleteApp(ProbeApp app)
        {
            var sites = await _siteRepo.GetAllSitesUnderApp(app.AppId);
            foreach (var site in sites)
            {
                await _siteRepo.DeleteSite(site);
            }
            _dbContext.Apps.Remove(app);
            await _dbContext.SaveChangesAsync();
        }
    }
}
