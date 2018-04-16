using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class PhoneViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public PhoneViewModel() { }
        public PhoneViewModel(AccountUser user) : base(user, 4, "Phone") { }
        public void Recover(AccountUser user)
        {
            base.Recover(user, 4, "Phone");
        }
        [Display(Name = "Current Phone Number")]
        public string CurrentPhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        [Required]
        [Phone]
        [Display(Name = "New Phone Number")]
        public string NewPhoneNumber { get; set; }
    }
}
