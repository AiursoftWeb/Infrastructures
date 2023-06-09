using System;
using System.Threading.Tasks;
using Aiursoft.Scanner.Abstract;
using Microsoft.Extensions.Logging;

namespace Aiursoft.XelNaga.Services;

/// <summary>
///     Retry engine.
/// </summary>
public class RetryEngine : ITransientDependency
{
    private static readonly Random Rnd = new();
    private readonly ILogger<RetryEngine> _logger;

    /// <summary>
    ///     Creates new retry engine.
    /// </summary>
    /// <param name="logger">Logger</param>
    public RetryEngine(ILogger<RetryEngine> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    ///     Run a task with retry.
    /// </summary>
    /// <typeparam name="T">Response type.</typeparam>
    /// <param name="taskFactory">Task factory.</param>
    /// <param name="attempts">Retry times.</param>
    /// <param name="when">On error event.</param>
    /// <returns>Response</returns>
    public async Task<T> RunWithTry<T>(
        Func<int, Task<T>> taskFactory,
        int attempts = 3,
        Predicate<Exception> when = null)
    {
        for (var i = 1; i <= attempts; i++)
        {
            try
            {
                _logger.LogDebug("Starting a job with retry... Attempt: {Attempt} (Starts from 1)", i);
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
                        _logger.LogWarning(e, $"A task that was asked to retry failed. But from the given condition is false, we gave up retry.");
                        throw;
                    }
                }

                if (i >= attempts)
                {
                    _logger.LogWarning(e,
                        "A task that was asked to retry failed. Maximum attempts {Max} already reached. We have to crash it", attempts);
                    throw;
                }

                _logger.LogCritical(e,
                    "A task that was asked to retry failed. Will retry soon. Current attempt is {Current}. maximum attempts is {Max}", i, attempts);

                await Task.Delay(ExponentialBackoffTimeSlot(i) * 1000);
            }
        }

        throw new InvalidOperationException("Code shall not reach here.");
    }

    /// <summary>
    ///     Please see <see href="https://en.wikipedia.org/wiki/Exponential_backoff">Exponential backoff </see> time slot.
    /// </summary>
    /// <param name="time">the time of trial</param>
    /// <returns>Time slot to wait.</returns>
    private static int ExponentialBackoffTimeSlot(int time)
    {
        var max = (int)Math.Pow(2, time);
        return Rnd.Next(0, max);
    }
}