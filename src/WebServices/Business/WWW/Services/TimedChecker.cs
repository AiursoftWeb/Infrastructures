﻿using Aiursoft.Scanner.Abstractions;
using Aiursoft.WWW.Data;
using Aiursoft.CSTools.Tools;
using Aiursoft.DbTools;

namespace Aiursoft.WWW.Services;

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
            _logger.LogInformation("Skip cleaner in development environment");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Timed Background Service is starting");
        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(10));
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
                var dbContext = scope.ServiceProvider.GetRequiredService<WWWDbContext>();
                await Clean(dbContext);
            }

            _logger.LogInformation("Cleaner task finished!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cleaner crashed!");
        }
    }

    private Task Clean(WWWDbContext dbContext)
    {
        var toDeleteTime = DateTime.UtcNow - TimeSpan.FromDays(90);
        dbContext.SearchHistories.Delete(t => t.SearchTime < toDeleteTime);
        return dbContext.SaveChangesAsync();
    }
}