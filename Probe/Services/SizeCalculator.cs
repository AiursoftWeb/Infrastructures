using Aiursoft.Scanner.Interfaces;
using System;
using System.Collections.Generic;

namespace Aiursoft.Probe.Services
{
    public class SizeCalculator : ITransientDependency
    {
        public IEnumerable<int> TwoPows()
        {
            yield return 0;
            for (int i = 1; true; i *= 2)
            {
                yield return i;
            }
        }

        public int Ceiling(int input)
        {
            if (input >= 10000)
            {
                return 10000;
            }
            foreach (var optional in TwoPows())
            {
                if (optional >= input)
                {
                    return optional;
                }
            }
            throw new InvalidOperationException($"Image size calculation failed with input: {input}.");
        }
    }
}
