using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Aiursoft.Gateway.Models;

public class OAuthPack
{
    public virtual int OAuthPackId { get; set; }
    public virtual int Code { get; set; }
    public virtual string ApplyAppId { get; set; }

    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public virtual DirectoryUser User { get; set; }

    public virtual string UserId { get; set; }

    public virtual DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public virtual DateTime UseTime { get; set; } = DateTime.MinValue;
    public virtual TimeSpan AliveTime { get; set; } = new(0, 20, 0);
    public virtual bool IsAlive => DateTime.UtcNow - CreateTime < AliveTime;
}