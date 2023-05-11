using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.XelNaga.Services;
using Aiursoft.XelNaga.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.XelNaga.Tests.Services;

[TestClass]
public class AsyncHelperTests
{
    private int[] array;

    private List<DemoBook> books;

    // 50 ms and get 200.
    // 1000 ms and get 4000.
    private readonly int expectedCount = new Random().Next(3000, 5000);
    private readonly int fakeWait = new Random().Next(40, 60);
    private readonly int threads = new Random().Next(150, 250);
    private double expectedTime => expectedCount * 1.0 * fakeWait / threads;
    private double expectedMaxWait => expectedTime * 3.2;
    private double expectedMinWait => expectedTime * 1.01;

    [TestInitialize]
    public void InitBooks()
    {
        array = new int[expectedCount];
        books = new List<DemoBook>();
        for (var i = 0; i < expectedCount; i++) books.Add(new DemoBook());
    }

    [TestMethod]
    public async Task TestTaskWillFinish()
    {
        var tasksFactories = new List<Func<Task>>();
        for (var i = 0; i < expectedCount; i++)
        {
            var copy = i;
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
        await books.ForEachInThreadsPool(async book =>
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
        await books.ForEachParallel(async book =>
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
        var i = 0;
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
            var i = new Random().Next(0, 1);
            await Task.Delay(1);
            return i + new Random().Next(1, 10);
        });
        Assert.IsTrue(result >= 1);
    }

    private async Task SetPosition(int i, IList<int> arrayToChange)
    {
        await Task.Delay(fakeWait);
        arrayToChange[i]++;
    }

    [TestMethod]
    public void TestTryAsyncSuccess()
    {
        var invoked = false;
        AsyncHelper.TryAsync(() => Task.CompletedTask, 3, _ =>
        {
            invoked = true;
            return Task.CompletedTask;
        });
        Assert.IsFalse(invoked);
    }

    [TestMethod]
    public void TestTryAsyncRetrySuccess()
    {
        var called = false;
        var invoked = false;

        AsyncHelper.TryAsync(() =>
        {
            if (called) return Task.CompletedTask;

            called = true;
            throw new Exception("");
        }, 3, _ =>
        {
            invoked = true;
            return Task.CompletedTask;
        });
        Assert.IsTrue(called);
        Assert.IsTrue(invoked);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void TestTryAsyncFailed()
    {
        var invoked = false;
        AsyncHelper.TryAsync(() => { throw new Exception(""); }, 2, _ =>
        {
            invoked = true;
            return Task.CompletedTask;
        });
        Assert.IsTrue(invoked);
    }
}