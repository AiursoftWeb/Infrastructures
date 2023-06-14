using System;
using System.Collections.Generic;
using Aiursoft.Directory.SDK.Models;
using Aiursoft.Directory.SDK.Models.API;

namespace Aiursoft.Account.Models.AccountViewModels;

public class ApplicationsViewModel : AccountViewModel
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public ApplicationsViewModel()
    {
    }

    public ApplicationsViewModel(AccountUser user) : base(user, "Applications")
    {
    }

    public List<DirectoryApp> Apps { get; set; }

    public IEnumerable<Grant> Grants { get; set; } = new List<Grant>();
}