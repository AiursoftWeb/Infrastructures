using Aiursoft.Handler.Abstract.Models;

namespace Aiursoft.Gateway.SDK.Models.API.UserViewModels
{
    public class View2FAKeyViewModel : AiurProtocol
    {
        public string TwoFAKey { get; set; }
        public string TwoFAQRUri { get; set; }
    }
}
