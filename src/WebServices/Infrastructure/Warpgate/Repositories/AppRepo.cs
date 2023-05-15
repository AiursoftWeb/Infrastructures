using System.Threading.Tasks;
using Aiursoft.Archon.SDK.Services;
using Aiursoft.Scanner.Abstract;
using Aiursoft.Warpgate.Data;
using Aiursoft.Warpgate.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warpgate.Repositories;

public class AppRepo : IScopedDependency
{
    private readonly ACTokenValidator _acTokenManager;
    private readonly WarpgateDbContext _dbContext;
    private readonly RecordRepo _recordRepo;

    public AppRepo(
        ACTokenValidator acTokenManager,
        WarpgateDbContext dbContext,
        RecordRepo recordRepo)
    {
        _acTokenManager = acTokenManager;
        _dbContext = dbContext;
        _recordRepo = recordRepo;
    }

    public async Task<string> GetAppId(string accessToken)
    {
        return (await GetApp(accessToken)).AppId;
    }

    public async Task<WarpgateApp> GetApp(string accessToken)
    {
        var appid = _acTokenManager.ValidateAccessToken(accessToken);
        var appLocal = await _dbContext.WarpApps.SingleOrDefaultAsync(t => t.AppId == appid);
        if (appLocal != null)
        {
            return appLocal;
        }

        appLocal = new WarpgateApp
        {
            AppId = appid
        };
        await _dbContext.WarpApps.AddAsync(appLocal);
        await _dbContext.SaveChangesAsync();
        return appLocal;
    }

    public async Task DeleteApp(WarpgateApp app)
    {
        var records = await _recordRepo.GetAllRecords(app.AppId);
        foreach (var record in records)
        {
            await _recordRepo.DeleteRecord(record);
        }

        _dbContext.WarpApps.Remove(app);
        await _dbContext.SaveChangesAsync();
    }
}