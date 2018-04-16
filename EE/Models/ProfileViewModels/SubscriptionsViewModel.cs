using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.ProfileViewModels
{
    public class SubscriptionsViewModel : ProfileViewModelBase
    {
        public IEnumerable<Subscription> MySubscriptions { get; set; }
    }
}
