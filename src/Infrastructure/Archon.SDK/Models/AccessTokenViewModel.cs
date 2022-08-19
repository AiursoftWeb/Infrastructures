#nullable enable
using Aiursoft.Handler.Models;
using System;

namespace Aiursoft.Archon.SDK.Models
{
    public class AccessTokenViewModel : AiurProtocol
    {
        public virtual string? AccessToken { get; set; }
        public virtual DateTime DeadTime { get; set; }
    }
}
