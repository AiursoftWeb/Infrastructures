using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{

    public class GenerateRecoveryCodesViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public GenerateRecoveryCodesViewModel() { }
        public GenerateRecoveryCodesViewModel(AccountUser user) : base(user, 7, "Authentication") { }
        public void Recover(AccountUser user)
        {
            base.Recover(user, 7, "Authentication");
        }

        public List<string> RecCodesKeyArray { get; set; }
    }
}
