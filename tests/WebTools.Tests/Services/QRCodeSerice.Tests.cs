using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Aiursoft.WebTools.Services;

namespace Aiursoft.WebTools.Tests.Services
{
    [TestClass]
    public class QRCodeSericeTests
    {
        private QRCodeService _qrCodeService;

        [TestInitialize]
        public void Initialize()
        {
            _qrCodeService = new QRCodeService();
        }

        [TestMethod]
        public void TestQRCode()
        {
            var myCode = _qrCodeService.ToQRCodeBase64("我能吞下玻璃而不伤身体。");
            Assert.IsTrue(myCode.Length > 10000);
            Assert.IsTrue(myCode.StartsWith("data:image/png;base64,"));
        }
    }
}
