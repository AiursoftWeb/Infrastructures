using System;

namespace Aiursoft.Colossus.Models.DashboardViewModels
{
    public class SettingsViewModel : LayoutViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public SettingsViewModel() { }
        public SettingsViewModel(ColossusUser user) : base(user, 2, "Settings") { }
        public void Recover(ColossusUser user)
        {
            RootRecover(user, 2, "Settings");
        }

        public long SiteSize { get; set; }
    }
}
