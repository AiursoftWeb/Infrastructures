using Aiursoft.XelNaga.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Aiursoft.XelNaga.Tests.Services
{
    [TestClass]
    public class CounterTests
    {
        [TestMethod]
        public void TestGetCurrent()
        {
            var counter = new Counter();
            Assert.AreEqual(0, counter.GetCurrent);
        }

        [TestMethod]
        public void TestGetUniqueNo()
        {
            var counter = new Counter();
            Assert.AreEqual(1, counter.GetUniqueNo());
        }

        [TestMethod]
        public void TestGetUniqueNoConcurrency()
        {
            var counter = new Counter();
            Thread[] threads = new Thread[3];
            for (int i = 0; i < 3; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(ThreadProc));
                t.Start(counter);
                threads[i] = t;
            }
            for(int i = 0; i < 3; i++)
            {
                threads[i].Join();
            }

            Assert.AreEqual(3, counter.GetCurrent);
        }

        private static void ThreadProc(object state)
        {
            if (state is Counter counter)
            {
                counter.GetUniqueNo();
            }
        }
    }
}
