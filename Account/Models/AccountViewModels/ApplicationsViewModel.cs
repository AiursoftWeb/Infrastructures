using Aiursoft.Pylon.Models.API;
using Aiursoft.Pylon.Models.Developer;
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

        public ICollection<App> Apps { get; set; } = new List<App>();

        public IEnumerable<Grant> Grants { get; set; } = new List<Grant>();
    }
}
