using System.Collections.Generic;

namespace Aiursoft.EE.Models.ProfileViewModels
{
    public class FollowingsViewModel : ProfileViewModelBase
    {
        public IEnumerable<Follow> Followings { get; set; }
        public bool IsMe { get; set; }
    }
}
