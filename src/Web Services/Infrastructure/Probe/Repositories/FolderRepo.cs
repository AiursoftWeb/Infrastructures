using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.Services;
using Aiursoft.Scanner.Interfaces;
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

        public FolderRepo(
            ProbeDbContext probeDbContext,
            FileRepo fileRepo,
            IStorageProvider storageProvider)
        {
            _dbContext = probeDbContext;
            _fileRepo = fileRepo;
            _storageProvider = storageProvider;
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

        public async Task DeleteFolder(int folderId)
        {
            var folder = await _dbContext
                .Folders
                .SingleOrDefaultAsync(t => t.Id == folderId);
            if (folder != null)
            {
                await DeleteFolderObject(folder);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
