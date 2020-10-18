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
        private readonly ChannelLiveJudge _channelLiveJudge;

        public TimedCleaner(
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory,
            StargateMemory memoryContext,
            ChannelLiveJudge channelLiveJudge)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _memoryContext = memoryContext;
            _channelLiveJudge = channelLiveJudge;
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
                var memoryContext = scope.ServiceProvider.GetRequiredService<StargateMemory>();
                await AllClean(dbContext, memoryContext);
            }
            _logger.LogInformation("Cleaner task finished!");
        }

        private async Task AllClean(StargateDbContext dbContext, StargateMemory memory)
        {
            try
            {
                var toDelete = dbContext
                    .Channels
                    .ToList()
                    .Where(t => _channelLiveJudge.IsDead(t.Id))
                    .ToList();
                memory.DeleteChannels(toDelete.Select(t => t.Id));
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
