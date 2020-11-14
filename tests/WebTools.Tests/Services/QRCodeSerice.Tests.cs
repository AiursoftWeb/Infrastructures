using Aiursoft.WebTools.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void TestChineseQRCode()
        {
            var myCode = _qrCodeService.ToQRCodeBase64("我能吞下玻璃而不伤身体。");
            Assert.IsTrue(myCode.Length > 5000, "Generated qrcode is too short.");
            Assert.IsTrue(myCode.Length < 50000,"Generated qrcode is too long.");
            Assert.IsTrue(myCode.StartsWith("data:image/png;base64,"));
        }

        [TestMethod]
        public void TestURLQRCode()
        {
            var myCode = _qrCodeService.ToQRCodeBase64("https://github.com/AiursoftWeb/Infrastructures/blob/dev/src/WebServices/Infrastructure/Wrapgate/Migrations/20200617163030_AddEnabled.Designer.cs");
            Assert.IsTrue(myCode.Length > 5000, "Generated qrcode is too short.");
            Assert.IsTrue(myCode.Length < 100000,"Generated qrcode is too long.");
            Assert.IsTrue(myCode.StartsWith("data:image/png;base64,"));
        }
    }
}
