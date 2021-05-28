using Aiursoft.Scanner.Interfaces;
using System;
using System.Threading.Tasks;

namespace Aiursoft.SDKTools.Services
{
    public class RetryEngine : ITransientDependency
    {
        private static Random rnd = new Random();

        public async Task<T> RunWithTry<T>(
            Func<int, Task<T>> taskFactory,
            int attempts = 3,
            Predicate<Exception> when = null)
        {
            for (var i = 1; i <= attempts; i++)
            {
                try
                {
                    var response = await taskFactory(i);
                    return response;
                }
                catch (Exception e)
                {
                    if (when != null)
                    {
                        var shouldRetry = when.Invoke(e);
                        if (!shouldRetry)
                        {
                            throw;
                        }
                    }

                    if (i >= attempts)
                    {
                        throw;
                    }

                    await Task.Delay(rnd.Next(0, (int)Math.Pow(2, i)) * 1000);
                }
            }

            throw new InvalidOperationException("Code shall not reach here.");
        }
    }
}
