using System;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class CreateSiteViewModel : LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public CreateSiteViewModel()
        {
        }

        public CreateSiteViewModel(ColossusUser user) : base(user, 0, "Quick upload")
        {
        }

        public void Recover(ColossusUser user)
        {
            RootRecover(user, 0, "Quick upload");
        }

        public string SiteName { get; set; }
    }
}
