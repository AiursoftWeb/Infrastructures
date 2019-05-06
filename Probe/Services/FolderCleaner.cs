using Aiursoft.Probe.Data;
using Aiursoft.Pylon.Models.Probe;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class FolderCleaner
    {
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly ProbeDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public FolderCleaner(
            ProbeDbContext dbContext,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task DeleteFolderAsync(Folder folder)
        {
            var subfolders = await _dbContext
                .Folders
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            foreach (var subfolder in subfolders)
            {
                await DeleteFolderAsync(subfolder);
            }
            var localFiles = await _dbContext
                .Files
                .Where(t => t.ContextId == folder.Id)
                .ToListAsync();
            foreach (var file in localFiles)
            {
                _dbContext.Files.Remove(file);
                var path = _configuration["StoragePath"] + $@"{_}Storage{_}{file.Id}.dat";
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            _dbContext.Folders.Remove(folder);
        }
    }
}
