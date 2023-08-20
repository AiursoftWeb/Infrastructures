namespace Aiursoft.EE.Models.ProfileViewModels;

public class FollowersViewModel : ProfileViewModelBase
{
    public IEnumerable<Follow> Followers { get; set; }
}