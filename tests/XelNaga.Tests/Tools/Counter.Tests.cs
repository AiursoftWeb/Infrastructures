using Aiursoft.XelNaga.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests.Tools
{
    [TestClass]
	public class CounterTests
	{
		[TestMethod]
		public async Task TestCounter()
		{
			var counter = new Counter();
			var obj = new object();
			var numbers = new int[10000];
			var tasksList = new ConcurrentBag<Task>();
			for (int i = 0; i < 100; i++)
			{
				var task = new TaskFactory().StartNew(() =>
				{
					for (int k = 0; k < 100; k++)
					{
						var uniqueNo = counter.GetUniqueNo();
						numbers[uniqueNo]++;
					}
				});
				lock (obj)
				{
					tasksList.Add(task);
				}
			}
			await Task.WhenAll(tasksList);
			Assert.AreEqual(counter.GetCurrent, 9999);
			Assert.AreEqual(numbers.Max(), 1);
			Assert.AreEqual(numbers.Min(), 1);
		}
	}
}
