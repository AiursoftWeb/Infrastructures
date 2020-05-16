using Aiursoft.Archon.SDK.Services;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Repositories
{
    public class AppRepo : IScopedDependency
    {
        private readonly ACTokenValidator _acTokenManager;
        private readonly ProbeDbContext _dbContext;
        private readonly SiteRepo _siteRepo;

        public AppRepo(
            ACTokenValidator acTokenManager,
            ProbeDbContext dbContext,
            SiteRepo siteRepo)
        {
            _acTokenManager = acTokenManager;
            _dbContext = dbContext;
            _siteRepo = siteRepo;
        }

        public async Task<string> GetAppId(string accessToken)
        {
            return (await GetApp(accessToken)).AppId;
        }

        public async Task<ProbeApp> GetApp(string accessToken)
        {
            var appid = _acTokenManager.ValidateAccessToken(accessToken);
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
