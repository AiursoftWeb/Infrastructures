using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.SDK.Models.API.AccountViewModels;

public class CodeToOpenIdViewModel : AiurResponse
{
    public string OpenId { get; set; }
    public string Scope { get; set; }
}