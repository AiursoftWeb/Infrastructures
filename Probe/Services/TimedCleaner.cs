using Aiursoft.Probe.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Services
{
    public class TimedCleaner : IHostedService, IDisposable
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public TimedCleaner(
            IConfiguration configuration,
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory)
        {
            Configuration = configuration;
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
                    var dbContext = scope.ServiceProvider.GetRequiredService<ProbeDbContext>();
                    await AllClean(dbContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
            }
        }

        private Task AllClean(ProbeDbContext dbContext)
        {
            return Task.CompletedTask;
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
