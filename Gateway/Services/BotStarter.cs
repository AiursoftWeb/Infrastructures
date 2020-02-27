using Aiursoft.Gateway.Bots;
using Aiursoft.Scanner.Interfaces;
using Kahla.SDK.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Gateway.Services
{
    public class BotStarter : IHostedService, IDisposable, ISingletonDependency
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public BotStarter(
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BotStarter Background Service is starting.");
            DoWork();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void DoWork()
        {
            try
            {
                _logger.LogInformation("bot starter task started!");
                using var scope = _scopeFactory.CreateScope();
                var bot = scope.ServiceProvider.GetService<SecurityBot>();
                scope.ServiceProvider.GetService<BotFactory>().BuildBotProperties(bot);
                var _ = bot.Start(false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
            }
        }

        public void Dispose()
        {
        }
    }
}
