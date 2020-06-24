using System;

namespace Aiursoft.XelNaga.Attributes
{
    public class CSVProperty : Attribute
    {
        public readonly string Name;
        public CSVProperty(string name)
        {
            Name = name;
        }
    }
}
