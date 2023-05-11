using System.Collections.Generic;
using Aiursoft.Handler.Models;
using Aiursoft.SDKTools.Attributes;

namespace Aiursoft.Stargate.SDK.Models.ChannelViewModels;

public class ViewMyChannelsViewModel : AiurProtocol
{
    [IsGuidOrEmpty] public string AppId { get; set; }

    public IEnumerable<Channel> Channels { get; set; }
}