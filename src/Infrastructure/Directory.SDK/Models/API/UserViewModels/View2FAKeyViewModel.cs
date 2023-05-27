using Aiursoft.Handler.Models;

namespace Aiursoft.Directory.SDK.Models.API.UserViewModels;

public class View2FAKeyViewModel : AiurProtocol
{
    public string TwoFAKey { get; set; }
    public string TwoFAQRUri { get; set; }
}