using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.Services;
using Aiursoft.Scanner.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Probe.Repositories;

public class FileRepo : IScopedDependency
{
    private readonly CanonQueue _cannonQueue;
    private readonly ProbeDbContext _dbContext;

    public FileRepo(
        ProbeDbContext dbContext,
        CanonQueue cannonQueue)
    {
        _dbContext = dbContext;
        _cannonQueue = cannonQueue;
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

    public void DeleteFile(File file)
    {
        _dbContext.Files.Remove(file);
        _cannonQueue.QueueWithDependency<FileDeleter>(async fileDeleteService =>
        {
            await fileDeleteService.DeleteOnDisk(file);
        });
    }

    public async Task DeleteFileById(int fileId)
    {
        var file = await _dbContext.Files.SingleOrDefaultAsync(t => t.Id == fileId);
        if (file != null)
        {
            DeleteFile(file);
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

    public async Task UpdateName(int id, string newFileName)
    {
        var file = await _dbContext.Files.SingleAsync(t => t.Id == id);
        file.FileName = newFileName;
        _dbContext.Files.Update(file);
        await _dbContext.SaveChangesAsync();
    }
}