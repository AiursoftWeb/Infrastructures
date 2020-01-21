using System;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class DisableTwoFAViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public DisableTwoFAViewModel()
        {
        }
        public DisableTwoFAViewModel(AccountUser user) : base(user, "Two-factor Authentication") { }
    }
}
