using Aiursoft.XelNaga.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.XelNaga.Tests.Models.Tests;

[TestClass]
public class MIMETests
{
    [TestMethod]
    public void TestCanHandle()
    {
        Assert.IsTrue(MIME.CanHandle("aaaa.mp4"));
        Assert.IsTrue(MIME.CanHandle(".mp4"));
        Assert.IsFalse(MIME.CanHandle("aaa.exe"));
        Assert.IsFalse(MIME.CanHandle(".exe"));
    }

    [TestMethod]
    public void TestIsVideo()
    {
        Assert.IsTrue(MIME.IsVideo("aaaa.mp4"));
        Assert.IsTrue(MIME.IsVideo(".mp4"));
        Assert.IsTrue(MIME.IsVideo("aaaa.webm"));
        Assert.IsTrue(MIME.IsVideo(".webm"));
        Assert.IsTrue(MIME.IsVideo("aaaa.ogg"));
        Assert.IsTrue(MIME.IsVideo(".ogg"));
        Assert.IsFalse(MIME.IsVideo("aaa.exe"));
        Assert.IsFalse(MIME.IsVideo(".exe"));
    }

    [TestMethod]
    public void TestGetType()
    {
        Assert.AreEqual(MIME.GetContentType(".mp4"), "video/mp4");
        Assert.AreEqual(MIME.GetContentType(".html"), "text/html");
        Assert.AreEqual(MIME.GetContentType(".png"), "image/png");
        Assert.AreEqual(MIME.GetContentType(".tiff"), "image/tiff");
        Assert.AreEqual(MIME.GetContentType(".exe"), "application/octet-stream");
        Assert.AreEqual(MIME.GetContentType(".dll"), "application/octet-stream");
        Assert.AreEqual(MIME.GetContentType(".msi"), "application/octet-stream");
    }
}