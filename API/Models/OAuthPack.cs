using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.API.Models
{
    public class OAuthPack
    {
        public virtual int OAuthPackId { get; set; }
        public virtual int Code { get; set; }
        public virtual string ApplyAppId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public virtual APIUser User { get; set; }
        public virtual string UserId { get; set; }

        public virtual bool IsUsed { get; set; }
        public virtual DateTime CreateTime { get; set; } = DateTime.Now;
        public virtual TimeSpan AliveTime { get; set; } = new TimeSpan(0, 20, 0);
        public virtual bool IsAlive => DateTime.Now - CreateTime < AliveTime;
    }
}
