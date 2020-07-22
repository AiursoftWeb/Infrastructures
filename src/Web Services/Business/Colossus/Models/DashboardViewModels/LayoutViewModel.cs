using System;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public LayoutViewModel() { }
        public LayoutViewModel(ColossusUser user, string title)
        {
            RootRecover(user, title);
        }

        public bool JustHaveUpdated { get; set; } = false;

        public void RootRecover(ColossusUser user, string title)
        {
            UserName = user.NickName;
            EmailConfirmed = user.EmailConfirmed;
            Title = title;
            HasASite = !string.IsNullOrWhiteSpace(user.SiteName);
        }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Title { get; set; }
        public bool HasASite { get; set; }
    }
}
