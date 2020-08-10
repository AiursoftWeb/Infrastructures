using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Scanner.Interfaces;

namespace Aiursoft.WWW.Services
{
    public class BlackListPorivder
    {
        public string[] BlackListItem;
        public BlackListPorivder(string[] list)
        {
            BlackListItem = list
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();
        }

        public bool InBlackList(string input)
        {
            var url = new Uri(input);
            var domain = url.Host;
            return BlackListItem.Any(t => domain.ToLower().Trim().EndsWith(t.ToLower().Trim()));
        }
    }
}
