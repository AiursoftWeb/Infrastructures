using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class PhoneViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public PhoneViewModel() { }
        public PhoneViewModel(AccountUser user) : base(user, "Phone") { }
        public void Recover(AccountUser user)
        {
            RootRecover(user, "Phone");
        }
        [Display(Name = "Current Phone Number")]
        public string CurrentPhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public SelectList AvailableZoneNumbers { get; set; }

        [Required]
        [Display(Name = "Zone number")]
        public string ZoneNumber { get; set; }

        [Required]
        [Phone]
        [Display(Name = "New Phone Number")]
        public string NewPhoneNumber { get; set; }
    }
}
