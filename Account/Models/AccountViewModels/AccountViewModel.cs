using System;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public AccountViewModel() { }
        public AccountViewModel(AccountUser user, int activePanel, string title)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Recover(user, activePanel, title);
        }

        public virtual bool ModelStateValid { get; set; } = true;
        public virtual bool JustHaveUpdated { get; set; } = false;

        public virtual void Recover(AccountUser user, int activePanel, string title)
        {
            EmailConfirmed = user.EmailConfirmed;
            UserName = user.NickName;
            ActivePanel = activePanel;
            Title = title;
        }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string UserName { get; set; }
        public virtual int ActivePanel { get; set; }
        public virtual string Title { get; set; }
    }
}
