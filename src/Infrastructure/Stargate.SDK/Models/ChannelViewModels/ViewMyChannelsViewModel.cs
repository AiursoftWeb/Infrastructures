using System.Collections.Generic;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.XelNaga.Attributes;

namespace Aiursoft.Stargate.SDK.Models.ChannelViewModels;

public class ViewMyChannelsViewModel : AiurResponse
{
    [IsGuidOrEmpty] public string AppId { get; set; }

    public IEnumerable<Channel> Channels { get; set; }
}