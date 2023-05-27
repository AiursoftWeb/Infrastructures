using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Models.API;
using Newtonsoft.Json;

namespace Aiursoft.Directory.Models;

public class AppGrant : Grant
{
    [JsonIgnore] public int AppGrantId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(DirectoryUserId))]
    public DirectoryUser User { get; set; }

    public override AiurUserBase UserInfo => User;
}