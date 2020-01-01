using Aiursoft.XelNaga.Models;

namespace Aiursoft.SDK.Models.API.AccountViewModels
{
    public class CodeToOpenIdViewModel : AiurProtocol
    {
        public string OpenId { get; set; }
        public string Scope { get; set; }
    }
}
