using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.ProfileViewModels
{
    public class OverviewViewModel : ProfileViewModelBase
    {
        public IEnumerable<Course> CoursesDisplaying { get; set; }
    }
}
