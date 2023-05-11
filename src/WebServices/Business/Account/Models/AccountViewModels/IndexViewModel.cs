using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Account.Models.AccountViewModels;

public class IndexViewModel : AccountViewModel
{
    [Obsolete(error: true, message: "This method is only for framework!")]
    public IndexViewModel()
    {
    }

    public IndexViewModel(AccountUser user) : base(user, "Profile")
    {
        Recover(user);
    }

    [Required]
    [MaxLength(20)]
    [Display(Name = "Nickname")]
    public virtual string NickName { get; set; }

    [MaxLength(80)]
    [Display(Name = "Bio")]
    public virtual string Bio { get; set; }

    public void Recover(AccountUser user)
    {
        RootRecover(user, "Profile");
        NickName = user.NickName;
        Bio = user.Bio;
    }
}