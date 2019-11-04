using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.OAuthViewModels
{
    public class TwoFAAuthorizeConfirmViewModel : FinishAuthInfo
    {
        [Required]
        [MinLength(6)]
        [MaxLength(6)]
        [Display(Name = "Verify Code")]
        public string VerifyCode { get; set; }
    }
}