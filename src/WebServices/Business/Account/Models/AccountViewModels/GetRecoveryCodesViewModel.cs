using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Account.Models.AccountViewModels;

public class GetRecoveryCodesViewModel : AccountViewModel
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public GetRecoveryCodesViewModel()
    {
    }

    public GetRecoveryCodesViewModel(AccountUser user) : base(user, "Two-factor Authentication")
    {
    }

    [FromQuery(Name = "success")] public bool Success { get; set; }

    public List<string> NewRecoveryCodesKey { get; set; }
}