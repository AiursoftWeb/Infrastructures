using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class FolderOperator : ITransientDependency
    {
        private readonly ProbeDbContext _dbContext;
        private readonly IStorageProvider _storageProvider;

        public FolderOperator(
            ProbeDbContext dbContext,
            IConfiguration configuration,
            AiurCache cache,
            IStorageProvider storageProvider)
        {
            _dbContext = dbContext;
            _storageProvider = storageProvider;
        }

        public async Task DeleteFolder(Folder folder)
        {
            var subfolders = await _dbContext
                .Folders
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            foreach (var subfolder in subfolders)
            {
                await DeleteFolder(subfolder);
            }
            var localFiles = await _dbContext
                .Files
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            foreach (var file in localFiles)
            {
                await DeleteFile(file);
            }
            _dbContext.Folders.Remove(folder);
        }

        public async Task DeleteFile(File file)
        {
            _dbContext.Files.Remove(file);
            var haveDaemon = await _dbContext.Files.Where(f => f.Id != file.Id).AnyAsync(f => f.HardwareId == file.HardwareId);
            if (!haveDaemon)
            {
                _storageProvider.Delete(file.HardwareId);
            }
        }

        public async Task<long> GetFolderSite(Folder folder)
        {
            long size = 0;
            var subfolders = await _dbContext
                .Folders
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            foreach (var subfolder in subfolders)
            {
                size += await GetFolderSite(subfolder);
            }
            var localFiles = await _dbContext
                .Files
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            size += localFiles.Sum(t => _storageProvider.GetSize(t.HardwareId));
            return size;
        }
    }
}
