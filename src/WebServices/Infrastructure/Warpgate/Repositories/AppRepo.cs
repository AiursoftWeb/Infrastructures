using Aiursoft.Directory.SDK.Services;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.Warpgate.Data;
using Aiursoft.Warpgate.SDK.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warpgate.Repositories;

public class AppRepo : IScopedDependency
{
    private readonly AiursoftAppTokenValidator _aiursoftAppTokenManager;
    private readonly WarpgateDbContext _dbContext;
    private readonly RecordRepo _recordRepo;

    public AppRepo(
        AiursoftAppTokenValidator aiursoftAppTokenManager,
        WarpgateDbContext dbContext,
        RecordRepo recordRepo)
    {
        _aiursoftAppTokenManager = aiursoftAppTokenManager;
        _dbContext = dbContext;
        _recordRepo = recordRepo;
    }

    public async Task<string> GetAppId(string accessToken)
    {
        return (await GetApp(accessToken)).AppId;
    }

    public async Task<WarpgateApp> GetApp(string accessToken)
    {
        var appid = await _aiursoftAppTokenManager.ValidateAccessTokenAsync(accessToken);
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