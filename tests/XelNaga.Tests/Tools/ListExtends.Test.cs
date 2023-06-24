using System.Collections.Generic;
using System.Linq;
using Aiursoft.CSTools.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.XelNaga.Tests.Tools;

[TestClass]
public class ListExtendsTests
{
    [TestMethod]
    public void Shuffle()
    {
        var list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 11 };

        list.Shuffle();

        var shuffled =
            list[0] != 0 ||
            list[1] != 1 ||
            list[2] != 2 ||
            list[3] != 3;

        var countNotChanged = list.Distinct().Count() == list.Count - 1;

        var inRange = list.Max() == 11 && list.Min() == 0;

        Assert.IsTrue(shuffled && countNotChanged && inRange);
    }
}