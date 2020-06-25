using Aiursoft.Scanner.Interfaces;
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
        private readonly ChannelLiveJudger _channelLiveJudger;

        public TimedCleaner(
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory,
            StargateMemory memoryContext,
            ChannelLiveJudger channelLiveJudger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _memoryContext = memoryContext;
            _channelLiveJudger = channelLiveJudger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");
            // Start cleaner after one day.
            // Because when stargate starts, all channels are treated dead.
            _timer = new Timer(DoWork, null, TimeSpan.FromDays(1), TimeSpan.FromMinutes(10));
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

        private async Task AllClean(StargateDbContext dbContext)
        {
            try
            {
                if (_memoryContext.Messages.Any())
                {
                    var middleMessage = _memoryContext.Messages.Average(t => t.Id);
                    _memoryContext.Messages.RemoveAll(t => t.Id < middleMessage);
                }
                var toDelete = dbContext
                    .Channels
                    .ToList()
                    .Where(t => _channelLiveJudger.IsDead(t.Id))
                    .ToList();
                dbContext.Channels.RemoveRange(toDelete);
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
