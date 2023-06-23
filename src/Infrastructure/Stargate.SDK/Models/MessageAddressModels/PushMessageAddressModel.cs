using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Stargate.SDK.Models.MessageAddressModels;

public class PushMessageAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    [Required] public int ChannelId { get; set; }

    public string MessageContent { get; set; }
}