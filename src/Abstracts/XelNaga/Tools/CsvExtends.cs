using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Aiursoft.CSTools.Attributes;

namespace Aiursoft.CSTools.Tools;

public static class CsvExtends
{
    public static byte[] ToCsv<T>(this IReadOnlyCollection<T> items) where T : new()
    {
        var csv = new StringBuilder();
        var type = typeof(T);
        var properties = new List<PropertyInfo>();
        var title = new StringBuilder();
        foreach (var prop in type.GetProperties().Where(t => t.GetCustomAttributes(typeof(CsvProperty), true).Any()))
        {
            properties.Add(prop);
            var attribute = prop.GetCustomAttributes(typeof(CsvProperty), true).FirstOrDefault();
            title.Append($@"""{(attribute as CsvProperty)?.Name}"",");
        }

        csv.AppendLine(title.ToString().Trim(','));
        foreach (var item in items)
        {
            var newLine = new StringBuilder();
            foreach (var propValue in properties
                         .Select(prop => prop.GetValue(item)?.ToString() ?? "null")
                         .Select(propValue => propValue
                             .Replace("\r", string.Empty)
                             .Replace("\n", string.Empty)
                             .Replace("\\", string.Empty)
                             .Replace("\"", string.Empty)
                         ))
            {
                newLine.Append($@"""{propValue}"",");
            }

            csv.AppendLine(newLine.ToString().Trim(','));
        }

        return csv.ToString().StringToBytes();
    }
}