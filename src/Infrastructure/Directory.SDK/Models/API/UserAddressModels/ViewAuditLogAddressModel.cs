using System.ComponentModel.DataAnnotations;
using Aiursoft.AiurProtocol;
using Aiursoft.Directory.SDK.Attributes;

namespace Aiursoft.Directory.SDK.Models.API.UserAddressModels;

public class ViewAuditLogAddressModel : Pager
{
    [Required] [IsAccessToken] public string AccessToken { get; set; }

    [Required] public string OpenId { get; set; }
}