using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Stargate.SDK.Models.ChannelAddressModels;

public class CreateChannelAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    public string Description { get; set; }
}