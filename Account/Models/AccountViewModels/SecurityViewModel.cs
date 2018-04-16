using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class SecurityViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public SecurityViewModel() { }
        public SecurityViewModel(AccountUser user) : base(user, 2, "Security") { }
        public void Recover(AccountUser user)
        {
            base.Recover(user, 2, "Avatar");
        }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Repeat Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
        public string RepeatPassword { get; set; }
    }
}
