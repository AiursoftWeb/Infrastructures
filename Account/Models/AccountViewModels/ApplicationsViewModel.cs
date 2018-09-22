using Aiursoft.Pylon.Models.Developer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class ApplicationsViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public ApplicationsViewModel()
        {
        }
        public ApplicationsViewModel(AccountUser user) : base(user, 5, "Avatar") { }

        public ICollection<App> Apps { get; set; } = new List<App>();
    }
}
