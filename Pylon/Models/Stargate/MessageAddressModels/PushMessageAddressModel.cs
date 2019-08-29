using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Stargate.MessageAddressModels
{
    public class PushMessageAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public int ChannelId { get; set; }
        public string MessageContent { get; set; }
    }
}
