using System.Threading.Tasks;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Probe.Repositories;

public class AppRepo : IScopedDependency
{
    private readonly AiursoftAppTokenValidator _aiursoftAppTokenManager;
    private readonly ProbeDbContext _dbContext;
    private readonly SiteRepo _siteRepo;

    public AppRepo(
        AiursoftAppTokenValidator aiursoftAppTokenManager,
        ProbeDbContext dbContext,
        SiteRepo siteRepo)
    {
        _aiursoftAppTokenManager = aiursoftAppTokenManager;
        _dbContext = dbContext;
        _siteRepo = siteRepo;
    }

    public async Task<string> GetAppId(string accessToken)
    {
        return (await GetApp(accessToken)).AppId;
    }

    public async Task<ProbeApp> GetApp(string accessToken)
    {
        var appid = await _aiursoftAppTokenManager.ValidateAccessTokenAsync(accessToken);
        var appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.AppId == appid);
        if (appLocal != null)
        {
            return appLocal;
        }

        appLocal = new ProbeApp
        {
            AppId = appid
        };
        await _dbContext.Apps.AddAsync(appLocal);
        await _dbContext.SaveChangesAsync();
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