using Aiursoft.Gateway.SDK.Models.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class EmailViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public EmailViewModel() { }
        public EmailViewModel(AccountUser user) : base(user, "Email") { }

        public IEnumerable<AiurUserEmail> Emails { get; set; }
        public string PrimaryEmail { get; set; }
        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string NewEmail { get; set; }
        public void Recover(AccountUser user)
        {
            base.Recover(user, "Email");
        }
    }
}
