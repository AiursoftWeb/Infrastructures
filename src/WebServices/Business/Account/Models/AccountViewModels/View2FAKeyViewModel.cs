namespace Aiursoft.Account.Models.AccountViewModels;

public class View2FAKeyViewModel : AccountViewModel
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public View2FAKeyViewModel()
    {
    }

    public View2FAKeyViewModel(AccountUser user) : base(user, "Two-factor Authentication")
    {
    }

    public string NewTwoFAKey { get; set; }
    public string QRCodeSrc { get; set; }
}