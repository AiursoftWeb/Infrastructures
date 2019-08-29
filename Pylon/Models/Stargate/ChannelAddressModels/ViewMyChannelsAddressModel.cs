using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Stargate.ChannelAddressModels
{
    public class ViewMyChannelsAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
