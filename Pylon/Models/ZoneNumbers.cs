using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aiursoft.Pylon.Models
{
    public class ZoneNumbers
    {
        public static SelectList BuildSelectList()
        {
            return new SelectList(Numbers.Select(t => new KeyValuePair<string, string>($"+{t.Value} {t.Key}", "+" + t.Value)),
                nameof(KeyValuePair<string, string>.Value),
                nameof(KeyValuePair<string, string>.Key),
                Numbers.First().Value);
        }

        public static readonly Dictionary<string, int> Numbers = new Dictionary<string, int>
        {
            { "P.R.China", 86 },
            { "United States", 1 },
            { "Australia", 61 },
        };
    }
}
