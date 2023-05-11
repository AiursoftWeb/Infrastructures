using System;

namespace Aiursoft.XelNaga.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CSVProperty : Attribute
{
    public readonly string Name;

    public CSVProperty(string name)
    {
        Name = name;
    }
}