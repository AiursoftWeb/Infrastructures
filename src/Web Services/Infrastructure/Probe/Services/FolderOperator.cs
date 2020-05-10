using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    [Obsolete]
    public class FolderOperator : IScopedDependency
    {
        private readonly ProbeDbContext _dbContext;
        private readonly IStorageProvider _storageProvider;

        public FolderOperator(
            ProbeDbContext dbContext,
            IStorageProvider storageProvider)
        {
            _dbContext = dbContext;
            _storageProvider = storageProvider;
        }

        public async Task<long> GetFolderSize(Folder folder)
        {
            long size = 0;
            var subfolders = await _dbContext
                .Folders
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            foreach (var subfolder in subfolders)
            {
                size += await GetFolderSize(subfolder);
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
