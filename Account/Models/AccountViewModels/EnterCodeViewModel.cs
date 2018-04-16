using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class EnterCodeViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public EnterCodeViewModel() { }
        public EnterCodeViewModel(AccountUser user) : base(user, 3, "Enter Code") { }
        public void Recover(AccountUser user)
        {
            base.Recover(user, 3, "Enter Code");
        }
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        public string NewPhoneNumber { get; set; }
    }
}
