using System;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public AccountViewModel() { }
        public AccountViewModel(AccountUser user, string title)
        {
            Recover(user, title);
        }

        public virtual bool ModelStateValid { get; set; } = true;
        public virtual bool JustHaveUpdated { get; set; } = false;

        public virtual void Recover(AccountUser user, string title)
        {
            EmailConfirmed = user.EmailConfirmed;
            UserName = user.NickName;
            Title = title;
        }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Title { get; set; }
    }
}
