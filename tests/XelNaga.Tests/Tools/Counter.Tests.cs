using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.CSTools.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.XelNaga.Tests.Tools;

[TestClass]
public class CounterTests
{
    [TestMethod]
    public async Task TestCounter()
    {
        var counter = new Counter();
        Assert.AreEqual(counter.GetCurrent, 0);
        var obj = new object();
        var numbers = new int[10000];
        var tasksList = new ConcurrentBag<Task>();
        for (var i = 0; i < 100; i++)
        {
            var task = Task.Run(() =>
            {
                for (var k = 0; k < 100; k++)
                {
                    var uniqueNo = counter.GetUniqueNo();
                    numbers[uniqueNo - 1]++;
                }
            });
            lock (obj)
            {
                tasksList.Add(task);
            }
        }

        await Task.WhenAll(tasksList);
        Assert.AreEqual(counter.GetCurrent, 10000);
        Assert.AreEqual(numbers.Max(), 1);
        Assert.AreEqual(numbers.Min(), 1);
    }
}