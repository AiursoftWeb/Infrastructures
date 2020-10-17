using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.XelNaga.Tests.Services
{
    [TestClass]
    public class AsyncHelperTests
    {
        private List<DemoBook> books;
        private int[] array;
        // 50 ms and get 200.
        // 1000 ms and get 4000.
        private int expectedCount = new Random().Next(3000, 5000);
        private int threads = new Random().Next(150, 250);
        private int fakeWait = new Random().Next(40, 60);
        private double expectedTime => (expectedCount * 1.0 * fakeWait) / threads;
        private double expectedMaxWait => expectedTime * 2.0;
        private double expectedMinWait => expectedTime * 1.1;

        [TestInitialize]
        public void InitBooks()
        {
            array = new int[expectedCount];
            books = new List<DemoBook>();
            for (int i = 0; i < expectedCount; i++)
            {
                books.Add(new DemoBook());
            }
        }

        [TestMethod]
        public async Task TestTaskWillFinish()
        {
            var tasksFactories = new List<Func<Task>>();
            for (int i = 0; i < expectedCount; i++)
            {
                int copy = i;
                tasksFactories.Add(() => SetPosition(copy, array));
            }

            var startTime = DateTime.UtcNow;
            await AsyncHelper.InvokeTasksByQueue(tasksFactories, threads);
            var endTime = DateTime.UtcNow;
            var executionTime = endTime - startTime;

            Assert.IsTrue(executionTime > TimeSpan.FromSeconds(expectedMinWait / 1000));
            Assert.IsTrue(executionTime < TimeSpan.FromSeconds(expectedMaxWait / 1000));
            Assert.AreEqual(array.Min(), 1);
            Assert.AreEqual(array.Max(), 1);
        }

        [TestMethod]
        public async Task TestAllItemsChangedInPool()
        {
            var startTime = DateTime.UtcNow;
            await books.ForEachInThreadsPool(async (book) =>
            {
                await Task.Delay(fakeWait);
                book.Id++;
            }, threads);
            var endTime = DateTime.UtcNow;
            var executionTime = endTime - startTime;

            Assert.IsTrue(executionTime > TimeSpan.FromSeconds(expectedMinWait / 1000));
            Assert.IsTrue(executionTime < TimeSpan.FromSeconds(expectedMaxWait / 1000));
            Assert.AreEqual(books.Select(t => t.Id).Min(), 1);
            Assert.AreEqual(books.Select(t => t.Id).Max(), 1);
        }

        [TestMethod]
        public async Task TestAllItemsChangedParallel()
        {
            var startTime = DateTime.UtcNow;
            await books.ForEachParallel(async (book) =>
            {
                await Task.Delay(fakeWait);
                book.Id++;
            });
            var endTime = DateTime.UtcNow;

            var executionTime = endTime - startTime;

            Assert.IsTrue(executionTime < TimeSpan.FromSeconds(0.6));
            Assert.AreEqual(books.Select(t => t.Id).Min(), 1);
            Assert.AreEqual(books.Select(t => t.Id).Max(), 1);
        }

        [TestMethod]
        public void TestRunSync()
        {
            int i = 0;
            AsyncHelper.RunSync(async () =>
            {
                await Task.Delay(1);
                i++;
            });
            Assert.AreEqual(i, 1);
        }

        [TestMethod]
        public void TestRunSyncWithResult()
        {
            var result = AsyncHelper.RunSync(async () =>
            {
                int i = 0;
                await Task.Delay(1);
                return i + 1;
            });
            Assert.AreEqual(result, 1);
        }

        private async Task SetPosition(int i, int[] array)
        {
            await Task.Delay(fakeWait);
            array[i]++;
        }
    }
}
