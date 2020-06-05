using Aiursoft.Archon.SDK.Services;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.Wrapgate.Data;
using Aiursoft.Wrapgate.SDK.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Wrapgate.Repositories
{
    public class AppRepo : IScopedDependency
    {
        private readonly ACTokenValidator _acTokenManager;
        private readonly WrapgateDbContext _dbContext;
        private readonly RecordRepo _recordRepo;

        public AppRepo(
            ACTokenValidator acTokenManager,
            WrapgateDbContext dbContext,
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

        public async Task<WrapgateApp> GetApp(string accessToken)
        {
            var appid = _acTokenManager.ValidateAccessToken(accessToken);
            var appLocal = await _dbContext.WrapApps.SingleOrDefaultAsync(t => t.AppId == appid);
            if (appLocal == null)
            {
                appLocal = new WrapgateApp
                {
                    AppId = appid
                };
                _dbContext.WrapApps.Add(appLocal);
                await _dbContext.SaveChangesAsync();
            }
            return appLocal;
        }

        public async Task DeleteApp(WrapgateApp app)
        {
            var records = await _recordRepo.GetAllRecords(app.AppId);
            foreach (var record in records)
            {
                await _recordRepo.DeleteRecord(record);
            }
            _dbContext.WrapApps.Remove(app);
            await _dbContext.SaveChangesAsync();
        }
    }
}
