using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aiursoft.Probe.Data;
using Aiursoft.Scanner.Abstract;
using Aiursoft.CSTools.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Probe.Services;

public class TimedCleaner : IHostedService, IDisposable, ISingletonDependency
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer _timer;

    public TimedCleaner(
        ILogger<TimedCleaner> logger,
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment env)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _env = env;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_env.IsDevelopment() || !EntryExtends.IsProgramEntry())
        {
            _logger.LogInformation("Skip cleaner in development environment");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Timed Background Service is starting");
        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(10));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Background Service is stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        try
        {
            _logger.LogInformation("Cleaner task started!");
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProbeDbContext>();
                var storageProvider = scope.ServiceProvider.GetRequiredService<IStorageProvider>();
                await AllClean(dbContext, storageProvider);
            }

            _logger.LogInformation("Cleaner task finished!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cleaner crashed!");
        }
    }

    private async Task AllClean(ProbeDbContext dbContext, IStorageProvider storageProvider)
    {
        var files = await dbContext.Files.ToListAsync();
        foreach (var file in files)
        {
            if (storageProvider.ExistInHardware(file.HardwareId))
            {
                continue;
            }

            _logger.LogWarning(
                "Cleaner message: File with Id: {HardwareId} was found in database but not found on disk! Deleting record in database...", file.HardwareId);
            // delete file in db.
            dbContext.Files.Remove(file);
        }

        await dbContext.SaveChangesAsync();
        var storageFiles = storageProvider.GetAllFileNamesInHardware();
        foreach (var file in storageFiles)
        {
            if (files.Any(t => t.HardwareId == file))
            {
                continue;
            }

            _logger.LogWarning(
                "Cleaner message: File with hardware Id: {File} was found on disk but not found in database! Consider delete that file on disk!", file);
            // delete file in disk
            storageProvider.DeleteToTrash(file);
        }
    }
}