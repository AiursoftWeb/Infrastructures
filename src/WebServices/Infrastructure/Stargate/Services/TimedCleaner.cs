using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstractions;
using Aiursoft.Stargate.Data;
using Aiursoft.CSTools.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Stargate.Services;

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
        // Start cleaner after one day.
        // Because when stargate starts, all channels are treated dead.
        _timer = new Timer(DoWork, null, TimeSpan.FromDays(1), TimeSpan.FromMinutes(10));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Background Service is stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        _logger.LogInformation("Cleaner task started!");
        using (var scope = _scopeFactory.CreateScope())
        {
            var memoryContext = scope.ServiceProvider.GetRequiredService<StargateMemory>();
            AllClean(memoryContext);
        }

        _logger.LogInformation("Cleaner task finished!");
    }

    private void AllClean(StargateMemory memory)
    {
        try
        {
            var toDelete = memory.GetDeadChannels();
            memory.DeleteChannels(toDelete.Select(t => t.Id));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cleaner crashed!");
        }
    }
}