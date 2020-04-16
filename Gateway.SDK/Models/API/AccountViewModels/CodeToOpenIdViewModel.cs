using Aiursoft.Handler.Models;

namespace Aiursoft.Gateway.SDK.Models.API.AccountViewModels
{
    public class CodeToOpenIdViewModel : AiurProtocol
    {
        public string OpenId { get; set; }
        public string Scope { get; set; }
    }
}
