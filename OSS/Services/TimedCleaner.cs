using Aiursoft.OSS.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.OSS.Services
{
    public class TimedCleaner : IHostedService, IDisposable
    {
        private IConfiguration _configuration { get; }
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly IServiceScopeFactory _scopeFactory;

        public TimedCleaner(
            IConfiguration configuration,
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _scopeFactory = scopeFactory;
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
                    var dbContext = scope.ServiceProvider.GetRequiredService<OSSDbContext>();
                    await AllClean(dbContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
            }
        }

        private async Task AllClean(OSSDbContext _dbContext)
        {
            var outdatedFiles = (await _dbContext.OSSFile.Include(t => t.BelongingBucket).ToListAsync())
                .Where(t => t.UploadTime + new TimeSpan(t.AliveDays, 0, 0, 0) < DateTime.UtcNow)
                .ToList();

            foreach (var file in outdatedFiles)
            {
                var path = $@"{_configuration["StoragePath"]}{_}Storage{_}{file.BelongingBucket.BucketName}{_}{file.FileKey}.dat";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                _dbContext.OSSFile.Remove(file);
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully cleaned all trash.");

            await DeleteInvalidRecords(_dbContext);
            await DeleteInvalidFiles(_dbContext);
        }

        public async Task DeleteInvalidRecords(OSSDbContext _dbContext)
        {
            // Delete records that not in storage.
            foreach (var file in _dbContext.OSSFile)
            {
                var path = $@"{_configuration["StoragePath"]}{_}Storage{_}{file.BelongingBucket.BucketName}{_}{file.FileKey}.dat";
                if (!File.Exists(path))
                {
                    _dbContext.Remove(file);
                    _logger.LogWarning($"Deleted the file record in database: {file.BelongingBucket.BucketName}.{file.FileKey} because it was not found in storage.");
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInvalidFiles(OSSDbContext _dbContext)
        {
            // Delete files that not in record
            foreach (var bucket in _dbContext.Bucket)
            {
                var bucketPath = $@"{_configuration["StoragePath"]}{_}Storage{_}{bucket.BucketName}{_}";
                if (!Directory.Exists(bucketPath))
                {
                    Directory.CreateDirectory(bucketPath);
                }
                foreach (var item in new DirectoryInfo(bucketPath).GetFiles())
                {
                    try
                    {
                        var fileid = Convert.ToInt32(Path.GetFileNameWithoutExtension(item.Name));
                        // If file is not exist in the database
                        if (!await _dbContext.OSSFile.Where(t => t.BucketId == bucket.BucketId).AnyAsync(t => t.FileKey == fileid))
                        {
                            _logger.LogWarning($"Deleted the file in disk: {item.FullName} because it was not found in database.");
                            File.Delete(item.FullName);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning($"An error occured while analysing the file {item.FullName} because {e.Message}. We will delete it directly!");
                        File.Delete(item.FullName);
                    }
                }
            }
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
