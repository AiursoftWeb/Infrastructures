using System.Linq;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol.Exceptions;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.DBTools;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.Services;
using Aiursoft.Scanner.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Probe.Repositories;

public class FolderRepo : IScopedDependency
{
    private readonly ProbeDbContext _dbContext;
    private readonly FileRepo _fileRepo;
    private readonly IStorageProvider _storageProvider;
    private readonly AiursoftAppTokenValidator _tokenManager;
    private readonly FolderLockDictionary _lockDictionary;

    public FolderRepo(
        ProbeDbContext probeDbContext,
        FileRepo fileRepo,
        IStorageProvider storageProvider,
        AiursoftAppTokenValidator tokenManager,
        FolderLockDictionary lockDictionary)
    {
        _dbContext = probeDbContext;
        _fileRepo = fileRepo;
        _storageProvider = storageProvider;
        _tokenManager = tokenManager;
        _lockDictionary = lockDictionary;
    }

    private async Task<Folder> GetSubFolder(int rootFolderId, string subFolderName)
    {
        return (await GetFolderFromId(rootFolderId)).SubFolders.FirstOrDefault(f => f.FolderName == subFolderName);
    }

    public Task<Folder> GetFolderFromId(int folderId)
    {
        return _dbContext
            .Folders
            .AsNoTracking()
            .Include(t => t.Files)
            .Include(t => t.SubFolders)
            .SingleOrDefaultAsync(t => t.Id == folderId);
    }

    public async Task<Folder> GetFolderAsOwner(string accessToken, string siteName, string[] folderNames,
        bool recursiveCreate = false)
    {
        var appid =await _tokenManager.ValidateAccessTokenAsync(accessToken);
        var site = await _dbContext
            .Sites
            .SingleOrDefaultAsync(t => t.SiteName.ToLower() == siteName.ToLower());
        if (site == null)
        {
            throw new AiurServerException(Code.NotFound, "Not found target site!");
        }

        if (site.AppId != appid)
        {
            throw new AiurServerException(Code.Unauthorized, "The target folder is not your app's folder!");
        }

        var rootFolder = await GetFolderFromId(site.RootFolderId);
        return await GetFolderFromPath(folderNames, rootFolder, recursiveCreate);
    }

    public async Task CreateNewFolder(int contextId, string name)
    {
        var folderLock = _lockDictionary.GetLock(contextId);
        await folderLock.WaitAsync();
        try
        {
            var conflict = await _dbContext.Folders
                .Where(t => t.ContextId == contextId)
                .EnsureUniqueString(t => t.FolderName, name);
            if (conflict)
            {
                throw new AiurServerException(Code.Conflict, $"There is already a record with name: '{name}'. Please try another new name.");
            }
            
            var newFolder = new Folder
            {
                ContextId = contextId,
                FolderName = name.ToLower()
            };
            _dbContext.Folders.Add(newFolder);
            await _dbContext.SaveChangesAsync();
        }
        finally
        {
            folderLock.Release();
        }
    }

    public async Task<Folder> GetFolderFromPath(string[] folderNames, Folder root, bool recursiveCreate)
    {
        if (root == null)
        {
            return null;
        }

        if (!folderNames.Any())
        {
            return await GetFolderFromId(root.Id);
        }

        var subFolderName = folderNames[0];
        var subFolder = await GetSubFolder(root.Id, subFolderName);
        if (!recursiveCreate || subFolder != null || string.IsNullOrWhiteSpace(subFolderName))
        {
            return await GetFolderFromPath(folderNames.Skip(1).ToArray(), subFolder, recursiveCreate);
        }

        subFolder = new Folder
        {
            ContextId = root.Id,
            FolderName = subFolderName
        };
        await _dbContext.Folders.AddAsync(subFolder);
        await _dbContext.SaveChangesAsync();
        return await GetFolderFromPath(folderNames.Skip(1).ToArray(), subFolder, true);
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
            _fileRepo.DeleteFile(file);
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

        // TODO: Refactor with await.
        size += localFiles.Sum(t => _storageProvider.GetSize(t.HardwareId).Result);
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
        var folderLock = _lockDictionary.GetLock(folderId);
        await folderLock.WaitAsync();
        try
        {
            var folder = await _dbContext
                .Folders
                .SingleOrDefaultAsync(t => t.Id == folderId);

            if (folder == null)
            {
                return;
            }

            await DeleteFolderObject(folder);
            if (saveChanges)
            {
                await _dbContext.SaveChangesAsync();
            }
        }
        finally
        {
            folderLock.Release();
        }
    }
}