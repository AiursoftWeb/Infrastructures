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
    public class FileRepo : IScopedDependency
    {
        private readonly ProbeDbContext _dbContext;
        private readonly IStorageProvider _storageProvider;
        private readonly AiurCache _cache;

        public FileRepo(
            ProbeDbContext dbContext,
            IStorageProvider storageProvider,
            AiurCache cache)
        {
            _dbContext = dbContext;
            _storageProvider = storageProvider;
            _cache = cache;
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
                if (file != null)
                {
                    _cache.Clear($"folder_object_{context.Id}");
                }
            }
            return file;
        }

        public async Task DeleteFileObject(File file)
        {
            _dbContext.Files.Remove(file);
            var haveDaemon = await _dbContext.Files.Where(f => f.Id != file.Id).AnyAsync(f => f.HardwareId == file.HardwareId);
            if (!haveDaemon)
            {
                _storageProvider.Delete(file.HardwareId);
            }
        }

        public async Task DeleteFile(int fileId)
        {
            var file = await _dbContext.Files.SingleOrDefaultAsync(t => t.Id == fileId);
            if (file != null)
            {
                await DeleteFileObject(file);
                _cache.Clear($"folder_object_{file.ContextId}");
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
            _dbContext.Files.Add(newFile);
            await _dbContext.SaveChangesAsync();
            _cache.Clear($"folder_object_{folderId}");
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
            _dbContext.Files.Add(newFile);
            await _dbContext.SaveChangesAsync();
        }
    }
}
