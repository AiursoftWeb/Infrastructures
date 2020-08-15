using Aiursoft.Scanner.Interfaces;
using System;
using System.Collections.Generic;

namespace Aiursoft.Probe.Services
{
    public class SizeCalculator : ITransientDependency
    {
        private IEnumerable<int> GetTwoPowers()
        {
            yield return 0;
            for (int i = 1; i <= 8192; i *= 2)
            {
                yield return i;
            }
        }

        public int Ceiling(int input)
        {
            if (input >= 8192)
            {
                return 8192;
            }
            foreach (var optional in GetTwoPowers())
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
