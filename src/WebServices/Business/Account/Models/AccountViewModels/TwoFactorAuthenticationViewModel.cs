using System;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class TwoFactorAuthenticationViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public TwoFactorAuthenticationViewModel()
        {
        }
        public TwoFactorAuthenticationViewModel(AccountUser user) : base(user, "Two-factor Authentication") { }

        public bool NewHas2FAKey { get; set; }
        public bool NewTwoFactorEnabled { get; set; }
    }
}

