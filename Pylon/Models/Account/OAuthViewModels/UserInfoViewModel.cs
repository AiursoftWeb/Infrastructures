using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.Account.OAuthViewModels
{
    public class UserInfoViewModel : AiurProtocal
    {
        public virtual AiurUserBase User { get; set; }
    }
}
