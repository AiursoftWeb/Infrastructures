using System;

namespace Aiursoft.CSTools.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CsvProperty : Attribute
{
    public readonly string Name;

    public CsvProperty(string name)
    {
        Name = name;
    }
}