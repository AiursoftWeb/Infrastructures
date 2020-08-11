using System;
using System.Linq;

namespace Aiursoft.SDK.Services
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
