using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Models.API.ApiViewModels
{
    public class AccessTokenViewModel : AiurProtocal
    {
        public virtual string AccessToken { get; set; }
        public virtual DateTime DeadTime { get; set; }
    }
}
