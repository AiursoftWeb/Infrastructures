using System.ComponentModel.DataAnnotations;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Observer.SDK.Models.EventAddressModels;

public class DeleteAppAddressModel
{
    [Required] public string AccessToken { get; set; }

    [Required] [IsGuidOrEmpty] public string AppId { get; set; }
}