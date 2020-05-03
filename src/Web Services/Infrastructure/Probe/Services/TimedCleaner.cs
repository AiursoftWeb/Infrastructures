using Aiursoft.Probe.Data;
using Aiursoft.Scanner.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class TimedCleaner : IHostedService, IDisposable, ISingletonDependency
    {
        private Timer _timer;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IWebHostEnvironment _env;

        public TimedCleaner(
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_env.IsDevelopment())
            {
                _logger.LogInformation("Skip cleaner in development environment.");
                return Task.CompletedTask;
            }
            _logger.LogInformation("Timed Background Service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(10));
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
                if (!storageProvider.ExistInHardware(file.HardwareId))
                {
                    _logger.LogInformation($"Cleaner message: File with Id: {file.HardwareId} was found in database but not found on disk! Deleting record in database...");
                    // delete file in db.
                    dbContext.Files.Remove(file);
                }
                await dbContext.SaveChangesAsync();
            }
            var storageFiles = storageProvider.GetAllFileNamesInHardware();
            foreach (var file in storageFiles)
            {
                if (!await dbContext.Files.AnyAsync(t => t.HardwareId == file))
                {
                    _logger.LogCritical($"Cleaner message: File with harewareId: {file} was found on disk but not found in database! Consider Delet file on disk!");
                }
            }
            return;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
