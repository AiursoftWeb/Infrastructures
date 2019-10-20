using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.Authentication;
using Aiursoft.Pylon.Services.Authentications.ToGitHubServer;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.Models.ThirdyPartyViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {
            UserDetail = new GitHubUserDetail();
        }
        // Display part.
        public string ProviderName { get; set; }
        public string AppImageUrl { get; set; }
        public bool CanFindAnAccountWithEmail { get; set; }
        public IAuthProvider Provider { get; set; }
        public string PreferedLanguage { get; set; }
        // Display and submit part
        public FinishAuthInfo OAuthInfo { get; set; }
        public IUserDetail UserDetail { get; set; }
    }
}
