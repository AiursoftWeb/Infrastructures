using Aiursoft.Probe.Data;
using Aiursoft.Pylon.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class TimedCleaner : IHostedService, IDisposable, ISingletonDependency
    {
        private Timer _timer;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly char _ = Path.DirectorySeparatorChar;

        public TimedCleaner(
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
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
                    await AllClean(dbContext);
                }
                _logger.LogInformation("Cleaner task finished!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleaner crashed!");
            }
        }

        private async Task AllClean(ProbeDbContext dbContext)
        {
            var files = await dbContext.Files.ToListAsync();
            foreach (var file in files)
            {
                var path = _configuration["StoragePath"] + $"{_}Storage{_}{file.Id}.dat";
                if (!File.Exists(path))
                {
                    _logger.LogInformation($"Cleaner message: File with Id: {file.Id} was found in database but not found on disk! Deleting record in database...");
                    // delete file in db.
                    dbContext.Files.Remove(file);
                    await dbContext.SaveChangesAsync();
                }
            }
            var storageFiles = Directory.GetFiles(_configuration["StoragePath"] + $"{_}Storage");
            foreach (var file in storageFiles)
            {
                var fileName = Convert.ToInt32(Path.GetFileNameWithoutExtension(file));
                if (!files.Any(t => t.Id == fileName))
                {
                    _logger.LogInformation($"Cleaner message: File with Id: {fileName} was found on disk but not found in database! Deleting file on disk...");
                    // delete file on disk.
                    File.Delete(file);
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
