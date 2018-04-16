using Aiursoft.Pylon.Models.Developer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public AppLayoutModel() { }
        public AppLayoutModel(DeveloperUser User, int ActivePanel)
        {
            this.Recover(User, ActivePanel);
        }
        public virtual void Recover(DeveloperUser User, int ActivePanel)
        {
            this.NickName = User.NickName;
            this.UserIconImageAddress = User.HeadImgUrl;
            this.ActivePanel = ActivePanel;
            this.AppCount = User.MyApps.Count();
            this.AllApps = User.MyApps;
        }
        public string NickName { get; set; }
        public string UserIconImageAddress { get; set; }
        public int ActivePanel { get; set; }
        public int AppCount { get; set; }
        public IEnumerable<App> AllApps { get; set; }
    }
}
