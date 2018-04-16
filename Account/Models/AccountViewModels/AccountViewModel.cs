using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public abstract class AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public AccountViewModel() { }
        public AccountViewModel(AccountUser user, int ActivePanel, string Title)
        {
            this.Recover(user, ActivePanel, Title);
        }

        public virtual bool ModelStateValid { get; set; } = true;
        public virtual bool JustHaveUpdated { get; set; } = false;

        public virtual void Recover(AccountUser user, int ActivePanel, string Title)
        {
            this.UserName = user.NickName;
            this.UserIconAddress = user.HeadImgUrl;
            this.ActivePanel = ActivePanel;
            this.Title = Title;
        }
        public virtual string UserName { get; set; }
        public virtual string UserIconAddress { get; set; }
        public virtual int ActivePanel { get; set; }
        public virtual string Title { get; set; }
    }
}
