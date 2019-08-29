using Aiursoft.Pylon.Models.Developer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public AppLayoutModel() { }
        public AppLayoutModel(DeveloperUser user, int activePanel)
        {
            this.RootRecover(user, activePanel);
        }
        public virtual void RootRecover(DeveloperUser user, int activePanel)
        {
            this.NickName = user.NickName;
            this.ActivePanel = activePanel;
            this.AppCount = user.MyApps.Count();
            this.AllApps = user.MyApps;
            this.EmailConfirmed = user.EmailConfirmed;
        }
        public bool EmailConfirmed { get; set; }
        public string NickName { get; set; }
        public int ActivePanel { get; set; }
        public int AppCount { get; set; }
        public IEnumerable<App> AllApps { get; set; }
    }
}
