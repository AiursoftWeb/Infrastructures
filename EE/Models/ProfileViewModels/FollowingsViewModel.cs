using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.ProfileViewModels
{
    public class FollowingsViewModel : ProfileViewModelBase
    {
        public IEnumerable<Follow> Followings { get; set; }
    }
}
