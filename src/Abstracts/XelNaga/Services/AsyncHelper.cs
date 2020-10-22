using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Services
{
    public static class AsyncHelper
    {
        public static void TryAsync(Func<Task> steps, int times, Action<Exception> onError = null)
        {
            for (var i = 1; i <= times; i++)
            {
                try
                {
                    RunSync(async () => await steps());
                    break;
                }
                catch (Exception e)
                {
                    onError?.Invoke(e);
                    if (i >= times)
                    {
                        throw;
                    }
                    Thread.Sleep(i * 15 * 1000);
                }
            }
        }

        public static void TryAsyncForever(Func<Task> steps, Action<Exception> onError = null) => TryAsync(steps, int.MaxValue, onError);

        public static async Task InvokeTasksByQueue(IEnumerable<Func<Task>> taskFactories, int maxDegreeOfParallelism)
        {
            var queue = new Queue<Func<Task>>(taskFactories);
            if (queue.Count == 0)
            {
                return;
            }
            var tasksInFlight = new List<Task>(maxDegreeOfParallelism);
            do
            {
                while (tasksInFlight.Count < maxDegreeOfParallelism && queue.Count != 0)
                {
                    var taskFactory = queue.Dequeue();
                    tasksInFlight.Add(taskFactory());
                }

                var completedTask = await Task.WhenAny(tasksInFlight).ConfigureAwait(false);
                await completedTask.ConfigureAwait(false);
                tasksInFlight.Remove(completedTask);
            }
            while (queue.Count != 0 || tasksInFlight.Count != 0);
        }

        private static readonly TaskFactory TaskFactory = new
            TaskFactory(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
            => TaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static void RunSync(Func<Task> func)
            => TaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static Task ForEachParallel<T>(this IEnumerable<T> items, Func<T, Task> function)
            => Task.WhenAll(items
                .Select(function));

        public static Task ForEachInThreadsPool<T>(this IEnumerable<T> items, Func<T, Task> function, int maxDegreeOfParallelism = 8)
            => InvokeTasksByQueue(items.Select<T, Func<Task>>(t => () => function(t)), maxDegreeOfParallelism);
    }
}
