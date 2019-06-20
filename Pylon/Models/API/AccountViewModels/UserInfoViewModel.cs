using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.API.AccountViewModels
{
    public class UserInfoViewModel : AiurProtocol
    {
        public virtual AiurUserBase User { get; set; }
    }
}
