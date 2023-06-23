using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Warpgate.SDK.Models.AddressModels;

public class ViewMyRecordsAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    public string Tag { get; set; }
}