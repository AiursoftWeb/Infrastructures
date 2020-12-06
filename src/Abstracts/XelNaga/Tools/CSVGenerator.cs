using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aiursoft.XelNaga.Tools
{
    public  class CSVGenerator : ITransientDependency
    {
        public byte[] BuildFromList<T>(IEnumerable<T> items) where T : new()
        {
            List<string> csv = new (items.Count() + 1);
            var type = typeof(T);
            List<string> title = new ();
            List<PropertyInfo> properties = new ();
            foreach (var prop in type.GetProperties().Where(t => t.GetCustomAttributes(typeof(CSVProperty), true).Any()))
            {
                properties.Add(prop);
                var attribute = prop.GetCustomAttributes(typeof(CSVProperty), true).FirstOrDefault();
                title.Add($@"""{(attribute as CSVProperty)?.Name}""");
            }

            csv.Add(string.Join(",", title));
            foreach (var item in items)
            {
                List<string> newLine = new (properties.Count);
                foreach(var prop in properties)
                {
                    var propValue = prop.GetValue(item)?.ToString() ?? "null";
                    propValue = propValue.Replace("\r", "").Replace("\n", "").Replace("\\", "");
                    newLine.Add($@"""{propValue}""");
                }

                csv.Add(string.Join(",", newLine));
            }

            return string.Join("\r\n", csv).ToUTF8WithDom();
        }
    }
}
