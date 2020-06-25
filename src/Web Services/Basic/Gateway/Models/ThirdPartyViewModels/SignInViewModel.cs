using Aiursoft.Gateway.SDK.Models;
using Aiursoft.Identity.Services.Authentication;
using Aiursoft.Identity.Services.Authentication.ToGoogleServer;

namespace Aiursoft.Gateway.Models.ThirdPartyViewModels
{
    public class SignInViewModel : FinishAuthInfo
    {
        public SignInViewModel()
        {
            UserDetail = new GoogleUserDetail();
        }
        public string ProviderName { get; set; }
        public string AppImageUrl { get; set; }
        public bool CanFindAnAccountWithEmail { get; set; }
        public IAuthProvider Provider { get; set; }
        public string PreferedLanguage { get; set; }
        public IUserDetail UserDetail { get; set; }
    }
}
