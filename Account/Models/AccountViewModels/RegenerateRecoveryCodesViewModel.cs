using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class RegenerateRecoveryCodesViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public RegenerateRecoveryCodesViewModel() { }
        public RegenerateRecoveryCodesViewModel(AccountUser user) : base(user, "Authentication") { }
        public void Recover(AccountUser user)
        {
            base.Recover(user,"Authentication");
        }
        public List<string> RecCodesKeyArray { get; set; }

    }
}
