using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Aiursoft.Pylon.Models.Stargate
{
    public class Channel
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string ConnectKey { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public TimeSpan LifeTime { get; set; } = new TimeSpan(days: 0, hours: 23, minutes: 59, seconds: 59);
        public bool IsAlive() => DateTime.Now < CreateTime + LifeTime;

        public string AppId { get; set; }
        [ForeignKey(nameof(AppId))]
        [JsonIgnore]
        public StargateApp App { get; set; }
    }
}
