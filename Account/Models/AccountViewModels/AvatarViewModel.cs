using System;

namespace Aiursoft.Account.Models.AccountViewModels
{
    public class AvatarViewModel : AccountViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public AvatarViewModel() { }
        public AvatarViewModel(AccountUser user) : base(user, 3, "Avatar") { }
        public void Recover(AccountUser user)
        {
            base.Recover(user, 3, "Avatar");
        }
    }
}
