using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.Directory.SDK.Models;

namespace Aiursoft.WWW.Models;

public class WWWUser : AiurUserBase
{
    [InverseProperty(nameof(SearchHistory.TriggerUser))]
    public IEnumerable<SearchHistory> MySearchHistory { get; set; }
}