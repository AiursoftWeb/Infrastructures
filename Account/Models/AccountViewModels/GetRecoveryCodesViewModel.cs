using System;
using System.Collections.Generic;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class GetRecoveryCodesViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public GetRecoveryCodesViewModel()
        {
        }
        public GetRecoveryCodesViewModel(AccountUser user) : base(user, "Two-factor Authentication") { }

        public List<string> NewRecoveryCodesKey { get; set; }
    }
}
