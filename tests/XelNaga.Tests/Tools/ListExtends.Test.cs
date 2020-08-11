using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Aiursoft.XelNaga.Tools;
using System.Linq;

namespace Aiursoft.XelNaga.Tests.Tools
{
    [TestClass]
    public class ListExtends
    {
        [TestMethod]
        public void Shuffle()
        {
            var list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

            list.Shuffle();

            var shuffled =
                list[0] != 0 ||
                list[1] != 1 ||
                list[2] != 2 ||
                list[3] != 3;

            var countNotChanged = list.Distinct().Count() == 8;

            var inRange = list.Max() == 7 && list.Min() == 0;

            Assert.IsTrue(shuffled && countNotChanged && inRange);
        }
    }
}
