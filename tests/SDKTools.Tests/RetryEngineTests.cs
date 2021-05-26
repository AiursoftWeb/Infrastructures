using System;
using System.Threading.Tasks;
using Aiursoft.SDKTools.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Office.Datacenter.CapacityDeployment.EmergencyPatch.API.UnitTests
{
    [TestClass]
    public class RetryEngineTests
    {
        private RetryEngine retryEngine;

        [TestInitialize]
        public void TestInitialize()
        {
            retryEngine = new RetryEngine();
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
                var result = await retryEngine.RunWithTry(
                attempts =>
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
