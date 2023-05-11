using Aiursoft.Identity.Services.Authentication;

namespace Aiursoft.Gateway.Models.ThirdPartyViewModels;

public class BindAccountViewModel
{
    public IUserDetail UserDetail { get; set; }
    public IAuthProvider Provider { get; set; }
    public GatewayUser User { get; set; }
}