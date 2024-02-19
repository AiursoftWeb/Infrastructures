using System.ComponentModel.DataAnnotations;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Directory.SDK.Models.API.AppsAddressModels;

public class AllUserGrantedAddressModel : Pager
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }
}