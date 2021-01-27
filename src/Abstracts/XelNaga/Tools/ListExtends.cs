using System;
using System.Collections.Generic;

namespace Aiursoft.XelNaga.Tools
{
    public static class ListExtends
    {
        private static readonly Random _rng = new Random();
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
    }
}
