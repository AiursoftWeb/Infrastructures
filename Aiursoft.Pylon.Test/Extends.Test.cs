using System;
using Xunit;
using Aiursoft.Pylon;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon.Models;

namespace Aiursoft.Pylon.Test
{
    public class TestController : Controller
    {

    }
    public class ExtendsTest
    {
        [Fact]
        public void ProtocalTest()
        {
            var t = new TestController();
            var value = Extends.Protocal(t, ErrorType.Success, "Success");
            Assert.NotNull(value);
        }
    }
}
