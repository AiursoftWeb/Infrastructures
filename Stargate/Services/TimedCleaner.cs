using Aiursoft.Stargate.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Services
{
    public class TimedCleaner : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private IServiceScopeFactory _scopeFactory;
        private TimeoutCleaner _timeoutCleaner;

        public TimedCleaner(
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory,
            TimeoutCleaner timeoutCleaner)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _timeoutCleaner = timeoutCleaner;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(3), TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                _logger.LogInformation("Cleaner task started!");
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<StargateDbContext>();
                    await _timeoutCleaner.AllClean(dbContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
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
