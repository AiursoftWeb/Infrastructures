using System;

namespace Aiursoft.XelNaga.Attributes
{
    public class CSVProperty : Attribute
    {
        public string Name;
        public CSVProperty(string name)
        {
            Name = name;
        }
    }
}
