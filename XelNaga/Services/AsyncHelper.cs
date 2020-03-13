using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Services
{
    public static class AsyncHelper
    {
        private static readonly TaskFactory _taskFactory = new
            TaskFactory(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
            => _taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static void RunSync(Func<Task> func)
            => _taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static Task ForEachParallel<T>(this IEnumerable<T> items, Func<T, Task> function)
        {
            var taskList = new List<Task>();
            foreach (var item in items)
            {
                taskList.Add(function(item));
            }
            return Task.WhenAll(taskList);
        }
    }
}
