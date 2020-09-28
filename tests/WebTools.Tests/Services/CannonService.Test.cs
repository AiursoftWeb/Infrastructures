using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aiursoft.Scanner;
using Aiursoft.WebTools.Tests.Models;

namespace Aiursoft.WebTools.Tests.Services
{
	[TestClass]
	public class CannonService
	{
		private IServiceProvider _serviceProvider;

		[TestInitialize]
		public void Init()
		{
			_serviceProvider = new ServiceCollection()
				.AddLogging()
				.AddLibraryDependencies()
				.BuildServiceProvider();
		}

		[TestMethod]
		public async Task TestCannon()
        {
			var controller = _serviceProvider.GetRequiredService<DemoController>();
			var startTime = DateTime.UtcNow;
			controller.DemoAction();
			var endTime = DateTime.UtcNow;
			Assert.IsTrue(endTime - startTime < TimeSpan.FromMilliseconds(1000), "Demo action should finish very fast.");
			Assert.AreEqual(false, DemoService.Done, "When demo action finished, work is not over yet.");
			await Task.Delay(300);
			Assert.AreEqual(true, DemoService.Done, "After a while, the async job is done.");
        }
	}
}
