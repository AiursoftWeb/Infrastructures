using System;
using System.Threading.Tasks;
using Aiursoft.Scanner.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aiursoft.XelNaga.Services;

/// <summary>
///     Retry engine.
/// </summary>
public class RetryEngine : ITransientDependency
{
    private static readonly Random rnd = new();
    private readonly ILogger<RetryEngine> logger;

    /// <summary>
    ///     Creates new retry engine.
    /// </summary>
    /// <param name="logger">Logger</param>
    public RetryEngine(ILogger<RetryEngine> logger)
    {
        this.logger = logger;
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
            try
            {
                logger.LogDebug($"Starting a job with retry. Attempt: {i}. (Starts from 1)");
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
                        logger.LogWarning(
                            $"A task that was asked to retry failed. {e.Message} But from the given condition is false, we gave up retry.");
                        throw;
                    }

                    logger.LogInformation(
                        $"A task that was asked to retry failed. {e.Message} With given condition is true.");
                }

                if (i >= attempts)
                {
                    logger.LogWarning(
                        $"A task that was asked to retry failed. {e.Message} Maximum attempts {attempts} already reached. We have to crash it.");
                    throw;
                }

                logger.LogCritical(
                    $"A task that was asked to retry failed. {e.Message} Current attempt is {i}. maximum attempts is {attempts}. Will retry soon...");

                await Task.Delay(ExponentialBackoffTimeSlot(i) * 1000);
            }

        throw new InvalidOperationException("Code shall not reach here.");
    }

    /// <summary>
    ///     Please see <see href="https://en.wikipedia.org/wiki/Exponential_backoff">Exponetial backoff </see> time slot.
    /// </summary>
    /// <param name="time">the time of trial</param>
    /// <returns>Time slot to wait.</returns>
    private static int ExponentialBackoffTimeSlot(int time)
    {
        var max = (int)Math.Pow(2, time);
        return rnd.Next(0, max);
    }
}