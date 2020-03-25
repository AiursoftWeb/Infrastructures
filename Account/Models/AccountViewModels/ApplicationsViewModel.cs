using Aiursoft.SDK.Models.API;
using Aiursoft.SDK.Models.Developer;
using System;
using System.Collections.Generic;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class ApplicationsViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public ApplicationsViewModel()
        {
        }
        public ApplicationsViewModel(AccountUser user) : base(user, "Applications") { }

        public IEnumerable<App> Apps { get; set; }

        public IEnumerable<Grant> Grants { get; set; } = new List<Grant>();
    }
}
