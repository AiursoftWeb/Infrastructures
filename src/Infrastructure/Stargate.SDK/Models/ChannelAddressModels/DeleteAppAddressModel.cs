using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Stargate.SDK.Models.ChannelAddressModels
{
    public class DeleteAppAddressModel
    {
        [Required]
        public string AppId { get; set; }
        [Required]
        public string AccessToken { get; set; }
    }
}
