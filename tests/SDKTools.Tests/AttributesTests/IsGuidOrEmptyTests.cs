using System.ComponentModel.DataAnnotations;
using System.Net;
using Aiursoft.SDKTools.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.SDKTools.Tests.AttributesTests;

[TestClass]
public class IsGuidOrEmptyTests
{
    private IsGuidOrEmpty _validator;

    [TestInitialize]
    public void CreateValidator()
    {
        _validator = new IsGuidOrEmpty();
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("509d2c00-65ac-43f1-a9a4-0260f4e18376")]
    [DataRow("18833427949641b09dbab71ab3b50592")]
    [DataRow("{1d13e7ff-165d-46eb-aa61-6d38b6b4c9e2}")]
    [DataRow("(4e14ac4a-1950-47bc-ac52-cbce49e00f55)")]
    [DataRow("{0x5a9ad38f,0xc291,0x44e8,{0x84,0xe9,0xa0,0xc9,0xbd,0x32,0x67,0x30}}")]
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