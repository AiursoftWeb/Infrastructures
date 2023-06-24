using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;
using Aiursoft.CSTools.Attributes;

namespace Aiursoft.Stargate.SDK.Models.ChannelAddressModels;

public class DeleteAppAddressModel
{
    [Required] [IsGuidOrEmpty] public string AppId { get; set; }

    [Required] [IsAccessToken] public string AccessToken { get; set; }
}