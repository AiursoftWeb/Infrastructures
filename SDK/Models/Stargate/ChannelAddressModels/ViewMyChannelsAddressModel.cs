using System.ComponentModel.DataAnnotations;

namespace Aiursoft.SDK.Models.Stargate.ChannelAddressModels
{
    public class ViewMyChannelsAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
