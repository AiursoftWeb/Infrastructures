using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Aiursoft.Scanner.Abstract;
using Aiursoft.XelNaga.Attributes;

namespace Aiursoft.XelNaga.Tools;

public class CSVGenerator : ITransientDependency
{
    public byte[] BuildFromCollection<T>(IEnumerable<T> items) where T : new()
    {
        var csv = new StringBuilder();
        var type = typeof(T);
        var properties = new List<PropertyInfo>();
        var title = new StringBuilder();
        foreach (var prop in type.GetProperties().Where(t => t.GetCustomAttributes(typeof(CSVProperty), true).Any()))
        {
            properties.Add(prop);
            var attribute = prop.GetCustomAttributes(typeof(CSVProperty), true).FirstOrDefault();
            title.Append($@"""{(attribute as CSVProperty)?.Name}"",");
        }

        csv.AppendLine(title.ToString().Trim(','));
        foreach (var item in items)
        {
            var newLine = new StringBuilder();
            foreach (var prop in properties)
            {
                var propValue = prop.GetValue(item)?.ToString() ?? "null";
                propValue = propValue.Replace("\r", "").Replace("\n", "").Replace("\\", "").Replace("\"", "");
                newLine.Append($@"""{propValue}"",");
            }

            csv.AppendLine(newLine.ToString().Trim(','));
        }

        return csv.ToString().StringToBytes();
    }
}