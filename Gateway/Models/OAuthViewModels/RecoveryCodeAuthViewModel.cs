using Aiursoft.SDK.Models;
using System.ComponentModel.DataAnnotations;


namespace Aiursoft.Gateway.Models.OAuthViewModels
{
    public class RecoveryCodeAuthViewModel : FinishAuthInfo
    {
        [Required]
        [StringLength(8)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }
}
