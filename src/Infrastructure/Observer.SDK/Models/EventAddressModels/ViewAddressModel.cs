using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Observer.SDK.Models.EventAddressModels;

public class ViewAddressModel
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }
}