using Aiursoft.XelNaga.Models;
using System;

namespace Aiursoft.SDK.Models.Archon
{
    public class AccessTokenViewModel : AiurProtocol
    {
        public virtual string AccessToken { get; set; }
        public virtual DateTime DeadTime { get; set; }
    }
}
