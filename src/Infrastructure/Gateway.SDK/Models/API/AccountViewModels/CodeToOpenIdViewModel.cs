using Aiursoft.Handler.Models;

namespace Aiursoft.Directory.SDK.Models.API.AccountViewModels;

public class CodeToOpenIdViewModel : AiurProtocol
{
    public string OpenId { get; set; }
    public string Scope { get; set; }
}