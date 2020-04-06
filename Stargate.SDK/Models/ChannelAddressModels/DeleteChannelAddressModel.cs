using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Stargate.SDK.Models.ChannelAddressModels
{
    public class DeleteChannelAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public int ChannelId { get; set; }
    }
}
