using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class FolderLocator : IScopedDependency
    {
        private readonly ProbeDbContext _dbContext;
        private readonly ACTokenManager _tokenManager;
        private readonly IMemoryCache _cache;

        public FolderLocator(
            ProbeDbContext dbContext,
            ACTokenManager tokenManager,
            IMemoryCache cache)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
            _cache = cache;
        }

        private async Task<Folder> GetFolderFromDB(int folderId)
        {
            var folder = await _dbContext
                .Folders
                .AsNoTracking()
                .Include(t => t.Files)
                .Include(t => t.SubFolders)
                .SingleOrDefaultAsync(t => t.Id == folderId);
            _cache.Set($"folder_object_{folderId}", folder);
            return folder;
        }

        private async Task<Folder> GetSubFolder(int rootFolderId, string subFolderName, bool tructCache = false)
        {
            if (!_cache.TryGetValue($"folder_object_{rootFolderId}", out Folder folderInCache))
            {
                // Folder not in cache.
                folderInCache = await GetFolderFromDB(rootFolderId);
                return folderInCache
                    .SubFolders
                    .SingleOrDefault(t => t.FolderName == subFolderName);
            }
            else
            {
                // Folder in cache.
                var subFolder = folderInCache.SubFolders.SingleOrDefault(t => t.FolderName == subFolderName);
                if (subFolder == null && !tructCache)
                {
                    // Subfolder is not in cache. Might because cache is out dated!
                    await GetFolderFromDB(rootFolderId);
                    return await GetSubFolder(rootFolderId, subFolderName, true);
                }
                else
                {
                    // Subfolder is not in cache. And cache refreshed. Return null.
                    return subFolder;
                }
            }
        }

        public string[] SplitToFolders(string folderNames) =>
            folderNames?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

        public (string[] folders, string fileName) SplitToFoldersAndFile(string folderNames)
        {
            if (folderNames == null || folderNames.Length == 0)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, "The root folder isn't a file!");
            }
            var foldersWithFileName = SplitToFolders(folderNames);
            var fileName = foldersWithFileName.Last();
            var folders = foldersWithFileName.Take(foldersWithFileName.Count() - 1).ToArray();
            return (folders, fileName);
        }

        public async Task<Folder> LocateSiteAndFolder(string accessToken, string siteName, string[] folderNames, bool recursiveCreate = false)
        {
            var appid = await _tokenManager.ValidateAccessToken(accessToken);
            var site = await _dbContext
                .Sites
                .Include(t => t.Root)
                .SingleOrDefaultAsync(t => t.SiteName.ToLower() == siteName.ToLower());
            if (site == null)
            {
                throw new AiurAPIModelException(ErrorType.NotFound, "Not found target site!");
            }
            if (site.AppId != appid)
            {
                throw new AiurAPIModelException(ErrorType.Unauthorized, "The target folder is not your app's folder!");
            }
            return await LocateAsync(folderNames, site.Root, recursiveCreate);
        }

        public async Task<Folder> LocateAsync(string[] folderNames, Folder root, bool recursiveCreate)
        {
            if (root == null) return null;
            if (!folderNames.Any())
            {
                root.SubFolders = await _dbContext
                    .Folders
                    .AsNoTracking()
                    .Where(t => t.ContextId == root.Id)
                    .ToListAsync();
                root.Files = await _dbContext
                    .Files
                    .AsNoTracking()
                    .Where(t => t.ContextId == root.Id)
                    .ToListAsync();
                return root;
            }
            var subFolderName = folderNames[0];
            var subFolder = await GetSubFolder(root.Id, subFolderName);
            if (recursiveCreate && subFolder == null && !string.IsNullOrWhiteSpace(subFolderName))
            {
                subFolder = new Folder
                {
                    ContextId = root.Id,
                    FolderName = subFolderName
                };
                _dbContext.Folders.Add(subFolder);
                await _dbContext.SaveChangesAsync();
            }
            return await LocateAsync(
                folderNames.Skip(1).ToArray(), subFolder, recursiveCreate);
        }

        public string GetValidFileName(IEnumerable<string> existingFileNames, string expectedFileName)
        {
            while (existingFileNames.Any(t => t.ToLower() == expectedFileName.ToLower()))
            {
                expectedFileName = "_" + expectedFileName;
            }
            return expectedFileName;
        }
    }
}
