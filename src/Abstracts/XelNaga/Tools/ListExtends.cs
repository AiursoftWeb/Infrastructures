using System;
using System.Collections.Generic;

namespace Aiursoft.XelNaga.Tools;

public static class ListExtends
{
    private static readonly Random Rng = new();

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}