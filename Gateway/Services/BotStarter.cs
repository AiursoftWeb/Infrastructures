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
        private readonly SecurityBot _bot;
        private readonly BotFactory _botFactory;
        private Timer _timer;

        public BotStarter(
            ILogger<TimedCleaner> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            using var scope = scopeFactory.CreateScope();
            _bot = scope.ServiceProvider.GetService<SecurityBot>();
            _botFactory = scope.ServiceProvider.GetService<BotFactory>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BotStarter Background Service is starting.");
            _timer = new Timer(DoWork, null, 5, Timeout.Infinite‬);
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                _logger.LogInformation("bot starter task started!");
                _botFactory.BuildBotProperties(_bot);
                await _bot.Start(false);
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
