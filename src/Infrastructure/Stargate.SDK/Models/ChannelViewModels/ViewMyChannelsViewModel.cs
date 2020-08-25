using Aiursoft.Handler.Models;
using Aiursoft.SDKTools.Attributes;
using System.Collections.Generic;

namespace Aiursoft.Stargate.SDK.Models.ChannelViewModels
{
    public class ViewMyChannelsViewModel : AiurProtocol
    {
        [IsGuidOrEmpty]
        public string AppId { get; set; }
        public IEnumerable<ChannelDetail> Channels { get; set; }
    }
}
