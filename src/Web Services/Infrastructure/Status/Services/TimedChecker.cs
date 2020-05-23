using Aiursoft.Scanner.Interfaces;
using Aiursoft.Status.Data;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.Status.Services
{
    public class TimedChecker : IHostedService, IDisposable, ISingletonDependency
    {
        private Timer _timer;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public TimedChecker(
            ILogger<TimedChecker> logger,
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
                    var dbContext = scope.ServiceProvider.GetRequiredService<StatusDbContext>();
                    var http = scope.ServiceProvider.GetRequiredService<HTTPService>();
                    await AllCheck(dbContext, http);
                }
                _logger.LogInformation("Cleaner task finished!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleaner crashed!");
            }
        }

        private async Task AllCheck(StatusDbContext dbContext, HTTPService http)
        {
            var items = await dbContext.MonitorRules.ToListAsync();
            foreach (var item in items)
            {
                _logger.LogInformation($"Checking status for: {item.ProjectName}");
                try
                {
                    var content = await http.Get(new AiurUrl(item.CheckAddress), false);
                    var success = content.Contains(item.ExpectedContent);
                    if (!success)
                    {
                        var errorMessage = $"Status check for {item.ProjectName} did not pass. Expected: {item.ExpectedContent}. Got content: {content}";
                        _logger.LogError(errorMessage);
                    }
                    item.LastHealthStatus = success;
                    item.LastCheckTime = DateTime.UtcNow;
                    dbContext.Update(item);
                }
                catch
                {
                    item.LastHealthStatus = false;
                    item.LastCheckTime = DateTime.UtcNow;
                }
                await dbContext.SaveChangesAsync();
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
