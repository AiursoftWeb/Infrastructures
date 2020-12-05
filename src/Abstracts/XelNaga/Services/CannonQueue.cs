using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Services
{
    public class CannonQueue : ISingletonDependency
    {
        private readonly SafeQueue<Func<Task>> _pendingTaskFactories = new SafeQueue<Func<Task>>();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly object loc = new object();
        private Task _engine = Task.CompletedTask;

        public CannonQueue(IServiceScopeFactory serviceScopeFactory)
        {
            _scopeFactory = serviceScopeFactory;
        }

        public void QueueNew(Func<Task> taskFactory)
        {
            _pendingTaskFactories.Enqueue(taskFactory);
            lock (loc)
            {
                if (_engine.IsCompleted)
                {
                    Console.WriteLine("Engine is sleeping. Trying to wake it up.");
                    _engine = RunTasksInQueue();
                }
            }
        }

        public void QueueWithDependency<T>(Func<T, Task> bullet)
        {
            QueueNew(() =>
            {
                using var scope = _scopeFactory.CreateScope();
                var dependency = scope.ServiceProvider.GetRequiredService<T>();
                try
                {
                    var task = bullet(dependency);
                    return task;
                }
                catch
                {

                }
                return Task.CompletedTask;
            });
        }

        private async Task RunTasksInQueue(int maxDegreeOfParallelism = 8)
        {
            var tasksInFlight = new List<Task>(maxDegreeOfParallelism);
            while (_pendingTaskFactories.Any())
            {
                while (tasksInFlight.Count < maxDegreeOfParallelism && _pendingTaskFactories.Any())
                {
                    Func<Task> taskFactory = _pendingTaskFactories.Dequeue();
                    tasksInFlight.Add(taskFactory());
                }

                Task completedTask = await Task.WhenAny(tasksInFlight).ConfigureAwait(false);
                await completedTask.ConfigureAwait(false);
                tasksInFlight.Remove(completedTask);
            }
        }
    }
}
