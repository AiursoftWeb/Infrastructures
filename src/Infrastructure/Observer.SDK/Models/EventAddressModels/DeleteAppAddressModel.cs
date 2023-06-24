using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;
using Aiursoft.CSTools.Attributes;

namespace Aiursoft.Observer.SDK.Models.EventAddressModels;

public class DeleteAppAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    [Required] [IsGuidOrEmpty] public string AppId { get; set; }
}