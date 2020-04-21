using Aiursoft.SDK.Services.Authentication;

namespace Aiursoft.Gateway.Models.ThirdyPartyViewModels
{
    public class BindAccountViewModel
    {
        public IUserDetail UserDetail { get; set; }
        public IAuthProvider Provider { get; set; }
        public GatewayUser User { get; set; }
    }
}
