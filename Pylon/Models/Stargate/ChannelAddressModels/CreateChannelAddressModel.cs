using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Stargate.ChannelAddressModels
{
    public class CreateChannelAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        public string Description { get; set; }
    }
}
