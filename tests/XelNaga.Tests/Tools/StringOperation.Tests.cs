using Aiursoft.XelNaga.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;

namespace Aiursoft.XelNaga.Tests.Tools
{
    [TestClass]
    public class StringOperation
    {
        [TestMethod]
        public void BytesToBase64()
        {
            var bytes = Encoding.ASCII.GetBytes("my_test_string_(17)[]@$");

            var result = bytes.BytesToBase64();

            Assert.AreEqual(result, "bXlfdGVzdF9zdHJpbmdfKDE3KVtdQCQ=");
        }

        [TestMethod]
        public void Base64ToBytes()
        {
            var base64 = "bXlfdGVzdF9zdHJpbmdfKDE3KVtdQCQ=";

            var result = base64.Base64ToBytes();

            var bytes = Encoding.ASCII.GetBytes("my_test_string_(17)[]@$");

            Assert.IsTrue(result.SequenceEqual(bytes));
        }

        [TestMethod]
        public void GetMd5()
        {
            var source = "my_test_string_(17)[]@$";

            var result = source.GetMD5();

            Assert.AreEqual(result, "0a4112dd96480d0f3eec8ce5b42082a6");
        }

    }
}
