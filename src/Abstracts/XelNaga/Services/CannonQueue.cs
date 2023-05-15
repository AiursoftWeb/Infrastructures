using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aiursoft.XelNaga.Services;

public class CannonQueue : ISingletonDependency
{
    private readonly ILogger<CannonQueue> _logger;
    private readonly SafeQueue<Func<Task>> _pendingTaskFactories = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly object loc = new();
    private Task _engine = Task.CompletedTask;

    public CannonQueue(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CannonQueue> logger)
    {
        _scopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public void QueueNew(Func<Task> taskFactory, bool startTheEngine = true)
    {
        _pendingTaskFactories.Enqueue(taskFactory);
        if (!startTheEngine)
        {
            return;
        }

        lock (loc)
        {
            if (!_engine.IsCompleted)
            {
                return;
            }

            _logger.LogDebug("Engine is sleeping. Trying to wake it up.");
            _engine = RunTasksInQueue();
        }
    }

    public void QueueWithDependency<T>(Func<T, Task> bullet)
    {
        QueueNew(async () =>
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dependency = scope.ServiceProvider.GetRequiredService<T>();
                try
                {
                    await bullet(dependency);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An error occurred with Cannon. Dependency: {typeof(T).Name}.");
                }
            }
        });
    }

    public async Task RunTasksInQueue(int maxDegreeOfParallelism = 8)
    {
        var tasksInFlight = new List<Task>(maxDegreeOfParallelism);
        while (_pendingTaskFactories.Any() || tasksInFlight.Any())
        {
            while (tasksInFlight.Count < maxDegreeOfParallelism && _pendingTaskFactories.Any())
            {
                var taskFactory = _pendingTaskFactories.Dequeue();
                tasksInFlight.Add(taskFactory());
                _logger.LogDebug(
                    $"Engine selected one job to run. Currently there are still {_pendingTaskFactories.Count()} jobs remaining. {tasksInFlight.Count} jobs running.");
            }

            var completedTask = await Task.WhenAny(tasksInFlight).ConfigureAwait(false);
            await completedTask.ConfigureAwait(false);
            _logger.LogInformation(
                $"Engine finished one job. Currently there are still {_pendingTaskFactories.Count()} jobs remaining. {tasksInFlight.Count} jobs running.");
            tasksInFlight.Remove(completedTask);
        }
    }
}