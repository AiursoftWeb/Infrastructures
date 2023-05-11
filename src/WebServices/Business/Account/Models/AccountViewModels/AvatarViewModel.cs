using System;

namespace Aiursoft.Account.Models.AccountViewModels;

public class AvatarViewModel : AccountViewModel
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public AvatarViewModel()
    {
    }

    public AvatarViewModel(AccountUser user) : base(user, "Avatar")
    {
    }

    public string NewIconAddress { get; set; }

    public void Recover(AccountUser user)
    {
        RootRecover(user, "Avatar");
    }
}