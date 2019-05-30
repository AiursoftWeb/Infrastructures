using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aiursoft.API.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Aiursoft.Pylon.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.API.Services
{
    public class TimedCleaner : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public TimedCleaner(
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory)
        {
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
                    var dbContext = scope.ServiceProvider.GetRequiredService<APIDbContext>();
                    await AllClean(dbContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
            }
        }

        public async Task AllClean(APIDbContext dbContext)
        {
            try
            {
                await ClearTimeOutOAuthPack(dbContext);
                _logger.LogInformation("Clean finished!");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public Task ClearTimeOutOAuthPack(APIDbContext dbContext)
        {
            dbContext.OAuthPack.Delete(t => t.UseTime + new TimeSpan(1, 0, 0, 0) < DateTime.UtcNow);
            dbContext.OAuthPack.Delete(t => !t.IsAlive);
            return dbContext.SaveChangesAsync();
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
