using Aiursoft.Probe.Data;
using Aiursoft.Pylon.Models.Probe;
using Aiursoft.Pylon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File = Aiursoft.Pylon.Models.Probe.File;

namespace Aiursoft.Probe.Services
{
    public class FolderOperator
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
            var tasks = new List<Task>();
            foreach (var subfolder in subfolders)
            {
                async Task addSubfolder()
                {
                    size += await GetFolderSite(subfolder);
                }
                tasks.Add(addSubfolder());
            }
            await Task.WhenAll(tasks);
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
