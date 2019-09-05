using System;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public LayoutViewModel() { }
        public LayoutViewModel(ColossusUser user, int activePanel, string title)
        {
            RootRecover(user, activePanel, title);
        }

        public bool ModelStateValid { get; set; } = true;
        public bool JustHaveUpdated { get; set; } = false;

        public void RootRecover(ColossusUser user, int activePanel, string title)
        {
            UserName = user.NickName;
            EmailConfirmed = user.EmailConfirmed;
            ActivePanel = activePanel;
            Title = title;
        }
        public string UserName { get; set; }
        public int ActivePanel { get; set; }
        public string Title { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
