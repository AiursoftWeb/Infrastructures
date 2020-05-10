using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
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

        public FolderRepo(
            ProbeDbContext probeDbContext,
            FileRepo fileRepo)
        {
            _dbContext = probeDbContext;
            _fileRepo = fileRepo;
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

        public async Task DeleteFolder(int folderId)
        {
            var folder = await _dbContext
                .Folders
                .SingleOrDefaultAsync(t => t.Id == folderId);
            if (folder != null)
            {
                await DeleteFolderObject(folder);
            }
        }
    }
}
