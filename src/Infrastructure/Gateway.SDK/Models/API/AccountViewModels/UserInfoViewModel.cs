using Aiursoft.Handler.Models;

namespace Aiursoft.Directory.SDK.Models.API.AccountViewModels;

public class UserInfoViewModel : AiurProtocol
{
    public virtual AiurUserBase User { get; set; }
}