using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Warpgate.SDK.Models.AddressModels;

public class DeleteAppAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    [Required] [IsGuidOrEmpty] public string AppId { get; set; }
}