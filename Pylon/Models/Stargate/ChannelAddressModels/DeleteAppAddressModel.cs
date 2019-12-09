using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.Stargate.ChannelAddressModels
{
    public class DeleteAppAddressModel
    {
        [Required]
        public string AppId { get; set; }
        [Required]
        public string AccessToken { get; set; }
    }
}
