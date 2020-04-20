using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDK.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File = Aiursoft.Probe.SDK.Models.File;

namespace Aiursoft.Probe.Services
{
    public class FolderOperator : ITransientDependency
    {
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly ProbeDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly AiurCache _cache;

        public FolderOperator(
            ProbeDbContext dbContext,
            IConfiguration configuration,
            AiurCache cache)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _cache = cache;
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
                DeleteFile(file);
            }
            _dbContext.Folders.Remove(folder);
        }

        public void DeleteFile(File file)
        {
            _dbContext.Files.Remove(file);
            var path = _configuration["StoragePath"] + $@"{_}Storage{_}{file.Id}.dat";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
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
            size += localFiles.Sum(t => GetFileSizeWithCache(t));
            return size;
        }

        private long GetFileSizeWithCache(File file)
        {
            return _cache.GetAndCache($"file-size-cache-id-{file.Id}", () => GetFileSize(file));
        }
        private long GetFileSize(File file)
        {
            var path = _configuration["StoragePath"] + $@"{_}Storage{_}{file.Id}.dat";
            if (System.IO.File.Exists(path))
            {
                return new FileInfo(path).Length;
            }
            else
            {
                return 0;
            }
        }
    }
}
