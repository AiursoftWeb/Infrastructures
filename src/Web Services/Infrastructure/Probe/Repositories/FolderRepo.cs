using Aiursoft.Archon.SDK.Services;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.Services;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Repositories
{
    public class FolderRepo : IScopedDependency
    {
        private readonly ProbeDbContext _dbContext;
        private readonly FileRepo _fileRepo;
        private readonly IStorageProvider _storageProvider;
        private readonly AiurCache _cache;
        private readonly ACTokenManager _tokenManager;

        public FolderRepo(
            ProbeDbContext probeDbContext,
            FileRepo fileRepo,
            IStorageProvider storageProvider,
            AiurCache cache,
            ACTokenManager tokenManager)
        {
            _dbContext = probeDbContext;
            _fileRepo = fileRepo;
            _storageProvider = storageProvider;
            _cache = cache;
            _tokenManager = tokenManager;
        }

        private Task<Folder> GetFolderFromIdWithoutCache(int folderId)
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
            var folder = await _cache.GetAndCacheWhen(
                cacheKey: $"folder_object_{rootFolderId}",
                backup: () => GetFolderFromIdWithoutCache(rootFolderId),
                when: (folder) => folder.SubFolders.Any(f => f.FolderName == subFolderName));
            return folder.SubFolders.SingleOrDefault(f => f.FolderName == subFolderName);
        }

        public Task<Folder> GetFolderFromId(int folderId)
        {
            return _cache.GetAndCache($"folder_object_{folderId}", () => GetFolderFromIdWithoutCache(folderId));
        }

        public async Task<Folder> GetFolderAsOwner(string accessToken, string siteName, string[] folderNames, bool recursiveCreate = false)
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
            return await GetFolderFromPath(folderNames, site.Root, recursiveCreate);
        }

        public Task<bool> FolderExists(int contextId, string newFolderName)
        {
            return _dbContext
                .Folders
                .Where(t => t.ContextId == contextId)
                .AnyAsync(t => t.FolderName == newFolderName.ToLower());
        }

        public Task CreateNewFolder(int contextId, string name)
        {
            var newFolder = new Folder
            {
                ContextId = contextId,
                FolderName = name.ToLower(),
            };
            _dbContext.Folders.Add(newFolder);
            return _dbContext.SaveChangesAsync();
        }

        public async Task<Folder> GetFolderFromPath(string[] folderNames, Folder root, bool recursiveCreate)
        {
            if (root == null) return null;
            if (!folderNames.Any())
            {
                return await GetFolderFromId(root.Id);
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
            return await GetFolderFromPath(folderNames.Skip(1).ToArray(), subFolder, recursiveCreate);
        }

        private async Task DeleteFolderObject(Folder folder)
        {
            var subfolders = await _dbContext
                .Folders
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            foreach (var subfolder in subfolders)
            {
                await DeleteFolderObject(subfolder);
            }
            var localFiles = await _dbContext
                .Files
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            foreach (var file in localFiles)
            {
                await _fileRepo.DeleteFileObject(file);
            }
            _dbContext.Folders.Remove(folder);
        }

        private async Task<long> GetFolderObjectSize(Folder folder)
        {
            long size = 0;
            var subfolders = await _dbContext
                .Folders
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            foreach (var subfolder in subfolders)
            {
                size += await GetFolderObjectSize(subfolder);
            }
            var localFiles = await _dbContext
                .Files
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            size += localFiles.Sum(t => _storageProvider.GetSize(t.HardwareId));
            return size;
        }

        public async Task<long> GetFolderSize(int folderId)
        {
            var folder = await _dbContext
                .Folders
                .SingleOrDefaultAsync(t => t.Id == folderId);
            if (folder != null)
            {
                return await GetFolderObjectSize(folder);
            }
            return 0;
        }

        public async Task DeleteFolder(int folderId, bool saveChanges = true)
        {
            var folder = await _dbContext
                .Folders
                .SingleOrDefaultAsync(t => t.Id == folderId);
            if (folder != null)
            {
                await DeleteFolderObject(folder);
                if (saveChanges)
                {
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
