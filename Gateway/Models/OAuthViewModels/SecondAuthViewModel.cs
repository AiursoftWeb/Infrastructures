using Aiursoft.Pylon.Models;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Gateway.Models.OAuthViewModels
{
    public class SecondAuthViewModel : FinishAuthInfo
    {
        [Required]
        [StringLength(8)]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "Don't ask me this again on this device.")]
        public bool DontAskMeOnIt { get; set; }
        [Display(Name = "Login with recovery code.")]
        public bool AuthWay { get; set; }
    }
}
