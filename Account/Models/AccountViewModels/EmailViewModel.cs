using Aiursoft.Pylon.Models.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class EmailViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public EmailViewModel() { }
        public EmailViewModel(AccountUser user) : base(user, 1, "Avatar") { }

        public IEnumerable<AiurUserEmail> Emails { get; set; }
        public string PrimaryEmail { get; set; }
        [Required]
        public string NewEmail { get; set; }
        public void Recover(AccountUser user)
        {
            base.Recover(user, 1, "Avatar");
        }
    }
}
