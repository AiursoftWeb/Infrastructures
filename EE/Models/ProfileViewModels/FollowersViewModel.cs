using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.ProfileViewModels
{
    public class FollowersViewModel : ProfileViewModelBase
    {
        public IEnumerable<Follow> Followers { get; set; }
    }
}
