using System.Collections.Generic;

namespace Aiursoft.EE.Models.ProfileViewModels;

public class OverviewViewModel : ProfileViewModelBase
{
    public IEnumerable<Course> CoursesDisplaying { get; set; }
}