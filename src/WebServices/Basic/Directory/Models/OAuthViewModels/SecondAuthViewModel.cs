using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Models;

namespace Aiursoft.Directory.Models.OAuthViewModels;

public class SecondAuthViewModel : FinishAuthInfo
{
    [Required]
    [StringLength(6)]
    [Display(Name = "Verification Code")]
    public string VerifyCode { get; set; }

    [Display(Name = "Don't ask me this again on this device.")]
    public bool DoNotAskMeOnIt { get; set; }
}