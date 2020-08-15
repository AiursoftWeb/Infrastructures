using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Repositories
{
    public class SiteRepo : IScopedDependency
    {
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
                throw new AiurAPIModelException(ErrorType.NotFound, $"Could not find a site with name: '{siteName}'");
            }
            if (site.AppId != appid)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "The site you tried to access is not your app's site.");
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
}
