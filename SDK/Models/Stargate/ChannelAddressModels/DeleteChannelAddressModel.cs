using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.Stargate.ChannelAddressModels
{
    public class DeleteChannelAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public int ChannelId { get; set; }
    }
}
