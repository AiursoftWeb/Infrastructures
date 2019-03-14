using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Models.Archon
{
    public class AccessTokenViewModel : AiurProtocol
    {
        public virtual string AccessToken { get; set; }
        public virtual DateTime DeadTime { get; set; }
    }
}
