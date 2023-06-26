using Aiursoft.AiurProtocol;

namespace Aiursoft.Directory.SDK.Models.API.AccountViewModels;

public class UserInfoViewModel : AiurResponse
{
    public virtual AiurUserBase User { get; set; }
}