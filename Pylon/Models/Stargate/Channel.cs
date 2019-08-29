using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.Stargate
{
    public class Channel
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string ConnectKey { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public double LifeTime { get; set; } = new TimeSpan(days: 10, hours: 0, minutes: 0, seconds: 0).TotalSeconds;
        public bool IsAlive() => DateTime.UtcNow < CreateTime + TimeSpan.FromSeconds(LifeTime);

        public string AppId { get; set; }
        [ForeignKey(nameof(AppId))]
        [JsonIgnore]
        public StargateApp App { get; set; }
    }
}
