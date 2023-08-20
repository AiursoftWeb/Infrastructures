using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Directory.SDK.Models.API;

public class Grant
{
    public string AppId { get; set; }
    public DateTime GrantTime { get; set; } = DateTime.UtcNow;
    public string DirectoryUserId { get; set; }

    [NotMapped] public virtual AiurUserBase UserInfo { get; set; }
}