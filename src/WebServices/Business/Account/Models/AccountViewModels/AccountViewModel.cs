namespace Aiursoft.Account.Models.AccountViewModels;

public class AccountViewModel
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public AccountViewModel()
    {
    }

    public AccountViewModel(AccountUser user, string title)
    {
        RootRecover(user, title);
    }

    public virtual bool JustHaveUpdated { get; set; }
    public virtual bool EmailConfirmed { get; set; }
    public virtual string UserName { get; set; }
    public virtual string Title { get; set; }

    public void RootRecover(AccountUser user, string title)
    {
        EmailConfirmed = user.EmailConfirmed;
        UserName = user.NickName;
        Title = title;
    }
}