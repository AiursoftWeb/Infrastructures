using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.SDK.Models.API.AppsAddressModels;

public class AccessTokenViewModel : AiurResponse
{
    public virtual string AccessToken { get; set; }
    public virtual DateTime DeadTime { get; set; }
}