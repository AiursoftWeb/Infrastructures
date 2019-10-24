using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.Authentication;
using Aiursoft.Pylon.Services.Authentication.ToGoogerServer;

namespace Aiursoft.Gateway.Models.ThirdyPartyViewModels
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
