using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Pylon.Models.Stargate
{
    public class StargateApp
    {
        [Key]
        public string Id { get; set; }

        [JsonIgnore]
        [InverseProperty(nameof(Channel.App))]
        public List<Channel> Channels { get; set; }
    }
}
