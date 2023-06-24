using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aiursoft.Observer.Data;
using Aiursoft.Scanner.Abstraction;
using Aiursoft.CSTools.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Observer.Services;

public class TimedCleaner : IHostedService, IDisposable, ISingletonDependency
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer _timer;

    public TimedCleaner(
        ILogger<TimedCleaner> logger,
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment env)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _env = env;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_env.IsDevelopment() || !EntryExtends.IsProgramEntry())
        {
            _logger.LogInformation("Skip cleaner in test environment");
            return Task.CompletedTask;
        }

        _logger.LogInformation("TimedCleaner Background Service is starting");
        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(60));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Background Service is stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        try
        {
            _logger.LogInformation("Cleaner task started!");
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ObserverDbContext>();
                await AllClean(dbContext);
            }

            _logger.LogInformation("Cleaner task finished!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cleaner crashed!");
        }
    }

    private async Task AllClean(ObserverDbContext dbContext)
    {
        var oldestRecordTime = DateTime.UtcNow - TimeSpan.FromDays(7);
        var items = await dbContext.ErrorLogs.Where(t => t.LogTime < oldestRecordTime).ToListAsync();
        dbContext.ErrorLogs.RemoveRange(items);
        await dbContext.SaveChangesAsync();
    }
}