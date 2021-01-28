using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.Services;
using Aiursoft.Scanner.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Repositories
{
    public class FileRepo : IScopedDependency
    {
        private readonly ProbeDbContext _dbContext;
        private readonly IStorageProvider _storageProvider;

        public FileRepo(
            ProbeDbContext dbContext,
            IStorageProvider storageProvider)
        {
            _dbContext = dbContext;
            _storageProvider = storageProvider;
        }

        public async Task<File> GetFileInFolder(Folder context, string fileName)
        {
            var file = context.Files?.SingleOrDefault(t => t.FileName == fileName);
            if (file == null)
            {
                file = await _dbContext
                    .Files
                    .Where(t => t.ContextId == context.Id)
                    .SingleOrDefaultAsync(t => t.FileName == fileName);
            }
            return file;
        }

        public async Task DeleteFile(File file)
        {
            _dbContext.Files.Remove(file);
            var haveDaemon = await _dbContext.Files.Where(f => f.Id != file.Id).AnyAsync(f => f.HardwareId == file.HardwareId);
            if (!haveDaemon)
            {
                _storageProvider.DeleteToTrash(file.HardwareId);
            }
        }

        public async Task DeleteFileById(int fileId)
        {
            var file = await _dbContext.Files.SingleOrDefaultAsync(t => t.Id == fileId);
            if (file != null)
            {
                await DeleteFile(file);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> SaveFileToDb(string fileName, int folderId, long size)
        {
            var newFile = new File
            {
                FileName = fileName, //file.FileName,
                ContextId = folderId,
                FileSize = size
            };
            await _dbContext.Files.AddAsync(newFile);
            await _dbContext.SaveChangesAsync();
            return newFile.HardwareId;
        }

        public async Task CopyFile(string fileName, long fileSize, int contextId, string hardwareId)
        {
            var newFile = new File
            {
                FileName = fileName,
                FileSize = fileSize,
                ContextId = contextId,
                HardwareId = hardwareId
            };
            await _dbContext.Files.AddAsync(newFile);
            await _dbContext.SaveChangesAsync();
        }
    }
}
