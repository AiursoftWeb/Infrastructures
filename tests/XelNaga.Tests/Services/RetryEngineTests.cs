using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests
{
    // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
    [TestClass]
    public class RetryEngineTests
    {
        private RetryEngine retryEngine;

        [TestInitialize]
        public void TestInitialize()
        {
            var logger = LoggerFactory.Create(l => { }).CreateLogger<RetryEngine>();
            retryEngine = new RetryEngine(logger);
        }

        [TestMethod]
        public async Task RetrySuccess()
        {
            var result = await retryEngine.RunWithTry(
            attempts =>
            {
                if (attempts == 1)
                {
                    throw new InvalidOperationException("Fake Exception.");
                }

                return Task.FromResult(12345);
            }, attempts: 2, when: e => e is InvalidOperationException);
            Assert.AreEqual(12345, result);
        }

        [TestMethod]
        public async Task RetryFailure()
        {
            try
            {
                await retryEngine.RunWithTry(attempts =>
                {
                    if (attempts == 1)
                    {
                        throw new InvalidOperationException("Fake Exception.");
                    }

                    if (attempts == 2)
                    {
                        throw new NotImplementedException("Fake Exception.");
                    }

                    return Task.FromResult(12345);
                }, attempts: 2, when: e => e is InvalidOperationException);

                Assert.Fail("Shouldn't suppress NotImplementedException.");
            }
            catch (NotImplementedException)
            {
            }
        }
    }
}
