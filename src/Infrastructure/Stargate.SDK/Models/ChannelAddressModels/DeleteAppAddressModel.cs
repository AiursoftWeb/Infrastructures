using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Stargate.SDK.Models.ChannelAddressModels;

public class DeleteAppAddressModel
{
    [Required] [IsGuidOrEmpty] public string AppId { get; set; }

    [Required] public string AccessToken { get; set; }
}