using Aiursoft.AiurProtocol;
using Aiursoft.DbTools;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Probe.Repositories;

public class SiteRepo : IScopedDependency
{
    private static readonly SemaphoreSlim _createSiteLock = new(1, 1);
    private readonly ProbeDbContext _dbContext;
    private readonly FolderRepo _folderRepo;

    public SiteRepo(
        ProbeDbContext dbContext,
        FolderRepo folderRepo)
    {
        _dbContext = dbContext;
        _folderRepo = folderRepo;
    }

    public async Task<Site> GetSiteByNameUnderApp(string siteName, string appid)
    {
        var site = await GetSiteByName(siteName);
        if (site == null)
        {
            throw new AiurServerException(Code.NotFound, $"Could not find a site with name: '{siteName}'");
        }

        if (site.AppId != appid)
        {
            throw new AiurServerException(Code.Unauthorized,
                "The site you tried to access is not your app's site.");
        }

        return site;
    }

    public Task<Site> GetSiteByName(string siteName)
    {
        var lowerSiteName = siteName.ToLower();
        return _dbContext.Sites.SingleOrDefaultAsync(t => t.SiteName.ToLower() == lowerSiteName);
    }

    public async Task<Site> CreateSite(string newSiteName, bool openToUpload, bool openToDownload, string appid)
    {
        await _createSiteLock.WaitAsync();
        try
        {
            var conflict = await _dbContext.Sites.EnsureUniqueString(t => t.SiteName, newSiteName);
            if (conflict)
            {
                throw new AiurServerException(Code.Conflict, $"There is already a record with name: '{newSiteName}'. Please try another new name.");
            }
            var newRootFolder = new Folder
            {
                FolderName = "blob"
            };
            await _dbContext.Folders.AddAsync(newRootFolder);
            await _dbContext.SaveChangesAsync();
            var site = new Site
            {
                AppId = appid,
                SiteName = newSiteName.ToLower(),
                RootFolderId = newRootFolder.Id,
                OpenToUpload = openToUpload,
                OpenToDownload = openToDownload
            };
            await _dbContext.Sites.AddAsync(site);
            await _dbContext.SaveChangesAsync();
            return site;
        }
        finally
        {
            _createSiteLock.Release();
        }
    }

    public Task<List<Site>> GetAllSitesUnderApp(string appid)
    {
        return _dbContext
            .Sites
            .Where(t => t.AppId == appid)
            .ToListAsync();
    }

    public async Task DeleteSite(Site site)
    {
        await _folderRepo.DeleteFolder(site.RootFolderId, false);
        _dbContext.Sites.Remove(site);
        await _dbContext.SaveChangesAsync();
    }

    public Task UpdateSite(Site site)
    {
        _dbContext.Sites.Update(site);
        return _dbContext.SaveChangesAsync();
    }
}