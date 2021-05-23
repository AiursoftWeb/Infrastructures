using Aiursoft.SDKTools.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDKTools.Tests.AttributesTests
{
    [TestClass]
    public class ValidFolderNameTests
    {
        private ValidFolderName _validator;

        [TestInitialize]
        public void CreateValidator()
        {
            _validator = new ValidFolderName();
        }

        [TestMethod]
        [DataRow("aaaa.txt")]
        [DataRow("aaaatxt")]
        [DataRow("asdfasdfadfasdfasdfadf___^&(ad")]
        public void PassingTests(object inputValue)
        {
            Assert.AreEqual(ValidationResult.Success, _validator.TestEntry(inputValue));
        }

        [TestMethod]
        [DataRow("asdfasdfadfasdfasdfadf___^&(ad*")]
        [DataRow(@"
")]
        [DataRow("")]
        [DataRow("    ")]
        public void FailingTests(object inputValue)
        {
            Assert.AreNotEqual(ValidationResult.Success, _validator.TestEntry(inputValue));
        }
    }
}
