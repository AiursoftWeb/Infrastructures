using System.ComponentModel.DataAnnotations;
using Aiursoft.Directory.SDK.Models;

namespace Aiursoft.Directory.Models.OAuthViewModels;

public class RecoveryCodeAuthViewModel : FinishAuthInfo
{
    [Required]
    [StringLength(8)]
    [Display(Name = "Recovery Code")]
    public string RecoveryCode { get; set; }
}