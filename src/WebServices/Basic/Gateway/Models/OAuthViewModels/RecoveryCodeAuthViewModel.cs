using System.ComponentModel.DataAnnotations;
using Aiursoft.Gateway.SDK.Models;

namespace Aiursoft.Gateway.Models.OAuthViewModels;

public class RecoveryCodeAuthViewModel : FinishAuthInfo
{
    [Required]
    [StringLength(8)]
    [Display(Name = "Recovery Code")]
    public string RecoveryCode { get; set; }
}