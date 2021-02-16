using System;

namespace Aiursoft.Wrap.Models.DashboardViewModels
{
    public class LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public LayoutViewModel() { }
        public LayoutViewModel(WrapUser user, string title)
        {
            RootRecover(user, title);
        }

        public bool JustHaveUpdated { get; set; } = false;

        public void RootRecover(WrapUser user, string title)
        {
            UserName = user.NickName;
            EmailConfirmed = user.EmailConfirmed;
            Title = title;
        }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Title { get; set; }
    }
}
