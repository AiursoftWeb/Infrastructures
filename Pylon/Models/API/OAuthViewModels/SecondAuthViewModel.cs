using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Pylon.Models.API.OAuthViewModels
{
    public class SecondAuthViewModel : FinishAuthInfo
    {
        [Required]
        [MinLength(6)]
        [MaxLength(6)]
        [Display(Name = "Verify Code")]
        public string VerifyCode { get; set; }

        [Display(Name = "Don't ask me this again on this device.")]
        public bool DontAskMeOnIt { get; set; }
    }
}