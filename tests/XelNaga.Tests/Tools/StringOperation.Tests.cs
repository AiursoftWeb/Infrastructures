using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Aiursoft.XelNaga.Tools;

namespace Aiursoft.XelNaga.Tests.Tools
{
    [TestClass]
    public class StringOperation
    {
        [TestMethod]
        public void BytesToBase64()
        {
            var bytes = Encoding.ASCII.GetBytes("my_test_string");

            var result = bytes.BytesToBase64();

            Assert.AreEqual(result, "bXlfdGVzdF9zdHJpbmc=");
        }
    }
}
