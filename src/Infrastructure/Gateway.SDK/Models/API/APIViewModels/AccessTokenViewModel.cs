using System;
using Aiursoft.Handler.Models;

namespace Aiursoft.Gateway.SDK.Models.API.APIViewModels;

public class AccessTokenViewModel : AiurProtocol
{
    public virtual string AccessToken { get; set; }
    public virtual DateTime DeadTime { get; set; }
}