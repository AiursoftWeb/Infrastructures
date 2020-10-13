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
        const int expectedCount = 4000;

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
            await AsyncHelper.InvokeTasksByQueue(tasksFactories, 200);
            var endTime = DateTime.UtcNow;
            var executionTime = endTime - startTime;

            Assert.IsTrue(executionTime > TimeSpan.FromSeconds(1.1));
            Assert.IsTrue(executionTime < TimeSpan.FromSeconds(1.7));
            Assert.AreEqual(array.Min(), 1);
            Assert.AreEqual(array.Max(), 1);
        }

        [TestMethod]
        public async Task TestAllItemsChangedInPool()
        {
            var startTime = DateTime.UtcNow;
            await books.ForEachInThreadsPool(async (book) =>
            {
                await Task.Delay(50);
                book.Id++;
            }, 200);
            var endTime = DateTime.UtcNow;
            var executionTime = endTime - startTime;

            Assert.IsTrue(executionTime > TimeSpan.FromSeconds(1.1));
            Assert.IsTrue(executionTime < TimeSpan.FromSeconds(1.7));
            Assert.AreEqual(books.Select(t => t.Id).Min(), 1);
            Assert.AreEqual(books.Select(t => t.Id).Max(), 1);
        }

        [TestMethod]
        public async Task TestAllItemsChangedParallel()
        {
            var startTime = DateTime.UtcNow;
            await books.ForEachParallel(async (book) =>
            {
                await Task.Delay(50);
                book.Id++;
            });
            var endTime = DateTime.UtcNow;

            var executionTime = endTime - startTime;

            Assert.IsTrue(executionTime < TimeSpan.FromSeconds(0.6));
            Assert.AreEqual(books.Select(t => t.Id).Min(), 1);
            Assert.AreEqual(books.Select(t => t.Id).Max(), 1);
        }

        private async Task SetPosition(int i, int[] array)
        {
            await Task.Delay(50);
            array[i]++;
        }
    }
}
