using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.ProfileViewModels
{
    public class CoursesViewModel : ProfileViewModelBase
    {
        public IEnumerable<Course> AllCourses{ get; set; }
    }
}
