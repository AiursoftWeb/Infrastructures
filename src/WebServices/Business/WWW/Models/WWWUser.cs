using Aiursoft.Gateway.SDK.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.WWW.Models
{
    public class WWWUser : AiurUserBase
    {
        [InverseProperty(nameof(SearchHistory.TriggerUser))]
        public IEnumerable<SearchHistory> MySearchHistory { get; set; }
    }
}
