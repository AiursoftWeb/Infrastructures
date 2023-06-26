using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.SDK.Models.API.UserViewModels;

public class View2FAKeyViewModel : AiurResponse
{
    public string TwoFAKey { get; set; }
    public string TwoFAQRUri { get; set; }
}