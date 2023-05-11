using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Stargate.SDK.Models.ChannelAddressModels;

public class ViewMyChannelsAddressModel
{
    [Required] public string AccessToken { get; set; }
}