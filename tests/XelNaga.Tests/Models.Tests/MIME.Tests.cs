using Aiursoft.XelNaga.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.XelNaga.Tests.Models.Tests;

[TestClass]
public class MIMETests
{
    [TestMethod]
    public void TestCanHandle()
    {
        Assert.IsTrue(Mime.CanHandle("aaaa.mp4"));
        Assert.IsTrue(Mime.CanHandle(".mp4"));
        Assert.IsFalse(Mime.CanHandle("aaa.exe"));
        Assert.IsFalse(Mime.CanHandle(".exe"));
    }

    [TestMethod]
    public void TestIsVideo()
    {
        Assert.IsTrue(Mime.IsVideo("aaaa.mp4"));
        Assert.IsTrue(Mime.IsVideo(".mp4"));
        Assert.IsTrue(Mime.IsVideo("aaaa.webm"));
        Assert.IsTrue(Mime.IsVideo(".webm"));
        Assert.IsTrue(Mime.IsVideo("aaaa.ogg"));
        Assert.IsTrue(Mime.IsVideo(".ogg"));
        Assert.IsFalse(Mime.IsVideo("aaa.exe"));
        Assert.IsFalse(Mime.IsVideo(".exe"));
    }

    [TestMethod]
    public void TestGetType()
    {
        Assert.AreEqual(Mime.GetContentType(".mp4"), "video/mp4");
        Assert.AreEqual(Mime.GetContentType(".html"), "text/html");
        Assert.AreEqual(Mime.GetContentType(".png"), "image/png");
        Assert.AreEqual(Mime.GetContentType(".tiff"), "image/tiff");
        Assert.AreEqual(Mime.GetContentType(".exe"), "application/octet-stream");
        Assert.AreEqual(Mime.GetContentType(".dll"), "application/octet-stream");
        Assert.AreEqual(Mime.GetContentType(".msi"), "application/octet-stream");
    }
}