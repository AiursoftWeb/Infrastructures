using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
