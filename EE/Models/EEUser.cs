using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Pylon.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.EE.Models
{
    public class EEUser : AiurUserBase
    {
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
