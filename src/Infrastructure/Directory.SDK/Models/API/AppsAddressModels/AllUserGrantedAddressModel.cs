using System.ComponentModel.DataAnnotations;
using Aiursoft.AiurProtocol;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Directory.SDK.Models.API.AppsAddressModels;

public class AllUserGrantedAddressModel : Pager
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }
}