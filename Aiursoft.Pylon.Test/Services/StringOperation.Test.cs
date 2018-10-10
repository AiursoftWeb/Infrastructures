using System;
using Xunit;
using System.Collections.Generic;
using System.Text;
using Aiursoft.Pylon.Services;

namespace Aiursoft.Pylon.Test.Services
{
    public class StringOperationTest
    {
        [Fact]
        public void GetMD5Test()
        {
            var hash = StringOperation.GetMD5("Helloworld!");
            Assert.Equal("60b42bafc85da5e3a4c6e51204eb2144", hash);
        }
    }
}
