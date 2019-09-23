using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class TwoFactorAuthenticationViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public TwoFactorAuthenticationViewModel() { }
        public TwoFactorAuthenticationViewModel(AccountUser user) : base(user, 6, "Authentication") { }
        public void Recover(AccountUser user)
        {
            base.Recover(user, 6, "Authentication");
        }

        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }
    }
}
