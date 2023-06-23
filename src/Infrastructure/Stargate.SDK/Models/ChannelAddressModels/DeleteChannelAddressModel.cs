using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Stargate.SDK.Models.ChannelAddressModels;

public class DeleteChannelAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    [Required] public int ChannelId { get; set; }
}