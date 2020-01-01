using Aiursoft.SDK.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.EE.Models
{
    public class EEUser : AiurUserBase
    {
        public string LongDescription { get; set; }

        [InverseProperty(nameof(Subscription.User))]
        public IEnumerable<Subscription> Subscriptions { get; set; }

        [InverseProperty(nameof(Follow.Trigger))]
        public IEnumerable<Follow> Following { get; set; }

        [InverseProperty(nameof(Follow.Receiver))]
        public IEnumerable<Follow> Followers { get; set; }

        [InverseProperty(nameof(Course.Owner))]
        public IEnumerable<Course> CoursesCreated { get; set; }
    }
}
