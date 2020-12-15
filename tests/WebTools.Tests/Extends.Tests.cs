using Aiursoft.WebTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Aiursoft.WebTools.Tests
{
    [TestClass]
    public class ExtendsTests
    {
        [TestMethod]
        public void IsMobileBrowserYes()
        {
            var httpRequestMock = new Mock<HttpRequest>(MockBehavior.Strict);
            var header = new Dictionary<string, StringValues>(){
                { "User-Agent", new StringValues("Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1")}
            };

            httpRequestMock.Setup(h => h.Headers).Returns(new HeaderDictionary(header));
            Assert.AreEqual(true,  httpRequestMock.Object.IsMobileBrowser());
            httpRequestMock.VerifyAll();
        }

        [TestMethod]
        public void IsMobileBrowserNo()
        {
            var httpRequestMock = new Mock<HttpRequest>(MockBehavior.Strict);
            var header = new Dictionary<string, StringValues>(){
                { "User-Agent", new StringValues("Mozilla/5.0 (Windows NT 10.0; WOW64; rv:56.0) Gecko/20100101 Firefox/56.0")}
            };

            httpRequestMock.Setup(h => h.Headers).Returns(new HeaderDictionary(header));
            Assert.AreEqual(false,  httpRequestMock.Object.IsMobileBrowser());
            httpRequestMock.VerifyAll();
        }
    }
}