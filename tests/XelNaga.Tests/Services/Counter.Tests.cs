using System.Threading;
using Aiursoft.XelNaga.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.XelNaga.Tests.Services;

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
        var threads = new Thread[3];
        for (var i = 0; i < 3; i++)
        {
            var t = new Thread(ThreadProc);
            t.Start(counter);
            threads[i] = t;
        }

        for (var i = 0; i < 3; i++) threads[i].Join();

        Assert.AreEqual(3, counter.GetCurrent);
    }

    private static void ThreadProc(object state)
    {
        if (state is Counter counter) counter.GetUniqueNo();
    }
}