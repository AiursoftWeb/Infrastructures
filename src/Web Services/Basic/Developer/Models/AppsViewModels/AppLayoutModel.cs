using System;
using System.Collections.Generic;
using System.Linq;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public AppLayoutModel() { }
        public AppLayoutModel(DeveloperUser user)
        {
            this.RootRecover(user);
        }
        public void RootRecover(DeveloperUser user)
        {
            this.NickName = user.NickName;
            this.AppCount = user.MyApps.Count();
            this.AllApps = user.MyApps;
            this.EmailConfirmed = user.EmailConfirmed;
        }
        public bool EmailConfirmed { get; set; }
        public string NickName { get; set; }
        public int AppCount { get; set; }
        public IEnumerable<DeveloperApp> AllApps { get; set; }
    }
}
