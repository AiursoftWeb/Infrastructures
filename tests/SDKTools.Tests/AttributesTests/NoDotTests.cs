using System.ComponentModel.DataAnnotations;
using System.Net;
using Aiursoft.SDKTools.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.SDKTools.Tests.AttributesTests;

[TestClass]
public class NoDotTests
{
    private NoDot _validator;

    [TestInitialize]
    public void CreateValidator()
    {
        _validator = new NoDot();
    }

    [TestMethod]
    [DataRow(nameof(HttpStatusCode))]
    [DataRow("aaaatxt")]
    [DataRow("aaaatxt")]
    [DataRow("asdfasdfadfasdfasdfadf___^&(ad")]
    public void PassingTests(object inputValue)
    {
        Assert.AreEqual(ValidationResult.Success, _validator.TestEntry(inputValue));
    }

    [TestMethod]
    [DataRow(typeof(HttpStatusCode))]
    [DataRow(HttpStatusCode.OK)]
    [DataRow(5)]
    [DataRow("asdfa.sdfadfasdfasdfadf___^&(ad*")]
    [DataRow(@".
")]
    [DataRow("\u2029.\u2028.")]
    [DataRow("\u2028.\u2029")]
    [DataRow(".")]
    [DataRow("   . ")]
    public void FailingTests(object inputValue)
    {
        Assert.AreNotEqual(ValidationResult.Success, _validator.TestEntry(inputValue));
    }
}