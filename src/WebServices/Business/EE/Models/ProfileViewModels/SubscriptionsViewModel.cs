using System.Collections.Generic;

namespace Aiursoft.EE.Models.ProfileViewModels;

public class SubscriptionsViewModel : ProfileViewModelBase
{
    public IEnumerable<Subscription> MySubscriptions { get; set; }
}