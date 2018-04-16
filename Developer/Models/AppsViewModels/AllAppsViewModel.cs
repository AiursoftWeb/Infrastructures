using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Developer.Models.AppsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class AllAppsViewModel : AppLayoutModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public AllAppsViewModel() { }
        public AllAppsViewModel(DeveloperUser User) : base(User, 1)
        {
        }
    }
}
