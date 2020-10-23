using Aiursoft.XelNaga.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.XelNaga.Models;

namespace Aiursoft.XelNaga.Tests.Models.Tests
{
    [TestClass]
    public class AiurUrlTests
    {
        [TestMethod]
        public void TestBasic()
        {
            var url = new AiurUrl("https://www.bing.com", "Home", "Search", new
            {
                Question = "MyQuestion",
                Count = 10,
                Email = "aaa@bbb.com"
            });
            var result = url.ToString();
            Assert.AreEqual("https://www.bing.com/Home/Search?question=MyQuestion&count=10&email=aaa%40bbb.com", result);
        }

        [TestMethod]
        public void TestComplicated()
        {
            var url = new AiurUrl("https://www.bing.com", "Home", "Search", new TestAddressModel
            {
                Question = "MyQuestion",
                Count = 10,
                Email = "aaa@bbb.com",
                MyNull = null,
                CreateTime = DateTime.Parse("2020-01-01 14:15:16")
            });
            var result = url.ToString();
            Assert.AreEqual("https://www.bing.com/Home/Search?question=MyQuestion&count=10&emailaddress=aaa%40bbb.com&createtime=2020-01-01T14%3A15%3A16.0000000", result);
        }
    }
}
