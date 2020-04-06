using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Stargate.SDK.Models
{
    public class Channel
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string ConnectKey { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public string AppId { get; set; }
        [ForeignKey(nameof(AppId))]
        [JsonIgnore]
        public StargateApp App { get; set; }
    }
}
