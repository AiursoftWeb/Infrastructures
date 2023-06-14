using System;
using Aiursoft.Handler.Models;

namespace Aiursoft.Directory.SDK.Models.API.AppsAddressModels;

public class AccessTokenViewModel : AiurProtocol
{
    public virtual string AccessToken { get; set; }
    public virtual DateTime DeadTime { get; set; }
}