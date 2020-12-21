using Aiursoft.XelNaga.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.XelNaga.Tests.Services
{
    [TestClass]
    public class CounterTests
    {
        private Counter counter;

        [TestInitialize]
        public void Init()
        {
            counter = new Counter();
        }

        [TestMethod]
        public void TestGetCurrent()
        {
            Assert.AreEqual(-1, counter.GetCurrent);
        }

        [TestMethod]
        public void TestGetUniqueNo()
        {
            Assert.AreEqual(0, counter.GetUniqueNo());
        }
    }
}
