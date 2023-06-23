using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Stargate.SDK.Models.ChannelAddressModels;

public class ViewMyChannelsAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }
}