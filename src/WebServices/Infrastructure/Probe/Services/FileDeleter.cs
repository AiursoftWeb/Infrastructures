using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.Directory.SDK.Services;
using Aiursoft.Observer.SDK.Services.ToObserverServer;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Scanner.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Probe.Services;

public class FileDeleter : ITransientDependency
{
    private readonly DirectoryAppTokenService _directoryAppTokenService;
    private readonly ObserverService _eventService;
    private readonly ProbeDbContext _probeDbContext;
    private readonly RetryEngine _retryEngine;
    private readonly IStorageProvider _storageProvider;

    public FileDeleter(
        ProbeDbContext probeDbContext,
        RetryEngine retryEngine,
        IStorageProvider storageProvider,
        ObserverService eventService,
        DirectoryAppTokenService directoryAppTokenService)
    {
        _probeDbContext = probeDbContext;
        _retryEngine = retryEngine;
        _storageProvider = storageProvider;
        _eventService = eventService;
        _directoryAppTokenService = directoryAppTokenService;
    }

    public async Task DeleteOnDisk(File file)
    {
        try
        {
            var haveDaemon = await _probeDbContext.Files.Where(f => f.Id != file.Id)
                .AnyAsync(f => f.HardwareId == file.HardwareId);
            if (!haveDaemon)
            {
                await _retryEngine.RunWithRetry(_ =>
                {
                    _storageProvider.DeleteToTrash(file.HardwareId);
                    return Task.CompletedTask;
                }, 10);
            }
        }
        catch (Exception e)
        {
            // TODO: Observer should integrate logger.
            var token = await _directoryAppTokenService.GetAccessTokenAsync();
            await _eventService.LogExceptionAsync(token, e, "Deleter");
        }
    }
}