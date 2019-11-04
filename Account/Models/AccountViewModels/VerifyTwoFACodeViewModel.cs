using System;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class VerifyTwoFACodeViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public VerifyTwoFACodeViewModel()
        {
        }
        public VerifyTwoFACodeViewModel(AccountUser user) : base(user, "Two-factor Authentication") { }

        public string Code { get; set; }
    }
}
