using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Stargate.SDK.Models.ChannelAddressModels;

public class CreateChannelAddressModel
{
    [Required] public string AccessToken { get; set; }

    public string Description { get; set; }
}