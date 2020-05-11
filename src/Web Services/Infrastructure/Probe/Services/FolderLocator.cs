using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Services;
using Microsoft.EntityFrameworkCore;
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
        private readonly AiurCache _aiurCache;

        public FolderLocator(
            ProbeDbContext dbContext,
            ACTokenManager tokenManager,
            AiurCache aiurCache)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
            _aiurCache = aiurCache;
        }

        private Task<Folder> GetFolderFromDB(int folderId)
        {
            return _dbContext
                .Folders
                .AsNoTracking()
                .Include(t => t.Files)
                .Include(t => t.SubFolders)
                .SingleOrDefaultAsync(t => t.Id == folderId);
        }

        private async Task<Folder> GetSubFolder(int rootFolderId, string subFolderName)
        {
            var folder = await _aiurCache.GetAndCacheWhen(
                cacheKey: $"folder_object_{rootFolderId}",
                backup: () => GetFolderFromDB(rootFolderId),
                when: (folder) => folder.SubFolders.Any(f => f.FolderName == subFolderName));
            return folder.SubFolders.SingleOrDefault(f => f.FolderName == subFolderName);
        }

        public Task<Folder> GetFolderWithFiles(int folderId)
        {
            return _aiurCache.GetAndCache($"folder_object_{folderId}", () => GetFolderFromDB(folderId));
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
                return await GetFolderWithFiles(root.Id);
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
            return await LocateAsync(folderNames.Skip(1).ToArray(), subFolder, recursiveCreate);
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
