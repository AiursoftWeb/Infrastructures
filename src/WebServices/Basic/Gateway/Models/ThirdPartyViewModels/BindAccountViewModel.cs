using Aiursoft.Identity.Services.Authentication;

namespace Aiursoft.Directory.Models.ThirdPartyViewModels;

public class BindAccountViewModel
{
    public IUserDetail UserDetail { get; set; }
    public IAuthProvider Provider { get; set; }
    public DirectoryUser User { get; set; }
}