using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.OAuthViewModels
{
    public class TwoFAAuthorizeConfirmViewModel : FinishAuthInfo
    {
        // Display part:
        public string AppName { get; set; }
        public string UserNickName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Verify Code")]
        public string VerifyCode { get; set; }
    }
}