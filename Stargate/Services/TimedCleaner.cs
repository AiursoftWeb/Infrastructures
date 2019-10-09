using Aiursoft.Pylon.Interfaces;
using Aiursoft.Stargate.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Stargate.Services
{
    public class TimedCleaner : IHostedService, IDisposable, ISingletonDependency
    {
        private Timer _timer;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly StargateMemory _memoryContext;

        public TimedCleaner(
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory,
            StargateMemory memoryContext)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _memoryContext = memoryContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(3), TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Cleaner task started!");
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<StargateDbContext>();
                await AllClean(dbContext);
            }
            _logger.LogInformation("Cleaner task finished!");
        }

        public async Task AllClean(StargateDbContext dbContext)
        {
            try
            {
                _memoryContext.Messages.RemoveAll(t => t.CreateTime < DateTime.UtcNow - new TimeSpan(0, 1, 0));
                dbContext.Channels.RemoveRange(dbContext.Channels.Where(t => DateTime.UtcNow > t.CreateTime + TimeSpan.FromSeconds(t.LifeTime)));
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cleaner crashed!");
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
