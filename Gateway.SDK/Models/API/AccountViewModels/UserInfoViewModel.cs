using Aiursoft.Handler.Abstract.Models;

namespace Aiursoft.Gateway.SDK.Models.API.AccountViewModels
{
    public class UserInfoViewModel : AiurProtocol
    {
        public virtual AiurUserBase User { get; set; }
    }
}
